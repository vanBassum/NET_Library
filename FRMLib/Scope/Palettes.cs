using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public static class Palettes
    {
        public static Pen Red { get { return new Pen(Color.FromArgb(unchecked((int)0xffe6194B))); } }
        public static Pen Green { get { return new Pen(Color.FromArgb(unchecked((int)0xff3cb44b))); } }
        public static Pen Yellow { get { return new Pen(Color.FromArgb(unchecked((int)0xffffe119))); } }
        public static Pen Blue { get { return new Pen(Color.FromArgb(unchecked((int)0xff4363d8))); } }
        public static Pen Orange { get { return new Pen(Color.FromArgb(unchecked((int)0xfff58231))); } }
        public static Pen Purple { get { return new Pen(Color.FromArgb(unchecked((int)0xff911eb4))); } }
        public static Pen Cyan { get { return new Pen(Color.FromArgb(unchecked((int)0xff42d4f4))); } }
        public static Pen Magenta { get { return new Pen(Color.FromArgb(unchecked((int)0xfff032e6))); } }
        public static Pen Lime { get { return new Pen(Color.FromArgb(unchecked((int)0xffbfef45))); } }
        public static Pen Pink { get { return new Pen(Color.FromArgb(unchecked((int)0xfffabed4))); } }
        public static Pen Teal { get { return new Pen(Color.FromArgb(unchecked((int)0xff469990))); } }
        public static Pen Lavender { get { return new Pen(Color.FromArgb(unchecked((int)0xffdcbeff))); } }
        public static Pen Brown { get { return new Pen(Color.FromArgb(unchecked((int)0xff9A6324))); } }
        public static Pen Beige { get { return new Pen(Color.FromArgb(unchecked((int)0xfffffac8))); } }
        public static Pen Maroon { get { return new Pen(Color.FromArgb(unchecked((int)0xff800000))); } }
        public static Pen Mint { get { return new Pen(Color.FromArgb(unchecked((int)0xffaaffc3))); } }
        public static Pen Olive { get { return new Pen(Color.FromArgb(unchecked((int)0xff808000))); } }
        public static Pen Apricot { get { return new Pen(Color.FromArgb(unchecked((int)0xffffd8b1))); } }
        public static Pen Navy { get { return new Pen(Color.FromArgb(unchecked((int)0xff000075))); } }
        public static Pen Grey { get { return new Pen(Color.FromArgb(unchecked((int)0xffa9a9a9))); } }

        public static Pen[] DistinctivePallet { get; } = new Pen[]
        {
            Red,
            Green,
            Yellow,
            Blue,
            Orange,
            Purple,
            Cyan,
            Magenta,
            Lime,
            Pink,
            Teal,
            Lavender,
            Brown,
            Beige,
            Maroon,
            Mint,
            Olive,
            Apricot,
            Navy,
            Grey,
            /*
             new Pen(Color.FromArgb(230, 25, 75)),
             new Pen(Color.FromArgb(60, 180, 75)),
             new Pen(Color.FromArgb(255, 225, 25)),
             new Pen(Color.FromArgb(0, 130, 200)),
             new Pen(Color.FromArgb(245, 130, 48)),
             new Pen(Color.FromArgb(145, 30, 180)),
             new Pen(Color.FromArgb(70, 240, 240)),
             new Pen(Color.FromArgb(240, 50, 230)),
             new Pen(Color.FromArgb(210, 245, 60)),
             new Pen(Color.FromArgb(250, 190, 190)),
             new Pen(Color.FromArgb(0, 128, 128)),
             //new Pen(Color.FromArgb(230, 190, 255)),
             new Pen(Color.FromArgb(170, 110, 40)),
             new Pen(Color.FromArgb(255, 250, 200)),
             new Pen(Color.FromArgb(128, 0, 0)),
             new Pen(Color.FromArgb(170, 255, 195)),
             new Pen(Color.FromArgb(128, 128, 0)),
             new Pen(Color.FromArgb(255, 215, 180)),
             new Pen(Color.FromArgb(0, 0, 128))*/
        };


        public static Pen GenericPallet(int i)
        {
            return new Pen(HVS.ToColor(Hue((uint)i), 1f, 1f));
        }



        static double Hue(uint i)
        {

            if (i % 2 == 0)
            {
                if (i == 0)
                    return 0;

                uint curPow = i;

                while (!IsPowerOfTwo(curPow))
                    curPow -= 2;

                double size = 180f / (double)curPow;

                return Hue(i - curPow) + size;
            }
            else
            {
                return Hue(i - 1) + 180;
            }
        }


        static private bool IsPowerOfTwo(uint x)
        {
            return (x & (x - 1)) == 0;
        }

    }

    internal static class HVS
    {
        public static Color ToColor(double h, double S, double V)
        {
            double H = h;
            while (H < 0) { H += 360; };
            while (H >= 360) { H -= 360; };
            double R, G, B;
            if (V <= 0)
            { R = G = B = 0; }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color

                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;

                    // Blue is the dominant color

                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;

                    // Red is the dominant color

                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.

                    default:
                        //LFATAL("i Value error in Pixel conversion, Value is %d", i);
                        R = G = B = V; // Just pretend its black/white
                        break;
                }
            }

            R = Clamp((int)(R * 255.0));
            G = Clamp((int)(G * 255.0));
            B = Clamp((int)(B * 255.0));

            return Color.FromArgb((int)R, (int)G, (int)B);

        }

        /// <summary>
        /// Clamp a value to 0-255
        /// </summary>
        static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }
    }

}
