using FormsLib.Scope.Enums;
using FormsLib.Scope.Graphics;
using FormsLib.Scope.Helpers;
using FormsLib.Scope.InputHandling;
using FormsLib.Scope.Models;
using FormsLib.Scope.Rendering;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Windows.Forms;

namespace FormsLib.Scope.Controls
{
    public partial class GraphPlotView : UserControl
    {
        private readonly PictureBox pictureBox1;
        private readonly PictureBox pictureBox2;
        private readonly PictureBox pictureBox3;

        private GraphController graphController = new GraphController();
        public GraphController GraphController
        {
            get { return graphController; }
            set
            {
                if (graphController != null) graphController.OnRefreshRequested -= DataSource_OnRefreshRequested;
                graphController = value;
                if (graphController != null) graphController.OnRefreshRequested += DataSource_OnRefreshRequested;
                
            }
        }

        public GraphPlotView()
        {
            InitializeComponent();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();

            this.Controls.Add(pictureBox1);
            pictureBox1.Controls.Add(pictureBox2);
            pictureBox2.Controls.Add(pictureBox3);

            pictureBox1.Dock = DockStyle.Fill;
            pictureBox2.Dock = DockStyle.Fill;
            pictureBox3.Dock = DockStyle.Fill;

            pictureBox1.BackColor = Color.Transparent;
            pictureBox2.BackColor = Color.Transparent;
            pictureBox3.BackColor = Color.Transparent;

            pictureBox1.Paint += (s, e) =>
            {
                var graphics = new GDIGraphics(e.Graphics);
                var viewport = new Rectangle(0, 0, pictureBox1.Width - 1, pictureBox1.Height - 1);
                var calc = new GraphicCalculator(graphController, viewport);
                var renderer = new BackgroundRenderer(graphController, graphics, calc);
                renderer.Render();
            };

            pictureBox2.Paint += (s, e) =>
            {
                var graphics = new GDIGraphics(e.Graphics);
                var viewport = new Rectangle(0, 0, pictureBox2.Width - 1, pictureBox2.Height - 1);
                var calc = new GraphicCalculator(graphController, viewport);
                var renderer = new GraphPlotRenderer(graphController, graphics, calc);
                renderer.Render();
            };

            InitInputHandlers(pictureBox3);

        }

        private void DataSource_OnRefreshRequested(object? sender, RefreshDetails e)
        {
            
        }

        private void InitInputHandlers(Control control)
        {
            control.MouseWheel += (s, e) => HandleInput(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseWheel(this, ev));
            control.MouseDown += (s, e) => HandleInput(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseDown(this, ev));
            control.MouseUp += (s, e) => HandleInput(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseUp(this, ev));
            control.MouseMove += (s, e) => HandleInput(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseMove(this, ev));
            control.MouseClick += (s, e) => HandleInput(e, (IInputHandler a, InputEventArgs ev) => a.HandleMouseClick(this, ev));


            Handlers.Add(new Panning());
            Handlers.Add(new Zooming());
        }

        private IInputHandler? activeHandler;
        public List<IInputHandler> Handlers { get; } = new List<IInputHandler>();
        void HandleInput(MouseEventArgs e, Action<IInputHandler, InputEventArgs> func)
        {

            var viewport = new Rectangle(0, 0, pictureBox3.Width - 1, pictureBox3.Height - 1);
            var calc = new GraphicCalculator(graphController, viewport);
            var args = new InputEventArgs(GraphController, calc)
            {
                Delta = e.Delta,
                IsActive = false,
                MouseButton = e.Button,
                RequestRedraw = false,
                ScreenPosition = e.Location.ToVector2()
            };

            if (activeHandler != null)
                func(activeHandler, args);
            else
            {
                foreach (var action in Handlers)
                {
                    func(action, args);
                    if (args.IsActive)
                    {
                        activeHandler = action;
                        break;
                    }
                }
            }

            if (!args.IsActive)
                activeHandler = null;
            if (args.RequestRedraw)
            {
                pictureBox1.Refresh();
                pictureBox2.Refresh();
                pictureBox3.Refresh();
            }
        }
    }
}
