namespace FormsLib.Misc
{
    public class TLOGGER
    {
        public ConsoleStream Stream { get; set; }
        public LogLevel Level { get; set; } = LogLevel.INFO;



        public void LOGV(string msg)
        {
            if (Level <= LogLevel.VERBOSE)
                Stream.Write(msg + "\n");
        }

        public void LOGD(string msg)
        {
            if (Level <= LogLevel.DEBUG)
                Stream.Write(msg + "\n");
        }

        public void LOGI(string msg)
        {
            if (Level <= LogLevel.INFO)
                Stream.Write(msg + "\n");
        }

        public void LOGE(string msg)
        {
            if (Level <= LogLevel.ERROR)
                Stream.Write(msg + "\n");
        }
    }

    public enum LogLevel
    {
        NONE = 0,
        VERBOSE = 1,
        DEBUG = 2,
        INFO = 3,
        ERROR = 4,

    }
}
