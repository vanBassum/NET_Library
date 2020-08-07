using FRMLib.Scope.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using STDLib.Misc;
using System.Xml.Schema;

namespace FRMLib.Scope.MathFunctions
{
    public class Invert : Trace
    {
        [TraceViewAttribute]
        public Trace T1 { get { return GetPar<Trace>(null); } set 
            { 
                SetPar(value); 
                Recalculate(); 
                value.Points.ListChanged += (a,b) => Recalculate(); 
            } 
        }


        public override void Recalculate()
        {
            if(T1 != null)
            {
                Points.Clear();
                foreach (var pt in T1.Points)
                    Points.Add(pt.X, -pt.Y);
            }
        }
    }
}
