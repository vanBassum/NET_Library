using STDLib.Math;

namespace FRMLib.Scope
{
    public class Mark : PointD
    {
        public Mark(PointD pt) : base(pt.X, pt.Y)
        {
        }
        public Mark(double x, double y) : base(x, y)
        {
        }

        public string Text { get; set; }

    }


}
