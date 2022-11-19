using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PdfRepresantation.Extensions;
using PdfRepresantation.Log;

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
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                paths = file.ConvertToHtml(targetDir, ref paths);
            }

            CreateSaveUrlsJson(paths);
        }

        [TestMethod]
        public void ConvertToJson()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                paths = file.ConvertToJson(targetDir, ref paths);
            }

            CreateSaveUrlsJson(paths);
        }

        [TestMethod]
        public void ConvertToJsonTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                paths = file.ConvertToJsonTables(targetDir, ref paths);
            }

            CreateSaveUrlsJson(paths);
        }

        [TestMethod]
        public void ConvertToXml()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                paths = file.ConvertToXml(targetDir, ref paths);
            }

            CreateSaveUrlsJson(paths);
        }

        [TestMethod]
        public void ConvertToXmlTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                paths = file.ConvertToXmlTables(targetDir, ref paths);
            }

            CreateSaveUrlsJson(paths);
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

        private static void CreateSaveUrlsJson(List<string> paths)
        {
            var json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("urls.js", $"urls={json};");
        }
    }
}