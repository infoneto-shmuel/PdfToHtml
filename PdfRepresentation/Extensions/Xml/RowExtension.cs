using System;
using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class RowExtension
    {
        public static double GetMinimalHeight(this Row firstRow, Row secondRow)
        {
            return Math.Min(secondRow.Top, firstRow.Top);
        }

        public static List<Cell> GetRowCellsAsList(this Row row)
        {
            return row.Cells.OrderBy(c => c.GetLeft()).ToList();
        }

        public static double GetVerticalDistance(this Row firstRow, Row secondRow)
        {
            return secondRow.Top < firstRow.Top
                ? firstRow.Bottom - secondRow.Top
                : secondRow.Bottom - firstRow.Top;
        }
    }
}