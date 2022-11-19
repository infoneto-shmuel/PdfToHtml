using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Logic
{
    public class TextParser 
    {
        public readonly List<PdfTextBlock> texts = new List<PdfTextBlock>();
        public readonly Dictionary<PdfFont, FontDetails> fonts = new Dictionary<PdfFont, FontDetails>();
        private readonly PageContext pageContext;

        internal TextParser(PageContext pageContext)
        {
            this.pageContext = pageContext;
        }

        public virtual void ParseText(TextRenderInfo textRenderInfo)
        {
            var text = textRenderInfo.GetText();
            LineSegment baseline = textRenderInfo.GetBaseline();
            if (textRenderInfo.GetRise() != 0)
            {
                Matrix m = new Matrix(0.0f, -textRenderInfo.GetRise());
                baseline = baseline.TransformBy(m);
            }
            var start = baseline.GetStartPoint();
            LineSegment ascentLine = textRenderInfo.GetAscentLine();
            PdfTextBlock item = new PdfTextBlock
            {
                Value = text,
                Bottom = pageContext.PageHeight - start.Get(Vector.I2),
                Top = pageContext.PageHeight - ascentLine.GetStartPoint().Get(Vector.I2),
                Left = start.Get(Vector.I1),
                Width = baseline.GetEndPoint().Get(Vector.I1) - start.Get(Vector.I1),
                FontSize = FontManager.Instance.GetFontSize(textRenderInfo, baseline, ascentLine),
                StrokeColore = ColorManager.Instance.GetColor(textRenderInfo),
                CharSpacing = textRenderInfo.GetSingleSpaceWidth(),
                Font = GetFont(textRenderInfo),
            };
            RightToLeftManager.Instance.AssignRtl(item,textRenderInfo.GetUnscaledWidth()<0);
            pageContext.LinkManager.AssignLink(item);
            texts.Add(item);
        }
        private FontDetails GetFont(TextRenderInfo textRenderInfo)
        {
            var pdfFont = textRenderInfo.GetFont();
            if (!fonts.TryGetValue(pdfFont, out var font))
            {
                font = FontManager.Instance.CreateFont(pdfFont);
                fonts.Add(pdfFont, font);
            }

            return font;
        }
   
    }
}