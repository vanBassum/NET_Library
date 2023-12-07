using System.Diagnostics;
using System.Numerics;

namespace FormsLib.Scope.InputHandling
{
    internal class Panning : IInputHandler
    {
        private bool isDragging = false;
        private Vector2 startScreenPos;
        private float startOffset;

        public void HandleMouseClick(object sender, InputEventArgs e)
        {
            e.IsActive = isDragging;
        }

        public void HandleMouseWheel(object sender, InputEventArgs e)
        {

            e.IsActive = isDragging;
        }


        public void HandleMouseDown(object sender, InputEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Left)
            {
                isDragging = true;
                startScreenPos = e.ScreenPosition;
                startOffset = e.GraphController.Settings.HorizontalOffset;
                e.IsActive = true;
            }
        }

        public void HandleMouseUp(object sender, InputEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Left)
            {
                isDragging = false;
                e.IsActive = false;
            }
        }

        public void HandleMouseMove(object sender, InputEventArgs e)
        {
            if (isDragging)
            {
                float screenDistance = startScreenPos.X - e.ScreenPosition.X;

                // Calculate the number of screen pixels per division
                float pxPerDivision = e.Calculator.GraphViewport.Width / e.GraphController.Settings.HorizontalDivisions;

                // Calculate the new offset
                float newOffset = startOffset + screenDistance / pxPerDivision;

                // Update the horizontal offset
                e.GraphController.Settings.HorizontalOffset = newOffset * e.GraphController.Settings.HorizontalScale;

                // Mark for redraw and deactivate
                e.RequestRedraw = true;
                e.IsActive = false;

            }
        }
    }
}
