using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.Math.Figures
{
    public abstract partial class Figure
    {

        static bool GetCollision(Rectangle figA, Dot pt)
        {
            int i, j;
            bool c = false;
            V2D[] pts = figA.ToPoints();
            for (i = 0, j = pts.Length - 1; i < pts.Length; j = i++)
            {
                if (((pts[i].Y > pt.Position.Y) != (pts[j].Y > pt.Position.Y)) &&
                 (pt.Position.X < (pts[j].X - pts[i].X) * (pt.Position.Y - pts[i].Y) / (pts[j].Y - pts[i].Y) + pts[i].X))
                    c = !c;
            }
            return c;
        }


        static bool GetCollision(Line figA, Line figB)
        {
            V2D a = figA.P1;
            V2D b = figA.P2;
            V2D c = figB.P1;
            V2D d = figB.P2;


            double denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            double numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            double numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            // Detect coincident lines
            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            double r = numerator1 / denominator;
            double s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        public static bool GetCollision(Figure figa, Figure figb)
        {
            return (figa, figb) switch
            {
                (Line f1, Line f2) => GetCollision(f1, f2),
                (Rectangle f1, Dot f2) => GetCollision(f1, f2),
                _ => throw new NotImplementedException($"No collision detection yet between '{figa.GetType().Name}' and '{figb.GetType().Name}'"),
            };
        }

    }
}
