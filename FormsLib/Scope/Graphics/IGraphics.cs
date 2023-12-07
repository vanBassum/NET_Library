using FormsLib.Scope.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FormsLib.Scope.Graphics
{
    public interface IGraphics
    {
        public void Clear(Color color);
        public void DrawLine(Pen pen, Vector2 point1, Vector2 point2);
        public void DrawRectangle(Pen pen, RectangleF rectangle);
        public void DrawString(string text, Font font, Brush brush, Vector2 point, Vector2 size, StringFormat stringFormat);
        public void DrawString(string text, Font font, Brush brush, Vector2 point, StringFormat stringFormat);
        public void DrawCross(Pen pen, Vector2 point, Vector2 size);
        public void DrawSolidEllipse(Brush brush, Vector2 point, Vector2 size);






        //All kind of overloading
        public void DrawRectangle(Pen pen, Vector2 point, Vector2 size) => DrawRectangle(pen, new RectangleF(point.ToPointF(), size.ToSizeF()));
        public void DrawString(string text, Font font, Brush brush, Vector2 point) => DrawString(text, font, brush, point, StringFormat.GenericDefault);
        public void DrawString(string text, Font font, Brush brush, Vector2 point, Vector2 size) => DrawString(text, font, brush, point, size, StringFormat.GenericDefault);
    }
}
