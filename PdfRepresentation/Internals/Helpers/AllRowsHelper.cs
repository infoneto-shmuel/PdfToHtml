using System.Collections.Generic;
using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal class AllRowsHelper
    {
        public static List<List<Row>> GetAllRows(TablesModel tablesModel)
        {
            var allRows = new List<List<Row>>();
            foreach (var table in tablesModel!.Tables)
            {
                var rows = table.Rows.ToList();
                foreach (var row in rows)
                {
                    row.TableId = table.Id;
                }

                allRows.Add(rows);
            }

            return allRows;
        }
    }
}