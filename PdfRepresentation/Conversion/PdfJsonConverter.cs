using PdfRepresentation.Base.Conversion;
using PdfRepresentation.Json;

namespace PdfRepresentation.Conversion
{
    public class PdfJsonConverter : PdfConverter
    {

        public PdfJsonConverter()
        {
            writer = new PdfJsonWriter();
        }
    }
}