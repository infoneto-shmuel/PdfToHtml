using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PdfRepresantation.Extensions;
using PdfRepresantation.Log;
using PdfRepresantation.Test.Extensions;

namespace PdfRepresantation.Test
{
    //test the conversion
    //for those tests to run you need to put pdf files in the "File" direcory
    //and the result will be written in the "results" directory
    [TestClass]
    public class PdfRepresantationTest
    {
        private readonly PdfRepresantationClient client = new PdfRepresantationClient();

        [TestMethod]
        public void ConvertToHtml()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToHtml(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [TestMethod]
        public void ConvertToJson()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToJson(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [TestMethod]
        public void ConvertToJsonTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToJsonTables(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [TestMethod]
        public void ConvertToXml()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToXml(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [TestMethod]
        public void ConvertToXmlTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToXmlTables(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
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
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var buffer = File.ReadAllBytes(file.FullName);
                var htmlResult = await client.ConvertToHtmlAsync(buffer);
                File.WriteAllText(Path.Combine(TestsConstants.TargetDir, name + ".html"), htmlResult);
            }
        }

        //for this test you need to run the server first
        [TestMethod]
        public async Task ServerTestText()
        {
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                var buffer = await File.ReadAllBytesAsync(file.FullName);
                var textResult = await client.ConvertToTextAsync(buffer);
                await File.WriteAllTextAsync(Path.Combine(TestsConstants.TargetDir, name + ".txt"), textResult);
            }
        }
    }
}