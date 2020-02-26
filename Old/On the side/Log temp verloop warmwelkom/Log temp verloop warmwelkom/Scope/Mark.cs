using System.Drawing;

namespace Oscilloscope
{
    public class Mark
    {
        public double X { get; set; }
        public double Y { get; set; }
        public bool AttachYtoTrace { get; set; } = false;
        public string Text { get; set; } = "";
        public Color Colour { get; set; } = Color.White;


        public Mark()
        {

        }

        public Mark(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Mark(double x, double y, string text)
        {
            X = x;
            Y = y;
            Text = text;
        }

        public Mark(double x, double y, string text, Color colour)
        {
            X = x;
            Y = y;
            Text = text;
            Colour = colour;
        }
    }

}
