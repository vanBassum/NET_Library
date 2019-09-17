using MasterLibrary.Bindable;

namespace MasterLibrary.Bindable
{
    public class ThreadedBindingQueue<T> : ThreadedBindingList<T>
    {


        public void Enqueue(T item)
        {
            this.Add(item);
        }

        public T Dequeue()
        {
            T itm = this[0];
            this.RemoveAt(0);
            return itm;
        }

        public bool TryDequeue(out T itm)
        {
            itm = default(T);
            if (this.Count == 0)
                return false;

            itm = this[0];
            this.RemoveAt(0);
            return true;
        }


    }
}
