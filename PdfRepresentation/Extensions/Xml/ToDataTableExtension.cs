using System.Data;
using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{
    public static class ToDataTableExtension
    {
        public static DataTable ToDataTable(this TablesModel tableModels, params string[] columnNames)
        {
            return ToDataTable(tableModels, 1.0, out _, columnNames);
        }

        public static DataTable ToDataTable(this TablesModel tableModels, double cellTolerance, params string[] columnNames)
        {
            return ToDataTable(tableModels, cellTolerance, out _, columnNames);
        }

        public static DataTable ToDataTable(this TablesModel tableModels, double cellTolerance, out TablesModel square, params string[] columnNames)
        {
            square = tableModels.UniformTableRows((_, _) => cellTolerance);
            if (columnNames == null || columnNames.Length == 0)
            {
                columnNames = new string[square.Tables.FirstOrDefault()!.Rows.Length];
                for (var i = 0; i < columnNames.Length; i++)
                {
                    columnNames[i] = $"Column{i + 1}";
                }
            }
            var dataTable = new DataTable();
            for (var i = 0; i < columnNames.Length; i++)
            {
                dataTable.Columns.Add(new DataColumn(columnNames[i]));
            }

            for (var index = 0; index < square.Tables.FirstOrDefault()!.Rows.Length; index++)
            {
                var row = square.Tables.FirstOrDefault()!.Rows[index];

                var dataRow = dataTable.NewRow();
                for (var i = 0; i < System.Math.Min(row.Cells.Length, columnNames.Length) ; i++)
                {
                    var cell = row.Cells[i];
                    dataRow[i] = cell.Text;
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
    }
}
