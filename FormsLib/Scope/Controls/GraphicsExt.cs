using System.Drawing;
using System.Drawing.Imaging;

namespace FormsLib.Scope.Controls
{


    public static class GraphicsExt
    {
        /*
        public static void DrawArrow(this Graphics g, Pen p, Point pt, double dir, int size)
        {
            DrawCross(g, p, pt.X, pt.Y, size);
        }
        */

        static Dictionary<Font, int> heightTable = new Dictionary<Font, int>(); 

        public static int GetFontHeight(Font font)
        {
            int height = 0;
            if(!heightTable.TryGetValue(font, out height))
            {
                height = font.Height;
                heightTable[font] = height;
            }
            return height;
        }


        public static void DrawArrow(this Graphics g, Pen p, Point pt, double xDir, double yDir, float size)
        {

            PointF[] fig = new PointF[] {
                new PointF(0, 0),
                new PointF(-1, 1),
                new PointF(2, 0),
                new PointF(-1, -1),
                new PointF(0,0 ),
            };

            g.DrawFigure(p, pt, fig, xDir, yDir, size, true);
        }

        /// <summary>
        /// Orient the image to the right
        /// </summary>
        /// <param name="g"></param>
        /// <param name="p"></param>
        /// <param name="pts"></param>
        /// <param name="xDir"></param>
        /// <param name="yDir"></param>
        /// <param name="size"></param>
        static void DrawFigure(this Graphics g, Pen p, PointF pos, PointF[] fig, double xDir, double yDir, float size, bool solid = false)
        {
            for (int i = 0; i < fig.Length; i++)
            {
                fig[i].X *= size;
                fig[i].Y *= size;

                fig[i] = RotatePoint(fig[i], new PointF(0, 0), xDir, yDir);

                fig[i].X += pos.X;
                fig[i].Y += pos.Y;
            }

            if (solid)
                g.FillPolygon(new SolidBrush(p.Color), fig);
            else
                g.DrawLines(p, fig);
        }


