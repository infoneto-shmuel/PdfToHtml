using System.Xml.Serialization;
using CoreLibrary.Utility.Serialization;
using PdfRepresentation.Internals.Serialization;

namespace PdfRepresentation.Model.Xml
{
    [XmlRoot(ElementName = "Cell")]
    public partial class Cell : ToXmlJson
    {
        [XmlAttribute(AttributeName = "Left")] 
        public string Left { get; set; }

        public bool ShouldSerializeLeft()
        {
            return XmlDimensionsSerialization.ShouldSerializeDimensions;
        }

        [XmlAttribute(AttributeName = "Bottom")]
        public string Bottom { get; set; }

        [XmlAttribute(AttributeName = "Top")] 
        public string Top { get; set; }

        public bool ShouldSerializeTop()
        {
            return XmlDimensionsSerialization.ShouldSerializeDimensions;
        }

        [XmlAttribute(AttributeName = "Width")]
        public string Width { get; set; }

        public bool ShouldSerializeWidth()
        {
            return XmlDimensionsSerialization.ShouldSerializeDimensions;
        }

        [XmlText] public string Text { get; set; } = string.Empty;

        public override string ToString()
        {
            return ObjectSerializer.Instance.Serialize(this);
        }
    }
}