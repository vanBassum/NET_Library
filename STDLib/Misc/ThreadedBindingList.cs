using FRMLib.Scope;
using System;
using System.ComponentModel;
using System.Threading;

namespace STDLib.Misc
{
    public class ThreadedBindingList<T> : BindingList<T>
    {
        private readonly SynchronizationContext ctx;
        public ThreadedBindingList()
        {
            ctx = SynchronizationContext.Current;
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
                for (int i = 0; i < this.Count; i++)
                    if (predicate(this[i]))
                    {
                        this.RemoveAt(i);
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