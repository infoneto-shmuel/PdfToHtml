using PdfRepresantation.Base.Conversion;
using PdfRepresantation.Xml;

namespace PdfRepresantation.Conversion
{
    public class PdfXmlConverter : PdfConverter
    {

        public PdfXmlConverter()
        {
            writer = new PdfXmlWriter();
        }
    }
}