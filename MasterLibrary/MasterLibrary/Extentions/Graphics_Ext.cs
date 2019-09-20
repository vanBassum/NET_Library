using System.Drawing;
using MasterLibrary.MathX;

namespace MasterLibrary.Extentions
{
    public static class Graphics_Ext
    {
        public static void DrawCircle(this Graphics g, Pen pen, PointF center, double radius) => g.DrawCircle(pen, center, (float)radius);
        public static void DrawCircle(this Graphics g, Pen pen, PointF center, float radius)
        {
            float x = center.X - radius;
            float y = center.Y - radius;
            float width = 2 * radius;
            float height = 2 * radius;
            g.DrawEllipse(pen, x, y, width, height);
        }


        public static void DrawCross(this Graphics g, Pen pen, PointF center, float radius)
        {
            PointF p1 = new PointF(center.X - radius, center.Y - radius);
            PointF p2 = new PointF(center.X + radius, center.Y + radius);
            PointF p3 = new PointF(center.X - radius, center.Y + radius);
            PointF p4 = new PointF(center.X + radius, center.Y - radius);

            g.DrawLine(pen, p1, p2);
            g.DrawLine(pen, p3, p4);
        }



        public static void DrawV2D(this Graphics g, Pen p, V2D vector, V2D origin)
        {
            V2D temp = vector;
            temp.Y *= -1;
            g.DrawLine(p, origin, origin + temp);
        }
    }

}
