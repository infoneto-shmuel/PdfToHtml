using System.IO;
using PdfRepresentation.Base;
using PdfRepresentation.Model.Config;
using PdfRepresentation.Model.Pdf;
using PdfRepresentation.Serialization;

namespace PdfRepresentation.Xml
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

        public void SaveAsXmlTables(PdfDetails pdf, string path)
        {
            var content = SaveAsXml(pdf);
            File.WriteAllText(path, content);
        }

        public string SaveAsXml(PdfDetails pdf)
        {
            return ToTables(pdf).SerializeAsXml();
        }
    }

}
