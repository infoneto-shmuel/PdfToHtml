using System.Collections.Generic;
using System.Text;
using PdfRepresantation.Base;
using PdfRepresantation.Interfaces;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Html.Base
{
    public abstract class PdfHtmlWriterBase :  PdfWriterBase
    {
        protected PdfShapeHtmlWriter ShapeWriter { get; set; }
        protected PdfTextHtmlWriter TextWriter { get; set; }
        protected PdfImageHtmlWriter ImageWriter { get; set; }

        protected virtual void StartTag(StringBuilder stringBuilder, string title, bool? rightToLeft,
            Dictionary<FontDetails, int> fontRef,
            IEnumerable<TextLineDetails> allLines, string tagName)
        {
            stringBuilder.Append($@"<{tagName}>
<head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
    <title>").Append(title).Append(@"</title>");
            AddStyle(fontRef, allLines, stringBuilder);
            ShapeWriter.AddScript(stringBuilder);
            stringBuilder.Append(@"    
    <script>
        function init() {");
            TextWriter.AddScriptInit(stringBuilder);
            stringBuilder.Append(@"
        }
    </script>");
            stringBuilder.Append($@"
</head>
<body onload=""init()""");
            if (rightToLeft.HasValue)
                stringBuilder.Append(@" dir=""").Append(rightToLeft.Value ? "rtl" : "ltr").Append("\"");
            stringBuilder.Append(">");
        }

        protected virtual void EndTag(StringBuilder stringBuilder, string tagName)
        {
            stringBuilder.Append($@"
</body>
</{tagName}>");
        }

        private void AddStyle(Dictionary<FontDetails, int> fontRef,
            IEnumerable<TextLineDetails> allLines,
            StringBuilder sb)
        {
            sb.Append(@"
    <style>");
            TextWriter.AddTextStyle(sb);
            ImageWriter.AddStyle(sb);
            ShapeWriter.AddStyle(sb);
            AddGlobalStyle(sb);
            TextWriter.AddFontStyle(fontRef, allLines, sb);
            sb.Append(@"
    </style>");
        }

        private static void AddGlobalStyle(StringBuilder sb)
        {
            sb.Append(@"
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
        }
    }
}