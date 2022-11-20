using System.Collections.Generic;
using CoreLibrary.Extensions.BaseTypes;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal class TableCompacter
    {
        public static IList<Table> CompactTables(IEnumerable<Row> sourceRows)
        {
            int oldCellsCount = -1;
            var tables = new List<Table>();
            var rows = sourceRows as IList<Row> ?? sourceRows.ToList<Row>();
            for (var index = 0; index < rows.Count; index++)
            {
                if (rows[index].Cells.Length != oldCellsCount)
                {
                    oldCellsCount = rows[index].Cells.Length;
                    var table = new Table();
                    tables.Add(table);

                    var newRows = new List<Row> { rows[index] };
                    if (index + 1 < rows.Count)
                    {
                        index++;
                        while (index < rows.Count && rows[index].Cells.Length == oldCellsCount)
                        {
                            newRows.Add(rows[index]);
                            index++;
                        }

                        index--;
                    }

                    table.Rows = newRows.ToArray();
                }
            }

            return tables;
        }
    }
}