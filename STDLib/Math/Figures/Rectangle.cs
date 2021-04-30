namespace STDLib.Math.Figures
{
    public class Rectangle : Figure
    {
        public V2D Position { get; set; } = new V2D();
        public SizeD Size { get; set;  } = new V2D();

        public double X { get => Position.X; set => Position.X = value; }
        public double Y { get => Position.Y; set => Position.Y = value; }
        public double Width { get => Size.Width; set => Size.Width = value; }
        public double Height { get => Size.Height; set => Size.Height = value; }

        public V2D[] ToPoints()
        {
            return new V2D[] {
                new V2D(Position.X, Position.Y),
                new V2D(Position.X + Size.Width, Position.Y),
                new V2D(Position.X + Size.Width, Position.Y + Size.Height),
                new V2D(Position.X, Position.Y + Size.Height), 
            };
        }

        public Line[] ToLines()
        {
            V2D[] points = ToPoints();

            return new Line[] {
                new Line(points[0], points[1]),
                new Line(points[1], points[2]),
                new Line(points[2], points[3]),
                new Line(points[3], points[0]),
            };
        }


    }


}
