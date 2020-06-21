﻿using System;
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
using Rectangle = System.Drawing.Rectangle;

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
            var image = data.GetImage();
            var maskStream = image.GetPdfObject().GetAsStream(name);
            if (maskStream == null)
                return false;
            var bitmapImage = Bitmap.FromStream(new MemoryStream(bytes)) as Bitmap;


            var maskImage = new PdfImageXObject(maskStream);
            var bytesMask = maskImage.GetImageBytes();
            var bitmapMask = Bitmap.FromStream(new MemoryStream(bytesMask)) as Bitmap;
   
            var bitmapResult = ApplyMask(bitmapImage, bitmapMask);
       
            using (var memoryStream = new MemoryStream())
            {
                bitmapResult.Save(memoryStream, bitmapImage.RawFormat);
                bytes = memoryStream.GetBuffer();
            }

            return true;
        }

        private static Bitmap ApplyMask(Bitmap input, Bitmap mask)
        {
            Bitmap output = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
            output.MakeTransparent();
            var rect = new Rectangle(0, 0, input.Width, input.Height);

            var bitsMask = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bitsInput = input.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var bitsOutput = output.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            unsafe
            {
                for (int y = 0; y < input.Height; y++)
                {
                    byte* ptrMask = (byte*) bitsMask.Scan0 + y * bitsMask.Stride;
                    byte* ptrInput = (byte*) bitsInput.Scan0 + y * bitsInput.Stride;
                    byte* ptrOutput = (byte*) bitsOutput.Scan0 + y * bitsOutput.Stride;
                    for (int x = 0; x < input.Width; x++)
                    {
                        var index = 4 * x;
                        ptrOutput[index] = ptrInput[index]; // blue
                        ptrOutput[index + 1] = ptrInput[index + 1]; // green
                        ptrOutput[index + 2] = ptrInput[index + 2]; // red

                        ptrOutput[index + 3] = ptrMask[index]; //alpha
                    }
                }
            }

            mask.UnlockBits(bitsMask);
            input.UnlockBits(bitsInput);
            output.UnlockBits(bitsOutput);

            return output;
        }
    }
}