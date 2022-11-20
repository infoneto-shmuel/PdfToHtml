using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ConfigurationLibrary.Arguments;
using CoreLibrary.Model.Configuration.Hosting;
using CoreLibrary.Model.Serialization;
using CoreLibrary.Utility.Serialization;
using Newtonsoft.Json;
using PdfRepresentation.Extensions;
using PdfRepresentation.Model.Xml;
using PdfRepresentation.Test.Properties;
using Xunit;

namespace PdfRepresentation.Test
{
    public class TablesToDataTablesTests
    {
        public TablesToDataTablesTests()
        {
            ArgumentGetterHost.Instance.ArgumentGetter = ArgumentGetter.Instance;
        }

        [Fact]
        public void WhenCompactingTableWithPagesUsingJson_PagesAreRemoved()
        {
            var tableModels = JsonConvert.DeserializeObject<TablesModel>(Resources._001_json);
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
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._003_xml, SerializationType.Xml);
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
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._002_xml, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            var compactedTableModels = tableModels.CompactTableRows("[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
            Assert.True(compacted.Length < normal.Length);
            Assert.Equal(5, compactedTableModels.Tables[0].Rows.Length);
        }

        [Fact]
        public void WhenMultipleQuiteAlignedSingleValueColumnRowsExists_ThenRowsAreCompacted()
        {
            var tableModels =
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._004_xml, SerializationType.Xml);
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
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources._005_xml, SerializationType.Xml);
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
                ObjectSerializer.Instance.Deserialize<TablesModel>(Resources.Normal_xml, SerializationType.Xml);
            var normal = ObjectSerializer.Instance.Serialize(tableModels!, SerializationType.Xml);
            tableModels.StartRegex = "Whole Vehicle Type Approval";
            tableModels.StopRegex = "Table [0-9]+[:]";
            var compactedTableModels = tableModels.CompactTableRows(new Regex("[-][0]+1"), "[0-9]+ of [0-9]+");
            var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);

            Assert.True(compacted.Length < normal.Length);
        }

        [Fact]
        public void WhenConvertingToDataTables_ThenDataTablesAreProduced()
        {
            var directoryInfo = new DirectoryInfo(TestsConstants.PdfDir);
            if (directoryInfo.Exists)
            {
                foreach (var file in directoryInfo.EnumerateFiles().Where(d => d.Name.ToLower() != "001.pdf"))
                {
                    var tableModels = file.ConvertToTableModels();
                    var normal = ObjectSerializer.Instance.Serialize(tableModels, SerializationType.Xml);
                    tableModels.StartRegex = "Whole Vehicle Type Approval";
                    tableModels.StopRegex = "Table [0-9]+[:]";
                    var compactedTableModels = tableModels.CompactTableRows(new Regex("[-][0]+1"), "[0-9]+ of [0-9]+");
                    var compacted = ObjectSerializer.Instance.Serialize(compactedTableModels, SerializationType.Xml);
                    Assert.True(compacted.Length < normal.Length);
                    DoWork(compactedTableModels);
                }
            }
        }

        private static void DoWork(TablesModel tablesModel)
        {
            var tableIndex = 0;
            var rowIndex = 0;
            while (tableIndex >= 0 && rowIndex >= 0)
            {
                var wholeVehicleTypeApproval = tablesModel.FindByText("Whole Vehicle Type Approval",
                    ref tableIndex, out rowIndex, out _);

                var vehicle = wholeVehicleTypeApproval.GetCellTextAtOffsets("VEHICLE", rowIndex, 1, 0);
                var approval = wholeVehicleTypeApproval.GetCellTextAtOffsets("APPROVAL", rowIndex, 1, 0);

                var changesToApproval =
                    tablesModel.FindByText("CHANGES TO APPROVAL", ref tableIndex, out rowIndex, out _);
                var maxValue = int.MaxValue;
                var valueTuples = new List<(int tableIndex, int rowIndex)>();
                for (var index = rowIndex + 1; index < Math.Min(changesToApproval.Rows.Length, maxValue); index++)
                {
                    var row = changesToApproval.Rows[index];
                    var text = row.Cells.Last().Text;
                    var r = index + 1;
                    changesToApproval.FindByText(text, false, ref r, out _);
                    if (r >= 0)
                    {
                        valueTuples.Add((tableIndex, r));
                        maxValue = Math.Min(maxValue, r);
                    }
                    else
                    {
                        var annexIndex = tableIndex + 1;
                        tablesModel.FindByText(text, false, ref annexIndex, out r, out _);
                        if (r >= 0)
                        {
                            valueTuples.Add((annexIndex, r));
                        }
                    }
                }

                tableIndex++;
                changesToApproval = tablesModel.Tables[tableIndex];
            }
        }
    }
}