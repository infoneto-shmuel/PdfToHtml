using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PdfRepresantation.Model
{
    public class TableModel
    {
        [JsonIgnore]
        [XmlIgnore]
        internal List<RowModel> RowModels { get; set; } = new List<RowModel>();

        [XmlAttribute]
        public string Id { get; set; }

        public bool ShouldSerializeId()
        {
            return !string.IsNullOrEmpty(Id);
        }

        [XmlElement("Rows")]
        public RowModel[] Rows
        {
            get { return RowModels.ToArray();}
            set { RowModels = value?.ToList()?? new List<RowModel>(); }
        }

        public override string ToString()
        {
            return RowModels.Count.ToString();
        }
    }
}