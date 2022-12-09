using System.Collections.Generic;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Extensions.Xml
{

    public static class ListOfRowsExtension
    {
        public static int GetStartIndex(this List<Row> rows, string startRegex)
        {
            var startIndex = -1;
            if (!string.IsNullOrEmpty(startRegex))
            {
                for (var index = 0; index < rows.Count; index++)
                {
                    var r = rows[index];
                    var cellNo = 0;
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
    }
}