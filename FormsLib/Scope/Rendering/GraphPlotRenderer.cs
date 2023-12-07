using FormsLib.Extentions;
using FormsLib.Scope.Graphics;
using FormsLib.Scope.Helpers;
using FormsLib.Scope.Models;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.AxHost;

namespace FormsLib.Scope.Rendering
{
    public class GraphPlotRenderer
    {
        private readonly GraphController graphController;
        private readonly IGraphics graphics;
        private readonly GraphicCalculator calculator;
        public GraphPlotRenderer(GraphController graphController, IGraphics graphics, GraphicCalculator calculator)
        {
            this.graphController = graphController;
            this.graphics = graphics;
            this.calculator = calculator;
        }

        public void Render()
        {

            if (graphController == null)
            {
                graphics.DrawString("No graphController bound", new Font("Ariel", 8.0f), Brushes.White, calculator.GraphViewport.Location.ToVector2());
                return;
            }
            var stopwatch = Stopwatch.StartNew();

            
            //Loop through traces
            foreach (var trace in graphController.Traces)
            {
                TraceRenderer traceRenderer = new TraceRenderer(graphController, trace, graphics, calculator);
                traceRenderer.Render();
            }

            stopwatch.Stop();
#if DEBUG

            var debugInfo = $"D: {stopwatch.ElapsedMilliseconds.ToString().PadRight(5)} ms";
            var pos = new Vector2(calculator.GraphViewport.X + calculator.GraphViewport.Width - 60, calculator.GraphViewport.Y + 20);
            graphics.DrawString(debugInfo, graphController.Settings.Font, Brushes.White, pos);
#endif
            /*
            try
            {
                foreach (Label marker in graphController.Labels)
                {
                    if (marker is TraceLabel lm)
                    {
                        if (graphController.Traces.Contains(lm.Trace))
                        {
                            float pxPerUnits_ver = viewPort.Height / (graphController.Settings.VerticalDivisions * lm.Trace.Scale);
                            Func<Vector2, Point> convert = (p) => new Point(
                                (int)((p.X + graphController.Settings.HorizontalOffset) * pxPerUnits_hor) + viewPort.X,
                                (int)(zeroPos - (p.Y + lm.Trace.Offset) * pxPerUnits_ver));

                            marker.Draw(g, graphController.Settings.Style, viewPort, convert);
                        }
                    }
                    else if (marker is FreeGraphLabel fm)
                    {
                        double pxPerUnits_ver = viewPort.Height / (graphController.Settings.VerticalDivisions * 1);
                        Func<Vector2, Point> convert = (p) => new Point(
                            (int)((p.X + graphController.Settings.HorizontalOffset) * pxPerUnits_hor) + viewPort.X,
                            (int)(zeroPos - (p.Y + 0) * pxPerUnits_ver));

                        marker.Draw(g, graphController.Settings.Style, viewPort, convert);
                    }

                }
            }
            catch (Exception ex)
            {
                g.DrawString(ex.Message, graphController.Settings.Style.Font, errBrush, new Point(0, (errNo++) * graphController.Settings.Style.Font.Height));
            }

            try
            {
                foreach (IScopeDrawable drawable in graphController.Drawables)
                {
                    Func<Vector2, Point> convert = (p) => new Point((int)((p.X + graphController.Settings.HorizontalOffset) * pxPerUnits_hor), (int)(viewPort.Height / 2 - p.Y));
                    drawable.Draw(g, convert);

                }
            }
            catch (Exception ex)
            {
                g.DrawString(ex.Message, graphController.Settings.Style.Font, errBrush, new Point(0, (errNo++) * graphController.Settings.Style.Font.Height));
            }


            stopwatch.Stop();
#if DEBUG
            g.DrawString($"D: {stopwatch.ElapsedMilliseconds.ToString().PadRight(5)} ms", graphController.Settings.Style.Font, Brushes.White, new Point(viewPort.Width, 1 * graphController.Settings.Style.Font.Height), new StringFormat() { Alignment = StringAlignment.Far });
#endif
            */
        }


    }



}
