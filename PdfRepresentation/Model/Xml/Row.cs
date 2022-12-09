using System;
using System.Linq;
using System.Xml.Serialization;
using CoreLibrary.Utility.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Extensions;
using PdfRepresentation.Internals.Serialization;

namespace PdfRepresentation.Model.Xml
{
    [XmlRoot(ElementName = "Rows")]
    public class Row : ToXmlJson
    {
        [XmlElement(ElementName = "Cell")] public Cell[] Cells { get; set; }

        [XmlAttribute(AttributeName = "TableId")]
        public string TableId { get; set; }

        public bool ShouldSerializeTable()
        {
            return !string.IsNullOrEmpty(TableId);
        }

        public int Page
        {
            get
            {
                int page = -1;
                if (TableId != null)
                {
                    var indexOf = TableId.IndexOf("-", StringComparison.InvariantCultureIgnoreCase);
                    if (indexOf >= 0)
                    {
                        int.TryParse(TableId.Substring(0, indexOf), out page);
                    }
                }

                return page;
            }
        }

        public bool ShouldSerializePage()
        {
            return false;
        }

        public int TableNumber
        {
            get
            {
                int page = -1;
                if (TableId != null)
                {
                    var indexOf = TableId.IndexOf("-", StringComparison.InvariantCultureIgnoreCase);
                    if (indexOf >= 0)
                    {
                        int.TryParse(TableId.Substring(indexOf + 1), out page);
                    }
                }

                return page;
            }
        }

        public bool ShouldSerializeTableNumber()
        {
            return false;
        }

        public double Top
        {
            get { return Cells.Min(c => c.GetTop()); }
        }

        public bool ShouldSerializeTop()
        {
            return XmlDimensionsSerialization.ShouldSerializeDimensions;
        }

        public double Left
        {
            get { return Cells.Min(c => c.GetLeft()); }
        }

        public bool ShouldSerializeLeft()
        {
            return XmlDimensionsSerialization.ShouldSerializeDimensions;
        }

        [XmlIgnore]
        [JsonIgnore]
        public double Right
        {
            get { return Cells.Max(c => c.GetBottom()); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public double Bottom
        {
            get { return Cells.Max(c => c.GetBottom()); }
        }

        [XmlIgnore]
        [JsonIgnore]
        public double Width
        {
            get { return Right - Left; }
        }

        [XmlIgnore]
        [JsonIgnore]
        public double Height
        {
            get { return Bottom - Top; }
        }

        public override string ToString()
        {
            return ObjectSerializer.Instance.Serialize(this);
        }
    }
}