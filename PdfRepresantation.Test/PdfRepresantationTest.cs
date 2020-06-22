using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace PdfRepresantation.Test
{
    //test the conversion
    //for those tests to run you need to put pdf files in the "File" direcory
    //and the result will be written in the "results" directory
    [TestClass]
    public class PdfRepresantationTest
    {
        private string sourceDir = "Files";
        private string targetDir = "Results";

        static PdfRepresantationTest()
        {
            if (Directory.GetCurrentDirectory().Contains("netcoreapp"))
                Directory.SetCurrentDirectory(Path.Combine("..", "..", ".."));
        }

        [TestMethod]
        public void ConvertToHtml()
        {
            var paths =new List<string>();
            var htmlWriter = new PdfHtmlWriter(new HtmlWriterConfig{UseCanvas = true});
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
//                if(name!="pdf-031")
//                    continue;
                var details = PdfDetailsFactory.Create(file.FullName);
                var target = Path.Combine(targetDir, name + ".html");
                paths.Add(target);
                htmlWriter.SaveAsHtml(details, target);
            }
            var json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("urls.js", $"urls={json};");

        }

  
        private PdfRepresantationClient client = new PdfRepresantationClient();

        //for this test you need to run the server first
        [TestMethod]
        public async Task ServerTestHtml()
        {
            foreach (var file in new DirectoryInfo(sourceDir).EnumerateFiles())
            {
                var name = Path.GetFileNameWithoutExtension(file.Name);
                //if(name!="pdf-001")
                //    continue;
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
                //if(name!="pdf-001")
                //    continue;
                var buffer = File.ReadAllBytes(file.FullName);
                var textResult = await client.ConvertToTextAsync(buffer);
                File.WriteAllText(Path.Combine(targetDir, name + ".txt"), textResult);
            }
        }
    }
}