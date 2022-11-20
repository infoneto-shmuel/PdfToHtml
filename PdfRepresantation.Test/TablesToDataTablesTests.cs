using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PdfRepresantation.Extensions;
using PdfRepresantation.Test.Extensions;

namespace PdfRepresantation.Test
{
    [TestClass]
    public class TablesToDataTablesTests
    {
        [TestMethod]
        public void ConvertToDataTablesTest()
        {
            List<string> paths = null;
            var directoryInfo = new DirectoryInfo(TestsConstants.PdfDir);
            if (directoryInfo.Exists)
            {
                foreach (var file in directoryInfo.EnumerateFiles())
                {
                    paths = file.ConvertToDataTables(ref paths);
                }
            }

            paths.CreateSaveUrlsJson();
        }
    }
}
