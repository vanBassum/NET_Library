using System;
using System.Collections.Generic;
using System.Text;

namespace STDLib.Math.Figures
{
    public abstract partial class Figure
    {
        static V2D[] Intersections(Line l1, Line l2)
        {
            if (!GetCollision(l1, l2))
                return new V2D[0];
            V2D a = l1.P1;
            V2D b = l1.P2;
            V2D c = l2.P1;
            V2D d = l2.P2;

            double denominator2 = (a.X - b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X - d.X);
            double pX = 0;
            double pY = 0;

            if (denominator2 != 0)
            {
                pX = ((a.X * b.Y - a.Y * b.X) * (c.X - d.X) - (a.X - b.X) * (c.X * d.Y - c.Y * d.X)) / denominator2;
                pY = ((a.X * b.Y - a.Y * b.X) * (c.Y - d.Y) - (a.Y - b.Y) * (c.X * d.Y - c.Y * d.X)) / denominator2;
                return new V2D[] { new V2D(pX, pY) };
            }

            return new V2D[0];
        }



        //http://paulbourke.net/geometry/circlesphere/
        static V2D[] Intersections(Circle c1, Circle c2)
        {
            V2D p0 = c1.CenterPos;
            V2D p1 = c2.CenterPos;
            double r0 = c1.Radius;
            double r1 = c2.Radius;
            double d = (p1 - p0).Magnitude;

            if (d > r0 + r1)                    //Cicles dont collide
                return new V2D[0];

            if (d < System.Math.Abs(r0 + r1))   //Circle is contained within the other.
                return new V2D[0];

            if (d == 0)                         //the circles are coincident and there are an infinite number of solutions.
                return new V2D[0];


            double a = (r0 * r0 - r1 * r1 + d * d) / (2 * d);
            double h = System.Math.Sqrt(r0 * r0 - a * a);

            V2D p2 = p0 + a * (p1 - p0) / d;
            V2D p = new V2D(-h * (p1.Y - p0.Y) / d, h * (p1.X - p0.X) / d);

            return new V2D[] { p2 + p, p2 - p };
        }

        static V2D[] Intersections(Circle circle, Line line) => Intersections(line, circle);
        static V2D[] Intersections(Line line, Circle circle)
        {
            V2D p0 = line.P1;
            V2D p1 = line.P2;
            V2D p2 = (circle.CenterPos - p0).ProjectTo(p1 - p0) + p0;

            double a = (circle.CenterPos - p2).Magnitude;
            double h = System.Math.Sqrt(circle.Radius * circle.Radius - a * a);

            V2D p = p0 - p2;
            p.Magnitude = h;

            V2D px1 = p2 + p;
            V2D px2 = p2 - p;

            List<V2D> pts = new List<V2D>();

            if (System.Math.Abs((px1 - p0).Angle - (px1 - p1).Angle) > System.Math.PI / 2)
                pts.Add(px1);
            if (System.Math.Abs((px2 - p0).Angle - (px2 - p1).Angle) > System.Math.PI / 2)
                pts.Add(px2);
            return pts.ToArray();
        }

        static V2D[] Intersections(Circle circle, Rectangle rect) => Intersections(rect, circle);
        static V2D[] Intersections(Rectangle rect, Circle circle)
        {
            List<V2D> pts = new List<V2D>();
            Line[] lines = rect.ToLines();
            pts.AddRange(GetIntersections(lines[0], circle));
            pts.AddRange(GetIntersections(lines[1], circle));
            pts.AddRange(GetIntersections(lines[2], circle));
            pts.AddRange(GetIntersections(lines[3], circle));
            return pts.ToArray();
        }

        static V2D[] Intersections(Line line, Rectangle rect) => Intersections(rect, line);
        static V2D[] Intersections(Rectangle rect, Line line)
        {
            List<V2D> pts = new List<V2D>();
            Line[] lines = rect.ToLines();
            pts.AddRange(GetIntersections(lines[0], line));
            pts.AddRange(GetIntersections(lines[1], line));
            pts.AddRange(GetIntersections(lines[2], line));
            pts.AddRange(GetIntersections(lines[3], line));
            return pts.ToArray();
        }

        static V2D[] Intersections(Rectangle rect1, Rectangle rect2)
        {
            List<V2D> pts = new List<V2D>();
            Line[] lines1 = rect1.ToLines();
            Line[] lines2 = rect2.ToLines();

            for (int i = 0; i < lines1.Length; i++)
            {
                pts.AddRange(GetIntersections(lines1[i], lines2[0]));
                pts.AddRange(GetIntersections(lines1[i], lines2[1]));
                pts.AddRange(GetIntersections(lines1[i], lines2[2]));
                pts.AddRange(GetIntersections(lines1[i], lines2[3]));
            }
            return pts.ToArray();
        }


        public static V2D[] GetIntersections(Figure figa, Figure figb)
        {
            return (figa, figb) switch
            {
                (Line f1, Line f2) => Intersections(f1, f2),
                (Line f1, Circle f2) => Intersections(f1, f2),
                (Line f1, Rectangle f2) => Intersections(f1, f2),

                (Circle f1, Line f2) => Intersections(f1, f2),
                (Circle f1, Circle f2) => Intersections(f1, f2),
                (Circle f1, Rectangle f2) => Intersections(f1, f2),

                (Rectangle f1, Line f2) => Intersections(f1, f2),
                (Rectangle f1, Circle f2) => Intersections(f1, f2),
                (Rectangle f1, Rectangle f2) => Intersections(f1, f2),
                _ => throw new NotImplementedException($"No intersection detection yet between '{figa.GetType().Name}' and '{figb.GetType().Name}'"),
            };
        }
    }
}
