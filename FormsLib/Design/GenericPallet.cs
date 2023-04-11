using System.Drawing;

namespace FormsLib.Design
{
    public class GenericPallet : IPalette
    {
        public Color this[int index] 
        {
            get => HVS.ToColor(Hue((uint)index), 1f, 1f);
            set 
            {
                throw new Exception("Not possible");
            }
        }

        private double Hue(uint i)
        {
            if (i % 2 == 0)
            {
                if (i == 0) return 0;
                uint curPow = i;
                while (!IsPowerOfTwo(curPow))
                    curPow -= 2;
                double size = 180f / (double)curPow;
                return Hue(i - curPow) + size;
            }
            else
                return Hue(i - 1) + 180;
        }

        private bool IsPowerOfTwo(uint x) => (x & x - 1) == 0;
    }

}
