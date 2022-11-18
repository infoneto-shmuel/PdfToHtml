using System.Collections.Generic;

namespace PdfRepresantation.Model.Pdf
{
    public class PageDetails
    {
        public List<TextLineDetails> Lines { get; set; }
        public List<ImageDetails> Images { get; set; }
        public List<ShapeDetails> Shapes { get; set; }
        public List<FontDetails> Fonts { get; set; }
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