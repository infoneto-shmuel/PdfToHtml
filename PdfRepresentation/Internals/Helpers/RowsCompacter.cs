using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal class RowsCompacter
    {
        public static List<Row> CompactRows(List<List<Row>> allRows)
        {
            var list = allRows.FirstOrDefault();
            for (var i = 1; i < allRows.Count; i++)
            {
                var rows = allRows[i];
                list?.AddRange(rows);
            }

            return list;
        }
    }
}