using System.Numerics;

namespace FormsLib.Scope.InputHandling
{
    internal class Zooming : IInputHandler
    {
        public float ZoomFactor { get; set; } = 1.25f;

        public void HandleMouseClick(object sender, InputEventArgs e)
        {

        }

        public void HandleMouseWheel(object sender, InputEventArgs e)
        {
            Vector2 mouseScreenPosition = e.ScreenPosition;
            float zoomFactor = e.Delta > 0 ? this.ZoomFactor : 1f / this.ZoomFactor; // Adjust the zoom factor as needed

            // Calculate the new scale
            e.GraphController.Settings.HorizontalScale *= zoomFactor;

           //if (e.GraphController.Settings.HorizontalScale < MinZoom)
           //    e.GraphController.Settings.HorizontalScale = MinZoom;
           //if (e.GraphController.Settings.HorizontalScale > MaxZoom)
           //    e.GraphController.Settings.HorizontalScale = MaxZoom;

            // Adjust the offset to pivot around the mouse position
            //e.GraphController.Settings.HorizontalOffset = mouseScreenPosition.X - mouseScreenPosition.X / zoomFactor;
            e.RequestRedraw = true;
        }



        public void HandleMouseDown(object sender, InputEventArgs e)
        {
        }

        public void HandleMouseUp(object sender, InputEventArgs e)
        {
        }

        public void HandleMouseMove(object sender, InputEventArgs e)
        {
        }
    }
}
