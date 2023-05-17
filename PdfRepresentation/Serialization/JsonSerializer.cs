using System;
using Newtonsoft.Json;

namespace PdfRepresentation.Serialization
{
    public static class JsonSerializer
    {
        public static string SerializeAsJson<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                return JsonConvert.SerializeObject(value,
                    Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}
