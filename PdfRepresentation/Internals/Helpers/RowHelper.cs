using System.Collections.Generic;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal static class RowHelper
    {
        public static bool EnsureSecondRowCellsIfModified(Row secondRow, List<Cell> secondRowCells, bool returnValue)
        {
            if (returnValue)
            {
                secondRow.Cells = secondRowCells.ToArray();
            }

            return returnValue;
        }
    }
}
