namespace FormsLib.Scope.InputHandling
{

    public interface IInputHandler
    {
        void HandleMouseWheel(object sender, InputEventArgs e);
        void HandleMouseDown(object sender, InputEventArgs e);
        void HandleMouseUp(object sender, InputEventArgs e);
        void HandleMouseMove(object sender, InputEventArgs e);
        void HandleMouseClick(object sender, InputEventArgs e);
    }
}
