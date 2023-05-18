using System;
using System.Linq;
using PdfRepresentation.Internals.Helpers;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class UniformerExtension
    {
        public static TablesModel UniformTableRows(this TablesModel tablesModel,
            Func<Cell, Cell, double> getHorizontalTolerance)
        {
            var compatibleLefts = tablesModel.GetCompatibleCellsByLefts(getHorizontalTolerance, out var rows);
            foreach (var row in rows)
            {
                UniformRow(row, compatibleLefts, getHorizontalTolerance);
            }

            return new TablesModel
            {
                Tables = new[]
                {
                    new Table
                    {
                        Rows = rows.ToArray()
                    }
                }
            };
        }

        public static Row UniformRow(this Row row, Cell[] compatibleLefts, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            var rowCells = row.Cells.ToList();
            var modified = false;
            foreach (var cell in compatibleLefts)
            {
                if (rowCells.GetFirstCompatibleCell(cell, getHorizontalTolerance) != null)
                {
                    continue;
                }

                modified = true;
                InsertPointHelper.InsertOrAddCell(InsertPointHelper.FindInsertPoint(rowCells, cell), rowCells, cell);
            }

            if (modified)
            {
                row.Cells = rowCells.ToArray();
            }

            return row;
        }
    }
}