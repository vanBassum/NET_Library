using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;

namespace STDLib.JBVProtocol
{
    public static class Logger
    {
        public static object consoleLock = new object();
        public static int length = 30;

        public static void LOGI(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            file = Path.GetFileNameWithoutExtension(file);
            Log(ConsoleColor.Green, message, $"{file}.{member}");
        }

        public static void LOGW(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            file = Path.GetFileNameWithoutExtension(file);
            Log(ConsoleColor.Yellow, message, $"{file}.{member}");
        }

        public static void LOGE(string message, [CallerFilePath] string file = "", [CallerMemberName] string member = "")
        {
            file = Path.GetFileNameWithoutExtension(file);
            Log(ConsoleColor.Red, message, $"{file}.{member}") ;
        }

        

        static void Log(ConsoleColor color, string message, string prefix)
        {

            int len = prefix.Length;
            if (len > length)
                length = len;

            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine($"{prefix}: {new string(' ', length - len)}{message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }


    }

}