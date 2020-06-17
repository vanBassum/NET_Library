using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRMLib.Scope
{
    public static class Palettes
    {
        public static Pen[] DistinctivePallet { get; } = new Pen[]
        {
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
             new Pen(Color.FromArgb(230, 190, 255)),
             new Pen(Color.FromArgb(170, 110, 40)),
             new Pen(Color.FromArgb(255, 250, 200)),
             new Pen(Color.FromArgb(128, 0, 0)),
             new Pen(Color.FromArgb(170, 255, 195)),
             new Pen(Color.FromArgb(128, 128, 0)),
             new Pen(Color.FromArgb(255, 215, 180)),
             new Pen(Color.FromArgb(0, 0, 128))
        };
    }
}
