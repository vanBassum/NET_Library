using System.Drawing;

namespace FRMLib.Scope.MathTypes
{


    public class DeltaX : IMathType
    {
        public string Name { get { return this.GetType().Name; } }
        public IMathType Self { get { return this; } }

        public object GetValue(ScopeController scope, Marker m1, Marker m2)
        {



            return "todo";
        }

        public void Plot(Graphics g)
        {
            //TODO
        }
    }




}
