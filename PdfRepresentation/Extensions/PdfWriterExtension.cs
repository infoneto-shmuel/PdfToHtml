using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using PdfRepresentation.Html;
using PdfRepresentation.Json;
using PdfRepresentation.Logic;
using PdfRepresentation.Model.Config;
using PdfRepresentation.Model.Pdf;
using PdfRepresentation.Model.Xml;
using PdfRepresentation.Xml;

namespace PdfRepresentation.Extensions
{
    public static class PdfWriterExtension
    {
        private const string Html = ".html";
        private const string Json = ".json";
        private const string Xml = ".xml";

        public static TablesModel ConvertToTableModels(this FileInfo file)
        {
            List<string> paths = null;
            return ConvertToTableModels(file, ref paths, out _);
        }

        public static TablesModel ConvertToTableModels(this FileInfo file, out string xml)
        {
            List<string> paths = null;
            return ConvertToTableModels(file, ref paths, out xml);
        }

        public static TablesModel ConvertToTableModels(this FileInfo file,
            ref List<string> paths, out string xml)
        {
            xml = file.GetPdfDetails(Xml, null, ref paths, out _).GetTablesModelXml();

            var ser = new XmlSerializer(typeof(TablesModel));
            TablesModel tablesModel;
            using var stringReader = new StringReader(xml);
            using (var reader = XmlReader.Create(stringReader))
            {
                tablesModel = (TablesModel)ser.Deserialize(reader);
            }

            paths.Add(xml);
            return tablesModel;
        }

        public static List<string> ConvertToHtml(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToHtml(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToHtml(this FileInfo file, string targetDirectory, ref List<string> paths)
        {
            var details = GetPdfDetails(file, Html, targetDirectory, ref paths, out var target);
            var writer = new PdfHtmlWriter(new HtmlWriterConfig { UseCanvas = false });
            writer.SaveAs(details, target, false);
            return paths;
        }

        public static List<string> ConvertToHtmlTables(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToHtmlTables(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToHtmlTables(this FileInfo file, string targetDirectory,
            ref List<string> paths)
        {
            var details = file.GetPdfDetails(Html, targetDirectory, ref paths, out var target);
            var writer = new PdfXmlWriter();
            var tableModels = writer.ToTables(details);

            var doc = new XDocument();
            doc.AddFirst(new XDocumentType("html", null, null, null));
            var body = new XElement("body");
            foreach (var tableModel in tableModels)
            {
                var table = new XElement("table", new XAttribute("style", "width:100%"));
                body.Add(table);
                foreach (var tableRow in tableModel.Rows)
                {
                    var row = new XElement("tr");
                    table.Add(row);
                    foreach (var tableRowCell in tableRow.Cells)
                    {
                        var cell = new XElement("td", tableRowCell.InnerText);
                        row.Add(cell);
                    }
                }
            }

            doc.Add(new XElement("html", body));

            using var xmlWriter = new XmlTextWriter(target, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            doc.WriteTo(xmlWriter);
            xmlWriter.Flush();
            xmlWriter.Close();

            return paths;
        }

        public static List<string> ConvertToJson(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToJson(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToJson(this FileInfo file, string targetDirectory, ref List<string> paths)
        {
            var details = file.GetPdfDetails(Json, targetDirectory, ref paths, out var target);
            var writer = new PdfJsonWriter();
            writer.SaveAs(details, target, true);
            return paths;
        }

        public static List<string> ConvertToJsonContents(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToJsonContents(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToJsonContents(this FileInfo file, string targetDirectory,
            ref List<string> paths)
        {
            var details = file.GetPdfDetails(Xml, targetDirectory, ref paths, out _);
            var writer = new PdfJsonWriter();

            paths.Add(writer.SaveAsJson(details));
            return paths;
        }

        public static List<string> ConvertToJsonTables(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToJsonTables(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToJsonTables(this FileInfo file, string targetDirectory,
            ref List<string> paths)
        {
            var writer = new PdfJsonWriter();
            var details = file.GetPdfDetails(Json, targetDirectory, ref paths, out var target);
            writer.SaveAsJsonTables(details, target);
            return paths;
        }

        public static List<string> ConvertToXml(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToXml(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToXml(this FileInfo file, string targetDirectory, ref List<string> paths)
        {
            var details = file.GetPdfDetails(Xml, targetDirectory, ref paths, out var target);
            var writer = new PdfXmlWriter();
            writer.SaveAs(details, target);
            return paths;
        }

        public static List<string> ConvertToXmlContents(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToXmlContents(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToXmlContents(this FileInfo file, string targetDirectory,
            ref List<string> paths)
        {
            var details = file.GetPdfDetails(Xml, targetDirectory, ref paths, out _);
            var writer = new PdfXmlWriter();

            paths.Add(writer.SaveAsXml(details));
            return paths;
        }

        public static List<string> ConvertToXmlTables(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToXmlTables(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToXmlTables(this FileInfo file, string targetDirectory,
            ref List<string> paths)
        {
            var details = file.GetPdfDetails(Xml, targetDirectory, ref paths, out var target);
            var writer = new PdfXmlWriter();
            writer.SaveAsXmlTables(details, target);
            return paths;
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