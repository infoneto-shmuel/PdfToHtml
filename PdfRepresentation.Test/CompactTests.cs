using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ConfigurationLibrary.Arguments;
using CoreLibrary.Model.Configuration.Hosting;
using CoreLibrary.Model.Serialization;
using CoreLibrary.Utility.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Extensions.Xml;
using PdfRepresentation.Model.Xml;
using PdfRepresentation.Test.Properties;
using Xunit;

namespace PdfRepresentation.Test
{
    public class CompactTests
    {
        public CompactTests()
        {
            ArgumentGetterHost.Instance.ArgumentGetter = ArgumentGetter.Instance;
        }

        [Fact]
        public void WhenCompactingTableWithPagesUsingJson_PagesAreRemoved()
        {
            var tableModels = JsonConvert.DeserializeObject<TablesModel>(Encoding.UTF8.GetString(Resources._001));
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            var compactedTableModels = tableModels.CompactTableRows("[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
            Assert.True(compacted.Length < normal.Length);
            Assert.True(tableModels.Tables.Length > compactedTableModels.Tables.Length);
            Assert.Equal(2, compactedTableModels.Tables[0].Rows.Length);
        }

        [Fact]
        public void WhenCompactingTablesJoiningPages_RowsAreCompacted()
        {
            var tableModels =
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._003, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            var compactedTableModels = tableModels.CompactTableRows(new Regex("[-][0]+1"), "[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
            Assert.True(compacted.Length < normal.Length);
            Assert.Equal(2, compactedTableModels.Tables.Last().Rows.Length);
            Assert.Equal(3, compactedTableModels.Tables.Last().Rows[0].Cells.Length);
        }

        [Fact]
        public void WhenCompactingTableWithPagesUsingXml_PagesAreRemoved()
        {
            var tableModels =
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._002, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            var compactedTableModels = tableModels.CompactTableRows("[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
            Assert.True(compacted.Length < normal.Length);
            Assert.Equal(4, compactedTableModels.Tables[1].Rows.Length);
        }

        [Fact]
        public void WhenMultipleQuiteAlignedSingleValueColumnRowsExists_ThenRowsAreCompacted()
        {
            var tableModels =
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._004, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            var compactedTableModels = tableModels.CompactTableRows();
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
            Assert.True(compacted.Length < normal.Length);
            Assert.Equal(26, compactedTableModels.Tables.Last().Rows.Length);
            Assert.Equal(3, compactedTableModels.Tables.Last().Rows[0].Cells.Length);
        }

        [Fact]
        public void WhenStopExpressionAndMultipleQuiteAlignedSingleValueColumnRowsExists_ThenRowsAreCompacted()
        {
            var tableModels =
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._005, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            tableModels.StopRegex = "Table [0-9]+[:]";
            ObjectSerializer.Instance.DefaultSerializationType = SerializationType.Json;
            var compactedTableModels = tableModels.CompactTableRows("[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
            Assert.True(compacted.Length < normal.Length);
            Assert.Equal(2, compactedTableModels.Tables.Last().Rows.Length);
            Assert.Equal(4, compactedTableModels.Tables.First().Rows[0].Cells.Length);
        }

        [Fact]
        public void WhenGettingWholeVehicleApproval_ThenResultIsProduced()
        {
            var tableModels =
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources.Normal, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            tableModels.StartRegex = "Whole Vehicle Type Approval";
            tableModels.StopRegex = "Table [0-9]+[:]";
            var compactedTableModels = tableModels.CompactTableRows(new Regex("[-][0]+1"), "[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);

            Assert.True(compacted.Length < normal.Length);
        }
    }
}