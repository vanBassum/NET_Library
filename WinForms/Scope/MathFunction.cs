using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public abstract class MathFunction
    {
        public string Name { get { return this.GetType().Name; } }
        public abstract object Calculate(MathItem mathItem);
        public virtual void Draw(Graphics g, MathItem mathItem, Func<double, int> scaleY, Func<double, int> scaleX) { }
        public MathFunction Self { get { return this; } }

        public override string ToString()
        {
            return Name;
        }
    }




}
