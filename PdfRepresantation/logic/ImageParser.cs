using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using iText.IO.Image;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Wmf;
using iText.Kernel.Pdf.Xobject;
using Org.BouncyCastle.Crypto.Modes;
using Image = iText.Layout.Element.Image;
using IOException = iText.IO.IOException;

namespace PdfRepresantation
{
    public class ImageParser
    {
        public readonly IList<PdfImageDetails> images = new List<PdfImageDetails>();
        private readonly float pageHeight;
        private readonly float pageWidth;

        public ImageParser(float pageHeight, float pageWidth)
        {
            this.pageHeight = pageHeight;
            this.pageWidth = pageWidth;
        }

        public virtual void ParseImage(ImageRenderInfo data)
        {
            byte[] bytes;
            try
            {
                bytes = data.GetImage().GetImageBytes();
            }
            catch (IOException e)
            {
                //wrong format of image
                Console.WriteLine(e);
                return;
            }

            if (!MergeMask(data, PdfName.SMask, ref bytes))
                MergeMask(data, PdfName.Mask, ref bytes);


            // var start = data.GetStartPoint();
            var ctm = data.GetImageCtm();
            var width = ctm.Get(Matrix.I11);
            var height = ctm.Get(Matrix.I22);
            var x = ctm.Get(Matrix.I31);
            var y = pageHeight - ctm.Get(Matrix.I32);
            images.Add(new PdfImageDetails
            {
                Buffer = bytes,
                Bottom = y,
                Top = y - height,
                Left = x,
                Right = pageWidth - x - width,
                Width = width,
                Height = height,
            });
        }

        static ImageParser()
        {
            try
            {
                nativeField = typeof(Bitmap)
                    .GetField("nativeImage", BindingFlags.NonPublic | BindingFlags.Instance);
                var nestedType = typeof(Bitmap).Assembly.GetType("System.Drawing.SafeNativeMethods")
                    .GetNestedType("Gdip", BindingFlags.NonPublic | BindingFlags.Static);
                MethodInfo methodSetPixel = nestedType
                    .GetMethod("GdipBitmapSetPixel", BindingFlags.NonPublic | BindingFlags.Static);
                setPixel = (Func<HandleRef, int, int, int, int>) Delegate.CreateDelegate(
                    typeof(Func<HandleRef, int, int, int, int>), null, methodSetPixel);
             }
            catch (Exception)
            {
            }
        }

        private static Func<HandleRef, int, int, int, int> setPixel;
        private static FieldInfo nativeField;
        static int transparent = Color.Transparent.ToArgb();

        private bool MergeMask(ImageRenderInfo data, PdfName name, ref byte[] bytes)
        {
            //return true;
            var image = data.GetImage();
            var maskStream = image.GetPdfObject().GetAsStream(name);
            if (maskStream == null)
                return false;
            var bitmapImage = Bitmap.FromStream(new MemoryStream(bytes)) as Bitmap;


            var maskImage = new PdfImageXObject(maskStream);
            var bytesMask = maskImage.GetImageBytes();
            var bitmapMask = Bitmap.FromStream(new MemoryStream(bytesMask)) as Bitmap;
            HandleRef handleRef;
            try
            {
                var nativeImage = nativeField.GetValue(bitmapImage);
                handleRef = new HandleRef((object) bitmapImage, (IntPtr) nativeImage);
            }
            catch (Exception)
            {
            }

            for (int x = 0; x < image.GetWidth(); x++)
            for (int y = 0; y < image.GetHeight(); y++)
            {
                
                var pixelMask = bitmapMask.GetPixel(x, y);
                if (pixelMask.R == 0)
                {
                    if (setPixel == null)
                        bitmapImage.SetPixel(x, y, Color.Transparent);
                    else 
                        setPixel(handleRef, x, y, transparent);
                }
                else if (pixelMask.R != 255)
                {
                    var old = bitmapImage.GetPixel(x, y);
                    old = Color.FromArgb(255 - pixelMask.R, old.R, old.G, old.B);
                    if (setPixel == null)
                    {
                         bitmapImage.SetPixel(x, y, old);
                    }
                    else
                    {
                        setPixel(handleRef, x, y, old.ToArgb());
                        
                    }
                }
            }

            var memoryStream = new MemoryStream();
            bitmapImage.Save(memoryStream, bitmapImage.RawFormat);
            bytes = memoryStream.GetBuffer();
            return true;
        }
    }
}