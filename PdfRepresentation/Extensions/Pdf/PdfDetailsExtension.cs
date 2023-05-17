using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using PdfRepresentation.Logic;
using PdfRepresentation.Model.Pdf;
using PdfRepresentation.Xml;

namespace PdfRepresentation.Extensions.Pdf
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

        public static PdfDetails GetPdfDetails(this FileInfo file, string extension,
            string targetDirectory, ref List<string> paths, out string target)
        {
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            paths ??= new List<string>();
            var name = Path.GetFileNameWithoutExtension(file.Name);
            var details = PdfDetailsFactory.Create(file.FullName);
            if (!string.IsNullOrEmpty(targetDirectory))
            {
                target = Path.Combine(targetDirectory, name + extension);
                paths.Add(target);
            }
            else
            {
                target = null;
            }

            return details;
        }
    }
}
