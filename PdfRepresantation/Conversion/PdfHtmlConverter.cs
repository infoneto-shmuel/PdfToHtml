using PdfRepresantation.Base.Conversion;
using PdfRepresantation.Html;

namespace PdfRepresantation.Conversion
{
    public class PdfHtmlConverter : PdfConverter
    {

        public PdfHtmlConverter()
        {
            writer = new PdfHtmlWriter();
        }
    }
}