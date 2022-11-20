using System;
using System.Collections.Generic;
using System.Globalization;
using PdfRepresentation.Extensions;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{

    internal static class CellHelper
    {
        public static Cell CreateCell(List<Cell> firstRowCells, int firstRowCellIndex, List<Cell> secondRowCells,
            int secondRowCellIndex)
        {
            return new Cell
            {
                Top = secondRowCells[secondRowCellIndex].Top,
                Left = firstRowCells[firstRowCellIndex].Left,
                Width = firstRowCells[firstRowCellIndex].GetWidth().ToString(CultureInfo.InvariantCulture),
                Bottom = secondRowCells[secondRowCellIndex].Bottom,
                Text = string.Empty
            };
        }

        public static (int firstRowCellIndex, int secondRowCellIndex) GetIndexesForCellsAtSameLeftPoint(
            List<Cell> firstRowCells,
            List<Cell> secondRowCells, Func<Cell, Cell, double> getHorizontalTolerance = null, int startIndex = 0)
        {
            int firstRowCellIndex;
            var secondRowCellIndex = -1;
            for (firstRowCellIndex = startIndex; firstRowCellIndex < firstRowCells.Count; firstRowCellIndex++)
            {
                var distanceLessThanTolerance = false;
                for (secondRowCellIndex = 0; secondRowCellIndex < secondRowCells.Count; secondRowCellIndex++)
                {
                    if (IsDistanceLessThanTolerance(firstRowCells, firstRowCellIndex, secondRowCells,
                            secondRowCellIndex,
                            getHorizontalTolerance))
                    {
                        distanceLessThanTolerance = true;
                        break;
                    }
                }

                if (distanceLessThanTolerance)
                {
                    break;
                }

                secondRowCellIndex = -1;
            }

            UnknownHelper.SetUnknownIfNeeded(firstRowCells, ref firstRowCellIndex, ref secondRowCellIndex);

            return (firstRowCellIndex, secondRowCellIndex);
        }

        public static (int firstRowCellIndex, int secondRowCellIndex) GetIndexesForCellsAtSameLeftPointRightToLeft(
            List<Cell> firstRowCells, List<Cell> secondRowCells, Func<Cell, Cell, double> getHorizontalTolerance = null,
            int startIndex = 0)
        {
            int firstRowCellIndex;
            var secondRowCellIndex = -1;
            for (firstRowCellIndex = firstRowCells.Count - 1; firstRowCellIndex >= startIndex; firstRowCellIndex--)
            {
                var distanceLessThanTolerance = false;
                for (secondRowCellIndex = secondRowCells.Count - 1; secondRowCellIndex >= 0; secondRowCellIndex--)
                {
                    if (IsDistanceLessThanTolerance(firstRowCells, firstRowCellIndex, secondRowCells,
                            secondRowCellIndex,
                            getHorizontalTolerance))
                    {
                        distanceLessThanTolerance = true;
                        break;
                    }
                }

                if (distanceLessThanTolerance)
                {
                    break;
                }

                secondRowCellIndex = -1;
            }

            UnknownHelper.SetUnknownIfNeeded(firstRowCells, ref firstRowCellIndex, ref secondRowCellIndex);

            return (firstRowCellIndex, secondRowCellIndex);
        }

        public static bool IsDistanceLessThanTolerance(List<Cell> firstRowCells, int firstRowCellIndex,
            List<Cell> secondRowCells,
            int secondRowCellIndex,
            Func<Cell, Cell, double> getHorizontalTolerance)
        {
            var firstRowCell = firstRowCells[firstRowCellIndex];
            var secondRowCell = secondRowCells[secondRowCellIndex];
            return Math.Abs(secondRowCell.GetLeft() - firstRowCell.GetLeft()) <
                   HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance)
                       .Invoke(firstRowCell, secondRowCell);
        }

        public static (string newLeft, string oldLeft) SetSecondRowCellLeft(List<Cell> firstRowCells,
            int firstRowCellIndex,
            List<Cell> secondRowCells, int secondRowCellIndex)
        {
            var oldLeft = secondRowCells[secondRowCellIndex].Left;
            secondRowCells[secondRowCellIndex].Left = firstRowCells[firstRowCellIndex].Left;
            return (firstRowCells[firstRowCellIndex].Left, oldLeft);
        }

        public static void TrimSecondRowCellText(List<Cell> secondRowCells, int firstRowCellIndex,
            int secondRowCellIndex)
        {
            secondRowCells[secondRowCellIndex].Text =
                secondRowCells[secondRowCellIndex]?.Text?.TrimStart();
        }
    }
}