using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PdfRepresentation.Test.Extensions
{
    public static class TestsExtension
    {
        public static void CreateSaveUrlsJson(this List<string> paths)
        {
            var json = JsonConvert.SerializeObject(paths, Formatting.Indented);
            File.WriteAllText("urls.json", $"urls={json};");
        }
    }
}
