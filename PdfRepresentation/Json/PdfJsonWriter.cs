using System.IO;
using PdfRepresentation.Base;
using PdfRepresentation.Model.Config;
using PdfRepresentation.Model.Pdf;
using PdfRepresentation.Serialization;

namespace PdfRepresentation.Json
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

        public void SaveAsJsonTables(PdfDetails pdf, string path)
        {
            var content = ToTables(pdf).SerializeAsJson();
            File.WriteAllText(path, content);
        }

        public string SaveAsJson(PdfDetails pdf)
        {
            return ToTables(pdf).SerializeAsJson();
        }

    }
}