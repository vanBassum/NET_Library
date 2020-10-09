using System.Threading;
using System;

namespace STDLib.Misc
{
    public class TaskEvents
    {
        object lck = new object();
        UInt32 bits = 0;
        SemaphoreSlim sem = new SemaphoreSlim(0, 1);
    

        public void SetBits(UInt32 val)
        {
            lock (lck)
            {
                bits |= val;
                if(sem.CurrentCount == 0)
                    sem.Release();
            }
        }


        public UInt32 WaitOne()
        {
            
            sem.Wait();
            lock (lck)
            {
                UInt32 val = bits;
                bits = 0;
                return val;
            }
        }
    }
}
