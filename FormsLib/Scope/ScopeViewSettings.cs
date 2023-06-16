using CoreLib.Misc;
using FormsLib.Design;

namespace FormsLib.Scope
{
    public class Style : PropertySensitive
    {
        public string Name { get => GetPar("New style"); set => SetPar(value); }
        public Color BackgroundColor { get => GetPar(Color.Black); set => SetPar(value); }
        public Color ForegroundColor { get => GetPar(Color.FromArgb(0xA0, 0xA0, 0xA0)); set => SetPar(value); }
        public Pen GridZeroPen { get => GetPar(new Pen(Color.FromArgb(0xA0, 0xA0, 0xA0))); set => SetPar(value); }
        public Pen GridPen { get => GetPar(new Pen(Color.FromArgb(0x30, 0x30, 0x30)) { DashPattern = new float[] { 4.0F, 4.0F } }); set => SetPar(value); }
        public Font Font { get => GetPar(new Font("Ariel", 8.0f)); set => SetPar(value); }
        public int DetailDetectRadius { get => GetPar(7); set => SetPar(value); }
        public int DetailWindowWidth { get => GetPar(300); set => SetPar(value); }
        public bool AlwaysDetectRadius { get => GetPar(false); set => SetPar(value); }
    }

    public class ScopeViewSettings : PropertySensitive
    {
        public Style Style { get { return GetPar(new Style()); } set { SetPar(value); value.PropertyChanged += (s, e) => InvokePropertyChanged(e); } }

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

        /// <summary>
        /// Snapsize of horizontal axis
        /// </summary>
        public double HorSnapSize { get { return GetPar<double>(1); } set { SetPar(value); } }

        /// <summary>
        /// Function to convert X values to string.
        /// </summary>
        public Func<double, string> HorizontalToHumanReadable { get; set; } = (a) => a.ToString("F1");

        public VerticalZeroPosition ZeroPosition { get { return GetPar<VerticalZeroPosition>(VerticalZeroPosition.Middle); } set { SetPar(value); } }
        public VerticalZeroPosition GridZeroPosition { get { return GetPar<VerticalZeroPosition>(VerticalZeroPosition.Middle); } set { SetPar(value); } }
        public DrawPosVertical DrawScalePosVertical { get { return GetPar<DrawPosVertical>(DrawPosVertical.Right); } set { SetPar(value); } }
        public DrawPosHorizontal DrawScalePosHorizontal { get { return GetPar<DrawPosHorizontal>(DrawPosHorizontal.Bottom); } set { SetPar(value); } }


        public void SetHorizontal(DateTime from, DateTime untill)
        {
            HorScale = (untill - from).Ticks / HorizontalDivisions;
            HorOffset = -from.Ticks;
        }


        public DateTime GetStartDate()
        {
            long from = (long)-HorOffset;
            return new DateTime(from);
        }

        public DateTime GetEndDate()
        {
            long from = (long)-HorOffset;
            long until = (long)(from + HorScale * HorizontalDivisions);
            return new DateTime(until);
        }

        public void ApplySettings(ScopeViewSettings settings)
        {
            Style = settings.Style;
            HorizontalDivisions = settings.HorizontalDivisions;
            VerticalDivisions = settings.VerticalDivisions;
            HorOffset = settings.HorOffset;
            HorScale = settings.HorScale;
            HorSnapSize = settings.HorSnapSize;
            HorizontalToHumanReadable = settings.HorizontalToHumanReadable;
            ZeroPosition = settings.ZeroPosition;
            GridZeroPosition = settings.GridZeroPosition;
            DrawScalePosVertical = settings.DrawScalePosVertical;
            DrawScalePosHorizontal = settings.DrawScalePosHorizontal;
        }
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
