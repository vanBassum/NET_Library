using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Linq;

namespace Oscilloscope
{
    public class ScopeClass
    {
        public ScopeSettings Settings { get; set; } = new ScopeSettings();
        public BindingList<Trace> Traces { get; set; } = new BindingList<Trace> { };
        public BindingList<MarkerLine> MarkerLines { get; set; } = new BindingList<MarkerLine> { };


        public void AutoScale(bool HorScaleOnly = false)
        {
            double xMin = double.MaxValue;
            double xMax = double.MinValue;

            for (int i = 0; i < Traces.Count; i++)
            {
                if(!HorScaleOnly)
                    Traces[i].AutoScale(Settings.Grid.VerticalDivisions);

                foreach(PointD pt in Traces[i].Points)
                {
                    if (pt.X < xMin)
                        xMin = pt.X;

                    if (pt.X > xMax)
                        xMax = pt.X;
                }
            }
            Settings.TimeBase = (xMax - xMin); // / Settings.Grid.HorizontalDivisions;
            Settings.Offset = -xMin;
            //XMax = xMax;
        }

        public void AddMarkerLine(double X)
        {
            MarkerLines.Add(new MarkerLine(this) { X = X, IsDate = false});
        }

        public void AddMarkerLine(DateTime X)
        {
            MarkerLines.Add(new MarkerLine(this) { X = X.Ticks, IsDate = true });
        }

        public IEnumerable<PointD> GetDataBetweenMarkers(MarkerLine m1, MarkerLine m2, Trace trace)
        {
            double x1 = m1.X;
            double x2 = m2.X;
                
            if(x1 > x2)
            {
                x2 = m1.X;
                x1 = m2.X;
            }

            var tracePoints = from pt in trace.Points
                                where pt.X >= x1 && pt.X <= x2
                                select pt;
            
            return tracePoints;
        }
        


        
    }
}
