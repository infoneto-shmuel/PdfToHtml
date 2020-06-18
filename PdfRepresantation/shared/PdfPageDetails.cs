using System.Collections.Generic;

namespace PdfRepresantation
{
    public class PdfPageDetails
    {
        public IList<PdfTextLineDetails> Lines { get; set; }
        public IList<PdfImageDetails> Images { get; set; }
        public IList<ShapeDetails> Shapes { get; set; }
        public IList<PdfFontDetails> Fonts { get; set; }
        public int PageNumber { get; set; }
        public bool RightToLeft { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public override string ToString() =>
            "<PAGE " + PageNumber +
            ">\r\n---------------------------------------------------------------------------\r\n" +
            string.Join("", Lines) + "\r\n";
    }
}