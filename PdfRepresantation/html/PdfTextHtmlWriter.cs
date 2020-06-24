using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace PdfRepresantation
{
    public class PdfTextHtmlWriter
    {
        NumberFormatInfo formatNumInClassName = new NumberFormatInfo {NumberDecimalSeparator = "-"};

        public virtual void AddFontStyle(Dictionary<PdfFontDetails, int> fontRef,
            IEnumerable<PdfTextLineDetails> allLines,
            StringBuilder sb)
        {
            foreach (var size in allLines.SelectMany(l => l.Texts).Select(t => Math.Round(t.FontSize * 2)).Distinct())
            {
                sb.Append(@"
        .font-size-").Append((size / 2).ToString(formatNumInClassName))
                    .Append("{font-size:").Append(size / 2).Append("px;}");
            }

            foreach (var pair in fontRef)
            {

                sb.Append(@"
        .bold{font-weight: bold;}");
                sb.Append(@"
        .font").Append(pair.Value + 1).Append("{font-family:\"").Append(pair.Key.FontFamily)
                    .Append("\",\"").Append(pair.Key.BasicFontFamily).Append("\"; ");
                if (pair.Key.Bold)
                    sb.Append(" font-weight: bold;");
                if (pair.Key.Italic)
                    sb.Append(" font-style: italic;");
                sb.Append('}');
            }
        }


        public virtual void AddLine(PdfPageDetails page, Dictionary<PdfFontDetails, int> fontRef,
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

        protected virtual void AddText(PdfTextResult text,
            Dictionary<PdfFontDetails, int> fontRef, StringBuilder sb)
        {
            var b = text.StrokeColore?.GetBrightness();
            if (b > 0.9)
            {
                sb.Append($@"<span class=""dark-back");
                AddFontClass(text, fontRef, sb);
                sb.Append(@""">");
                AddText(text.Value, sb);
                sb.Append(@"</span>");
            }

            sb.Append($@"<span class=""baseline");
            AddFontClass(text, fontRef, sb);
            sb.Append("\" style=\"");
            AddColor(text, sb);
            sb.Append(@""">");
            AddText(text.Value, sb);
            sb.Append(@"</span>");
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

        protected void AddFontClass(PdfTextResult text,
            Dictionary<PdfFontDetails, int> fontRef, StringBuilder sb)
        {
            sb.Append($@" font").Append(fontRef[text.Font] + 1);
            if(text.Font.Bold)
                sb.Append($@" bold");
            sb.Append(" font-size-")
                .Append((Math.Round(text.FontSize * 2) / 2).ToString(formatNumInClassName));
        }

        protected virtual void AddText(string text, StringBuilder sb)
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
            PdfHtmlWriter.AppendColor(text.StrokeColore.Value, sb);
            sb.Append(";");
        }

        public virtual void AddTextStyle(StringBuilder sb)
        {
            sb.Append(@"
        .line{
            position:absolute;
            min-width:fit-content;
        }
        .baseline{vertical-align:baseline;}
        .dark-back{
            background-color:#dedede;
            position:absolute;
            z-index:-1;
            color:transparent;
        }");
        }
    }
}