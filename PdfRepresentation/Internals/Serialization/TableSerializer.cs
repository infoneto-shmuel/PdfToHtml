using System.Linq;
using PdfRepresentation.Model.Xml;

namespace PdfRepresentation.Internals.Serialization
{
    internal class TableSerializer<T>
    {
        public int[] Dimensions { get; }
        public T Data { get; }

        public TableSerializer()
        {
        }

        public TableSerializer(int[] dimensions, T data)
        {
            Dimensions = dimensions;
            Data = data;
        }

        public static TableSerializer<T> GetTableSerializer(T parent, Row[] rows)
        {
            return new TableSerializer<T>(new[]
            {
                rows.Length,
                rows.FirstOrDefault()?.Cells.Length ?? 0
            }, parent);
        }

        public static TableSerializer<T> GetTableSerializer<TRow>(T parent, TRow[] rows)
        {
            return new TableSerializer<T>(new[]
            {
                rows.Length
            }, parent);
        }
    }
}