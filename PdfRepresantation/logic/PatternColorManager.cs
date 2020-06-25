using System;
using System.Collections.Generic;
using System.Drawing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Colorspace;
using iText.Kernel.Pdf.Function;

namespace PdfRepresantation
{
    class PatternColorManager
    {
        public static Color? GetColor(PdfPattern pattern, float alpha)
        {
            //not supported yet
            return null;
            switch (pattern)
            {
                case PdfPattern.Shading shading:
                    return GetColor(shading, alpha);
                case PdfPattern.Tiling tiling:
                    return GetColor(tiling, alpha);
                default: throw new IndexOutOfRangeException();
            }
        }

        private static Color? GetColor(PdfPattern.Shading shading, float alpha)
        {
            var shadingDict = shading.GetShading();
            var shadingConstructed = PdfShading.MakeShading(shadingDict);
            switch (shadingConstructed)
            {
                case PdfShading.Axial axial:
                    var axialFunction = (PdfDictionary) axial.GetFunction();
                    GetFunctionDetails(axialFunction);
                    var coords = GetItemsFloat(axial.GetCoords());
                    var domain = GetItemsFloat(axial.GetDomain());
                    var extend = GetItemsBool(axial.GetExtend());

                    break;
                case PdfShading.CoonsPatchMesh coonsPatchMesh:
                    break;
                case PdfShading.FreeFormGouraudShadedTriangleMesh freeFormGouraudShadedTriangleMesh: break;
                case PdfShading.FunctionBased functionBased: break;
                case PdfShading.LatticeFormGouraudShadedTriangleMesh latticeFormGouraudShadedTriangleMesh:
                    break;
                case PdfShading.Radial radial: break;
                case PdfShading.TensorProductPatchMesh tensorProductPatchMesh: break;
            }

            return null;
        }

        private static Color? GetColor(PdfPattern.Tiling tiling, float alpha)
        {
            switch (tiling.GetTilingType())
            {
                case PdfPattern.Tiling.TilingType.CONSTANT_SPACING:
                case PdfPattern.Tiling.TilingType.CONSTANT_SPACING_AND_FASTER_TILING:
                case PdfPattern.Tiling.TilingType.NO_DISTORTION:
                    break;
            }

            var box = tiling.GetBBox();
            var xStep = tiling.GetXStep();
            var yStep = tiling.GetYStep();
            var colored = tiling.IsColored();
            return null;
        }


        private static PdfDictionary[] GetItemsDict(PdfArray array)
        {
            return GetItems(array, (a, i) => a.GetAsDictionary(i));
        }

        private static float[] GetItemsFloat(PdfArray array)
        {
            return GetItems(array, (a, i) => a.GetAsNumber(i).FloatValue());
        }

        private static int[] GetItemsInt(PdfArray array)
        {
            return GetItems(array, (a, i) => a.GetAsNumber(i).IntValue());
        }

        private static T[] GetItems<T>(PdfArray array, Func<PdfArray, int, T> getOne)
        {
            if (array == null)
                return null;
            T[] result = new T[array.Size()];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = getOne(array, i);
            }

            return result;
        }

        private static bool[] GetItemsBool(PdfArray array)
        {
            return GetItems(array, (a, i) => a.GetAsBoolean(i).GetValue());
        }

        private static void GetFunctionDetails(PdfDictionary dict)
        {
            var function = new PdfFunction(dict);
            int[] domain;
            int[] encode;
            switch (function.GetFunctionType())
            {
                case 0:
                    domain = GetItemsInt(dict.GetAsArray(PdfName.Domain));
                    GetItemsInt(dict.GetAsArray(PdfName.Size));
                    dict.GetAsInt(PdfName.BitsPerSample);
                    dict.GetAsInt(PdfName.Order);
                    encode = GetItemsInt(dict.GetAsArray(PdfName.Encode));
                   var decode = GetItemsInt(dict.GetAsArray(PdfName.Decode));
                   var range = GetItemsInt(dict.GetAsArray(PdfName.Range));
                    break;
                case 2:
                    domain = GetItemsInt(dict.GetAsArray(PdfName.Domain));
                    var color1 = GetItemsFloat(dict.GetAsArray(PdfName.C0));
                    var color2 = GetItemsFloat(dict.GetAsArray(PdfName.C1));
                    var n=dict.GetAsInt(PdfName.N);
                    break;
                case 3:
                    domain = GetItemsInt(dict.GetAsArray(PdfName.Domain));
                    foreach (var sub in GetItemsDict(dict.GetAsArray(PdfName.Functions)))
                    {
                        GetFunctionDetails(sub);
                    }

                    var bounds = GetItemsFloat(dict.GetAsArray(PdfName.Bounds));
                    encode = GetItemsInt(dict.GetAsArray(PdfName.Encode));
                    break;
                case 4:
                    domain = GetItemsInt(dict.GetAsArray(PdfName.Domain));
                    break;
            }
        }
    }
}