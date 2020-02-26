using System;

namespace Log_temp_verloop_warmwelkom
{
    public abstract class LogItemBase
    {
        public abstract DateTime Timestamp { get; set; }


        public abstract bool TryPopulate(string data);

    }
}
