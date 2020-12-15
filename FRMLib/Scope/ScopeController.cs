﻿using STDLib.Misc;
using System;

namespace FRMLib.Scope
{
    public class ScopeController
    {
        public ThreadedBindingList<Trace> Traces { get; private set; } = new ThreadedBindingList<Trace>();
        public ThreadedBindingList<Cursor> Cursors { get; private set; } = new ThreadedBindingList<Cursor>();
        public ThreadedBindingList<Marker> Markers { get; private set; } = new ThreadedBindingList<Marker>();
        public ThreadedBindingList<MathItem> MathItems { get; private set; } = new ThreadedBindingList<MathItem>();
        public Func<double, string> HorizontalToHumanReadable { get; set; } = TicksToString;


        public void Clear()
        {
            MathItems.Clear();
            Cursors.Clear();
            Markers.Clear();
            Traces.Clear();
        }

        static string TicksToString(double ticks)
        {
            DateTime dt = new DateTime((long)ticks);
            return dt.ToString("dd-MM-yyyy") + " \r\n" + dt.ToString("HH:mm:ss");
        }
    }






}
