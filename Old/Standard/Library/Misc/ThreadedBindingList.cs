using System;
using System.ComponentModel;
using System.Threading;


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
        lock(this)
        {
            for (int i = 0; i < this.Count; i++)
                if (predicate(this[i]))
                {
                    this.RemoveAt(i);
                    i--;
                }
        }
    }



    /*
    public T FirstOrDefault(Predicate<T> predicate)
    {
        T result = default;

        if (ctx == null)
        {
            foreach (T itm in this)
                if (predicate(itm))
                    result = itm;
        }
        else
        {
            ctx.Send(delegate
            {
                result = FirstOrDefault(predicate);
            }, null);
        }
        return result;
    }

    public int Find(Predicate<T> predicate)
    {
        int result = -1;

        if (ctx == null)
        {
            for (int i = 0; i < this.Count; i++)
                if (predicate(this[i]))
                    return i;
        }
        else
        {
            ctx.Send(delegate
            {
                result = Find(predicate);
            }, null);
        }
        return result;

    }

    public void RemoveWhere(Predicate<T> predicate)
    {
        if (ctx == null)
        {
            for (int i = 0; i < this.Count; i++)
                if (predicate(this[i]))
                {
                    this.RemoveAt(i);
                    i--;
                }
        }
        else
        {
            ctx.Send(delegate
            {
                RemoveWhere(predicate);
            }, null);
        }
    }
    */
}

