using FormsLib.Scope.Enums;
using FormsLib.Scope.Models;
using System.Drawing;
using System.Numerics;

namespace FormsLib.Scope.Rendering
{
    public class GraphicCalculator
    {
        private readonly GraphController graphController;
        public int PxPerColumn { get; }
        public int PxPerRow { get; }
        public int ZeroPos { get; }
        public RectangleF GraphViewport { get; }
        
        /// <summary>
        /// Viewport of the column next to the graph for displaying scale information ect.
        /// </summary>
        public RectangleF IndicatorColViewport { get; }
        
        /// <summary>
        /// Viewport of the row next to the graph for displaying scale information ect.
        /// </summary>
        public RectangleF IndicatorRowViewport { get; }

        public GraphicCalculator(GraphController graphController, Rectangle viewPort)
        {
            this.graphController = graphController;
            float indicatorColW = 45;
            float indicatorRowH = 25;
            float graphW = viewPort.Width - indicatorColW;
            float graphH = viewPort.Height - indicatorRowH;

            int columns = graphController.Settings.HorizontalDivisions;
            int rows = graphController.Settings.VerticalDivisions;
            int pxPerColumn = (int)(graphW / columns);
            int pxPerRow = (int)(graphH / rows);
            int restWidth = (int)(graphW % columns);
            int restHeight = (int)(graphH % rows);

            graphW = pxPerColumn * columns;
            graphH = pxPerRow * rows;
            indicatorColW += restWidth;
            indicatorRowH += restHeight;

            float indicatorRowY = graphController.Settings.DrawScalePosHorizontal == DrawPosHorizontal.Top ? 0 : graphH;
            float indicatorColX = graphController.Settings.DrawScalePosVertical == DrawPosVertical.Left ? 0 : graphW;
            float indicatorRowX = graphController.Settings.DrawScalePosVertical == DrawPosVertical.Right ? 0 : indicatorColW;
            float indicatorColY = graphController.Settings.DrawScalePosHorizontal == DrawPosHorizontal.Bottom ? 0 : indicatorRowH;

            float graphX = graphController.Settings.DrawScalePosVertical == DrawPosVertical.Right ? 0 : indicatorColW;
            float graphY = graphController.Settings.DrawScalePosHorizontal == DrawPosHorizontal.Bottom ? 0 : indicatorRowH;
            float indicatorColH = graphH;
            float indicatorRowW = graphW;

            float zeroPos = 0;
            switch (graphController.Settings.ZeroPosition)
            {
                case VerticalZeroPosition.Middle:
                    zeroPos = graphH / 2;
                    break;
                case VerticalZeroPosition.Top:
                    zeroPos = 0;
                    break;
                case VerticalZeroPosition.Bottom:
                    zeroPos = graphH;
                    break;
            }

            PxPerColumn = pxPerColumn;
            PxPerRow = pxPerRow;
            ZeroPos = (int)zeroPos;
            GraphViewport = new RectangleF(graphX, graphY, graphW, graphH);
            IndicatorColViewport = new RectangleF(indicatorColX, indicatorColY, indicatorColW, indicatorColH);
            IndicatorRowViewport = new RectangleF(indicatorRowX, indicatorRowY, indicatorRowW, indicatorRowH);
        }


        public Func<Vector2, Vector2> WorldToScreen(GraphTrace trace)
        {
            return (point) => new Vector2 
            (
                
                (point.X - graphController.Settings.HorizontalOffset) * PxPerColumn / graphController.Settings.HorizontalScale,
                ZeroPos - (point.Y - trace.Offset) * PxPerRow / trace.Scale
            );
        }

        public Func<Vector2, Vector2> ScreenToWorld(GraphTrace trace)
        {
            return (point) => new Vector2
            (
                point.X * graphController.Settings.HorizontalScale / PxPerColumn + graphController.Settings.HorizontalOffset,
                (ZeroPos - point.Y) * trace.Scale / PxPerRow + trace.Offset
            );
        }

        public Func<Vector2, Vector2> ScreenToWorld()
        {
            return (point) => new Vector2
            (
                point.X * graphController.Settings.HorizontalScale / PxPerColumn + graphController.Settings.HorizontalOffset,
                0
            );
        }

        public static bool IsPointWithinViewport(RectangleF viewport, Vector2 pt)
        {
            return
                pt.X >= viewport.X &&
                pt.X <= viewport.X + viewport.Width &&
                pt.Y >= viewport.Y &&
                pt.Y <= viewport.Y + viewport.Height;
        }
    }
}
