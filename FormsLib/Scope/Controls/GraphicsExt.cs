namespace FormsLib.Scope.Controls
{

    public static class GraphicsExt
    {
        static Dictionary<Font, int> heightTable = new Dictionary<Font, int>();

        public static int GetFontHeight(Font font)
        {
            int height = 0;
            if (!heightTable.TryGetValue(font, out height))
            {
                height = font.Height;
                heightTable[font] = height;
            }
            return height;
        }
        public static void DrawArrow(this Graphics g, Pen p, Point pt, double xDir, double yDir, float size)
        {
            PointF[] fig = {
                new PointF(0, 0),
                new PointF(-1, 1),
                new PointF(2, 0),
                new PointF(-1, -1),
                new PointF(0, 0)
            };

            // Pre-calculate rotation and size scaling outside the loop for efficiency
            DrawFigure(g, p, pt, fig, xDir, yDir, size, true);
        }

        static void DrawFigure(this Graphics g, Pen p, PointF pos, PointF[] fig, double xDir, double yDir, float size, bool solid = false)
        {
            float cosAngle = (float)xDir;
            float sinAngle = (float)yDir;

            // Pre-allocated array for transformed points
            PointF[] transformedFig = new PointF[fig.Length];

            for (int i = 0; i < fig.Length; i++)
            {
                // Apply size scaling and rotation in one step
                float scaledX = fig[i].X * size;
                float scaledY = fig[i].Y * size;

                transformedFig[i] = new PointF(
                    cosAngle * scaledX - sinAngle * scaledY + pos.X,
                    sinAngle * scaledX + cosAngle * scaledY + pos.Y
                );
            }

            if (solid)
                g.FillPolygon(new SolidBrush(p.Color), transformedFig);
            else
                g.DrawLines(p, transformedFig);
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

        public static void DrawCross(this Graphics g, Pen p, Rectangle viewPort, double x, double y, int size, string text, Font font)
        {
            DrawCross(g, p, viewPort, x, y, size);

            // Calculate how many lines the text contains
            int enters = text.Count(c => c == '\n') + (text.EndsWith('\n') ? 0 : 1);

            int fontHeight = GetFontHeight(font);
            Rectangle textRect = new Rectangle((int)x + 8, (int)y - (fontHeight * enters), viewPort.Width - 8, fontHeight * enters);

            // Reuse solid brush for better performance
            using (SolidBrush brush = new SolidBrush(p.Color))
            {
                g.DrawFitTextToRectangle(p, viewPort, textRect, text, font);
            }
        }

        public static void DrawCross(this Graphics g, Pen p, Rectangle viewPort, Point pt, int size)
        {
            DrawCross(g, p, viewPort, pt.X, pt.Y, size);
        }


        public static void DrawFitTextToRectangle(this Graphics g, Pen p, Rectangle viewPort, Rectangle rect, string text, Font font)
        {
            // Early exit if the width is too small
            if (rect.Width <= 3)
                return;

            // Check if the rectangle is outside the viewport bounds
            if (rect.Right <= viewPort.X || rect.X >= viewPort.Right)
                return;

            // Adjust the rectangle if it extends outside the viewport on the left or right
            if (rect.X < viewPort.X)
            {
                int delta = viewPort.X - rect.X;
                rect.X = viewPort.X;
                rect.Width -= delta;
            }

            if (rect.Right > viewPort.Right)
            {
                int delta = rect.Right - viewPort.Right;
                rect.Width -= delta;
            }

            // Precalculate the line height
            int lineHeight = GetFontHeight(font);

            // Only split the text if necessary
            string[] lines = text.Contains("\n") ? text.Split('\n') : new string[] { text };

            // Create a reusable rectangle for drawing
            Rectangle drawRect = new Rectangle(rect.X, rect.Y, rect.Width, lineHeight);

            // Cache the TextFormatFlags
            const TextFormatFlags textFormatFlags = TextFormatFlags.NoPadding;

            // Draw each line, only if within the rectangle bounds
            for (int i = 0; i < lines.Length; i++)
            {
                int yPosition = rect.Y + i * lineHeight;
                if (yPosition + lineHeight > rect.Bottom)
                    break; // No need to continue if the line is outside the rectangle bounds

                if (yPosition >= rect.Y)
                {
                    drawRect.Y = yPosition; // Reuse the rectangle by updating Y position
                    TextRenderer.DrawText(g, lines[i], font, drawRect, p.Color, textFormatFlags);
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

                if (p1.X < viewPort.X)
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
            int viewPortRight = viewPort.X + viewPort.Width;
            int viewPortLeft = viewPort.X + 5;
            int rectRight = rect.X + rect.Width;

            // Early return if rectangle is outside the viewport
            if (rect.X > viewPortRight || rectRight < viewPort.X)
                return;

            // Adjust rectangle to fit within the viewport
            if (rect.X < viewPortLeft)
            {
                int delta = viewPortLeft - rect.X;
                rect.X = viewPortLeft;
                rect.Width -= delta;
                closeBegin = false;
            }

            if (rectRight > viewPortRight - 5)
            {
                int delta = (viewPortRight - 5) - rectRight;
                rect.Width += delta;
                closeEnd = false;
            }

            // Calculate bracketWidth, spaceBegin, spaceEnd
            int bracketWidth = rect.Height / 2;
            if (rect.Width < (bracketWidth * 2))
                bracketWidth = rect.Width / 2;

            int spaceBegin = closeBegin ? (closeEnd ? bracketWidth : bracketWidth / 2) : 0;
            int spaceEnd = closeEnd ? (closeBegin ? bracketWidth : bracketWidth / 2) : 0;
            int midY = rect.Y + rect.Height / 2;

            // Calculate top and other values only once
            double top = rect.Y;
            int div = 6;
            double space = rect.Height / (double)div;
            int wibber = rect.Height / 4;

            // Draw the opening arrows or zigzag if closeBegin is false
            if (closeBegin)
            {
                g.DrawLine(p, rect.X, midY, rect.X + spaceBegin, rect.Y);
                g.DrawLine(p, rect.X, midY, rect.X + spaceBegin, rect.Y + rect.Height);
            }
            else
            {
                for (int i = 0; i < div; i++)
                {
                    int startY = (int)(top + space * i);
                    int endY = (int)(top + space * (i + 1));
                    g.DrawLine(p, rect.X - ((i % 2 == 1) ? wibber : 0), startY, rect.X - ((i % 2 == 0) ? wibber : 0), endY);
                }
            }

            // Draw the closing arrows or zigzag if closeEnd is false
            if (closeEnd)
            {
                g.DrawLine(p, rect.X + rect.Width, midY, rect.X + rect.Width - spaceEnd, rect.Y);
                g.DrawLine(p, rect.X + rect.Width, midY, rect.X + rect.Width - spaceEnd, rect.Y + rect.Height);
            }
            else
            {
                for (int i = 0; i < div; i++)
                {
                    int startY = (int)(top + space * i);
                    int endY = (int)(top + space * (i + 1));
                    g.DrawLine(p, rect.X + rect.Width + ((i % 2 == 1) ? wibber : 0), startY, rect.X + rect.Width + ((i % 2 == 0) ? wibber : 0), endY);
                }
            }

            // Draw the rectangle's horizontal lines
            g.DrawLine(p, rect.X + spaceBegin, rect.Y, rect.X + rect.Width - spaceEnd, rect.Y);
            g.DrawLine(p, rect.X + spaceBegin, rect.Y + rect.Height, rect.X + rect.Width - spaceEnd, rect.Y + rect.Height);

            // Draw the text inside the rectangle
            Rectangle textRect = new Rectangle(rect.X + spaceBegin, rect.Y, rect.Width - spaceBegin - spaceEnd, rect.Height);
            DrawFitTextToRectangle(g, p, viewPort, textRect, text, font);
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

    }

}
