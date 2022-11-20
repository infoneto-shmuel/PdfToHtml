using PdfRepresantation.Base.Conversion;
using PdfRepresantation.Json;

namespace PdfRepresantation.Conversion
{
    public class PdfJsonConverter : PdfConverter
    {

        public PdfJsonConverter()
        {
            writer = new PdfJsonWriter();
        }
    }
}