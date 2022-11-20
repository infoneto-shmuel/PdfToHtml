using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfRepresantation.Extensions;
using PdfRepresantation.Test.Extensions;

namespace PdfRepresantation.Test
{
    [TestClass]
    public class TablesToHtmlTests
    {
        [TestMethod]
        public void ConvertToHtmlTables()
        {
            var paths = new List<string>();
            foreach (var file in new DirectoryInfo(TestsConstants.SourceDir).EnumerateFiles())
            {
                paths = file.ConvertToHtmlTables(TestsConstants.TargetDir, ref paths);
            }

            paths.CreateSaveUrlsJson();
        }
    }
}
