using FRMLib.Scope.Controls;
using STDLib.Misc;
using System;
using System.Drawing;

namespace FRMLib.Scope
{
    public class Trace : PropertySensitive
    {
        [TraceViewAttribute(Text = "", Width = 20)]
        public Pen Pen { get { return GetPar(Pens.Red); } set { SetPar(value); } }

        [TraceViewAttribute(Width = 20, Text = "")]
        public bool Visible { get { return GetPar(true); } set { SetPar(value); } }
        [TraceViewAttribute]
        public string Name { get { return GetPar("New Trace"); } set { SetPar(value); } }
        //[TraceViewAttribute]
        public string Unit { get { return GetPar(""); } set { SetPar(value); } }
        [TraceViewAttribute(Width = 50)]
        public double Scale { get { return GetPar<double>(1f); } set { SetPar<double>(value); } }
        [TraceViewAttribute(Width = 50)]
        public double Offset { get { return GetPar<double>(0f); } set { SetPar<double>(value); } }
        [TraceViewAttribute(Width = 50)]
        public int Layer { get { return GetPar(10); } set { SetPar(value); } }
        public ThreadedBindingList<PointD> Points { get; } = new ThreadedBindingList<PointD>();
        public DrawStyles DrawStyle { get { return GetPar(DrawStyles.Lines); } set { SetPar(value); } }
        public DrawOptions DrawOption { get { return GetPar(DrawOptions.None); } set { SetPar(value); } }
        public Func<double, string> ToHumanReadable { get { return GetPar( new Func<double, string>((a) => a.ToHumanReadable(3)) ); } set { SetPar(value); } }
        public PointD Minimum { get { return GetPar(PointD.Empty); } set { SetPar(value); } }
        public PointD Maximum { get { return GetPar(PointD.Empty); } set { SetPar(value); } }

        public Trace()
        {
            Points.ListChanged += Points_ListChanged;
        }

        private void Points_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            switch(e.ListChangedType)
            {
                case System.ComponentModel.ListChangedType.ItemAdded:
                    DoMinMax(Points[e.NewIndex]);
                    break;

                default:
                    //TODO: This has a serious potential to be optimized!
                    Minimum = PointD.Empty;
                    Maximum = PointD.Empty;
                    foreach (PointD pt in Points)
                        DoMinMax(pt);
                    break;
            }
        }

        void DoMinMax(PointD pt)
        {
            Minimum.KeepMinimum(pt);
            Maximum.KeepMaximum(pt);
        }

        public enum DrawStyles
        {
            Points,
            Lines,
            NonInterpolatedLine,
            DiscreteSingal,
            //State,
        }

        [Flags]
        public enum DrawOptions
        {
            None = 0,
            ShowCrosses = 1,
            //ExtendBegin,
            //ExtendEnd,
        }
    }

   

}
