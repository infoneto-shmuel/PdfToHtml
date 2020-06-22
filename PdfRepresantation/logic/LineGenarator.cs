using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfRepresantation
{
    class LineGenarator
    {
        private readonly float pageWidth;
        private readonly IGrouping<float, PdfTextBlock> group;
        private readonly bool pageRTL;
        private List<PdfTextLineDetails> lines;

        IList<PdfTextBlock> blocks;
        float left, right, top;
        PdfTextBlock last;

        public IEnumerable<PdfTextLineDetails> Lines
        {
            get
            {
                if (lines == null)
                    CalculateLines();

                return lines;
            }
        }

        public LineGenarator(float pageWidth, bool pageRtl, IGrouping<float, PdfTextBlock> @group)
        {
            this.pageWidth = pageWidth;
            pageRTL = pageRtl;
            this.@group = @group;
        }

        void AddLine()
        {
            if (blocks.Count == 0)
                return;
            var lineTexts = MergeTextInLine();
            if (lineTexts.All(t => string.IsNullOrWhiteSpace(t.Value)))
                return;
            lines.Add(new PdfTextLineDetails
            {
                Bottom = @group.Key,
                Left = left,
                Right = pageWidth - right,
                Texts = lineTexts,
                Top = top,
                Width = right - left,
                Height = @group.Key - top,
            });
        }

        void InitProperties()
        {
            blocks = new List<PdfTextBlock>();
            left = int.MaxValue;
            right = 0;
            top = group.Key;
            last = null;
        }

        private void CalculateLines()
        {
            lines = new List<PdfTextLineDetails>();
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
                        {
                            AddLine();
                            InitProperties();
                        }
                        else
                        {
                            AddSpace(current);
                        }
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
        }

        private void AddSpace(PdfTextBlock current)
        {
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


        private IList<PdfTextResult> MergeTextInLine()
        {
            var result = new LinkedList<PdfTextResult> { };
            LinkedListNode<PdfTextResult> firstNode = null;
            PdfTextBlock last = null;
            PdfTextResult text = null;
            PdfLinkResult link = null;
            for (var index = 0; index < blocks.Count; index++)
            {
                var current = blocks[index];
                bool currentRTL =
                    RightToLeftManager.Instance.AssignNeutral(pageRTL, current, blocks, index);
                bool opositeDirection = pageRTL != currentRTL;
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
                    SeperateRtlLtr(opositeDirection, pageRTL, current, last, text);
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
        private void SeperateRtlLtr(bool opositeDirection, bool pageRtl,
            PdfTextBlock current, PdfTextBlock lastBlock, PdfTextResult lastText)
        {
            if (opositeDirection)
            {
                if (lastBlock?.IsRightToLeft == pageRtl &&
                    //!lastBlock.IsDigit && 
                    !RightToLeftManager.Instance.IsNeutral(lastBlock.Value[lastBlock.Value.Length - 1]))
                {
                    lastText.Value = lastText.Value + " ";
                }
            }
            else
            {
                if (lastBlock?.IsRightToLeft == !pageRtl &&
                    //  !lastBlock.IsDigit && 
                    !RightToLeftManager.Instance.IsNeutral(current.Value[0]))
                {
                    current.Value = " " + current.Value;
                }
            }
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
    }
}