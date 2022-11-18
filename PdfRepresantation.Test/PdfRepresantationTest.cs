using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PdfRepresantation.Html;
using PdfRepresantation.Json;
using PdfRepresantation.Log;
using PdfRepresantation.Logic;
using PdfRepresantation.Model.Config;
using PdfRepresantation.Xml;

namespace PdfRepresantation.Test
{
    //test the conversion
    //for those tests to run you need to put pdf files in the "File" direcory
    //and the result will be written in the "results" directory
    [TestClass]
    public class PdfRepresantationTest
    {
        private readonly PdfRepresantationClient client = new PdfRepresantationClient();
        private readonly string sourceDir = "Files";
        private readonly string targetDir = "Results";

        [TestMethod]
        public void ConvertToHtml()
        {
            var paths = new List<string>();
            var htmlWriter = new PdfHtmlWriter(new HtmlWriterConfig { UseCanvas = false });
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var details = PdfDetailsFactory.Create(file.FullName);
                var target = Path.Combine(targetDir, name + ".html");
                paths.Add(target);
                htmlWriter.SaveAs(details, target, false);
            }

            var json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("urls.js", $"urls={json};");
        }

        [TestMethod]
        public void ConvertToJson()
        {
            var paths = new List<string>();
            var htmlWriter = new PdfJsonWriter();
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var details = PdfDetailsFactory.Create(file.FullName);
                var target = Path.Combine(targetDir, name + ".json");
                paths.Add(target);
                htmlWriter.SaveAs(details, target);
            }

            var json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("urls.js", $"urls={json};");
        }

        [TestMethod]
        public void ConvertToXml()
        {
            var paths = new List<string>();
            var htmlWriter = new PdfXmlWriter();
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var details = PdfDetailsFactory.Create(file.FullName);
                var target = Path.Combine(targetDir, name + ".xml");
                paths.Add(target);
                htmlWriter.SaveAs(details, target);
            }

            var json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("urls.js", $"urls={json};");
        }

        [AssemblyInitialize]
        public static void Init(TestContext context)
        {
            if (Directory.GetCurrentDirectory().Contains("netcoreapp"))
            {
                Directory.SetCurrentDirectory(Path.Combine("..", "..", ".."));
            }

            Log.Log.logger = new ConsoleLogger { DebugSupported = true, InfoSupported = true, ErrorSupported = true };
        }

        //for this test you need to run the server first
        [TestMethod]
        public async Task ServerTestHtml()
        {
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var buffer = File.ReadAllBytes(file.FullName);
                var htmlResult = await client.ConvertToHtmlAsync(buffer);
                File.WriteAllText(Path.Combine(targetDir, name + ".html"), htmlResult);
            }
        }

        //for this test you need to run the server first
        [TestMethod]
        public async Task ServerTestText()
        {
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var buffer = await File.ReadAllBytesAsync(file.FullName);
                var textResult = await client.ConvertToTextAsync(buffer);
                await File.WriteAllTextAsync(Path.Combine(targetDir, name + ".txt"), textResult);
            }
        }
    }
}