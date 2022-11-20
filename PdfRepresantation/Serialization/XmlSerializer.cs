using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PdfRepresantation.Serialization
{
    public static class XmlSerializerExtension
    {
        public static string SerializeAsXml<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = new XmlTextWriter(stringWriter))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;
                    xmlserializer.Serialize(writer, value);
                }
                return stringWriter.GetStringBuilder().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }
    }
}