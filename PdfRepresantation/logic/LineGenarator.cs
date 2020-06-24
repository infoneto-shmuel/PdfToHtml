using System;
using System.Collections.Generic;
using System.Linq;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfRepresantation
{
    class LineGenarator
    {
        private readonly PageContext pageContext;
        private readonly IGrouping<int, PdfTextBlock> group;
        private List<PdfTextLineDetails> lines;

        IList<PdfTextBlock> blocks;
        float left, right, top;
        PdfTextBlock last;

        internal LineGenarator(PageContext pageContext, IGrouping<int, PdfTextBlock> group)
        {
            this.pageContext = pageContext;
            this.group = group;
        }

        public IEnumerable<PdfTextLineDetails> Lines
        {
            get
            {
                if (lines == null)
                    CalculateLines();

                return lines;
            }
        }


        void AddLine()
        {
            if (blocks.Count == 0)
                return;
            var lineTexts = MergeTextInLine();
            if (lineTexts.All(t => string.IsNullOrWhiteSpace(t.Value)))
                return;
            if (Log.DebugSupported)
                Log.Debug("line:" + string.Join("", lineTexts.Select(t => t.Value)));
            lines.Add(new PdfTextLineDetails
            {
                Bottom = @group.Key/2F,
                Left = left,
                Right = pageContext.PageWidth - right,
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
            if (last.Value == " ")
            {
                last.Width = current.Start - last.Start;
                if (pageContext.PageRTL)
                    last.Left = current.Right;
            }
            else if (current.Value == " ")
            {
                current.Width = current.End - last.End;
                if (!pageContext.PageRTL)
                    current.Left = last.Right;
            }
            else
            {
                Log.Debug($"adding space between '{last.Value}'-'{current.Value}'");
                blocks.Add(new PdfTextBlock
                {
                    Bottom = current.Bottom,
                    CharSpacing = current.CharSpacing,
                    Font = current.Font,
                    FontSize = current.FontSize,
                    StrokeColore = current.StrokeColore,
                    IsRightToLeft = current.IsRightToLeft,
                    Left = pageContext.PageRTL ? current.Right : last.Right,
                    Width = current.Start - last.End,
                    Top = current.Top,
                    Link = current.Link,
                    Value = " "
                });
            }
        }


        private IList<PdfTextResult> MergeTextInLine()
        {
            var result = new LinkedList<PdfTextResult> { };
            LinkedListNode<PdfTextResult> firstNode = null;
            PdfTextBlock lastBlock = null;
            PdfTextResult text = null;
            PdfLinkResult link = null;
            for (var index = 0; index < blocks.Count; index++)
            {
                var current = blocks[index];
                bool currentRTL =
                    RightToLeftManager.Instance.AssignNeutral(pageContext.PageRTL, current, blocks, index);
                bool opositeDirection = pageContext.PageRTL != currentRTL;
//                bool digitLtr = current.IsDigit && last?.IsDigit==true;
                if (lastBlock != null &&
                    Equals(lastBlock.StrokeColore, current.StrokeColore) &&
                    lastBlock.FontSize == current.FontSize &&
                    lastBlock.Font == current.Font &&
                    lastBlock.Link == current.Link &&
                    lastBlock.IsRightToLeft == current.IsRightToLeft)
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
                    SeperateRtlLtr(opositeDirection, pageContext.PageRTL, current, lastBlock, text);
                    text = new PdfTextResult
                    {
                        FontSize = current.FontSize,
                        Font = current.Font,
                        StrokeColore = current.StrokeColore,
                        Value = current.Value,
                    };
                    pageContext.LinkManager.AssignLink(current, text, ref link);

                    AddNewText(opositeDirection, result, text, ref firstNode);
                }

                lastBlock = current;
            }

            return result.ToArray();
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
                    Log.Debug("seperated:" + lastBlock.Value + "-" + current.Value);
                }
            }
            else
            {
                if (lastBlock?.IsRightToLeft == !pageRtl &&
                    //  !lastBlock.IsDigit && 
                    !RightToLeftManager.Instance.IsNeutral(current.Value[0]))
                {
                    current.Value = " " + current.Value;
                    Log.Debug("seperated:" + lastText.Value + "-" + current.Value);
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