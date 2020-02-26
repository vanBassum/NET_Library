using System.Drawing;

namespace Oscilloscope
{
    public class GridSettings
    {
        public Color GridColor { get; set; } = Color.White;
        public Color GridSubColor { get; set; } = Color.FromArgb(0x30, 0x30, 0x30);
        public int HorizontalDivisions { get; set; } = 10;
        public int VerticalDivisions { get; set; } = 8;

        public int HorizontalSubdiv { get => (HorizontalDivisions / LowestDiv(HorizontalDivisions)); }
        public int VerticalSubdiv { get => (VerticalDivisions / LowestDiv(VerticalDivisions)); }


        private static int LowestDiv(int num)
        {
            int i = 0;
            for (i = 2; i < num; i++)
                if (num % i == 0)
                    break;
            return i;
        }
    }

}
