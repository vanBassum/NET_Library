using System.Drawing;

namespace FRMLib.Scope.Controls
{
    public static class GraphicsExt
    {
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

                if (closeBegin)
                {
                    g.DrawLine(p, rect.X, midY, rect.X + spaceBegin, rect.Y);
                    g.DrawLine(p, rect.X, midY, rect.X + spaceBegin, rect.Y + rect.Height);
                }

                if (closeEnd)
                {
                    g.DrawLine(p, rect.X + rect.Width, midY, rect.X + rect.Width - spaceEnd, rect.Y);
                    g.DrawLine(p, rect.X + rect.Width, midY, rect.X + rect.Width - spaceEnd, rect.Y + rect.Height);
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
