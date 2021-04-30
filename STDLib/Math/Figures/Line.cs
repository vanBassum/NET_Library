namespace STDLib.Math.Figures
{
    public class Line : Figure
    {
        public V2D P1 { get; set; }
        public V2D P2 { get; set; }

        public Line()
        {

        }

        public Line(V2D p1, V2D p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Line(double x1, double y1, double x2, double y2)
        {
            P1 = new V2D(x1, y1);
            P2 = new V2D(x2, y2);
        }
    }
}
