using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{

    public static class TableModelsExtension
    {
        public static TablesModel CompactTableRows(this TablesModel tablesModel, params string[] textsToFilterRows)
        {
            return CompactTableRows(tablesModel, 0, new Regex[0], textsToFilterRows);
        }

        public static TablesModel CompactTableRows(this TablesModel tablesModel, int rowsToSkip,
            params string[] textsToFilterRows)
        {
            return CompactTableRows(tablesModel, rowsToSkip, new Regex[0], textsToFilterRows);
        }

        public static TablesModel CompactTableRows(this TablesModel tablesModel, Regex tableNameToCompact,
            params string[] textsToFilterRows)
        {
            return CompactTableRows(tablesModel, 0, new[] { tableNameToCompact }, textsToFilterRows);
        }

        public static TablesModel CompactTableRows(this TablesModel tablesModel, int rowsToSkip,
            Regex tableNameToCompact,
            params string[] textsToFilterRows)
        {
            return CompactTableRows(tablesModel, rowsToSkip, new[] { tableNameToCompact }, textsToFilterRows);
        }

        public static TablesModel CompactTableRows(this TablesModel tablesModel, int rowsToSkip,
            Regex[] tableNameToCompact,
            params string[] textsToFilterRows)
        {
            var rows = tablesModel.RemoveRowsByText(textsToFilterRows).CompactTablesInRows(tableNameToCompact);

            int startIndex = 0;
            int lastIndex = -1;
            if (rows.Count >= 2)
            {
                RowsMerger.MergeCurrentRowCellsWithNextRowCellsIfOverlapping(rows, tablesModel.GetVerticalTolerance);

                RowsMerger.MergeCurrentRowCellsWithPreviousRowCellsIfOverlapping(rows,
                    tablesModel.GetVerticalTolerance);

                (startIndex, lastIndex) = MissingCellsAdder.AddMissingCellsTopToBottom(rows, tablesModel.StartRegex,
                    tablesModel.StopRegex,
                    tablesModel.IsCompatibleVerticallyWith, tablesModel.GetHorizontalTolerance);


                MissingCellsAdder.AddMissingCellsBottomToTop(rows, startIndex, lastIndex,
                    tablesModel.GetHorizontalTolerance);
            }

            var sourceRows = rows.Skip(startIndex).Take(lastIndex - startIndex - rowsToSkip);
            var compactedTables = TableCompacter.CompactTables(sourceRows
                .CompactCompatibleRows(tablesModel.GetHorizontalTolerance)
                .CompactCompatibleColumns(tablesModel.GetHorizontalTolerance));
            var tables = compactedTables.ToArray();
            var models = new TablesModel
            {
                Tables = tables
            };
            return models;
        }

        public static List<Row> CompactTablesInRows(this TablesModel tablesModel)
        {
            return CompactTablesInRows(tablesModel, null);
        }

        public static List<Row> CompactTablesInRows(this TablesModel tablesModel, params Regex[] tableNameToCompacts)
        {
            JoinTablesByRegularExpressions(tablesModel, tableNameToCompacts);

            return CompactRows(GetAllRows(tablesModel));
        }

        public static Table FindByText(this TablesModel tablesModel, string text, ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            return FindByText(tablesModel, text, 0, true, ref tableIndex, out rowIndex, out cellIndex);
        }

        public static Table FindByText(this TablesModel tablesModel, string text, int tableOffset, ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            return FindByText(tablesModel, text, tableOffset, true, ref tableIndex, out rowIndex, out cellIndex);
        }

        public static Table FindByText(this TablesModel tablesModel, string text, bool useRegex, ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            return FindByText(tablesModel, text, 0, useRegex, ref tableIndex, out rowIndex, out cellIndex);
        }

        public static Table FindByText(this TablesModel tablesModel, string text, int tableOffset, bool useRegex,
            ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            if (tablesModel != null)
            {
                for (var index = tableIndex + tableOffset; index < tablesModel.Tables.Length; index++)
                {
                    var table = tablesModel.Tables[index];
                    rowIndex = 0;
                    if (table.FindByText(text, useRegex, ref rowIndex, out cellIndex) != null)
                    {
                        tableIndex = index;
                        return table;
                    }
                }
            }

            tableIndex = -1;
            rowIndex = -1;
            cellIndex = -1;
            return null;
        }

        public static void JoinTablesByRegularExpressions(this TablesModel tablesModel,
            params Regex[] tableNameToCompacts)
        {
            foreach (var tableNameToCompact in tableNameToCompacts)
            {
                for (var index = 0; index < tablesModel!.Tables.Length; index++)
                {
                    var table = tablesModel!.Tables[index];
                    if (tableNameToCompact.IsMatch(table.Id) && index > 0)
                    {
                        var previousTable = tablesModel!.Tables[index - 1];
                        var rows = previousTable.Rows.ToList();
                        rows.AddRange(table.Rows);
                        previousTable.Rows = rows.ToArray();
                        var tables = tablesModel!.Tables.ToList();
                        tables.RemoveAt(index);
                        tablesModel!.Tables = tables.ToArray();
                        index--;
                    }
                }
            }
        }

        public static TablesModel RemoveRowsByText(this TablesModel tablesModel, params string[] textsToFilterRows)
        {
            if (textsToFilterRows.Length == 0)
            {
                return tablesModel;
            }

            foreach (var table in tablesModel.Tables)
            {
                table.Rows = table.Rows.ToList().RemoveRowsByText(textsToFilterRows).ToArray();
            }

            return tablesModel;
        }

        private static List<Row> CompactRows(List<List<Row>> allRows)
        {
            var list = allRows.FirstOrDefault();
            for (var i = 1; i < allRows.Count; i++)
            {
                var rows = allRows[i];
                list?.AddRange(rows);
            }

            return list;
        }

        private static List<List<Row>> GetAllRows(TablesModel tablesModel)
        {
            var allRows = new List<List<Row>>();
            foreach (var table in tablesModel!.Tables)
            {
                var rows = table.Rows.ToList();
                foreach (var row in rows)
                {
                    row.TableId = table.Id;
                }

                allRows.Add(rows);
            }

            return allRows;
        }
    }
}