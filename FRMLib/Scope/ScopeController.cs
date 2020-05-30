using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using STDLib.Misc;

namespace FRMLib.Scope
{
    public class ScopeController
    {
        public ThreadedBindingList<Trace> Traces { get; private set; } = new ThreadedBindingList<Trace>();
        public ThreadedBindingList<Marker> Markers { get; private set; } = new ThreadedBindingList<Marker>();


    }


}
