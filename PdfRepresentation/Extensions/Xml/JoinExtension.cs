using System.Linq;
using System.Text.RegularExpressions;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class JoinExtension
    {
        public static void JoinTablesByRegularExpressions(this TablesModel tablesModel,
            params Regex[] tableNameToCompacts)
        {
            if (tableNameToCompacts != null)
            {
                foreach (var tableNameToCompact in tableNameToCompacts)
                {
                    for (var index = 0; index < tablesModel!.Tables.Length; index++)
                    {
                        var table = tablesModel!.Tables[index];
                        if (tableNameToCompact.IsMatch(table.Id) && index > 0)
                        {
                            var previousTable = tablesModel!.Tables[index - 1];
                            var rows = previousTable.Rows.ToList();
                            rows.AddRange(table.Rows);
                            previousTable.Rows = rows.ToArray();
                            var tables = tablesModel!.Tables.ToList();
                            tables.RemoveAt(index);
                            tablesModel!.Tables = tables.ToArray();
                            index--;
                        }
                    }
                }
            }
        }
    }
}