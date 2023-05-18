using System.Xml.Serialization;
using CoreLibrary.Utility.Serialization;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Internals.Serialization;

namespace PdfRepresentation.Model.Xml
{
    [XmlRoot(ElementName = "TableModel")]
    public class Table : ToXmlJson
    {
        [XmlElement(ElementName = "Rows")] public Row[] Rows { get; set; }

        [XmlAttribute(AttributeName = "Id")] public string Id { get; set; }

        public bool ShouldSerializeId()
        {
            return !string.IsNullOrEmpty(Id);
        }

        public override string ToString()
        {
            return SerializerHelper.SerializeAsString(()=>ObjectSerializer.Instance.Serialize(TableSerializer<Table>.GetTableSerializer(this, Rows)));
        }

    }
}