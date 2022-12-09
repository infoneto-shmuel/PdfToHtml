using System;
using System.Xml.Serialization;
using CoreLibrary.Model.Configuration.Hosting;
using CoreLibrary.Utility.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Extensions;
using PdfRepresentation.Extensions.Xml;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Internals.Serialization;

namespace PdfRepresentation.Model.Xml
{

    [XmlRoot(ElementName = "Tables")]
    public class TablesModel : ToXmlJson
    {
        static TablesModel()
        {
            var argumentGetter = ArgumentGetterHost.Instance.ArgumentGetter;
            HorizontalTolerance =
                (double)(argumentGetter?.DecimalFromArguments["HorizontalTolerance", (decimal)3.5] ?? (decimal)3.5);
            VerticalTolerance = (double)(argumentGetter?.DecimalFromArguments["VerticalTolerance", (decimal)1.0] ??
                                         (decimal)1.0);
        }

        [XmlElement(ElementName = "Table")] public Table[] Tables { get; set; }

        public string StopRegex { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public Func<Cell, Cell, double> GetHorizontalTolerance { get; set; } = DefaultGetHorizontalTolerance;

        [XmlIgnore]
        [JsonIgnore]
        public Func<Cell, Cell, double> GetVerticalTolerance { get; set; } = DefaultGetVerticalTolerance;

        [XmlIgnore]
        [JsonIgnore]
        public Func<Row, Row, bool> IsCompatibleVerticallyWith { get; set; } = DefaultIsCompatibleVerticallyWith;

        [XmlIgnore] [JsonIgnore] public static double HorizontalTolerance { get; set; }

        [XmlIgnore] [JsonIgnore] public static double VerticalTolerance { get; set; }
        public string StartRegex { get; set; }

        public static double DefaultGetHorizontalTolerance(Cell firstRowCell, Cell secondRowCell)
        {
            return Math.Max(firstRowCell.GetHeight(), secondRowCell.GetHeight()) * HorizontalTolerance;
        }

        public static double DefaultGetVerticalTolerance(Cell firstCell, Cell secondCell)
        {
            return Math.Max(firstCell.GetHeight(), secondCell.GetHeight()) * VerticalTolerance;
        }

        public static bool DefaultIsCompatibleVerticallyWith(Row firstRow, Row secondRow)
        {
            return firstRow.GetVerticalDistance(secondRow) > firstRow.GetMinimalHeight(secondRow) * VerticalTolerance;
        }

        public override string ToString()
        {
            return SerializerHelper.SerializeAsString(() =>
                ObjectSerializer.Instance.Serialize(TableSerializer<TablesModel>.GetTableSerializer(this, Tables)));
        }
    }
}