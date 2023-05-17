using System;
using System.Text.RegularExpressions;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class FindExtension
    {
        public static Cell FindByText(this Row row, string cellText, bool useRegx, ref int cellIndex)
        {
            if (row != null)
            {
                Regex regex = null;
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

        public static Table FindByText(this TablesModel tablesModel, string text, ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            return FindByText(tablesModel, text, 0, true, ref tableIndex, out rowIndex, out cellIndex);
        }

        public static Table FindByText(this TablesModel tablesModel, string text, int tableOffset, ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            return FindByText(tablesModel, text, tableOffset, true, ref tableIndex, out rowIndex, out cellIndex);
        }

        public static Table FindByText(this TablesModel tablesModel, string text, bool useRegex, ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            return FindByText(tablesModel, text, 0, useRegex, ref tableIndex, out rowIndex, out cellIndex);
        }

        public static Table FindByText(this TablesModel tablesModel, string text, int tableOffset, bool useRegex,
            ref int tableIndex,
            out int rowIndex, out int cellIndex)
        {
            if (tablesModel != null)
            {
                for (var index = tableIndex + tableOffset; index < tablesModel.Tables.Length; index++)
                {
                    var table = tablesModel.Tables[index];
                    rowIndex = 0;
                    if (table.FindByText(text, useRegex, ref rowIndex, out cellIndex) != null)
                    {
                        tableIndex = index;
                        return table;
                    }
                }
            }

            tableIndex = -1;
            rowIndex = -1;
            cellIndex = -1;
            return null;
        }
    }
}