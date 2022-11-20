using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{
    public static class RowExtension
    {
        public static Cell FindByText(this Row row, string cellText, bool useRegx, ref int cellIndex)
        {
            if (row != null)
            {
                Regex regex=null;
                if (useRegx)
                {
                    regex = new Regex(cellText);
                }
                for (var index = cellIndex; index < row.Cells.Length; index++)
                {
                    var cell = row.Cells[index];
                    if (regex != null)
                    {
                        if (regex.IsMatch(cell.Text))
                        {
                            cellIndex = index;
                            return cell;
                        }
                    }
                    else if (cell.Text.Equals(cellText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        cellIndex = index;
                        return cell;
                    }
                }
            }

            cellIndex = -1;
            return null;
        }

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