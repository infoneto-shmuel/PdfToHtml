using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PdfRepresantation
{
    public class PdfHtmlWriter
    {
        public string ConvertPage(PdfPageDetails page)
        {
            var sb = new StringBuilder();
            var fontRef = CreateFontRef(page.Fonts);
            var allLines = page.Lines;
            StartHtml(sb, "pdf page " + page.PageNumber, page.RightToLeft, fontRef, allLines);
            AddPage(page, fontRef, sb);
            EndHtml(sb);
            return sb.ToString();
        }

        public string ConvertPdf(PdfDetails pdf)
        {
            var sb = new StringBuilder();
            var fontRef = CreateFontRef(pdf.Fonts);
            var allLines = pdf.Pages.SelectMany(p => p.Lines);
            StartHtml(sb, "pdf converted to html", null, fontRef, allLines);
            foreach (var page in pdf.Pages)
            {
                AddPage(page, fontRef, sb);
            }

            EndHtml(sb);
            return sb.ToString();
        }

        private static Dictionary<PdfFontDetails, int> CreateFontRef(IEnumerable<PdfFontDetails> fonts)
        {
            var fontRef = fonts
                .Select((f, i) => new {f, i})
                .ToDictionary(a => a.f, a => a.i);
            return fontRef;
        }

        public void StartHtml(StringBuilder sb, string title, bool? rightToLeft,
            Dictionary<PdfFontDetails, int> fontRef,
            IEnumerable<PdfTextLineDetails> allLines)
        {
            sb.Append($@"<html>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
    <title>").Append(title).Append(@"</title>");
            AddStyle(fontRef, allLines, sb);
            AddScript(sb);
            sb.Append($@"
</head>
<body");
            if (rightToLeft.HasValue)
                sb.Append(@" dir=""").Append(rightToLeft.Value ? "rtl" : "ltr").Append("\"");
            sb.Append(">");
        }

        public void EndHtml(StringBuilder sb)
        {
            sb.Append(@"
</body>
</html>");
        }

        public void AddPage(PdfPageDetails page, Dictionary<PdfFontDetails, int> fontRef, StringBuilder sb)
        {
            AddHeader(page, sb);
            sb.Append(@"
    <article class=""article"" dir=""").Append(page.RightToLeft ? "rtl" : "ltr")
                .Append("\" style=\"width: ").Append(Math.Round(page.Width))
                .Append("px;height:").Append(Math.Round(page.Height))
                .Append("px;\">");
            foreach (var pdfImage in page.Images)
            {
                AddImage(page, pdfImage, sb);
            }

            foreach (var line in page.Lines)
            {
                AddLine(page, fontRef, line, sb);
            }

            sb.Append(@"
    </article>");
               AddShapes(page, sb);            
        }

        protected virtual void AddHeader(PdfPageDetails page, StringBuilder sb)
        {
            sb.Append(@"
    <h2 class=""header"" style=""width: ").Append(Math.Round(page.Width))
                .Append("px;\">Page ").Append(page.PageNumber).Append("</h2>");
        }

        protected virtual void AddLine(PdfPageDetails page, Dictionary<PdfFontDetails, int> fontRef,
            PdfTextLineDetails line, StringBuilder sb)
        {
            sb.Append($@"
        <div class=""line"" style=""")
                .Append("right:").Append((int) line.Right)
                .Append("px;left:").Append((int) line.Left)
                .Append("px;top:").Append((int) (line.Top))
                .Append("px;width:").Append((int) (line.Width))
                .Append("px;bottom:").Append((int) (page.Height - line.Bottom))
                .Append("px\" >");
            PdfLinkResult link = null;
            foreach (var text in line.Texts)
            {
                if (text.LinkParent != null)
                {
                    if (text.LinkParent != link)
                        AddLink(link = text.LinkParent, fontRef, sb);
                    continue;
                }

                AddText(text, fontRef, sb);
            }

            sb.Append(@"</div>");
        }

        protected virtual void AddLink(PdfLinkResult link, Dictionary<PdfFontDetails, int> fontRef, StringBuilder sb)
        {
            sb.Append($@"<a href=""").Append(link.Link).Append("\">");
            foreach (var text in link.Children)
            {
                AddText(text, fontRef, sb);
            }

            sb.Append(@"</a>");
        }

        protected virtual void AddText(PdfTextResult text,
            Dictionary<PdfFontDetails, int> fontRef, StringBuilder sb)
        {
            sb.Append($@"<span class=""baseline font").Append(fontRef[text.Font] + 1)
                .Append(" font-size-").Append((Math.Round(text.FontSize * 2) / 2).ToString(formatNumInClassName))
                .Append("\" style=\"");
            AddColor(text, sb);
            sb.Append(@""">");
            AddText(text.Value, sb);
            sb.Append(@"</span>");
        }

        private static void AddText(string text, StringBuilder sb)
        {
            var textEncoded = HttpUtility.HtmlEncode(text);
            bool lastSpace = false;
            foreach (var c in textEncoded)
            {
                if (c == ' ')
                {
                    if (lastSpace)
                        sb.Append("&nbsp;");
                    else
                        sb.Append(c);
                    lastSpace = true;
                    continue;
                }

                lastSpace = false;
                if (char.IsLetterOrDigit(c))
                    sb.Append(c);
                else if (c <= 127)
                    sb.Append(c);
                else
                    sb.Append("&#").Append((int) c).Append(";");
            }
        }

        protected virtual void AddColor(PdfTextResult text, StringBuilder sb)
        {
            if (!text.StrokeColore.HasValue)
                return;
            sb.Append("color:");
            AppendColor(text.StrokeColore.Value, sb);
            sb.Append(";");
            var b = text.StrokeColore.Value.GetBrightness();
            if (b > 0.9)
                sb.Append("background-color:black;");
        }

        protected virtual void AddImage(PdfPageDetails page, PdfImageDetails image, StringBuilder sb)
        {
            sb.Append(@"
        <img class=""image"" height=""").Append(image.Height)
                .Append("\" width=\"")
                .Append(image.Width).Append("\" src=\"data:image/png;base64, ")
                .Append(Convert.ToBase64String(image.Buffer)).Append("\" style=\"")
                .Append(page.RightToLeft ? "right" : "left")
                .Append(":").Append((int) ((page.RightToLeft ? image.Right : image.Left)))
                .Append("px; top:").Append((int) (image.Top)).Append("px\">");
            ;
        }

        protected virtual void AddShapes(PdfPageDetails page, StringBuilder sb)
        {
            if (page.Shapes.Count == 0)
                return;

            sb.Append(@"
    <canvas class=""canvas"" id=""canvas-").Append(page.PageNumber)
                .Append("\" style=\"width: ")
                .Append(Math.Round(page.Width))
                .Append("px;height:").Append(Math.Round(page.Height))
                .Append("px;margin-top:-").Append(Math.Round(page.Height) + 2)
                .Append("px;\" width=\" ")
                .Append(Math.Round(page.Width))
                .Append("\" height=\"").Append(Math.Round(page.Height)).Append("\"></canvas>");

            sb.Append(@"
    <script>
        currentCanvas= document.getElementById('canvas-").Append(page.PageNumber).Append(@"');");
            foreach (var shape in page.Shapes)
            {
                AddShape(shape, sb);
            }

            sb.Append(@"
    </script>");
        }

        void AppendColor(Color? color, StringBuilder sb)
        {
            if (color.HasValue)
            {
                sb.Append("'");
                AppendColor(color.Value, sb);
                sb.Append("'");
            }
            else
                sb.Append("null");
        }

        void AppendColor(Color color, StringBuilder sb)
        {
            sb.Append("#").Append(color.R.ToString("X2"))
                .Append(color.G.ToString("X2"))
                .Append(color.B.ToString("X2"));
            if (color.A != 255)
                sb.Append(color.A.ToString("X2"));
        }

        protected virtual void AddShape(ShapeDetails shape, StringBuilder sb)
        {
            sb.Append(@"
        draw([");
            for (var i = 0; i < shape.Lines.Count; i++)
            {
                if (i != 0)
                    sb.Append(",");
                var line = shape.Lines[i];
                sb.Append("[").Append(line.Start.X).Append(",").Append(line.Start.Y)
                    .Append(",").Append(line.End.X).Append(",").Append(line.End.Y).Append("]");
            }

            sb.Append("],").Append((int) shape.ShapeOperation).Append(",");
            AppendColor(shape.StrokeColor, sb);
            sb.Append(",");
            AppendColor(shape.FillColor, sb);
            sb.Append(",").Append(shape.LineWidth)
                .Append(",").Append("null").Append(");");
        }

        protected virtual void AddScript(StringBuilder sb)
        {
            sb.Append(@"
    <script>
        var currentCanvas;
        function draw(lines,operation,strokeColor, fillColor, lineWidth,lineCap) {
             if (!currentCanvas.getContext)
                 return;
             var ctx = currentCanvas.getContext('2d');
             if (lineWidth) 
                 ctx.lineWidth = lineWidth;
             if (!lineCap) 
                 ctx.lineCap= lineCap;
             ctx.fillStyle=fillColor||'white';               
             ctx.strokeStyle=strokeColor||'black';
             ctx.beginPath();
             var lastLine=null;
             var drawLine=function (line) {
                 if (!lastLine||lastLine[2]!=line[0]||lastLine[3]!=line[1])
                     ctx.moveTo(line[0], line[1]);
                 ctx.lineTo(line[2], line[3]);
                 lastLine=line;
             };
             for (var i = 0; i < lines.length; i++) 
                 drawLine(lines[i]);
             switch (operation) {
                 case 1:ctx.stroke();break;
                 case 2:ctx.fill();break;
                 case 3:ctx.stroke();
                     ctx.fill();break;
             }
        }
    </script>");
        }

        NumberFormatInfo formatNumInClassName = new NumberFormatInfo {NumberDecimalSeparator = "-"};

        protected virtual void AddStyle(Dictionary<PdfFontDetails, int> fontRef,
            IEnumerable<PdfTextLineDetails> allLines,
            StringBuilder sb)
        {
            sb.Append(@"
    <style>
        .line{
            position:absolute;
            width:fit-content !important;
        }
        .baseline{vertical-align:baseline;}
        .image{position:absolute}
        .canvas{
            margin: 0 auto 0 auto;
            display: block;
        }
        .header{
            color: #795548;
            font-family: Arial;
            text-align: center;
            margin:20px auto 0 auto;
        }
        .article{
            border-style: groove;
            position:relative;
            margin: 0 auto 0 auto;
            border-width: 2px;
        }");
            foreach (var size in allLines.SelectMany(l => l.Texts).Select(t => Math.Round(t.FontSize * 2)).Distinct())
            {
                sb.Append(@"
        .font-size-").Append((size / 2).ToString(formatNumInClassName))
                    .Append("{font-size:").Append(size / 2).Append("px;}");
            }

            foreach (var pair in fontRef)
            {
                sb.Append(@"
        .font").Append(pair.Value + 1).Append("{font-family:\"").Append(pair.Key.FontFamily)
                    .Append("\",\"").Append(pair.Key.BasicFontFamily).Append("\"; ");
                if (pair.Key.Bold)
                    sb.Append(" font-weight: bold;");
                if (pair.Key.Italic)
                    sb.Append(" font-style: italic;");
                sb.Append('}');
            }


            sb.Append(@"
    </style>");
        }

        public void SaveAsHtml(PdfDetails pdf, string path)
        {
            var content = ConvertPdf(pdf);
            File.WriteAllText(path, content);
        }

        public void SaveAsHtmlInDir(PdfDetails pdf, string dirTarget, string prefix = "")
        {
            var dir = new DirectoryInfo(dirTarget);
            if (!dir.Exists)
                dir.Create();
            var sb = new StringBuilder();
            sb.Append(@"<html>
    <head><title>pdf</title></head>
    <body>");
            foreach (var page in pdf.Pages)
            {
                sb.Append($@"
        <div><a href=""").Append(prefix).Append("page").Append(page.PageNumber).Append(".html\">page ")
                    .Append(page.PageNumber).Append("</a></div>");
            }

            sb.Append(@"
    </body>
</html>");
            File.WriteAllText(Path.Combine(dirTarget, prefix + "index.html"), sb.ToString());
            foreach (var page in pdf.Pages)
            {
                var content = ConvertPage(page);
                File.WriteAllText(Path.Combine(dirTarget, $"{prefix}page{page.PageNumber}.html"), content);
            }
        }
    }
}