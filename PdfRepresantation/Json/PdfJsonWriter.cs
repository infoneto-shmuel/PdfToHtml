using PdfRepresantation.Base;
using PdfRepresantation.Model.Config;
using PdfRepresantation.Model.Pdf;
using PdfRepresantation.Serialization;

namespace PdfRepresantation.Json
{
    public class PdfJsonWriter : PdfXmlJsonWriterBase
    {
        public PdfJsonWriter(XmlJsonWriterConfig config = null) : base(config)
        {
        }

        public override string ConvertPage(PageDetails page)
        {
            return GetPageValue(page).SerializeAsJson();
        }

        public override string ConvertPdf(PdfDetails pdf)
        {
            return GetValue(pdf).SerializeAsJson();
        }
    }
}