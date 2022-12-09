using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class RemoveRowsExtension
    {
        public static TablesModel RemoveRowsByText(this TablesModel tablesModel, params string[] textsToFilterRows)
        {
            if (textsToFilterRows.Length == 0)
            {
                return tablesModel;
            }

            foreach (var table in tablesModel.Tables)
            {
                table.Rows = table.Rows.ToList().RemoveRowsByText(textsToFilterRows).ToArray();
            }

            return tablesModel;
        }

        public static List<Row> RemoveRowsByText(this List<Row> rows, params string[] texts)
        {
            foreach (var text in texts)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    for (var index = 0; index < rows.Count; index++)
                    {
                        var row = rows[index];
                        var cellIndex = 0;
                        if (row.FindByText(text, true, ref cellIndex) != null)
                        {
                            rows.RemoveAt(index);
                            index--;
                        }
                    }
                }
            }

            return rows;
        }
    }
}