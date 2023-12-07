using FormsLib.Scope.Helpers;
using FormsLib.Scope.Models;
using Microsoft.VisualBasic;

namespace FormsLib.Scope.InputHandling
{
    public class InputController
    {
        public event EventHandler? RequestRefresh;
        private readonly Control control;
        private IInputHandler? activeHandler;
        public List<IInputHandler> Handlers { get; } = new List<IInputHandler>();
        public GraphController GraphController { get; set; }

        public InputController(Control control, GraphController graphController)
        {
            this.control = control;
            this.GraphController = graphController;
            control.MouseWheel += (s, e) => Do(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseWheel(this, ev));
            control.MouseDown += (s, e) =>  Do(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseDown(this, ev));
            control.MouseUp += (s, e) =>    Do(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseUp(this, ev));
            control.MouseMove += (s, e) =>  Do(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseMove(this, ev));
            control.MouseClick += (s, e) => Do(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseClick(this, ev));

        }

        void Do(MouseEventArgs e, Action<IInputHandler, InputEventArgs> func)
        {

        }
    }
}
