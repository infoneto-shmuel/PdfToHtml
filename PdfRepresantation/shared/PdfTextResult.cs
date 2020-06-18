using System.Collections.Generic;
using System.Drawing;

namespace PdfRepresantation
{
    public class PdfTextResult
    {
        public string Value { get; set; }
        public float FontSize { get; set; }
        public PdfFontDetails Font { get; set; }
        public PdfLinkResult LinkParent { get; set; }
        public Color? StrokeColore { get; set; }
        public override string ToString() => Value;
    }
    public class PdfLinkResult:PdfTextResult
    {
        public IList<PdfTextResult> Children { get; set; }=new List<PdfTextResult>();
        public string Link { get; set; }
        public override string ToString() => string.Join("",Children)+"=>"+Link;
    }
}