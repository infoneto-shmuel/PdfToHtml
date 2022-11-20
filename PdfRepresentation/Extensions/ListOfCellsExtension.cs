using System;
using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{
    public static class ListOfCellsExtension
    {
        public static (string newLeft, string oldLeft) AlignSecondRowToFirstLeftToRightIfInHorizontalTolerance(
            this List<Cell> firstRowCells, int firstRowCellIndex,
            List<Cell> secondRowCells, int secondRowCellIndex, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            string newLeft = null;
            var oldLeft = secondRowCells[secondRowCellIndex].Left;
            if (secondRowCellIndex >= 0 &&
                secondRowCells[secondRowCellIndex].GetLeft() - firstRowCells[firstRowCellIndex].GetLeft() > 0.0 &&
                secondRowCells[secondRowCellIndex].GetLeft() - firstRowCells[firstRowCellIndex].GetLeft() <
                HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance)
                    .Invoke(firstRowCells[firstRowCellIndex], secondRowCells[secondRowCellIndex]) * 2 &&
                firstRowCells.IsDifferentLeft(firstRowCellIndex, secondRowCells, secondRowCellIndex))
            {
                (newLeft, oldLeft) = CellHelper.SetSecondRowCellLeft(firstRowCells, firstRowCellIndex, secondRowCells,
                    secondRowCellIndex);
            }

            return (newLeft, oldLeft);
        }

        public static (string newLeft, string oldLeft) AlignSecondRowToFirstRightToLeftIfInHorizontalTolerance(
            this List<Cell> firstRowCells, int firstRowCellIndex,
            List<Cell> secondRowCells,
            int secondRowCellIndex, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            string newLeft = null;
            var oldLeft = secondRowCells[secondRowCellIndex].Left;
            if (firstRowCells[firstRowCellIndex].GetLeft() - secondRowCells[secondRowCellIndex].GetLeft() > 0.0 &&
                firstRowCells[firstRowCellIndex].GetLeft() - secondRowCells[secondRowCellIndex].GetLeft() <
                HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance)
                    .Invoke(firstRowCells[firstRowCellIndex], secondRowCells[secondRowCellIndex]) * 2 &&
                firstRowCells.IsDifferentLeft(firstRowCellIndex, secondRowCells, secondRowCellIndex))
            {
                (newLeft, oldLeft) = CellHelper.SetSecondRowCellLeft(firstRowCells, firstRowCellIndex, secondRowCells,
                    secondRowCellIndex);
            }

            return (newLeft, oldLeft);
        }

        public static bool IsDifferentLeft(this List<Cell> firstRowCells, int firstRowCellIndex,
            List<Cell> secondRowCells,
            int secondRowCellIndex)
        {
            return secondRowCellIndex >= firstRowCells.Count ||
                   secondRowCells[secondRowCellIndex].Left !=
                   firstRowCells[secondRowCellIndex].Left;
        }

        public static bool ExistsCompatibleCell(this List<Cell> secondRowCells, Cell rowCell,
            Func<Cell, Cell, double> getHorizontalTolerance)
        {
            return secondRowCells.Any(c =>
                Math.Abs(c.GetLeft() - rowCell.GetLeft()) >
                HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance)(c, rowCell));
        }
    }
}