using System.Collections.Generic;
using System.IO;
using PdfRepresantation.Html;
using PdfRepresantation.Json;
using PdfRepresantation.Logic;
using PdfRepresantation.Model.Config;
using PdfRepresantation.Model.Pdf;
using PdfRepresantation.Xml;

namespace PdfRepresantation.Extensions
{
    public static class PdfWriterExtension
    {
        public static List<string> ConvertToHtml(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToHtml(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToHtml(this FileInfo file, string targetDirectory, ref List<string> paths)
        {
            var details = GetPdfDetails(file, ".html", targetDirectory, ref paths, out var target);
            var writer = new PdfHtmlWriter(new HtmlWriterConfig { UseCanvas = false });
            writer.SaveAs(details, target, false);
            return paths;
        }

        public static List<string> ConvertToJson(this FileInfo file, string targetDirectory)
        {
            List<string> paths = null;
            return ConvertToJson(file, targetDirectory, ref paths);
        }

        public static List<string> ConvertToJson(this FileInfo file, string targetDirectory, ref List<string> paths)
        {
            var details = file.GetPdfDetails(".json", targetDirectory, ref paths, out var target);
            var writer = new PdfJsonWriter();
            writer.SaveAs(details, target, true);
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
            var details = file.GetPdfDetails(".json", targetDirectory, ref paths, out var target);
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
            var details = file.GetPdfDetails(".xml", targetDirectory, ref paths, out var target);
            var writer = new PdfXmlWriter();
            writer.SaveAs(details, target);
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
            var details = file.GetPdfDetails(".xml", targetDirectory, ref paths, out var target);
            var writer = new PdfXmlWriter();
            writer.SaveAsXmlTables(details, target);
            return paths;
        }

        public static PdfDetails GetPdfDetails(this FileInfo file, string extension,
            string targetDirectory, ref List<string> paths, out string target)
        {
            paths ??= new List<string>();
            var name = Path.GetFileNameWithoutExtension(file.Name);
            var details = PdfDetailsFactory.Create(file.FullName);
            target = Path.Combine(targetDirectory, name + extension);
            paths.Add(target);
            return details;
        }
    }
}