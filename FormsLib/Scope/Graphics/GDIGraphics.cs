using FormsLib.Scope.Helpers;
using System.Numerics;

namespace FormsLib.Scope.Graphics
{
    public class GDIGraphics : IGraphics
    {
        private readonly System.Drawing.Graphics graphics;
        public GDIGraphics(System.Drawing.Graphics graphics)
        {
            this.graphics = graphics;
        }


        public void Clear(Color color) => graphics.Clear(color);
        public void DrawLine(Pen pen, Vector2 point1, Vector2 point2) => graphics.DrawLine(pen, point1.ToPoint(), point2.ToPoint());
        public void DrawRectangle(Pen pen, RectangleF rectangle) => graphics.DrawRectangle(pen, rectangle);
        public void DrawString(string text, Font font, Brush brush, Vector2 point, Vector2 size, StringFormat stringFormat) => graphics.DrawString(text, font, brush, new Rectangle(point.ToPoint(), size.ToSize()), stringFormat);
        public void DrawString(string text, Font font, Brush brush, Vector2 point, StringFormat stringFormat) => graphics.DrawString(text, font, brush, point.ToPoint(), stringFormat);

        public void DrawCross(Pen pen, Vector2 point, Vector2 size)
        {
            var p1 = point - size / 2;
            var p2 = point + size / 2;

            graphics.DrawLine(pen, p1.ToPoint(), p2.ToPoint());
            graphics.DrawLine(pen, new PointF(p1.X, p2.Y), new PointF(p2.X, p1.Y));
        }

        public void DrawSolidEllipse(Brush brush, Vector2 point, Vector2 size)
        {
            graphics.FillEllipse(brush, new RectangleF(point.ToPoint(), size.ToSize()));
        }


    }
}
