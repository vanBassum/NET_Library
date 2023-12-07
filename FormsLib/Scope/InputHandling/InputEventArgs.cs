using FormsLib.Scope.Models;
using FormsLib.Scope.Rendering;
using System.Numerics;

namespace FormsLib.Scope.InputHandling
{
    public class InputEventArgs
    {
        public GraphController GraphController { get; set; }
        public GraphicCalculator Calculator { get; set; }
        public Vector2 ScreenPosition          { get; set; }
        public bool RequestRedraw              { get; set; }
        public MouseButtons MouseButton        { get; set; }
        public bool IsActive                   { get; set; }
        public int Delta                       { get; set; }

        public InputEventArgs(GraphController graphController, GraphicCalculator calculator)
        {
            GraphController = graphController;
            Calculator = calculator;
        }
    }
}
