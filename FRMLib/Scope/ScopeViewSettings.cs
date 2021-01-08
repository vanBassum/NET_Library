using STDLib.Misc;
using System.Drawing;

namespace FRMLib.Scope
{
    public class ScopeViewSettings : PropertySensitive
    {
        public Color BackgroundColor { get { return GetPar(Color.Black); } set { SetPar(value); } }
        public Pen GridZeroPen { get { return GetPar(new Pen(Color.FromArgb(0xA0, 0xA0, 0xA0))); } set { SetPar(value); } }
        public Pen GridPen { get { return GetPar(new Pen(Color.FromArgb(0x30, 0x30, 0x30)){ DashPattern = new float[] { 4.0F, 4.0F }}); } set { SetPar(value); } }

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

        public VerticalZeroPosition ZeroPosition { get { return GetPar<VerticalZeroPosition>(VerticalZeroPosition.Middle); } set { SetPar(value); } }

        public VerticalZeroPosition GridZeroPosition { get { return GetPar<VerticalZeroPosition>(VerticalZeroPosition.Middle); } set { SetPar(value); } }

        public Font Font { get { return GetPar(new Font("Ariel", 8.0f)); } set { SetPar(value); } }

        public DrawPosVertical DrawScalePosVertical { get { return GetPar<DrawPosVertical>(DrawPosVertical.None); } set { SetPar(value); } }
        public DrawPosHorizontal DrawScalePosHorizontal { get { return GetPar<DrawPosHorizontal>(DrawPosHorizontal.None); } set { SetPar(value); } }
    }

    public enum VerticalZeroPosition
    {
        Top,
        Middle,
        Bottom
    }

    public enum DrawPosVertical
    {
        None,
        Left,
        Right,
    }

    public enum DrawPosHorizontal
    {
        None,
        Top,
        Bottom,
    }

}
