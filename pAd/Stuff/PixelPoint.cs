using System.Xml.Serialization;

namespace Pingvi
{
    /// <summary>
    ///     Represent pixel coordinates on screen
    /// </summary>
    public class PixelPoint
    {
        public PixelPoint()
        {
        }

        public PixelPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        [XmlAttribute]
        public int X { get; set; }

        [XmlAttribute]
        public int Y { get; set; }
    }
}