using System.Drawing;

namespace Oscilloscope
{
    public class ScopeSettings
    {
        public double Offset { get; set; } = 0f;
        public double TimeBase { get; set; } = 10f;
        public string HorizontalUnit { get; set; } = "s";
        public bool HorizontalIsDate = false;
        public GridSettings Grid { get; set; } = new GridSettings();
        public Color BackgroundColor { get; set; } = Color.Black;
    }

}
