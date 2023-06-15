namespace FormsLib.Maths
{
    public class Linear
    {
        //A * X + B
        double A { get; set; }
        double B { get; set; }


        public static Linear FromSamples(double x1, double x2, double y1, double y2)
        {
            Linear lin = new Linear();
            lin.A = (y2 - y1) / (x2 - x1);
            lin.B = y1 - lin.A * x1;
            return lin;
        }

        public double Y(double X)
        {
            return A * X + B;
        }

        public double X(double Y)
        {
            return (Y - B) / A;
        }
    }
}
