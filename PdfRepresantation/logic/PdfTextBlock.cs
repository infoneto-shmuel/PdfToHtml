using System.Drawing;

namespace PdfRepresantation
{
    public class PdfTextBlock
    {
        public bool? IsRightToLeft { get; set; }
        public float Left { get; set; }
        public float Width { get; set; }
        public float Start { get; set; }
        public float End { get; set; }
        public float Right => Left + Width;
        public float Bottom { get; set; }
        public float Top { get; set; }
        public string Value { get; set; }
        public float FontSize { get; set; }
        public PdfFontDetails Font { get; set; }
        public Color? StrokeColore { get; set; }
        public float CharSpacing { get; set; }
        public string Link { get; set; }
//        public bool IsDigit { get; set; }

        public override string ToString() => Value;
    }
}