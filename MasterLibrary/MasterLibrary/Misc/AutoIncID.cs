using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Misc
{
    public class AutoIncID
    {
        private static object nextIDLock = new object();
        private static int nextID = 0;
        public int ID { get; private set; }

        public AutoIncID()
        {
            lock(nextIDLock)
            {
                ID = nextID++;
            }
        }

    }
}
