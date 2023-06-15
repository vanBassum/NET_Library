namespace FormsLib.Maths
{
    public class SizeD
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SizeD()
        {

        }

        public SizeD(double width, double height)
        {
            Width = width;
            Height = height;
        }
        /*
        public SizeD(decimal width, decimal height)
        {
            Width = (double)width;
            Height = (double)height;
        }*/


        public static implicit operator SizeD(V2D s) => new SizeD(s.X, s.Y);
        public static implicit operator V2D(SizeD s) => new V2D(s.Width, s.Height);


        /*
        public static explicit operator SizeD(Size s) => new SizeD(s.Width, s.Height);
        public static explicit operator SizeD(SizeF s) => new SizeD(s.Width, s.Height);
        public static SizeD operator /(SizeD a, double b) => new SizeD(a.Width / b, a.Height / b);
        */
    }

}
