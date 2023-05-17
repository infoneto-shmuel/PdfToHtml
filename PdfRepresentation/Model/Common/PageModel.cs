using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Model.Pdf;

namespace PdfRepresentation.Model.Common
{
    public class PageModel
    {
        private TableModel[] _tables;

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
            get => _tables;
            set
            {
                _tables = value;
                for (int i = 0; i < value?.Length; i++)
                {
                    _tables[i].Id = $"{PageNumber.ToString().PadLeft(PagesCount % 10 + 1, '0')}-{(i + 1).ToString().PadLeft(5, '0')}";
                }
            }
        }

        public int PagesCount { get; set; }

        public override string ToString()
        {
            return TablesList.Count.ToString();
        }
    }
}