using STDLib.Math;
using STDLib.Misc;
using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public class ScopeController
    {
        public ThreadedBindingList<Trace> Traces { get; private set; } = new ThreadedBindingList<Trace>();
        public ThreadedBindingList<Cursor> Cursors { get; private set; } = new ThreadedBindingList<Cursor>();
        public ThreadedBindingList<Marker> Markers { get; private set; } = new ThreadedBindingList<Marker>();
        public ThreadedBindingList<MathItem> MathItems { get; private set; } = new ThreadedBindingList<MathItem>();
        public ThreadedBindingList<IScopeDrawable> Drawables { get; private set; } = new ThreadedBindingList<IScopeDrawable>();
        public ScopeViewSettings Settings { get; private set; } = new ScopeViewSettings();

        public event EventHandler DoRedraw;

        public void RedrawAll()
        {
            DoRedraw?.Invoke(this, null);
        }
        
        public void Clear()
        {
            MathItems.Clear();
            Cursors.Clear();
            Markers.Clear();
            Traces.Clear();
        }


        public void ClearData()
        {
            MathItems.Clear();
            Cursors.Clear();
            Markers.Clear();
            foreach (var v in Traces)
                v.Points.Clear();
        }
    }

    public interface IScopeDrawable
    {
        PointD Point { get; set; }

        void Draw(Graphics g,  Func<PointD, Point> convert);

    }





}
