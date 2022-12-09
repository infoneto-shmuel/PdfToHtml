using System;
using System.Collections.Generic;
using PdfRepresentation.Extensions.Xml;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal class RowProcessingHelper
    {
        public static bool InitializeProcessing(Row firstRow, Row secondRow, Func<Cell, Cell, double> getHorizontalTolerance, out List<Cell> firstRowCells,
            out List<Cell> secondRowCells, out int firstRowCellIndex, out int secondRowCellIndex)
        {
            firstRowCells = firstRow.GetRowCellsAsList();
            secondRowCells = secondRow.GetRowCellsAsList();
            (firstRowCellIndex, secondRowCellIndex) =
                CellHelper.GetIndexesForCellsAtSameLeftPoint(firstRowCells, secondRowCells, getHorizontalTolerance);
            return false;
        }
    }
}
