using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace STDLib.Misc
{
    public class Logger
    {
        StreamWriter writer = null;
        private static Logger instance = new Logger();

        private static Logger Instance { get { lock (instance) { return instance; }; } }


        Logger()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
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

        public static void OpenFile(string file)
        {
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
            Logger.Instance.CloseFileInt();
        }

        public static void WriteLine(string message, [CallerMemberName] string memberName = "")
        {
            string timestamp = (DateTime.Now.Ticks / 10000).ToString();

            Logger.Instance.writer.WriteLine($"{timestamp}, {memberName}, {Regex.Escape(message)}");

        }
    }
}