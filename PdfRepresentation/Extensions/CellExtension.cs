using System;
using System.Globalization;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{
    public static class CellExtension
    {
        public static double GetBottom(this Cell cell)
        {
            double.TryParse(cell.Bottom, NumberStyles.Any, CultureInfo.InvariantCulture, out var bottom);
            return bottom;
        }

        public static double GetDistance(this Cell firstCell, Cell secondCell)
        {
            return secondCell.GetLeft() - firstCell.GetRight();
        }

        public static double GetHeight(this Cell cell)
        {
            return cell.GetBottom() - cell.GetTop();
        }

        public static double GetLeft(this Cell cell)
        {
            double.TryParse(cell.Left, NumberStyles.Any, CultureInfo.InvariantCulture, out var left);
            return left;
        }

        public static string GetPadding(this Cell firstCell, Cell secondCell)
        {
            var distance = GetDistance(firstCell, secondCell);
            return distance >= 0 && distance < firstCell.GetHeight() / 2 ? "" : " ";
        }

        public static double GetRight(this Cell cell)
        {
            return cell.GetLeft() + cell.GetWidth();
        }

        public static double GetTop(this Cell cell)
        {
            double.TryParse(cell.Top, NumberStyles.Any, CultureInfo.InvariantCulture, out var top);
            return top;
        }

        public static double GetWidth(this Cell cell)
        {
            double.TryParse(cell.Width, NumberStyles.Any, CultureInfo.InvariantCulture, out var width);
            return width;
        }

        public static bool IsOverlappingVertically(this Cell firstCell, Cell secondCell)
        {
            return (firstCell.GetTop() >= secondCell.GetTop() &&
                    firstCell.GetTop() <= secondCell.GetTop() + secondCell.GetHeight()) ||
                   (secondCell.GetTop() >= firstCell.GetTop() &&
                    secondCell.GetTop() <= firstCell.GetTop() + firstCell.GetHeight());
        }

        public static bool MergeWithNextCell(this Cell firstCell, Cell secondCell, Cell cellToMergeIn, Row nextRow,
            int nextIndex, Func<Cell, Cell, double> getVerticalTolerance = null)
        {
            var distance = firstCell.GetDistance(secondCell);
            var verticalTolerance = (getVerticalTolerance ?? TablesModel.DefaultGetVerticalTolerance).Invoke(firstCell, secondCell);

            if (distance >= 0 && distance < verticalTolerance && firstCell.IsOverlappingVertically(secondCell))
            {
                nextRow.Cells = nextRow.Cells.RemoveAt(nextIndex, out var nextRowCell);
                var padding = firstCell.GetPadding(secondCell);
                cellToMergeIn.Text += $"{padding}{nextRowCell.Text}";
                cellToMergeIn.Top = Math.Min(nextRowCell.GetTop(), cellToMergeIn.GetTop())
                    .ToString(CultureInfo.InvariantCulture);
                cellToMergeIn.Left = Math.Min(nextRowCell.GetLeft(), cellToMergeIn.GetLeft())
                    .ToString(CultureInfo.InvariantCulture);
                cellToMergeIn.Width = (Math.Max(nextRowCell.GetRight(), cellToMergeIn.GetRight()) -
                                       Math.Min(nextRowCell.GetLeft(), cellToMergeIn.GetLeft()))
                    .ToString(CultureInfo.InvariantCulture);
                cellToMergeIn.Bottom = Math.Max(nextRowCell.GetBottom(), cellToMergeIn.GetBottom())
                    .ToString(CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }
    }
}