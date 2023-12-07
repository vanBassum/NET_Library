using FormsLib.Scope.Enums;
using FormsLib.Scope.Graphics;
using FormsLib.Scope.Models;
using System.Diagnostics;
using System.Numerics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

namespace FormsLib.Scope.Rendering
{

    public class BackgroundRenderer
    {
        private readonly GraphController graphController;
        private readonly IGraphics graphics;
        private readonly GraphicCalculator calculations;
        public BackgroundRenderer(GraphController graphController, IGraphics graphics, GraphicCalculator calculations)
        {
            this.graphController = graphController;
            this.graphics = graphics;
            this.calculations = calculations;
        }

        public void Render()
        {
            var stopwatch = Stopwatch.StartNew();



            graphics.Clear(graphController.Settings.BackgroundColor);

            //Draw the viewport
            graphics.DrawRectangle(graphController.Settings.GridPen, calculations.GraphViewport);
            DrawHorizontalGridLines();
            DrawVerticalGridLines();
            DrawTracesScale();
            DrawHorizontalScale();

            stopwatch.Stop();
#if DEBUG

            var debugInfo = $"B: {stopwatch.ElapsedMilliseconds.ToString().PadRight(5)} ms";
            var pos = new Vector2(calculations.GraphViewport.X + calculations.GraphViewport.Width - 60, calculations.GraphViewport.Y + 10);
            graphics.DrawString(debugInfo, graphController.Settings.Font, Brushes.White, pos);
#endif
        }




        void DrawHorizontalGridLines()
        {
            //Draw midLine
            graphics.DrawLine(
                graphController.Settings.GridZeroPen,
                new Vector2(calculations.GraphViewport.X, calculations.ZeroPos),
                new Vector2(calculations.GraphViewport.X + calculations.GraphViewport.Width, calculations.ZeroPos));

            for (int i = 1; i < graphController.Settings.VerticalDivisions / 2; i++)
            {
                float y = i * calculations.PxPerRow;

                float y1 = calculations.GraphViewport.Y + calculations.ZeroPos + y;
                float y2 = calculations.GraphViewport.Y + calculations.ZeroPos - y;

                graphics.DrawLine(
                    graphController.Settings.GridPen,
                    new Vector2(calculations.GraphViewport.X, y1),
                    new Vector2(calculations.GraphViewport.X + calculations.GraphViewport.Width, y1));

                graphics.DrawLine(
                    graphController.Settings.GridPen,
                    new Vector2(calculations.GraphViewport.X, y2),
                    new Vector2(calculations.GraphViewport.X + calculations.GraphViewport.Width, y2));
            }
        }

        void DrawTracesScale()
        {
            float fontHeight = graphController.Settings.Font.Height;
            float offset = graphController.Traces.Count(t => t.Visible/* && t.DrawOptions.HasFlag(DrawOptions.ShowScale)*/) * fontHeight;
            offset = -offset / 2;
            foreach (var trace in graphController.Traces)
            {
                Brush brush = new SolidBrush(trace.Color);
                var screenToUnits = calculations.ScreenToWorld(trace);

                for (int y = 1; y < graphController.Settings.VerticalDivisions; y++)
                {
                    Vector2 viewPos = new Vector2(0, y * calculations.PxPerRow);
                    Vector2 worldPos = screenToUnits(viewPos);
                    graphics.DrawString(worldPos.Y.ToString(), new Font("Arial", 8), brush, viewPos + calculations.IndicatorColViewport.Location.ToVector2() + Vector2.UnitY * offset);
                }

                offset += fontHeight;
            }
        }



        void DrawVerticalGridLines()
        {
            for (int i = 1; i < graphController.Settings.HorizontalDivisions; i++)
            {
                float x = i * calculations.PxPerColumn;
                graphics.DrawLine(
                    graphController.Settings.GridPen,
                    new Vector2(x, calculations.GraphViewport.Y),
                    new Vector2(x, calculations.GraphViewport.Y + calculations.GraphViewport.Height));
            }
        }


        void DrawHorizontalScale()
        {
            Brush brush = new SolidBrush(graphController.Settings.ForegroundColor);
            var screenToUnits = calculations.ScreenToWorld();

            for (int x = 1; x < graphController.Settings.HorizontalDivisions; x++)
            {
                Vector2 viewPos = new Vector2(x * calculations.PxPerColumn, 0);
                Vector2 worldPos = screenToUnits(viewPos);
                graphics.DrawString(worldPos.X.ToString(), new Font("Arial", 8), brush, viewPos + calculations.IndicatorRowViewport.Location.ToVector2());
            }
        }
    }
}
