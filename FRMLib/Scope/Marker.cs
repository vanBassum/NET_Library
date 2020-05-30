using FRMLib.Scope.Controls;
using STDLib.Misc;
using System.Collections.Generic;
using System.Drawing;

namespace FRMLib.Scope
{
    public class Marker : PropertySensitive
    {
        private static int nextId = 0;
        [TraceViewAttribute(Width = 25)]
        public int ID { get; /*private set;*/ } = nextId++;
        public Pen Pen { get { return GetPar(new Pen(Color.White) { DashPattern = new float[] { 4.0F, 4.0F, 8.0F, 4.0F } }); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 50)]
        public double X { get { return GetPar<double>(0); } set { SetPar<double>(value); } }

    }
}
