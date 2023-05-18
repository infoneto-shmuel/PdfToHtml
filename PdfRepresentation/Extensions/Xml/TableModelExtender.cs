using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using PdfRepresentation.Extensions.Pdf;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public class TableModelExtender
    {
        public static DataTable ConvertToDataTable(FileInfo file, out TablesModel tableModels, out TablesModel compactedTableModels,
            out TablesModel square, string startRegex, string stopRegex, Regex tableNameToCompact, string[] textsToFilterRows, string[] columnNames)
        {
            tableModels = file.ConvertToTableModels();
            tableModels.StartRegex = startRegex;
            tableModels.StopRegex = stopRegex;
            compactedTableModels = tableModels.CompactTableRows(tableNameToCompact, textsToFilterRows);

            return compactedTableModels.ToDataTable(10.0, out square, columnNames);
        }
    }
}
