using System.IO;
using PdfRepresantation.Base;
using PdfRepresantation.Model.Config;
using PdfRepresantation.Model.Pdf;
using PdfRepresantation.Serialization;

namespace PdfRepresantation.Xml
{
    public class PdfXmlWriter : PdfXmlJsonWriterBase
    {
        public PdfXmlWriter(XmlJsonWriterConfig config = null) : base(config)
        {
        }

        public override string ConvertPage(PageDetails page)
        {
            return GetPageValue(page).SerializeAsXml();
        }

        public override string ConvertPdf(PdfDetails pdf)
        {
            return GetValue(pdf).SerializeAsXml();
        }

        public void SaveAsXmlTables(PdfDetails pdf, string path, bool shouldSerializeAll = false)
        {
            var content = ToTables(pdf).SerializeAsXml();
            File.WriteAllText(path, content);
        }

    }
}