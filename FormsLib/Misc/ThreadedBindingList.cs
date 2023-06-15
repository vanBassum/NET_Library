using FormsLib.Maths;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace FormsLib.Misc
{
    public class ThreadedBindingList<T> : BindingList<T>
    {
        private readonly SynchronizationContext ctx;
        public ThreadedBindingList()
        {
            ctx = SynchronizationContext.Current;
        }

        public void AddRange(ICollection<T> collecion)
        {
            foreach (T t in collecion)
                Add(t);
        }

        protected override void OnAddingNew(AddingNewEventArgs e)
        {
            SynchronizationContext ctx = SynchronizationContext.Current;
            if (ctx == null)
            {
                BaseAddingNew(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseAddingNew(e);
                }, null);
            }
        }
        void BaseAddingNew(AddingNewEventArgs e)
        {
            base.OnAddingNew(e);
        }
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            if (ctx == null)
            {
                BaseListChanged(e);
            }
            else
            {
                ctx.Send(delegate
                {
                    BaseListChanged(e);
                }, null);
            }
        }
        void BaseListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
        }

        public void RemoveWhere(Predicate<T> predicate)
        {
            lock (this)
            {
                for (int i = 0; i < Count; i++)
                    if (predicate(this[i]))
                    {
                        RemoveAt(i);
                        i--;
                    }
            }
        }
    }

    public static class Ext
    {
        public static void Add(this ThreadedBindingList<PointD> list, double x, double y)
        {
            list.Add(new PointD(x, y));
        }
    }
}