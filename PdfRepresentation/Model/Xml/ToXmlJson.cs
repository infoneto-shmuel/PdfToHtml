using CoreLibrary.Model.Serialization;
using CoreLibrary.Utility.Serialization;

namespace PdfRepresentation.Model.Xml
{
    public class ToXmlJson
    {
        public string ToXml()
        {
            return ObjectSerializer.Instance.Serialize(this, SerializationType.Xml);
        }

        public string ToJson()
        {
            return ObjectSerializer.Instance.Serialize(this, SerializationType.Json);
        }
    }
}