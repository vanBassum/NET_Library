using FormsLib.Scope.Enums;
using FormsLib.Scope.Helpers;

namespace FormsLib.Scope.Models
{

    public class GraphSettings
    {
        /// <summary>
        /// Total number of divisions in the horizontal direction.
        /// </summary>
        public int HorizontalDivisions { get; set; } = 10;

        /// <summary>
        /// Total number of divisions in the vertical direction.
        /// </summary>
        public int VerticalDivisions { get; set; } = 8;

        /// <summary>
        /// The absolute amount to shift in the horizontal direction.
        /// </summary>
        public float HorizontalOffset { get; set; } = 0;

        /// <summary>
        /// The amount per division in the horizontal direction
        /// </summary>
        public float HorizontalScale { get; set; } = 10f;

        /// <summary>
        /// Function to convert X values to string.
        /// </summary>
        public Func<float, string> HorizontalToHumanReadable { get; set; } = (value) => StringHelpers.FormatFloatWithUnits(value, 3);
        public VerticalZeroPosition ZeroPosition { get; set; } = VerticalZeroPosition.Middle;
        public VerticalZeroPosition GridZeroPosition { get; set; } = VerticalZeroPosition.Middle;
        public DrawPosVertical DrawScalePosVertical { get; set; } = DrawPosVertical.Right;
        public DrawPosHorizontal DrawScalePosHorizontal { get; set; } = DrawPosHorizontal.Bottom;
        public Color BackgroundColor { get; set; } = Color.Black;
        public Color ForegroundColor { get; set; } = Color.FromArgb(0xA0, 0xA0, 0xA0);
        public Pen GridZeroPen { get; set; } = new Pen(Color.FromArgb(0xA0, 0xA0, 0xA0));
        public Pen GridPen { get; set; } = new Pen(Color.FromArgb(0x30, 0x30, 0x30));
        public Font Font { get; set; } = new Font("Ariel", 8.0f);                 
        public int DetailDetectRadius { get; set; } = 7;                          
        public int DetailWindowWidth { get; set; } = 300;                         
    }
}
