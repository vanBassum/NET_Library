using FormsLib.Scope.Controls;
using CoreLib.Misc;
using System.Collections.Generic;
using System.Drawing;

namespace FormsLib.Scope
{
    public class Cursor : PropertySensitive
    {
        private static HashSet<int> ids = new HashSet<int>();

        ~Cursor()
        {
            ids.Remove(ID);
        }

        private static int NextId
        {
            get
            {
                int i = 1;
                while (ids.Contains(i))
                    i++;
                ids.Add(i);
                return i;
            }
        }



        [TraceViewAttribute(Width = 25)]
        public int ID { get; /*private set;*/ } = NextId;
        public Pen Pen { get { return GetPar(new Pen(Color.White) { DashPattern = new float[] { 4.0F, 4.0F, 8.0F, 4.0F } }); } set { SetPar(value); } }
        [TraceViewAttribute(AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells)]
        public double X { get { return GetPar<double>(0); } set { SetPar<double>(value); } }
        public Cursor Self { get { return this; } }

    }
}
