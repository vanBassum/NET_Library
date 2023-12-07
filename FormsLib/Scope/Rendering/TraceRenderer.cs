using FormsLib.Scope.Enums;
using FormsLib.Scope.Graphics;
using FormsLib.Scope.Models;
using System.Numerics;

namespace FormsLib.Scope.Rendering
{
    public class TraceRenderer
    {
        private readonly GraphController graphController;
        private readonly GraphTrace trace;
        private readonly IGraphics graphics;
        private readonly GraphicCalculator calculator;

        private readonly Func<Vector2, Vector2> worldToScreen;
        private readonly Pen pen;
        private readonly Brush brush;


        public TraceRenderer(GraphController graphController, GraphTrace trace, IGraphics graphics, GraphicCalculator calculator)
        {
            this.graphController = graphController;
            this.trace = trace;
            this.graphics = graphics;
            this.calculator = calculator;
            this.pen = new Pen(trace.Color);
            this.brush = new SolidBrush(trace.Color);
            worldToScreen = calculator.WorldToScreen(trace);
        }




        public void Render()
        {
            if (!trace.Visible)
                return;

            switch (trace.DrawStyle)
            {
                case DrawStyles.Points:
                    DrawAsPoints();
                    break;
                case DrawStyles.Lines:
                    DrawAsLines();
                    break;
                case DrawStyles.NonInterpolatedLine:
                    DrawAsNonInterpolatedLine();
                    break;
                case DrawStyles.DiscreteSingal:
                    DrawAsDiscreteSingal();
                    break;
                case DrawStyles.State:
                    DrawAsState();
                    break;
                case DrawStyles.Cross:
                    DrawAsCross();
                    break;
                default:
                    throw new NotImplementedException($"Drawing of '{trace.DrawStyle}' is not supported yet.");

            }
        }

        void DrawAsPoints()
        {
            foreach(var point in trace.Points)
            {
                var screenPos = worldToScreen(point);
                if (!GraphicCalculator.IsPointWithinViewport(calculator.GraphViewport, screenPos))
                    continue;

                if (trace.DrawOptions.HasFlag(DrawOptions.ShowCrosses))
                    graphics.DrawCross(pen, screenPos, Vector2.One * 5f);

                graphics.DrawSolidEllipse(brush, screenPos, Vector2.One * 3f);
            }
        }

        void DrawAsLines()
        {
            Vector2 previous = Vector2.Zero;
            for (int i=0; i< trace.Points.Count; i++)
            {
                var point = trace.Points[i];
                var screenPos = worldToScreen(point);

                if (GraphicCalculator.IsPointWithinViewport(calculator.GraphViewport, screenPos) || GraphicCalculator.IsPointWithinViewport(calculator.GraphViewport, previous))
                {
                    if (trace.DrawOptions.HasFlag(DrawOptions.ShowCrosses))
                        graphics.DrawCross(pen, screenPos, Vector2.One * 5f);

                    if (i > 0)
                    {
                        graphics.DrawLine(pen, previous, screenPos);
                    }
                }
                previous = screenPos;
            }
        }

        void DrawAsNonInterpolatedLine()
        {
            throw new NotImplementedException($"Drawing of '{trace.DrawStyle}' is not supported yet.");
        }

        void DrawAsDiscreteSingal()
        {
            throw new NotImplementedException($"Drawing of '{trace.DrawStyle}' is not supported yet.");
        }

        void DrawAsState()
        {
            Vector2 previous = Vector2.Zero;
            string text = "";
            for (int i = 0; i < trace.Points.Count; i++)
            {
                var point = trace.Points[i];
                var screenPos = worldToScreen(point);
                if (GraphicCalculator.IsPointWithinViewport(calculator.GraphViewport, screenPos))
                {
                    if (trace.DrawOptions.HasFlag(DrawOptions.ShowCrosses))
                        graphics.DrawCross(pen, screenPos, Vector2.One * 5f);

                    if (i > 0)
                    {
                        RectangleF rect = new RectangleF(previous.X, trace.Offset * trace.Scale - 5 + calculator.ZeroPos + 20, screenPos.X - previous.X, 12);
                        DrawState(rect, text);
                    }
                }
                text = trace.ToHumanReadable(point.Y);
                previous = screenPos;
            }
        }

        void DrawAsCross()
        {
            throw new NotImplementedException($"Drawing of '{trace.DrawStyle}' is not supported yet.");
        }




        void DrawState(RectangleF rect, string text)
        {
            Font font = graphController.Settings.Font;
            bool closeBegin = true;
            bool closeEnd =   true;

            var viewPort = calculator.GraphViewport;

            if (rect.X < viewPort.X + 5)
            {
                float delta = viewPort.X + 5 - rect.X;
                rect.X = viewPort.X + 5;
                rect.Width -= delta;
                closeBegin = false;
            }

            if ((rect.X + rect.Width) > (viewPort.X + viewPort.Width - 5))
            {
                float delta = (viewPort.X + viewPort.Width - 5) - (rect.X + rect.Width);
                rect.Width += delta;
                closeEnd = false;
            }

            //Draw opening and closing arrow.
            float bracketWidth = rect.Height / 2;

            if (rect.Width < (bracketWidth * 2))
                bracketWidth = rect.Width / 2;

            float spaceBegin = closeBegin ? closeEnd ? bracketWidth : bracketWidth / 2 : 0;
            float spaceEnd = closeEnd ? closeBegin ? bracketWidth : bracketWidth / 2 : 0;
            float midY = rect.Y + rect.Height / 2;

            float top = rect.Y;
            int div = 6;
            float space = rect.Height / div;
            float wibber = rect.Height / 4;

            if (closeBegin)
            {
                graphics.DrawLine(pen, new Vector2(rect.X, midY), new Vector2(rect.X + spaceBegin, rect.Y));
                graphics.DrawLine(pen, new Vector2(rect.X, midY), new Vector2(rect.X + spaceBegin, rect.Y + rect.Height));
            }
            else
            {
                for (int i = 0; i < div; i++)
                    graphics.DrawLine(pen, new Vector2(rect.X - (((i % 2) == 1) ? wibber : 0), (int)(top + space * i)), new Vector2(rect.X - (((i % 2) == 0) ? wibber : 0), (int)(top + space * (i + 1))));
            }

            if (closeEnd)
            {
                graphics.DrawLine(pen, new Vector2(rect.X + rect.Width, midY), new Vector2(rect.X + rect.Width - spaceEnd, rect.Y));
                graphics.DrawLine(pen, new Vector2(rect.X + rect.Width, midY), new Vector2(rect.X + rect.Width - spaceEnd, rect.Y + rect.Height));
            }
            else
            {
                for (int i = 0; i < div; i++)
                    graphics.DrawLine(pen, new Vector2(rect.X + rect.Width + (((i % 2) == 1) ? wibber : 0), (int)(top + space * i)), new Vector2(rect.X + rect.Width + (((i % 2) == 0) ? wibber : 0), (int)(top + space * (i + 1))));
            }


            graphics.DrawLine(pen, new Vector2(rect.X + spaceBegin, rect.Y), new Vector2(rect.X + rect.Width - spaceEnd, rect.Y));
            graphics.DrawLine(pen, new Vector2(rect.X + spaceBegin, rect.Y + rect.Height), new Vector2(rect.X + rect.Width - spaceEnd, rect.Y + rect.Height));

            var point = new Vector2(rect.X + spaceBegin, rect.Y);
            var size = new Vector2(rect.Width - spaceBegin - spaceEnd, rect.Height);
            graphics.DrawString(text, font, brush, point, size);
        }


    }



}
