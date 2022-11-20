using PdfRepresentation.Base.Conversion;
using PdfRepresentation.Xml;

namespace PdfRepresentation.Conversion
{
    public class PdfXmlConverter : PdfConverter
    {

        public PdfXmlConverter()
        {
            writer = new PdfXmlWriter();
        }
    }
}