using MasterLibrary.Bindable;
using MasterLibrary.Extentions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MasterLibrary.Threading
{
    public delegate void WorkDone<P>(P workArg, object result);

    public class WorkerPool<T, P> where T : AWorker<P>
    {
        public event WorkDone<P> OnWorkerDone;
        public ThreadedBindingList<T> _Workers;
        private SynchronizationContext ctx;
        public ThreadedBindingQueue<P> _Work;

        public int WorkCount { get {return _Work.Count; } }

        public WorkerPool(int numberOfWorkers)
        {
            ctx = SynchronizationContext.Current;
            _Work = new ThreadedBindingQueue<P>();
            _Workers = new ThreadedBindingList<T>();

            _Workers.ListChanged += _Workers_ListChanged;

            for (int i = 0; i < numberOfWorkers; i++)
            {
                T worker = Activator.CreateInstance<T>();
                worker.OnWorkerDone += Worker_OnWorkerDone;
                _Workers.Add(worker);
            }
        }

        private void _Workers_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            switch(e.ListChangedType)
            {
                case System.ComponentModel.ListChangedType.ItemAdded:
                    Start();
                    break;
            }
        }

        private void Worker_OnWorkerDone(P sender, object result)
        {
            OnWorkerDone?.Invoke(sender, result);
            Start();
        }

        public void AddWork(P par)
        {
            _Work.Enqueue(par);
            Start();
        }


        void Start()
        {
            P par;

            if (_Work.TryDequeue(out par))
            {
                if (!TryStart(par))
                {
                    _Work.Enqueue(par);
                    return;
                }
            }
        }



        bool TryStart(P par)
        {
            int ind = -1;

            ctx?.Send(delegate
            {
                ind = _Workers.FindIndex(w => !w.IsBusy);
            }, null);

            if (ind != -1)
            {
                _Workers[ind].StartWork(par);
                return true;
            }
            return false;
        }

    }
}
