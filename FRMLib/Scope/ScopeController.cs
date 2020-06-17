using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FRMLib.Scope.Controls;
using STDLib.Misc;

namespace FRMLib.Scope
{
    public class ScopeController
    {
        public ThreadedBindingList<Trace> Traces { get; private set; } = new ThreadedBindingList<Trace>();
        public ThreadedBindingList<Marker> Markers { get; private set; } = new ThreadedBindingList<Marker>();
        public ThreadedBindingList<MathItem> MathItems { get; private set; } = new ThreadedBindingList<MathItem>();
        public Func<double, string> HorizontalToHumanReadable { get; set; } = TicksToString;



        static string TicksToString(double ticks)
        {
            DateTime dt = new DateTime((long)ticks);
            return dt.ToString("dd-MM-yyyy") + " \r\n" + dt.ToString("HH:mm:ss");
        }
    }


}
