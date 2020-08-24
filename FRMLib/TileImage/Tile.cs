using FRMLib.Scope;
using System.Drawing;

namespace FRMLib.TileImage
{
    public class Tile : RectangleD
    { 
        public string ImageFile { get; set; }

        public Tile()
        {

        }

        public Tile(string image, PointD pos)
        {
            ImageFile = image;
            Position = pos;
            Size = (V2D)Image.FromFile(image).Size;
        }

        public Image Image { get { return System.Drawing.Image.FromFile(ImageFile); } }

        /*
        /// <summary>
        /// Checks if a sertain point is within this tile.
        /// </summary>
        /// <returns></returns>
        public bool PointInTile(PointD pt)
        {
            PointD topLeft = Position;
            PointD botRight = (PointD)(Position + Size);

            if (pt.X >= topLeft.X && pt.X < botRight.X)
                if (pt.Y >= topLeft.Y && pt.Y < botRight.Y)
                    return true;
            return false;
        }
        */
    }


}
