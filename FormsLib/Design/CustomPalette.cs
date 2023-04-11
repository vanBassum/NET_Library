using System;
using System.Drawing;

namespace FormsLib.Design
{
    public class CustomPalette : IPalette 
    {
        protected virtual Color[] Colors { get; } = new Color[0];

        public CustomPalette(Color[] colors)
        {
            Colors = colors;
        }

        public Color this[int index]
        {
            get 
            {
                if (index >= Colors.Length)
                    index = Colors.Length - 1;
                return Colors[index]; 
            }
            set
            {
                if (index < Colors.Length)
                    Colors[index] = value;
            }
        }
    }
}
