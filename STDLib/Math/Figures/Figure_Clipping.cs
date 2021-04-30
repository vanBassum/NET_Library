using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.Math.Figures
{
    public abstract partial class Figure
    {
        #region Internet snippits
        // Liang-Barsky function by Daniel White @ http://www.skytopia.com/project/articles/compsci/clipping.html
        static bool LiangBarsky(Rectangle window,
                          ref Line line)
        {
            double x0src = line.P1.X;
            double y0src = line.P1.Y;
            double x1src = line.P2.X;
            double y1src = line.P2.Y;

            double edgeLeft = window.X;
            double edgeRight = window.X + window.Width;
            double edgeBottom = window.Y;
            double edgeTop = window.Y + window.Height;

            double t0 = 0.0; double t1 = 1.0;
            double xdelta = x1src - x0src;
            double ydelta = y1src - y0src;
            double p = 0;
            double q = 0;
            double r = 0;

            for (int edge = 0; edge < 4; edge++)
            {   // Traverse through left, right, bottom, top edges.
                if (edge == 0) { p = -xdelta; q = -(edgeLeft - x0src); }
                if (edge == 1) { p = xdelta; q = (edgeRight - x0src); }
                if (edge == 2) { p = -ydelta; q = -(edgeBottom - y0src); }
                if (edge == 3) { p = ydelta; q = (edgeTop - y0src); }
                r = q / p;
                if (p == 0 && q < 0) return false;   // Don't draw line at all. (parallel line outside)

                if (p < 0)
                {
                    if (r > t1) return false;         // Don't draw line at all.
                    else if (r > t0) t0 = r;            // Line is clipped!
                }
                else if (p > 0)
                {
                    if (r < t0) return false;      // Don't draw line at all.
                    else if (r < t1) t1 = r;         // Line is clipped!
                }
            }
            line.P1 = new V2D(x0src + t0 * xdelta, y0src + t0 * ydelta);
            line.P2 = new V2D(x0src + t1 * xdelta, y0src + t1 * ydelta);
            return true;        // (clipped) line is drawn
        }
        #endregion

        public static Figure[] Clipping(Rectangle window, Line line)
        {
            if(LiangBarsky(window, ref line))
            {
                return new Figure[] { line };
            }
            return new Figure[0];
        }
        
        
        public static Figure[] Clipping(Figure figa, Figure figb)
        {
            return (figa, figb) switch
            {
                (Rectangle f1, Line f2) => Clipping(f1, f2),
                _ => throw new NotImplementedException($"No clipping supported between '{figa.GetType().Name}' and '{figb.GetType().Name}'"),
            };
        }
    }

}
