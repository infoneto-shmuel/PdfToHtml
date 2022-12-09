using System.Globalization;

namespace PdfRepresentation.Model.Xml
{
    public partial class Cell
    {
        public double GetBottom()
        {
            double.TryParse(Bottom, NumberStyles.Any, CultureInfo.InvariantCulture, out var bottom);
            return bottom;
        }

        public double GetDistance(Cell cell)
        {
            return cell.GetLeft() - GetRight();
        }

        public double GetHeight()
        {
            return GetBottom() - GetTop();
        }

        public double GetLeft()
        {
            double.TryParse(Left, NumberStyles.Any, CultureInfo.InvariantCulture, out var left);
            return left;
        }

        public string GetPaddingTo(Cell cell)
        {
            var distance = GetDistance(cell);
            return distance >= 0 && distance < GetHeight() / 2 ? "" : " ";
        }

        public double GetRight()
        {
            return GetLeft() + GetWidth();
        }

        public double GetTop()
        {
            double.TryParse(Top, NumberStyles.Any, CultureInfo.InvariantCulture, out var top);
            return top;
        }

        public double GetWidth()
        {
            double.TryParse(Width, NumberStyles.Any, CultureInfo.InvariantCulture, out var width);
            return width;
        }

        public bool IsOverlappingVertically(Cell otherCell)
        {
            return (GetTop() >= otherCell.GetTop() &&
                    GetTop() <= otherCell.GetTop() + otherCell.GetHeight()) ||
                   (otherCell.GetTop() >= GetTop() &&
                    otherCell.GetTop() <= GetTop() + GetHeight());
        }
    }
}