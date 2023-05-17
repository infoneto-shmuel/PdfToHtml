using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PdfRepresentation.Html.Base;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Config;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Html
{
    public sealed class PdfHtmlWriter : PdfHtmlWriterBase
    {
        public PdfHtmlWriter(HtmlWriterConfig config = null)
        {
            if (config == null)
            {
                config = new HtmlWriterConfig();
            }

            if (config.DrawShapes)
            {
                ShapeWriter = CreateHelper.CreateShapeWriter(config);
            }

            TextWriter = CreateHelper.CreateTextWriter();
            ImageWriter = CreateHelper.CreateImageWriter(config);
        }

        public override string ConvertPdf(PdfDetails pdf)
        {
            return ConvertPdf(pdf, true);
        }

        public override string ConvertPage(PageDetails page)
        {
            return ConvertPage(page, true);
        }
            
        public string ConvertPage(PageDetails page, bool addHeader)
        {
            var stringBuilder = new StringBuilder();
            var fontRef = CreateFontRef(page.Fonts);
            var allLines = page.Lines;
            StartTag(stringBuilder, Title(page), page.RightToLeft, fontRef, allLines, "html");
            AddPage(page, fontRef, stringBuilder, addHeader);
            EndTag(stringBuilder, "html");
            return stringBuilder.ToString();
        }

        public string ConvertPdf(PdfDetails pdf, bool addHeader)
        {
            var stringBuilder = new StringBuilder();
            var fontRef = CreateFontRef(pdf.Fonts);
            var allLines = pdf.Pages.SelectMany(p => p.Lines);
            StartTag(stringBuilder, Title(pdf), null, fontRef, allLines, "html");
            foreach (var page in pdf.Pages)
            {
                AddPage(page, fontRef, stringBuilder, addHeader);
            }

            EndTag(stringBuilder, "html");
            return stringBuilder.ToString();
        }

        public void SaveAs(PdfDetails pdf, string path, bool addHeader)
        {
            var content = ConvertPdf(pdf, addHeader);
            File.WriteAllText(path, content);
        }

        private void AddHeader(PageDetails page, StringBuilder stringBuilder)
        {
            stringBuilder.Append(@"
    <h2 class=""header"" style=""width: ").Append(Math.Round(page.Width))
                .Append("px;\">Page ").Append(page.PageNumber).Append("</h2>");
        }

        private void AddPage(PageDetails page, Dictionary<FontDetails, int> fontRef, StringBuilder stringBuilder,
            bool addHeader)
        {
            if (addHeader)
            {
                AddHeader(page, stringBuilder);
            }

            stringBuilder.Append(@"
    <article class=""article"" dir=""").Append(page.RightToLeft ? "rtl" : "ltr")
                .Append("\" style=\"width: ").Append(Math.Round(page.Width))
                .Append("px;height:").Append(Math.Round(page.Height))
                .Append("px;\">");
            foreach (var pdfImage in page.Images)
            {
                ImageWriter.AddImage(page, pdfImage, stringBuilder);
            }

            foreach (var line in page.Lines)
            {
                TextWriter.AddLine(page, fontRef, line, stringBuilder);
            }

            stringBuilder.Append(@"
    </article>");
            AddShapes(page, stringBuilder);
        }

        private void AddShapes(PageDetails page, StringBuilder stringBuilder)
        {
            if (page.Shapes.Count == 0)
            {
                return;
            }

            ShapeWriter?.AddShapes(page, stringBuilder);
        }

        private static Dictionary<FontDetails, int> CreateFontRef(IEnumerable<FontDetails> fonts)
        {
            var fontRef = fonts
                .Select((f, i) => new { f, i })
                .ToDictionary(a => a.f, a => a.i);
            return fontRef;
        }

        private string Title(PdfDetails pdf)
        {
            return $"pdf converted to html with {pdf.Pages.Count} pages";
        }

        private string Title(PageDetails page)
        {
            return "pdf page " + page.PageNumber;
        }
    }
}