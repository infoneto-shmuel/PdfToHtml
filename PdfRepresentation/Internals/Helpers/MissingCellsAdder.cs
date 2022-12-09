using System;
using System.Collections.Generic;
using PdfRepresentation.Extensions.Xml;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal static class MissingCellsAdder
    {
        public static void AddMissingCellsBottomToTop(List<Row> rows,
            int startIndex,
            int lastIndex = -1, Func<Cell, Cell, double> getHorizontalTolerance = null)
        {
            if (lastIndex < 0)
            {
                lastIndex = rows.Count - 1;
            }
            for (var index = lastIndex; index >= startIndex+1; index--)
            {
                var firstRow = rows[index];
                var secondRow = rows[index - 1];
                var b = true;
                while (b)
                {
                    b = false;
                    b |= ProcessorsForRowsMerger.ProcessRowsLeftToRight(firstRow, secondRow, getHorizontalTolerance);
                    b |= ProcessorsForRowsMerger.ProcessRowsRightToLeft(firstRow, secondRow, getHorizontalTolerance);
                }
            }
        }

        public static (int startIndex, int lastIndex) AddMissingCellsTopToBottom(List<Row> rows, string startRegex, string stopRegex,
                Func<Row, Row, bool> isCompatibleVerticallyWith = null, Func<Cell, Cell, double> getHorizontalTolerance = null)
        {
            var startIndex = rows.GetStartIndex(startRegex);
            if (startIndex > 0)
            {
                for (var index = startIndex+1; index < rows.Count; index++)
                {
                    var stopIndex = rows.GetStopIndex(stopRegex, index - 1);

                    if (stopIndex>=0)
                    { 
                        return (startIndex,stopIndex);
                    }

                    var firstRow = rows[index - 1];
                    var secondRow = rows[index];
                    if ((isCompatibleVerticallyWith ?? TablesModel.DefaultIsCompatibleVerticallyWith).Invoke(firstRow, secondRow))
                    {
                        continue;
                    }

                    var b = true;
                    while (b)
                    {
                        b = false;
                        b |= ProcessorsForRowsMerger.ProcessRowsLeftToRight(firstRow, secondRow, getHorizontalTolerance);
                        b |= ProcessorsForRowsMerger.ProcessRowsRightToLeft(firstRow, secondRow, getHorizontalTolerance);
                    }
                }
            }
            return (startIndex, rows.Count - 1);
        }
    }
}