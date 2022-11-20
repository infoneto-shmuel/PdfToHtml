using System.Collections.Generic;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions
{

    public static class ListOfRowsExtension
    {
        public static int GetStartIndex(this List<Row> rows, string startRegex)
        {
            var cellNo = 0;
            var startIndex = -1;
            if (!string.IsNullOrEmpty(startRegex))
            {
                for (var index = 0; index < rows.Count; index++)
                {
                    var r = rows[index];
                    cellNo = 0;
                    if (r.FindByText(startRegex, true, ref cellNo) != null)
                    {
                        startIndex = index;
                        break;
                    }
                }
            }
            else
            {
                startIndex = 0;
            }

            return startIndex;
        }

        public static int GetStopIndex(this List<Row> rows, string stopRegex, int index)
        {
            var cellNo = 0;
            var stopIndex = -1;
            if (!string.IsNullOrEmpty(stopRegex) && rows[index].FindByText(stopRegex, true, ref cellNo) != null)
            {
                stopIndex = index - 1;
            }

            return stopIndex;
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