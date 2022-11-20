using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PdfRepresantation;
using PdfRepresentation.Extensions;
using PdfRepresentation.Logging;
using PdfRepresentation.Test.Extensions;
using Xunit;

namespace PdfRepresentation.Test
{
    //test the conversion
    //for those tests to run you need to put pdf files in the "File" direcory
    //and the result will be written in the "results" directory
    public class PdfRepresantationTest
    {
        private readonly PdfRepresantationClient client = new PdfRepresantationClient();

        public PdfRepresantationTest()
        {
            if (Directory.GetCurrentDirectory().Contains("netcoreapp"))
            {
                Directory.SetCurrentDirectory(Path.Combine("..", "..", ".."));
            }

            Log.logger = new ConsoleLogger { DebugSupported = true, InfoSupported = true, ErrorSupported = true };
        }

        [Fact]
        public void ConvertToHtml()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToHtml(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [Fact]
        public void ConvertToJson()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToJson(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [Fact]
        public void ConvertToJsonTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToJsonTables(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [Fact]
        public void ConvertToXml()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToXml(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        [Fact]
        public void ConvertToXmlTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToXmlTables(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }

        //for this test you need to run the server first
        [Fact(Skip = "not used")]
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
        [Fact(Skip = "not used")]
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