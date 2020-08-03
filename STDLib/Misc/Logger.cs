using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace STDLib.Misc
{
    public class Logger
    {
        StreamWriter writer = null;
        private static Logger instance = new Logger();

        private static Logger Instance { get { lock (instance) { return instance; }; } }

        private static string file;
        private static bool autoOpen;
        private System.Timers.Timer closeFileTimer = new System.Timers.Timer(1000);

       


        Logger()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            closeFileTimer.Elapsed += CloseFileTimer_Elapsed;
        }

        private void CloseFileTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Instance.closeFileTimer.Stop();
            Logger.CloseFile();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CloseFileInt();
        }

        private void CloseFileInt()
        {
            Logger.Instance.writer.Flush();
            Logger.Instance.writer.Close();
            Logger.Instance.writer.Dispose();
        }



        public static void SetFile(string file, bool autoOpen = true)
        {
            Logger.file = file;
            Logger.autoOpen = autoOpen;
        }

        public static void OpenFile()
        {
            Console.WriteLine("OpenLOG");
            string path = Path.GetDirectoryName(file);
            if (path != "")
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            Logger.Instance.writer = new StreamWriter(File.Open(file, FileMode.Append, FileAccess.Write));

        }

        public static void CloseFile()
        {
            Console.WriteLine("CloseLOG");
            Instance.CloseFileInt();
        }

        public static void WriteLine(string message, [CallerMemberName] string memberName = "")
        {
            if (autoOpen)
            {
                Instance.closeFileTimer.Stop();
                OpenFile();
            }
            string timestamp = (DateTime.Now.Ticks / 10000).ToString();

            Logger.Instance.writer.WriteLine($"{timestamp}, {memberName}, {Regex.Escape(message)}");
            Console.WriteLine($"Log {memberName}: '{message}'");
            if (autoOpen)
                Instance.closeFileTimer.Start();
        }
    }
}