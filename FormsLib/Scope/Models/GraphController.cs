
namespace FormsLib.Scope.Models
{
    public class RefreshDetails
    {

    }

    public class GraphController
    {
        public event EventHandler<RefreshDetails>? OnRefreshRequested;
        public List<GraphTrace> Traces { get; }   = new();
        public List<GraphMarker> Markers { get; } = new();
        public GraphSettings Settings { get; } = new GraphSettings();






        /*
        public event EventHandler? DoRedraw;
        public ThreadedBindingList<GraphTrace> Traces { get; } = new ThreadedBindingList<GraphTrace>();
        public ThreadedBindingList<GraphMarker> Markers { get; } = new ThreadedBindingList<GraphMarker>();
        public ThreadedBindingList<Label> Labels { get; } = new ThreadedBindingList<Label>();
        public ThreadedBindingList<IScopeDrawable> Drawables { get; } = new ThreadedBindingList<IScopeDrawable>();
        public GraphSettings Settings { get; } = new GraphSettings();




        public void RedrawAll()
        {
            DoRedraw?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            Markers.Clear();
            Labels.Clear();
            Traces.Clear();
        }


        public void ClearData()
        {
            Markers.Clear();
            Labels.Clear();
            foreach (var v in Traces)
                v.Points.Clear();
        }

        public static string TicksToString(double ticks)
        {
            DateTime dt = new DateTime((long)ticks);
            return dt.ToString("dd-MM-yyyy") + " \r\n" + dt.ToString("HH:mm:ss");
        }


        public GraphTrace GetOrCreateTraceByKey(string key, Action<GraphTrace>? configure = null)
        {
            GraphTrace? trace = Traces.FirstOrDefault(t => t.Key == key);
            if (trace == null)
            {
                trace = new GraphTrace { Key = key };
                Traces.Add(trace);
                configure?.Invoke(trace);
            }
            return trace;
        }
        */
    }

    //public interface IScopeDrawable
    //{
    //    PointD Point { get; set; }
    //    void Draw(Graphics g, Func<PointD, Point> convert);
    //
    //}
}
