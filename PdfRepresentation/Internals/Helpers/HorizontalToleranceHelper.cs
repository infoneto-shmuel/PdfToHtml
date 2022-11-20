using System;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Helpers
{
    internal static class HorizontalToleranceHelper
    {
        public static Func<Cell, Cell, double> EnsureGetHorizontalTolerance(Func<Cell, Cell, double> getHorizontalTolerance)
        {
            return (getHorizontalTolerance ?? TablesModel.DefaultGetHorizontalTolerance);
        }
    }
}
