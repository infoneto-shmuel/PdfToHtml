using System;
using System.Collections.Generic;
using System.Linq;
using CoreLibrary.Extensions.BaseTypes;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class CompatibleExtension
    {
        public static Cell[] GetCompatibleCellsByLefts(this TablesModel tableModels, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            return GetCompatibleCellsByLefts(tableModels, getHorizontalTolerance, out _);
        }

        public static Cell[] GetCompatibleCellsByLefts(this TablesModel tableModels,
            Func<Cell, Cell, double> getHorizontalTolerance, out List<Row> rows)
        {
            rows = tableModels.CompactTablesInRows();
            var cells = rows.Select(r => r.Cells.Select(c => c).ToArray()).Concat().ToArray();
            var list = new List<Cell>();
            var groupBy = cells.Where(c => c.GetWidth() > 0.0).Select(c => c.GetLeft()).Distinct();
            foreach (var d in groupBy)
            {
                var cell = cells.Where(c => c.GetWidth() > 0.0 && Math.Abs(c.GetLeft() - d) <= 0.0)
                    .OrderBy(c => c.GetWidth())
                    .FirstOrDefault();
                if (cell != null && list.GetFirstCompatibleCell(cell, getHorizontalTolerance) == null)
                {
                    list.Add(cell);
                }
            }

            return list.OrderBy(i => i.GetLeft()).ToArray();
        }

        public static double[] GetCompatibleLefts(this TablesModel tableModels, Func<Cell, Cell, double> getHorizontalTolerance)
        {
            return GetCompatibleCellsByLefts(tableModels, getHorizontalTolerance).Select(item => item.GetLeft()).ToArray();
        }
    }
}