using System;
using System.Globalization;
using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{
    public static class TableExtension
    {
        public static Row FindByText(this Table table, string text, ref int rowIndex, out int cellIndex)
        {
            return FindByText(table, text, true, ref rowIndex, out cellIndex);
        }

        public static Row FindByText(this Table table, string text, bool useRegex, ref int rowIndex, out int cellIndex)
        {
            if (table != null)
            {
                for (var index = rowIndex; index < table.Rows.Length; index++)
                {
                    var row = table.Rows[index];
                    cellIndex = 0;
                    if (row.FindByText(text, useRegex, ref cellIndex) != null)
                    {
                        rowIndex = index;
                        return row;
                    }
                }
            }

            rowIndex = -1;
            cellIndex = -1;
            return null;
        }

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

        public static double[] GetMediumLefts(this Table table)
        {
            var firstOrDefault = table.Rows.FirstOrDefault();
            if (firstOrDefault == null)
            {
                return new double[0];
            }

            var mediumLefts = new double[firstOrDefault.Cells.Length];
            foreach (var tableRow in table.Rows)
            {
                for (var c = 0; c < tableRow.Cells.OrderBy(i=>i.GetLeft()).ToArray().Length; c++)
                {
                    var cell = tableRow.Cells[c];
                    mediumLefts[c] += cell.GetLeft();
                }
            }

            for (int i = 0; i < mediumLefts.Length; i++)
            {
                mediumLefts[i] /= table.Rows.Length;
            }

            return mediumLefts;

        }

        public static void PutMissingCells(this Table table, int n, double[] firstOrDefault)
        {
            var row = table.Rows.FirstOrDefault();
            if (row != null && row.Cells.Length < n)
            {
                int c = 0;
                while (Math.Abs(row.Cells[c].GetLeft() - firstOrDefault[c]) / firstOrDefault[c] >
                       1 - TablesModel.HorizontalTolerance)
                {
                    c++;
                    if (c >= row.Cells.Length)
                    {
                        break;
                    }
                }

                if (c < row.Cells.Length)
                {
                    foreach (var r in table.Rows)
                    {
                        var cells = r.Cells.ToList();
                        cells.Insert(c, new Cell() { Left = firstOrDefault[c].ToString(CultureInfo.InvariantCulture) });
                        r.Cells = cells.ToArray();
                    }
                }
            }
        }
    }
}
