using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfRepresantation
{
    class LineGenarator
    {
        private readonly float pageWidth;

        public LineGenarator(float pageWidth)
        {
            this.pageWidth = pageWidth;
        }

        public IEnumerable<PdfTextLineDetails> CreateLines(IGrouping<float, PdfTextBlock> group, bool pageRTL)
        {
            var linesInRow = new List<PdfTextLineDetails>();
            IList<PdfTextBlock> blocks;
            float left, right, top;
            PdfTextBlock last;

            void InitProperties()
            {
                blocks = new List<PdfTextBlock>();
                left = int.MaxValue;
                right = 0;
                top = group.Key;
                last = null;
            }

            void AddLine()
            {
                if (blocks.Count == 0)
                    return;
                var lineTexts = MergeTextInLine(blocks, pageRTL,out var spacesAdded);
                if (lineTexts.Any(t => !string.IsNullOrWhiteSpace(t.Value)))
                {
                    if(spacesAdded>0)
                        if (pageRTL)
                            left -= spacesAdded;
                        else
                            right += spacesAdded;
                    linesInRow.Add(new PdfTextLineDetails
                    {
                        Bottom = group.Key,
                        Left = left,
                        Right = pageWidth - right,
                        Texts = lineTexts,
                        Top = top,
                        Width = right - left,
                        Height = group.Key - top,
                    });
                    
                }
                InitProperties();
            }

            InitProperties();
            foreach (var current in group)
            {
                if (string.IsNullOrWhiteSpace(current.Value) &&
                    (last == null || last.End >= current.End || string.IsNullOrWhiteSpace(last.Value)))
                    continue;
                if (last != null)
                {
                    if (last.End + last.CharSpacing < current.Start)
                    {
                        if (last.End + last.CharSpacing * 2 < current.Start)
                            AddLine();
                        else
                            blocks.Add(new PdfTextBlock
                            {
                                Bottom = current.Bottom,
                                CharSpacing = current.CharSpacing,
                                Font = current.Font,
                                FontSize = current.FontSize,
                                StrokeColore = current.StrokeColore,
                                IsRightToLeft = current.IsRightToLeft,
                                Left = pageRTL ? current.Right : last.Right,
                                Width = current.Start - last.End,
                                Top = current.Top,
                                Link = current.Link,
                                Value = " "
                            });
                    }
                }

                blocks.Add(current);
                if (left > current.Left)
                    left = current.Left;
                if (right < current.Right)
                    right = current.Right;
                if (top > current.Top)
                    top = current.Top;
                last = current;
            }

            AddLine();

            return linesInRow;
        }


//        private IList<PdfTextResult> MergeTextInLine(IList<PdfTextBlock> blocks, bool pageRTL)
//        {
//            var result = new LinkedList<PdfTextResult> { };
//            LinkedListNode<PdfTextResult> firstNode = null;
//            PdfTextBlock last = null;
//            PdfTextResult text = null;
//            PdfLinkResult link = null;
//            for (var index = 0; index < blocks.Count; index++)
//            {
//                var current = blocks[index];
//                bool currentRightToLeft = RightToLeftManager.Instance.AssignNeutral(pageRTL, current, blocks, index);
////                bool digitLtr = current.IsDigit && last?.IsDigit==true;
//                if (last != null &&
//                    Equals(last.StrokeColore, current.StrokeColore) &&
//                    last.FontSize == current.FontSize &&
//                    last.Font == current.Font &&
//                    last.Link == current.Link &&
//                    last.IsRightToLeft == current.IsRightToLeft)
//                {
//                    if (currentRightToLeft) //&&!digitLtr)
//                        text.Value = current.Value + text.Value;
//                    else
//
//                        text.Value += current.Value;
//                }
//                else
//                {
//                    var stateRtl =
//                        RightToLeftManager.Instance.PageElemtRtl(pageRTL, currentRightToLeft); // && !digitLtr);
//                    SeperateRtlLtr(stateRtl, current, last, text);
//                    text = new PdfTextResult
//                    {
//                        FontSize = current.FontSize,
//                        Font = current.Font,
//                        StrokeColore = current.StrokeColore,
//                        Value = current.Value,
//                    };
//                    AssignLink(current,text , ref link);
//                    
//                    AddNewText(stateRtl, result, text, ref firstNode);
//                }
//
//                last = current;
//            }
//
//            return result.ToArray();
//        }
        private IList<PdfTextResult> MergeTextInLine(IList<PdfTextBlock> blocks, bool pageRTL, out float spaceAdded)
        {
            var result = new LinkedList<PdfTextResult> { };
            LinkedListNode<PdfTextResult> firstNode = null;
            PdfTextBlock last = null;
            PdfTextResult text = null;
            PdfLinkResult link = null;
            spaceAdded = 0;
            for (var index = 0; index < blocks.Count; index++)
            {
                var current = blocks[index];
                bool currentRightToLeft =
                    RightToLeftManager.Instance.AssignNeutral(pageRTL, current, blocks, index);
                bool opositeDirection = pageRTL != currentRightToLeft;
//                bool digitLtr = current.IsDigit && last?.IsDigit==true;
                if (last != null &&
                    Equals(last.StrokeColore, current.StrokeColore) &&
                    last.FontSize == current.FontSize &&
                    last.Font == current.Font &&
                    last.Link == current.Link &&
                    last.IsRightToLeft == current.IsRightToLeft)
                {
                    if (opositeDirection) //&&!digitLtr)
                        text.Value = current.Value + text.Value;
                    else
                        text.Value += current.Value;
                }
                else
                {
//                    var stateRtl =
//                        RightToLeftManager.Instance.PageElemtRtl(pageRTL, currentRightToLeft); // && !digitLtr);
                    spaceAdded+=   SeperateRtlLtr(opositeDirection, pageRTL, current, last, text);
                    text = new PdfTextResult
                    {
                        FontSize = current.FontSize,
                        Font = current.Font,
                        StrokeColore = current.StrokeColore,
                        Value = current.Value,
                    };
                    AssignLink(current, text, ref link);

                    AddNewText(opositeDirection, result, text, ref firstNode);
                }

                last = current;
            }

            return result.ToArray();
        }

        private void AssignLink(PdfTextBlock current, PdfTextResult text, ref PdfLinkResult link)
        {
            if (current.Link != null)
            {
                if (!string.Equals(link?.Link, current.Link))
                    link = new PdfLinkResult {Link = current.Link};
                link.Children.Add(text);
                text.LinkParent = link;
            }
        }

//        private void SeperateRtlLtr(PageElemtRtl stateRtl,
//            PdfTextBlock current, PdfTextBlock lastBlock, PdfTextResult lastText)
//        {
//            switch (stateRtl)
//            {
//                case PageElemtRtl.Pright_Eright: break;
//                case PageElemtRtl.Pleft_Eleft:
//                    if (lastBlock?.IsRightToLeft == true &&
//                        //  !lastBlock.IsDigit && 
//                        !RightToLeftManager.Instance.IsNeutral(current.Value.Take(1)))
//                    {
//                        current.Value = " " + current.Value;
//                    }
//
//                    break;
//                case PageElemtRtl.Pright_Eleft:
//                    if (lastBlock?.IsRightToLeft == true &&
//                        //!lastBlock.IsDigit && 
//                        !RightToLeftManager.Instance.IsNeutral(lastBlock.Value.Take(1)))
//                    {
//                        lastText.Value = " " + lastText.Value;
//                    }
//
//                    break;
//                case PageElemtRtl.Pleft_Eright: break;
//                default: throw new ArgumentOutOfRangeException();
//            }
//        }
        private float SeperateRtlLtr(bool opositeDirection, bool pageRtl,
            PdfTextBlock current, PdfTextBlock lastBlock, PdfTextResult lastText)
        {
            if (opositeDirection)
            {
                if (lastBlock?.IsRightToLeft == pageRtl &&
                    //!lastBlock.IsDigit && 
                    !RightToLeftManager.Instance.IsNeutral(lastBlock.Value[lastBlock.Value.Length-1]))
                {
                    lastText.Value =  lastText.Value+" ";
                    return lastBlock.CharSpacing;
                }
            }
            else
            {
                if (lastBlock?.IsRightToLeft == !pageRtl &&
                    //  !lastBlock.IsDigit && 
                    !RightToLeftManager.Instance.IsNeutral(current.Value[0]))
                {
                    current.Value = " " + current.Value;
                    return current.CharSpacing;
                }
            }

            return 0;
        }

        private static void AddNewText(bool opositeDirection, LinkedList<PdfTextResult> result, PdfTextResult text,
            ref LinkedListNode<PdfTextResult> firstNode)
        {
            if (opositeDirection)
            {
                if (firstNode == null)
                    result.AddFirst(text);
                else
                    result.AddAfter(firstNode, text);
            }
            else
            {
                firstNode = result.AddLast(text);
            }
        }

//        private static void AddNewText(PageElemtRtl stateRtl, LinkedList<PdfTextResult> result, PdfTextResult text,
//            ref LinkedListNode<PdfTextResult> firstNode)
//        {
//            switch (stateRtl)
//            {
//                case PageElemtRtl.Pright_Eright:
//                    result.AddFirst(text);
//                    firstNode = null;
//                    break;
//                case PageElemtRtl.Pleft_Eleft:
//                    firstNode = result.AddLast(text);
//                    break;
//                case PageElemtRtl.Pright_Eleft:
//                    if (firstNode == null)
//                        result.AddFirst(text);
//                    else
//                        firstNode = result.AddAfter(firstNode, text);
//                    break;
//                case PageElemtRtl.Pleft_Eright:
//                    if (firstNode == null)
//                        result.AddFirst(text);
//                    else
//                        result.AddAfter(firstNode, text);
//                    break;
//                default: throw new ArgumentOutOfRangeException();
//            }
//        }
        public IList<PdfTextLineDetails> CreateLines(IList<PdfTextBlock> texts, bool pageRTL)
        {
            AssignStartEnd(texts, pageRTL);
            return texts
                .OrderBy(t => t.Start)
                .GroupBy(t => t.Bottom)
                .OrderBy(g => g.Key)
                .SelectMany(g => CreateLines(g, pageRTL))
                .ToList();
        }

        private void AssignStartEnd(IList<PdfTextBlock> texts, bool pageRTL)
        {
            foreach (var t in texts)
            {
                if (pageRTL)
                {
                    t.Start = pageWidth - t.Right;
                    t.End = pageWidth - t.Left;
                }
                else
                {
                    t.Start = t.Left;
                    t.End = t.Right;
                }
            }
        }
    }
}