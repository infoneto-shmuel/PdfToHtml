using System.Linq;

namespace PdfRepresentation.Extensions
{
    public static class ArrayExtension
    {
        public static TItem[] RemoveAt<TItem>(this TItem[] cells, int index, out TItem removedItem)
        {
            var list = cells.ToList();
            removedItem = list[index];
            list.RemoveAt(index);
            var newArray = list.ToArray();
            return newArray;
        }
    }
}
