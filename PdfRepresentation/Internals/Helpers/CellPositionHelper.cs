using System;
using System.Collections.Generic;
using PdfRepresentation.Extensions;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal static class CellPositionHelper
    {
        public static bool IsSecondRowCellAfterFirstRowCellWithTolerance(List<Cell> firstRowCells, int firstRowCellIndex,
            List<Cell> secondRowCells, int secondRowCellIndex, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            var distance = secondRowCells[secondRowCellIndex].GetLeft() - firstRowCells[firstRowCellIndex].GetLeft();
            var tolerance = HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance).Invoke(firstRowCells[firstRowCellIndex], secondRowCells[secondRowCellIndex]);
            return distance > 0 && distance > tolerance;
        }

        public static bool IsFirstRowCellAfterSecondRowCellWithTolerance(List<Cell> firstRowCells, int firstRowCellIndex,
            List<Cell> secondRowCells, int secondRowCellIndex, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            var distance = firstRowCells[firstRowCellIndex].GetLeft() - secondRowCells[secondRowCellIndex].GetLeft();
            var tolerance = HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance).Invoke(firstRowCells[firstRowCellIndex], secondRowCells[secondRowCellIndex]);
            return distance > 0 && distance > tolerance;
        }
    }
}
