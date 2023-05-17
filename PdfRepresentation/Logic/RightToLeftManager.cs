using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PdfRepresentation.Logic
{
    public class RightToLeftManager
    {
        public static readonly RightToLeftManager Instance = new RightToLeftManager();

        Regex regexRtl = new Regex(@"[\u0600-\u06FF\u0590-\u05FF]");
//        Regex regexRtlNeutral = new Regex(@"^[\s\[\]\(\)\:;\{\}\\\/\.\|\,=”""'\-\–\!\?\*\•]+$");
//        Regex regexDigit = new Regex(@"^[\d]+$");

        public bool IsRTL(string text)
        {
            return regexRtl.IsMatch(text);
        }

//        public bool IsDigit(string text)
//        {
//            return regexDigit.IsMatch(text);
//        }
        public bool IsNeutral(IEnumerable<char> text)
        {
            return text.All(IsNeutral);
//            return regexRtlNeutral.IsMatch(text as string??new string(text.ToArray()));
        }

        public bool IsNeutral(char c)
        {
            return !char.IsLetterOrDigit(c);
        }

        internal bool FindRightToLeft(List<PdfTextBlock> texts)
        {
            int rtls = 0, ltrs = 0;
            foreach (var b in texts)
            {
                switch (b.IsRightToLeft)
                {
                    case true:
                        rtls++;
                        break;
                    case false:
                        ltrs++;
                        break;
                }
            }

            return rtls > ltrs;
        }

        internal bool FindRtlOfNeutral(bool pageRightToLeft, List<PdfTextBlock> blocks, int index)
        {
//            for (int i = index - 1; i >= 0; i--)
//            {
//                if (blocks[i].IsRightToLeft == false)
//                    return false;
//            }
            bool foundOppositeStart = false;
            for (int i = index - 1; i >= 0; i--)
            {
                if (blocks[i].IsRightToLeft.HasValue)
                    if (blocks[i].IsRightToLeft.Value == pageRightToLeft)
                        return pageRightToLeft;
                    else
                    {
                        foundOppositeStart = true;
                        break;
                    }
            }

            bool foundOppositeEnd = false;

            for (int i = index + 1; i < blocks.Count; i++)
            {
                if (blocks[i].IsRightToLeft.HasValue)
                    if (blocks[i].IsRightToLeft.Value == pageRightToLeft)
                        return pageRightToLeft;
                    else
                    {
                        foundOppositeEnd = true;
                        break;
                    }
            }

            return foundOppositeStart && foundOppositeEnd ? !pageRightToLeft : pageRightToLeft;
//            if (!pageRightToLeft)
//            {
//                for (int i = index - 1; i >= 0; i--)
//                {
//                    if (blocks[i].IsRightToLeft.HasValue)
//                        return blocks[i].IsRightToLeft.Value;
//                }
//
//                return true;
//            }
//            else
//            {
//                for (int i = index + 1; i < blocks.Count; i++)
//                {
//                    if (blocks[i].IsRightToLeft.HasValue)
//                        return blocks[i].IsRightToLeft.Value;
//                }
//
//                return false;
//            }
        }

        public bool AssignNeutral(bool pageRightToLeft, PdfTextBlock current, List<PdfTextBlock> blocks, int index)
        {
            if (current.IsRightToLeft != null)
                return current.IsRightToLeft.Value;
            var currentRightToLeft = FindRtlOfNeutral(pageRightToLeft, blocks, index);
            current.IsRightToLeft = currentRightToLeft;
            if (currentRightToLeft) // && !current.IsDigit)
            {
                var chars = new char[current.Value.Length];
                for (var i = 0; i < current.Value.Length; i++)
                {
                    var c = current.Value[i];
                    switch (c)
                    {
                        case '[':
                            c = ']';
                            break;
                        case ']':
                            c = '[';
                            break;
                        case '(':
                            c = ')';
                            break;
                        case ')':
                            c = '(';
                            break;
                        case '{':
                            c = '}';
                            break;
                        case '}':
                            c = '{';
                            break;
                    }

                    chars[chars.Length - 1 - i] = c;
                }

                current.Value = new string(chars);
            }

            return currentRightToLeft;
        }

        public void AssignRtl(PdfTextBlock item, bool reversed)
        {
            var text = item.Value;
            if (IsRTL(text))
            {
                item.IsRightToLeft = true;
                if (!reversed)
                    ReverseText(item);
            }
            else if (!IsNeutral(text))
            {
                item.IsRightToLeft = false;
                if (reversed)
                    ReverseText(item);
            }

//            else if (IsDigit(text))
//            {
//                item.IsDigit = true;
//            }
        }

        private void ReverseText(PdfTextBlock item)
        {
            item.Value = new string(item.Value.Reverse().ToArray());
        }
    }
}