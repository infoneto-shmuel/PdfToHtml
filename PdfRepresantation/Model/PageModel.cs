using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Model
{
    public class PageModel
    {
        private TableModel[] tables;

        [XmlElement("Font")]
        public List<FontDetails> Fonts { get; set; }

        public bool ShouldSerializeFonts()
        {
            return PdfModel.ShouldSerializeAll;
        }

        public List<ImageDetails> Images { get; set; }

        public bool ShouldSerializeImages()
        {
            return PdfModel.ShouldSerializeAll;
        }

        public List<ShapeDetails> Shapes { get; set; }

        public bool ShouldSerializeShapes()
        {
            return PdfModel.ShouldSerializeAll;
        }

        [XmlAttribute]
        public int PageNumber { get; set; }

        [XmlAttribute]
        public bool RightToLeft { get; set; }

        public bool ShouldSerializeRightToLeft()
        {
            return PdfModel.ShouldSerializeAll;
        }

        [XmlAttribute]
        public float Width { get; set; }

        public bool ShouldSerializeWidth()
        {
            return PdfModel.ShouldSerializeAll;
        }

        [XmlAttribute]
        public float Height { get; set; }

        public bool ShouldSerializeHeight()
        {
            return PdfModel.ShouldSerializeAll;
        }

        [JsonIgnore]
        [XmlIgnore]
        internal List<List<List<TextLineDetails>>> TablesList { get; set; } = new List<List<List<TextLineDetails>>>();

        [XmlElement("Table")]
        public TableModel[] Tables
        {
            get => tables;
            set
            {
                tables = value;
                for (int i = 0; i < value?.Length; i++)
                {
                    tables[i].Id = $"{PageNumber}-{i + 1}";
                }
            }
        }

        public override string ToString()
        {
            return TablesList.Count.ToString();
        }
    }
}