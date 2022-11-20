using System.Collections.Generic;
using System.IO;
using PdfRepresentation.Extensions;
using PdfRepresentation.Test.Extensions;
using Xunit;

namespace PdfRepresentation.Test
{
    public class TablesToHtmlTests
    {
        [Fact]
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
