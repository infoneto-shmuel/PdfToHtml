using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class CompactTableExtension
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
            var rows = CompactTablesInRows(tablesModel.RemoveRowsByText(textsToFilterRows), tableNameToCompact);

            var startIndex = 0;
            var lastIndex = -1;
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
            tablesModel.JoinTablesByRegularExpressions(tableNameToCompacts);

            return RowsCompacter.CompactRows(AllRowsHelper.GetAllRows(tablesModel));
        }
    }
}