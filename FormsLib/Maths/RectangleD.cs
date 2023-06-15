namespace FormsLib.Maths
{
    public class RectangleD
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public SizeD Size { get { return new SizeD(Width, Height); } set { Width = value.Width; Height = value.Height; } }
        public PointD Position { get { return new PointD(X, Y); } set { X = value.X; Y = value.Y; } }


        public RectangleD()
        {

        }

        public RectangleD(V2D pos, V2D size)
        {
            X = pos.X;
            Y = pos.Y;
            Width = size.X;
            Height = size.Y;
        }


        /// <summary>
        /// Checks weter 2 rectangles collide.
        /// </summary>
        /// https://developer.mozilla.org/en-US/docs/Games/Techniques/2D_collision_detection
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Collides(RectangleD a, RectangleD b)
        {
            if (a.X < b.X + b.Width &&
               a.X + a.Width > b.X &&
               a.Y < b.Y + b.Height &&
               a.Y + a.Height > b.Y)
            {
                return true;
            }
            return false;
        }
    }
}
