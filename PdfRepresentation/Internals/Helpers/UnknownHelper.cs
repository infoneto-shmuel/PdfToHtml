using System.Collections.Generic;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal static class UnknownHelper
    {
        public static void SetUnknownIfNeeded(IList<Cell> firstRowCells, ref int firstRowCellIndex,
            ref int secondRowCellIndex)
        {
            if (firstRowCellIndex == firstRowCells.Count)
            {
                secondRowCellIndex = -1;
                firstRowCellIndex = -1;
            }
        }
    }
}
