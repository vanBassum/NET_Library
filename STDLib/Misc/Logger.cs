using STDLib.Saveable;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace STDLib.Misc
{
    /*
    /// <summary>
    /// A class to write to a log file.
    /// Use as a static class.
    /// First set the file with <see cref="SetFile"/>
    /// Then <see cref="WriteLine"/> can be used to write to the logfile.
    /// </summary>
    public class Logger
    {
        static LogList<LogEntry> LogList = new LogList<LogEntry>();
        public static string LogFile { get => LogList.LogFile; set => LogList.LogFile = value; }
        public static object ConsoleLock = new object();
        public static LogLevel ConsoleLog { get; set; } = (LogLevel)0xFF;
        public static LogLevel FileLog { get; set; } = LogLevel.Error | LogLevel.Fatal | LogLevel.Warning;

        public static void LOGI(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            Log(DateTime.Now, message, LogLevel.Info, file, member);
        }

        public static void LOGW(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            Log(DateTime.Now, message, LogLevel.Warning, file, member);
        }

        public static void LOGE(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            Log(DateTime.Now, message, LogLevel.Error, file, member);
        }

        public static void LOGF(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            Log(DateTime.Now, message, LogLevel.Fatal, file, member);
        }

        public static void Log(DateTime timestamp, string message, LogLevel level, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            if (timestamp == null)
                timestamp = DateTime.Now;
            LogEntry logEntry = new LogEntry(timestamp, message, level, file, member);
            Log(logEntry);
        }

        private static void Log(LogEntry logEntry)
        {
            string consoleMessage = $"{Path.GetFileNameWithoutExtension(logEntry.File)}.{logEntry.Member}:{logEntry.Message}";


            lock (ConsoleLock)
            {
                switch(logEntry.Level)
                {
                    case LogLevel.Debug:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    case LogLevel.Info:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case LogLevel.Warning:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LogLevel.Error:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LogLevel.Fatal:
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                }
                Console.WriteLine(consoleMessage);
                Console.ForegroundColor = ConsoleColor.White;
            }
            LogList.Add(logEntry);
        }
    }



    public class LogEntry
    {
        public DateTime Timestamp { get; set; }
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public string File { get; set; }
        public string Member { get; set; }

        public LogEntry()
        {

        }

        public LogEntry(DateTime timestamp, string message, LogLevel level, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            Timestamp = timestamp;
            Message = message;
            Level = level;
            File = file;
            Member = member;
        }


    }

    [Flags]
    public enum LogLevel
    {
        //Off     = 0,
        //All     = 0xFFFF,
        Debug = (1 << 0),
        Info = (1 << 1),
        Warning = (1 << 2),
        Error = (1 << 3),
        Fatal   = (1 << 4),
    }

    */

}