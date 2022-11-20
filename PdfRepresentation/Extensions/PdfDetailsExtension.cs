using System.Xml.Linq;
using PdfRepresentation.Model.Pdf;
using PdfRepresentation.Xml;

namespace PdfRepresentation.Extensions
{
    public static class PdfDetailsExtension
    {
        public static string GetTablesModelXml(this PdfDetails details)
        {
            var writer = new PdfXmlWriter();
            var xml = writer.SaveAsXml(details);
            var document = XDocument.Parse(xml);
            if (document.Root != null)
            {
                document.Root.Name = "Tables";
                foreach (var element in document.Root.Elements())
                {
                    if (element.Name == "TableModel")
                    {
                        element.Name = "Table";
                    }
                }
            }

            return document.ToString();
        }
    }
}
