using System.Collections.Generic;

namespace PdfRepresantation
{
    public class PdfTextLineDetails : PdfDetailsItem
    {
        public IList<PdfTextResult> Texts { get; set; } = new List<PdfTextResult>();

        public string InnerText => string.Join("", Texts);
        public override string ToString() => InnerText + "\r\n";
    }
}