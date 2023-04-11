using FormsLib.Scope.Controls;
using CoreLib.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;

namespace FormsLib.Scope
{
    public class MathItem : PropertySensitive
    {
        [TraceViewAttribute(Text = "", Width = 20)]
        public Pen Pen { get { return GetPar(Pens.Red); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 60)]
        public Trace Trace { get { return GetPar<Trace>(null); } set { SetPar(value); } }
        [TraceViewAttribute(Text = "M1", Width = 40)]
        public Cursor Marker1 { get { return GetPar<Cursor>(null); } set { SetPar(value); value.PropertyChanged += Recalculate; } }
        [TraceViewAttribute(Text = "M2", Width = 40)]
        public Cursor Marker2 { get { return GetPar<Cursor>(null); } set { SetPar(value); value.PropertyChanged += Recalculate; } }
        [TraceViewAttribute(Width = 80)]
        public MathFunction Function { get { return GetPar<MathFunction>(null); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 80)]
        public object Value { get { return GetPar<object>(null); } set { SetPar(value); } }


        public MathItem()
        {
            this.PropertyChanged += Recalculate;
        }

        private void Recalculate(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(this.Value))
            {
                if (Function != null && Marker1 != null && Marker2 != null && Trace != null)
                    Value = Function.Calculate(this);
            }
        }

        public void Draw(Graphics g, Func<double, int> scaleY, Func<double, int> scaleX)
        {
            if (Function != null && Marker1 != null && Marker2 != null && Trace != null)
                Function.Draw(g, this, scaleY, scaleX);
        }


        public static BindingList<MathFunction> MathFunctions { get; } = new BindingList<MathFunction>(GetMathFunctions());
        public static MathFunction[] GetMathFunctions()
        {
            var vs = AppDomain.CurrentDomain.GetAssemblies()
                                .SelectMany(s => s.GetTypes())
                                .Where(p => typeof(MathFunction).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract);
            List<MathFunction> mathCalculators = new List<MathFunction>();
            foreach (var v in vs)
                mathCalculators.Add((MathFunction)Activator.CreateInstance(v));
            return mathCalculators.ToArray();
        }


    }




}
