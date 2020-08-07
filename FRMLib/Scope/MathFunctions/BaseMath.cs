using STDLib.Misc;

namespace FRMLib.Scope.MathFunctions
{
    public abstract class BaseMath : PropertySensitive
    {
        public ThreadedBindingList<PointD> points;
        public BaseMath(ThreadedBindingList<PointD> points)
        {
            this.points = points;
        }

        public abstract void Recalculate();
    }
}
