using System;
using System.Collections.Generic;
using PdfRepresentation.Extensions.Xml;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{

    internal class RowsMerger
    {
        public static void MergeCurrentRowCellsWithNextRowCellsIfOverlapping(List<Row> rows,
            Func<Cell, Cell, double> getVerticalTolerance = null)
        {
            for (var index = 0; index < rows.Count - 1; index++)
            {
                var currentRow = rows[index];
                var nextRow = rows[index + 1];
                for (var currentIndex = 0; currentIndex < currentRow.Cells.Length; currentIndex++)
                {
                    for (var nextIndex = 0; nextIndex < nextRow.Cells.Length; nextIndex++)
                    {
                        currentRow.Cells[currentIndex].MergeWithNextCell(nextRow.Cells[nextIndex],
                            currentRow.Cells[currentIndex], nextRow, nextIndex, getVerticalTolerance);
                    }
                }

                if (nextRow.Cells.Length == 0)
                {
                    rows.RemoveAt(index + 1);
                }
            }
        }

        public static void MergeCurrentRowCellsWithPreviousRowCellsIfOverlapping(List<Row> rows,
            Func<Cell, Cell, double> getVerticalTolerance = null)
        {
            for (var index = 0; index < rows.Count - 1; index++)
            {
                var currentRow = rows[index];
                var nextRow = rows[index + 1];
                for (var currentIndex = 0; currentIndex < currentRow.Cells.Length; currentIndex++)
                {
                    for (var nextIndex = 0; nextIndex < nextRow.Cells.Length; nextIndex++)
                    {
                        if (currentIndex >= currentRow.Cells.Length)
                        {
                            continue;
                        }

                        nextRow.Cells[nextIndex].MergeWithNextCell(currentRow.Cells[currentIndex],
                            nextRow.Cells[nextIndex], currentRow, currentIndex, getVerticalTolerance);
                    }
                }

                if (currentRow.Cells.Length == 0)
                {
                    rows.RemoveAt(index);
                }
            }
        }
    }
}