using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class TableExtension
    {
        public static string GetCellTextAtOffsets(this Table table, string text, int rowIndex, int offset,
            int cellIndex)
        {
            return GetCellTextAtOffsets(table, text, true, rowIndex, offset, cellIndex);
        }

        public static string GetCellTextAtOffsets(this Table table, string text, bool useRegex, int rowIndex,
            int offset, int cellIndex)
        {
            table.FindByText(text, useRegex, ref rowIndex, out _);
            if (rowIndex >= 0)
            {
                return table.Rows[rowIndex + offset].Cells[cellIndex].Text;
            }

            return null;
        }
    }
}