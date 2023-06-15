using System;
using System.Threading;

namespace FormsLib.Misc
{
    public class TaskEvents
    {
        object lck = new object();
        uint bits = 0;
        SemaphoreSlim sem = new SemaphoreSlim(0, 1);


        public void SetBits(uint val)
        {
            lock (lck)
            {
                bits |= val;
                if (sem.CurrentCount == 0)
                    sem.Release();
            }
        }


        public uint WaitOne()
        {

            sem.Wait();
            lock (lck)
            {
                uint val = bits;
                bits = 0;
                return val;
            }
        }
    }
}
