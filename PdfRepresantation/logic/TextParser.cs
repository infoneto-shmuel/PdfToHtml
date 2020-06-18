using System.Collections.Generic;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas.Parser.Data;

namespace PdfRepresantation
{
    public class TextParser 
    {
        protected readonly FontManager fontManager = new FontManager();
        public readonly IList<PdfTextBlock> texts = new List<PdfTextBlock>();
        public readonly Dictionary<PdfFont, PdfFontDetails> fonts = new Dictionary<PdfFont, PdfFontDetails>();
        private readonly LinkManager linkManager;
        private readonly float pageHeight;
        private readonly float pageWidth;

        internal TextParser(float pageHeight, float pageWidth, LinkManager linkManager)
        {
            this.pageHeight = pageHeight;
            this.pageWidth = pageWidth;
            this.linkManager = linkManager;
        }

        public virtual void ParseText(TextRenderInfo textRenderInfo)
        {
            var text = textRenderInfo.GetText();
            LineSegment baseline = textRenderInfo.GetBaseline();
            var start = baseline.GetStartPoint();
            LineSegment ascentLine = textRenderInfo.GetAscentLine();
            PdfTextBlock item = new PdfTextBlock
            {
                Value = text,
                Bottom = pageHeight - start.Get(1),
                Top = pageHeight - ascentLine.GetStartPoint().Get(1),
                Left = start.Get(0),
                Width = baseline.GetEndPoint().Get(0) - start.Get(0),
                FontSize = fontManager.GetFontSize(textRenderInfo, baseline, ascentLine),
                StrokeColore = ColorManager.Instance.GetColor(textRenderInfo.GetStrokeColor()),
                CharSpacing = textRenderInfo.GetSingleSpaceWidth(),
                Font = GetFont(textRenderInfo),
            };
            RightToLeftManager.Instance.AssignRtl(item);
            linkManager.AssignLink(item);
            texts.Add(item);
        }
        private PdfFontDetails GetFont(TextRenderInfo textRenderInfo)
        {
            var pdfFont = textRenderInfo.GetFont();
            if (!fonts.TryGetValue(pdfFont, out var font))
            {
                font = fontManager.CreateFont(pdfFont);
                fonts.Add(pdfFont, font);
            }

            return font;
        }
   
    }
}