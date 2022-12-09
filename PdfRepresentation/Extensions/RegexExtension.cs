using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PdfRepresentation.Extensions
{
    public static class RegexExtension
    {
        public static string[] SplitWithRegexValue(this Regex regex, string value)
        {
            var matches = regex.Matches(value);
            if (matches.Count == 0 || matches.Count == 1)
            {
                return new[] { value };
            }
            var list = new List<string>();
            var previousIndex = 0;
            for (var index = 0; index < matches.Count; index++)
            {
                var match = matches[index];
                var matchIndex = match.Index;
                if (previousIndex != matchIndex)
                {
                    list.Add(value.Substring(previousIndex, matchIndex));
                    previousIndex = matchIndex;
                }

                if (index == matches.Count - 1)
                {
                    list.Add(value.Substring(matchIndex));
                }
            }

            var array = list.ToArray();
            return array;
        }
    }
}
