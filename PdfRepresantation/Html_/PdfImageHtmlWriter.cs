using System;
using System.IO;
using System.Text;
using PdfRepresantation.Model.Pdf;

namespace PdfRepresantation.Html
{
    public class PdfImageHtmlWriter
    {
        private int indexImage = 1;
        private readonly bool embeddedImages;
        private readonly string dirImages;

        public PdfImageHtmlWriter(bool embeddedImages, string dirImages)
        {
            this.embeddedImages = embeddedImages;
            this.dirImages = dirImages;
            if (embeddedImages&&dirImages != null && Directory.Exists(dirImages))
                Directory.CreateDirectory(dirImages);

        }

        public virtual void AddImage(PageDetails page, ImageDetails image, StringBuilder sb)
        {
            sb.Append(@"
        <img class=""image"" height=""").Append(image.Height)
                .Append("\" width=\"")
                .Append(image.Width).Append("\" src=\"");
            if (embeddedImages || dirImages == null)
            {
                sb.Append("data:image/png;base64, ")
                    .Append(Convert.ToBase64String(image.Buffer));
            }
            else
            {
                string path;
                lock (this)
                {
                    FileInfo file;
                    do file = new FileInfo(Path.Combine(dirImages, "image" + indexImage++ + ".png"));
                    while (file.Exists);
                    path = file.FullName;
                    File.WriteAllBytes(path, image.Buffer);
                }

                sb.Append(path);
            }

            sb.Append("\" style=\"")
                .Append(page.RightToLeft ? "right" : "left")
                .Append(":").Append((int) ((page.RightToLeft ? image.Right : image.Left)))
                .Append("px; top:").Append((int) (image.Top)).Append("px\">");
            ;
        }

        public virtual void AddStyle(StringBuilder sb)
        {
                sb.Append(@"
        .image{position:absolute}");
        }
    }
}