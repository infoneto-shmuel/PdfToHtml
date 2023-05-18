using System;
using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class EnumerableOfCellExtension
    {
        public static bool CompatibleCellExists(this IEnumerable<Cell> secondRowCells, Cell rowCell,
            Func<Cell, Cell, double> getHorizontalTolerance)
        {
            return GetFirstCompatibleCell(secondRowCells, rowCell, getHorizontalTolerance) != null;
        }

        public static Cell GetFirstCompatibleCell(this IEnumerable<Cell> rowCells, Cell rowCell, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            return rowCells.FirstOrDefault(c => Math.Abs(c.GetLeft() - rowCell.GetLeft()) <=
                                                HorizontalToleranceHelper.EnsureGetHorizontalTolerance(getHorizontalTolerance)(c, rowCell));
        }
    }
}
