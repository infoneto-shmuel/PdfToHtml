using System;
using System.Linq;
using PdfRepresentation.Extensions;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{

    internal static class ProcessorsForRowsMerger
    {
        public static bool ProcessRowsLeftToRight(Row firstRow, Row secondRow,
            Func<Cell, Cell, double>? getHorizontalTolerance = null)
        {
            var isModified = RowProcessingHelper.InitializeProcessing(firstRow, secondRow, getHorizontalTolerance,
                out var firstRowCells, out var secondRowCells, out var firstRowCellIndex, out var secondRowCellIndex);
            while (firstRowCellIndex >= 0 && secondRowCellIndex >= 0)
            {
                for (var index = 0; index < firstRow.Cells.Length; index++)
                {
                    (string newLeft, string oldLeft) =
                        firstRowCells.AlignSecondRowToFirstLeftToRightIfInHorizontalTolerance(index, secondRowCells,
                            secondRowCellIndex, getHorizontalTolerance);
                    if (CellPositionHelper.IsSecondRowCellAfterFirstRowCellWithTolerance(firstRowCells, index,
                            secondRowCells, secondRowCellIndex, getHorizontalTolerance) &&
                        !secondRowCells.ExistsCompatibleCell(firstRowCells[firstRowCellIndex], getHorizontalTolerance))
                    {
                        CellHelper.TrimSecondRowCellText(secondRowCells, index, secondRowCellIndex);
                        var cell = CellHelper.CreateCell(firstRowCells, index, secondRowCells, secondRowCellIndex);
                        if (secondRowCells.All(c =>
                                Math.Abs(c.GetLeft() - cell.GetLeft()) >
                                HorizontalToleranceHelper
                                    .EnsureGetHorizontalTolerance(getHorizontalTolerance)(c, cell)))
                        {
                            secondRowCells.Insert(index, cell);
                            secondRowCells = secondRowCells.OrderBy(s => s.GetLeft()).ToList();
                            isModified = true;
                        }
                    }
                }

                (firstRowCellIndex, secondRowCellIndex) =
                    CellHelper.GetIndexesForCellsAtSameLeftPoint(firstRowCells, secondRowCells, getHorizontalTolerance,
                        firstRowCellIndex + 1);
            }

            return RowHelper.EnsureSecondRowCellsIfModified(secondRow, secondRowCells, isModified);
        }

        public static bool ProcessRowsRightToLeft(Row firstRow, Row secondRow,
            Func<Cell, Cell, double>? getHorizontalTolerance = null)
        {
            var isModified = RowProcessingHelper.InitializeProcessing(firstRow, secondRow, getHorizontalTolerance,
                out var firstRowCells, out var secondRowCells, out var firstRowCellIndex, out var secondRowCellIndex);

            while (firstRowCells.Count > secondRowCells.Count && firstRowCellIndex >= 0 && secondRowCellIndex >= 0)
            {
                var notInserted = false;
                for (var index = firstRow.Cells.Length - 1; index >= 0; index--)
                {
                    (string newLeft, string oldLeft) =
                        firstRowCells.AlignSecondRowToFirstRightToLeftIfInHorizontalTolerance(index, secondRowCells,
                            secondRowCellIndex, getHorizontalTolerance);
                    if (CellPositionHelper.IsFirstRowCellAfterSecondRowCellWithTolerance(firstRowCells, index,
                            secondRowCells, secondRowCellIndex, getHorizontalTolerance))
                    {
                        CellHelper.TrimSecondRowCellText(secondRowCells, index, secondRowCellIndex);
                        var cell = CellHelper.CreateCell(firstRowCells, index, secondRowCells, secondRowCellIndex);
                        if (secondRowCells.All(c =>
                                Math.Abs(c.GetLeft() - cell.GetLeft()) >
                                HorizontalToleranceHelper
                                    .EnsureGetHorizontalTolerance(getHorizontalTolerance)(c, cell)))
                        {
                            secondRowCells.Insert(Math.Min(index, secondRow.Cells.Length), cell);
                            isModified = true;
                        }
                        else
                        {
                            notInserted = true;
                            break;
                        }
                    }
                }

                var oldFirstRowCellIndex = firstRowCellIndex;
                var oldSecondRowCellIndex = secondRowCellIndex;
                (firstRowCellIndex, secondRowCellIndex) =
                    CellHelper.GetIndexesForCellsAtSameLeftPointRightToLeft(firstRowCells, secondRowCells,
                        getHorizontalTolerance,
                        firstRowCellIndex - 1);
                if (notInserted || (oldFirstRowCellIndex == firstRowCellIndex &&
                                    oldSecondRowCellIndex == secondRowCellIndex) ||
                    secondRowCellIndex < 0)
                {
                    break;
                }
            }

            return RowHelper.EnsureSecondRowCellsIfModified(secondRow, secondRowCells, isModified);
        }
    }
}