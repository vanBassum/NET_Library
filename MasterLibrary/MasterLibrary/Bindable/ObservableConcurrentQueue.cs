using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Bindable
{
    public class ObservableConcurrentQueue<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        private readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public void Enqueue(T item)
        {
            queue.Enqueue(item);
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Add, item));
        }

        public bool TryDequeue(out T itm)
        {
            bool okidoki = queue.TryDequeue(out itm);
            if (CollectionChanged != null)
                CollectionChanged(this,
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove, itm));
            return okidoki;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
