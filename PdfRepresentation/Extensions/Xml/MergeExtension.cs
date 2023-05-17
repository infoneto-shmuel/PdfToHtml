using System;
using System.Globalization;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class MergeExtension
    {
        public static bool MergeWithNextCell(this Cell firstCell, Cell secondCell, Cell cellToMergeIn, Row nextRow,
            int nextIndex, Func<Cell, Cell, double> getVerticalTolerance = null)
        {
            var distance = firstCell.GetDistance(secondCell);
            var verticalTolerance = (getVerticalTolerance ?? TablesModel.DefaultGetVerticalTolerance).Invoke(firstCell, secondCell);

            if (distance >= 0 && distance < verticalTolerance && firstCell.IsOverlappingVertically(secondCell))
            {
                nextRow.Cells = CoreLibrary.Extensions.BaseTypes.ArrayExtender.RemoveAt(nextRow.Cells, nextIndex, out var nextRowCell);
                var padding = firstCell.GetPaddingTo(secondCell);
                cellToMergeIn.Text += $"{padding}{nextRowCell.Text}";
                cellToMergeIn.Top = Math.Min((double)nextRowCell.GetTop(), cellToMergeIn.GetTop())
                    .ToString(CultureInfo.InvariantCulture);
                cellToMergeIn.Left = Math.Min((double)nextRowCell.GetLeft(), cellToMergeIn.GetLeft())
                    .ToString(CultureInfo.InvariantCulture);
                cellToMergeIn.Width = (Math.Max((double)nextRowCell.GetRight(), cellToMergeIn.GetRight()) -
                                       Math.Min((double)nextRowCell.GetLeft(), cellToMergeIn.GetLeft()))
                    .ToString(CultureInfo.InvariantCulture);
                cellToMergeIn.Bottom = Math.Max((double)nextRowCell.GetBottom(), cellToMergeIn.GetBottom())
                    .ToString(CultureInfo.InvariantCulture);
                return true;
            }

            return false;
        }
    }
}
