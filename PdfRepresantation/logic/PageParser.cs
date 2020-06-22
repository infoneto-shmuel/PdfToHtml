using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfRepresantation
{
    public class PageParser : IEventListener
    {
        protected readonly ImageParser imageParser;
        protected readonly ShapeParser shapeParser;
        protected readonly TextParser textParser;
        private readonly LinesGenerator lineGenerator;
        protected readonly PdfPage page;
        protected readonly int pageNumber;
        private readonly float pageHeight;
        private readonly float pageWidth;

        public PageParser(PdfPage page, int pageNumber)
        {
            this.page = page;
            this.pageNumber = pageNumber;
            var pageSize = page.GetPageSize();
            pageHeight = pageSize.GetHeight();
            pageWidth = pageSize.GetWidth();
            var linkManager = new LinkManager(pageHeight, page);
            lineGenerator = new LinesGenerator(pageWidth);
            imageParser = new ImageParser(pageHeight, pageWidth);
            this.shapeParser = new ShapeParser(pageHeight, pageNumber);
            textParser = new TextParser(pageHeight, pageWidth, linkManager);
            linkManager.FindLinks();
        }

        public void EventOccurred(IEventData data, EventType type)
        {
            switch (type)
            {
                case EventType.BEGIN_TEXT:
                case EventType.END_TEXT:
                case EventType.CLIP_PATH_CHANGED: break;
                case EventType.RENDER_PATH:
                    shapeParser.ParsePath((PathRenderInfo) data);
                    break;
                case EventType.RENDER_TEXT:
                    textParser.ParseText((TextRenderInfo) data);
                    break;
                case EventType.RENDER_IMAGE:
                    imageParser.ParseImage((ImageRenderInfo) data);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }


        public ICollection<EventType> GetSupportedEvents()
        {
            return new[] {EventType.RENDER_TEXT, EventType.RENDER_IMAGE, EventType.RENDER_PATH};
        }

        public virtual PdfPageDetails CreatePageDetails()
        {
            var pageRightToLeft = RightToLeftManager.Instance.FindRightToLeft(textParser.texts);
            var lines = lineGenerator.CreateLines(textParser.texts, pageRightToLeft);

            return new PdfPageDetails
            {
                Lines = lines,
                RightToLeft = pageRightToLeft,
                Images = imageParser.images,
                PageNumber = pageNumber,
                Shapes = shapeParser.shapes,
                Fonts = textParser.fonts.Values.ToList(),
                Height = pageHeight,
                Width = pageWidth
            };
        }
    }
}