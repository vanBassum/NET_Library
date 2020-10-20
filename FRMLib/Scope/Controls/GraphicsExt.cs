using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FRMLib.Scope.Controls
{
    public static class GraphicsExt
    {
        /*
        public static void DrawArrow(this Graphics g, Pen p, Point pt, double dir, int size)
        {
            DrawCross(g, p, pt.X, pt.Y, size);
        }
        */

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

        public static void DrawCross(this Graphics g, Pen p, Point pt, int size)
        {
            DrawCross(g, p, pt.X, pt.Y, size);
        }

        public static void DrawCross(this Graphics g, Pen p, double x, double y, int size, string text, Font font)
        {
            DrawCross(g, p, x, y, size);
            g.DrawString(text, font, new SolidBrush(p.Color), (float)x, (float)y - 10);
        }

        public static void DrawCross(this Graphics g, Pen p, double x, double y, int size)
        {
            g.DrawLine(p, (float)x - size, (float)y - size, (float)x + size, (float)y + size);
            g.DrawLine(p, (float)x + size, (float)y - size, (float)x - size, (float)y + size);
        }

        public static void Drawpoint(this Graphics g, Brush brush, double x, double y, int size)
        {
            g.FillEllipse(brush, (float)x - size, (float)y - size, size*2, size*2);
        }

        public static void Drawpoint(this Graphics g, Brush brush, Point pt, int size)
        {
            Drawpoint(g, brush, pt.X, pt.Y, size);
        }

        public static void DrawState(this Graphics g, Pen p, Rectangle rect, string text, Font font, bool closeBegin = true, bool closeEnd = true)
        {
            if (false)
            {
                g.DrawRectangle(p, rect);
                rect.X += 2;
                rect.Width -= 4;
                DrawFitTextToRectangle(g, p, rect, text, font);
            }
            else
            {
                //Draw opening and closing arrow.
                int bracketWidth = rect.Height / 2;

                if (rect.Width < (bracketWidth * 2))
                    bracketWidth = rect.Width / 2;

                int spaceBegin = closeBegin ? closeEnd ? bracketWidth : bracketWidth/2 : 0;
                int spaceEnd = closeEnd ? closeBegin ? bracketWidth : bracketWidth/2 : 0;
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
                    for(int i=0; i < div; i++)
                        g.DrawLine(p, rect.X - (((i % 2) == 1) ? wibber : 0), (int)(top + space * i), rect.X - (((i % 2) == 0) ? wibber : 0),  (int)(top + space * (i + 1)));
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
                DrawFitTextToRectangle(g, p, textRect, text, font);
            }
        }


        static void DrawFitTextToRectangle(this Graphics g, Pen p, Rectangle rect, string text, Font font)
        {
            if (rect.Width > 3)
            {

                while (g.MeasureString(text, font).Width > rect.Width)
                    text = text.Substring(0, text.Length - 1);

                g.DrawString(text, font, new SolidBrush(p.Color), rect);
            }
        }




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
