using System;
using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{
    public static class RowEnumerableExtension
    {
        public static IEnumerable<Row> CompactCompatibleRows(this IEnumerable<Row> sourceRows, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            foreach (var sourceRow in sourceRows)
            {
                bool isModified = false;
                var sourceRowCells = sourceRow.Cells.ToList();
                for (int i = 0; i < sourceRowCells.Count - 2; i++)
                {
                    if (Math.Abs(sourceRowCells[i + 1].GetLeft() - sourceRowCells[i].GetLeft()) <
                        HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance).Invoke(sourceRowCells[i], sourceRowCells[i + 1]))
                    {
                        sourceRowCells[i].Text += sourceRowCells[i + 1].Text;
                        sourceRowCells.RemoveAt(i + 1);
                        isModified = true;
                    }
                }

                if (isModified)
                {
                    sourceRow.Cells = sourceRowCells.ToArray();
                }
            }

            return sourceRows;
        }

        public static List<Row> CompactCompatibleColumns(this IEnumerable<Row> rows, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            var sourceRows = rows.ToList();
            for (var i = 1; i < sourceRows.Count; i++)
            {
                var sourceRow = sourceRows[i];
                var n = 0;
                var firstIndex = 0;
                for (var index = 0; index < sourceRow.Cells.Length; index++)
                {
                    var r = sourceRow.Cells[index];
                    if (!string.IsNullOrEmpty(r.Text))
                    {
                        n++;
                        if (n > 1)
                        {
                            break;
                        }

                        firstIndex = index;
                    }
                }

                if (n != 1)
                {
                    continue;
                }

                var cell = sourceRow.Cells[firstIndex];
                var firstOrDefault = sourceRows[i - 1].Cells.FirstOrDefault(c =>
                    Math.Abs(c.GetLeft() - cell.GetLeft()) < HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance).Invoke(c, cell));
                if (firstOrDefault != null)
                {
                    firstOrDefault.Text += $"\n{cell.Text}";
                    firstOrDefault.Bottom = cell.Bottom;
                    sourceRows.RemoveAt(i);
                    i--;
                }
            }

            return sourceRows;
        }
    }
}
