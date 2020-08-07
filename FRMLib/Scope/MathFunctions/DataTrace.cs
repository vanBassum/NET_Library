using STDLib.Misc;

namespace FRMLib.Scope.MathFunctions
{
    public class DataTrace : BaseMath
    {
        public DataTrace(ThreadedBindingList<PointD> points) : base(points)
        {

        }

        public Trace T1 { get { return GetPar<Trace>(); } set { SetPar(value); value.PropertyChanged += (a, b) => Recalculate(); Recalculate(); } }

        public override void Recalculate()
        {
            points.Clear();
            foreach (PointD pt in T1.Points)
                points.Add(pt.X, -pt.Y);
        }
    }
}
