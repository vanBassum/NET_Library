using System.Drawing;

namespace FRMLib.Scope.MathTypes
{
    public interface IMathType
    {
        object GetValue(ScopeController scope, Marker m1, Marker m2);
        void Plot(Graphics g);
        string Name { get; }
        IMathType Self { get; }

    }




}
