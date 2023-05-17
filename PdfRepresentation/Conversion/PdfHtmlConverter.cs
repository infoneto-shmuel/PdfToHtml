using PdfRepresentation.Base.Conversion;
using PdfRepresentation.Html;

namespace PdfRepresentation.Conversion
{
    public class PdfHtmlConverter : PdfConverter
    {

        public PdfHtmlConverter()
        {
            writer = new PdfHtmlWriter();
        }
    }
}