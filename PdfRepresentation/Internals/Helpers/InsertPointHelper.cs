using System.Collections.Generic;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal class InsertPointHelper
    {
        public static int FindInsertPoint(List<Cell> rowCells, Cell cell)
        {
            int i;
            for (i = 0; i < rowCells.Count; i++)
            {
                if (rowCells[i].GetLeft() < cell.GetLeft())
                {
                    continue;
                }

                if (i >= 0)
                {
                    break;
                }
            }

            return i;
        }

        public static void InsertOrAddCell(int i, List<Cell> rowCells, Cell cell)
        {
            if (i >= rowCells.Count)
            {
                rowCells.Add(new Cell
                {
                    Bottom = rowCells[0].Bottom,
                    Left = cell.Left,
                    Text = string.Empty,
                    Top = rowCells[0].Top,
                    Width = cell.Width
                });
            }
            else
            {
                rowCells.Insert(i, new Cell
                {
                    Bottom = rowCells[0].Bottom,
                    Left = cell.Left,
                    Text = string.Empty,
                    Top = rowCells[0].Top,
                    Width = cell.Width
                });
            }
        }
    }
}