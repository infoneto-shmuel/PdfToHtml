using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Model
{
    public class PageModel
    {
        [XmlElement("Font")]
        public List<FontDetails> Fonts { get; set; }
        
        public List<ImageDetails> Images { get; set; }
        
        public List<ShapeDetails> Shapes { get; set; }

        [XmlAttribute]
        public int PageNumber { get; set; }

        [XmlAttribute]
        public bool RightToLeft { get; set; }

        [XmlAttribute]
        public float Width { get; set; }

        [XmlAttribute]
        public float Height { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        internal List<List<List<TextLineDetails>>> TablesList { get; set; }= new List<List<List<TextLineDetails>>>();

        [XmlElement("Table")]
        public TableModel[] Tables { get; set; }

        public override string ToString()
        {
            return TablesList.Count.ToString();
        }
    }
}