        static Point RotatePoint(PointF pointToRotate, PointF centerPoint, double xDir, double yDir)
        {
            return new Point
            {
                X =
                    (int)
                    (xDir * (pointToRotate.X - centerPoint.X) -
                    yDir * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (yDir * (pointToRotate.X - centerPoint.X) +
                    xDir * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        public static void DrawCross(this Graphics g, Pen p, Rectangle viewPort, Point pt, int size)
        {
            DrawCross(g, p, viewPort, pt.X, pt.Y, size);
        }

        public static void DrawCross(this Graphics g, Pen p, Rectangle viewPort, double x, double y, int size, string text, Font font)
        {
            DrawCross(g, p, viewPort, x, y, size);
            int enters = text.Count(c => c == '\n');
            if (!text.EndsWith('\n'))
                enters++;
            int fontHeight = GetFontHeight(font);
            Rectangle textRect = new Rectangle((int)x + 8, (int)y - (fontHeight * enters), viewPort.Width - 8, fontHeight * enters);
            g.DrawFitTextToRectangle(p, viewPort, textRect, text, font);
        }

        public static void DrawFitTextToRectangle(this Graphics g, Pen p, Rectangle viewPort, Rectangle rect, string text, Font font)
        {
            if (rect.Width > 3)
            {
                if (rect.X >= viewPort.X + viewPort.Width)
                    return;

                if (rect.X + rect.Width <= viewPort.X)
                    return;

                if (rect.X < viewPort.X)
                {
                    int delta = viewPort.X - rect.X;
                    rect.X = viewPort.X;
                    rect.Width -= delta;
                }

                if ((rect.X + rect.Width) > (viewPort.X + viewPort.Width))
                {
                    int delta = (viewPort.X + viewPort.Width) - (rect.X + rect.Width);
                    rect.Width += delta;
                }

                string[] lines = text.Split('\n');
                int lineHeight = GetFontHeight(font);



                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    var lineRectangle = new Rectangle(rect.X, rect.Y + i * lineHeight, rect.Width, lineHeight);

                    // Only draw the line if it's within the rectangle
                    if (lineRectangle.Y >= rect.Y && lineRectangle.Y + lineHeight <= rect.Bottom)
                    {
                        g.DrawString(line, font, new SolidBrush(p.Color), lineRectangle);
                    }
                }
            }
        }



        public static void DrawCross(this Graphics g, Pen p, Rectangle viewPort, double x, double y, int size)
        {
            g.DrawLine(p, viewPort, new Point((int)x - size, (int)y - size), new Point((int)x + size, (int)y + size), false, false);
            g.DrawLine(p, viewPort, new Point((int)x + size, (int)y - size), new Point((int)x - size, (int)y + size), false, false);
        }

        public static void Drawpoint(this Graphics g, Brush brush, double x, double y, int size)
        {
            g.FillEllipse(brush, (float)x - size, (float)y - size, size * 2, size * 2);
        }

        public static void Drawpoint(this Graphics g, Brush brush, Point pt, int size)
        {
            Drawpoint(g, brush, pt.X, pt.Y, size);
        }


        public static void DrawLine(this Graphics g, Pen p, Rectangle viewPort, Point p1, Point p2, bool extendBegin, bool extendEnd)
        {
            if (p2.X < p1.X)
            {
                var t = p1;
                p1 = p2;
                p2 = t;
            }

            if (p1.X > (viewPort.X + viewPort.Width))
                return;

            if (p2.X < viewPort.X)
                return;


            if (p1.X == p2.X)
                g.DrawLine(p, p1, p2);
            else
            {
                if (p2.X > (viewPort.X + viewPort.Width))
                {
                    float a = (float)(p2.Y - p1.Y) / (float)(p2.X - p1.X);
                    float b = p1.Y - (a * p1.X);
                    p2.X = viewPort.X + viewPort.Width;
                    p2.Y = (int)(p2.X * a + b);
                }

                if (p1.X <viewPort.X)
                {
                    float a = (float)(p2.Y - p1.Y) / (float)(p2.X - p1.X);
                    float b = p1.Y - (a * p1.X);
                    p1.X = viewPort.X;
                    p1.Y = (int)(p1.X * a + b);
                }

                g.DrawLine(p, p1, p2);
            }


            if (extendBegin)
                g.DrawArrow(p, p2, -1, 0, 3);

            if (extendEnd)
                g.DrawArrow(p, p1, 1, 0, 3);
        }

        public static void DrawState(this Graphics g, Pen p, Rectangle viewPort, Rectangle rect, string text, Font font, bool closeBegin = true, bool closeEnd = true)
        {
            if (false)
            {
                g.DrawRectangle(p, rect);
                rect.X += 2;
                rect.Width -= 4;
                DrawFitTextToRectangle(g, p, viewPort, rect, text, font);
            }
            else
            {
                if (rect.X > viewPort.X + viewPort.Width)
                    return;

                if (rect.X + rect.Width < viewPort.X)
                    return;

                if (rect.X < viewPort.X + 5)
                {
                    int delta = viewPort.X + 5 - rect.X;
                    rect.X = viewPort.X + 5;
                    rect.Width -= delta;
                    closeBegin = false;
                }

                if ((rect.X + rect.Width) > (viewPort.X + viewPort.Width - 5))
                {
                    int delta = (viewPort.X + viewPort.Width - 5) - (rect.X + rect.Width);
                    rect.Width += delta;
                    closeEnd = false;
                }



                //Draw opening and closing arrow.
                int bracketWidth = rect.Height / 2;

                if (rect.Width < (bracketWidth * 2))
                    bracketWidth = rect.Width / 2;

                int spaceBegin = closeBegin ? closeEnd ? bracketWidth : bracketWidth / 2 : 0;
                int spaceEnd = closeEnd ? closeBegin ? bracketWidth : bracketWidth / 2 : 0;
                int midY = rect.Y + rect.Height / 2;

                double top = rect.Y;
                int div = 6;
                double space = (double)rect.Height / (double)div;
                int wibber = rect.Height / 4;

                if (closeBegin)
                {
                    g.DrawLine(p, rect.X, midY, rect.X + spaceBegin, rect.Y);
                    g.DrawLine(p, rect.X, midY, rect.X + spaceBegin, rect.Y + rect.Height);
                }
                else
                {
                    for (int i = 0; i < div; i++)
                        g.DrawLine(p, rect.X - (((i % 2) == 1) ? wibber : 0), (int)(top + space * i), rect.X - (((i % 2) == 0) ? wibber : 0), (int)(top + space * (i + 1)));
                }

                if (closeEnd)
                {
                    g.DrawLine(p, rect.X + rect.Width, midY, rect.X + rect.Width - spaceEnd, rect.Y);
                    g.DrawLine(p, rect.X + rect.Width, midY, rect.X + rect.Width - spaceEnd, rect.Y + rect.Height);
                }
                else
                {
                    for (int i = 0; i < div; i++)
                        g.DrawLine(p, rect.X + rect.Width + (((i % 2) == 1) ? wibber : 0), (int)(top + space * i), rect.X + rect.Width + (((i % 2) == 0) ? wibber : 0), (int)(top + space * (i + 1)));
                }


                g.DrawLine(p, rect.X + spaceBegin, rect.Y, rect.X + rect.Width - spaceEnd, rect.Y);
                g.DrawLine(p, rect.X + spaceBegin, rect.Y + rect.Height, rect.X + rect.Width - spaceEnd, rect.Y + rect.Height);


                Rectangle textRect = new Rectangle(rect.X + spaceBegin, rect.Y, rect.Width - spaceBegin - spaceEnd, rect.Height);
                DrawFitTextToRectangle(g, p, viewPort, textRect, text, font);
            }
        }

        public static void DrawCircle(this Graphics g, Pen pen,
                                  float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
        



        //static void DrawPanel(this Graphics g, Color border, Color bg, Rectangle rect)
        //{
        //    Rectangle filled = new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2);
        //
        //}


        /*
        public static void DrawState(this Graphics g, Pen p, Rectangle rect, string text, Font font, bool closeBegin = true, bool closeEnd = true)
        {
            bool simple = true;
            if (simple)
            {
                g.DrawRectangle(p, rect);
                g.DrawString(text, font, new SolidBrush(p.Color), rect);
            }
            else
            {
                int halfSize = (int)(rect.Width / 2);
                if (halfSize > rect.Height / 2)
                    halfSize = rect.Height / 2;
                if (halfSize < 0)
                    halfSize = 0;

                int stateB = rect.X;
                int stateE = rect.X + rect.Width;

                if (closeBegin)
                {
                    g.DrawLine(p, stateB, rect.Y + rect.Height / 2, stateB + halfSize, rect.Y);
                    g.DrawLine(p, stateB, rect.Y + rect.Height / 2, stateB + halfSize, rect.Y + rect.Height);
                    stateB += halfSize;
                }

                if (closeEnd)
                {
                    g.DrawLine(p, stateE, rect.Y + rect.Height / 2, stateE - halfSize, rect.Y);
                    g.DrawLine(p, stateE, rect.Y + rect.Height / 2, stateE - halfSize, rect.Y + rect.Height);
                    stateE -= halfSize;
                }


                if (halfSize >= rect.Height / 2)
                {
                    g.DrawLine(p, stateB, rect.Y, stateE, rect.Y);
                    g.DrawLine(p, stateB, rect.Y + rect.Height, stateE, rect.Y + rect.Height);


                    if (stateB < 2)
                        stateB = 2;

                    rect = new Rectangle(stateB, rect.Y, stateE - stateB, rect.Height);

                    g.DrawString(text, font, new SolidBrush(p.Color), rect);
                }
            }
            
        }
        */
    }

}
