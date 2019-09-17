using System.Threading;

namespace MasterLibrary.Threading
{
    public abstract class AWorker<P>
    {
        public abstract event WorkDone<P> OnWorkerDone;
        public bool IsBusy { get; private set; } = false;
        protected abstract void StartWorker(P parameter);
        public P Work { get; private set; }
        private SynchronizationContext ctx;

        public AWorker()
        {
            ctx = SynchronizationContext.Current;
            OnWorkerDone += AWorker_OnWorkerDone;
        }

        private void AWorker_OnWorkerDone(P sender, object result)
        {
            ctx.Send(delegate
            {
                IsBusy = false;
            }, null);
        }

        public void StartWork(P parameter)
        {
            Work = parameter;
            ctx.Send(delegate
            {
                IsBusy = true;
            }, null);
            StartWorker(parameter);
        }
    }
}
