using STDLib.Misc;
using System.Drawing;

namespace FRMLib.Scope.Controls
{
    public class ScopeViewSettings : PropertySensitive
    {
        public Color BackgroundColor { get { return GetPar(Color.Black); } set { SetPar(value); } }
        public Pen GridPen { get { return GetPar(Pens.White); } set { SetPar(value); } }
        public Pen GridSubPen { get { return GetPar(new Pen(Color.FromArgb(0x30, 0x30, 0x30))); } set { SetPar(value); } }

        /// <summary>
        /// Total number of divisions in the horizontal direction.
        /// </summary>
        public int HorizontalDivisions { get { return GetPar(10); } set { SetPar(value); } }

        /// <summary>
        /// Total number of divisions in the vertical direction.
        /// </summary>
        public int VerticalDivisions { get { return GetPar(8); } set { SetPar(value); } }

        /// <summary>
        /// The absolute amount to shift in the horizontal direction.
        /// </summary>
        public double HorOffset { get { return GetPar<double>(0); } set { SetPar(value); } }

        /// <summary>
        /// The amount per division in the horizontal direction
        /// </summary>
        public double HorScale { get { return GetPar<double>(10); } set { SetPar(value); } }

        public Font Font { get { return GetPar(new Font("Ariel", 8.0f)); } set { SetPar(value); } }

    }

}
