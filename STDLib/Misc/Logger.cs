using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace STDLib.Misc
{
    /// <summary>
    /// A class to write to a log file.
    /// Use as a static class.
    /// First set the file with <see cref="SetFile"/>
    /// Then <see cref="WriteLine"/> can be used to write to the logfile.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// The datetime format to use.
        /// </summary>
        public static string DateTimeFormat { get { return "dd-MMM-yyyy HH:mm:ss.fff K"; } }
        StreamWriter writer = null;
        private static Logger instance = new Logger();
        private static Logger Instance { get { lock (instance) { return instance; }; } }

        private static string file;
        private static bool autoOpen;
        private System.Timers.Timer closeFileTimer = new System.Timers.Timer(1000);

        bool fileIsOpen = false;
        object writerLock = new object();


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
            lock (Instance.writerLock)
            {
                if (Instance.fileIsOpen)
                    Instance.writer?.Flush();
                Instance.writer?.Close();
                Instance.writer?.Dispose();
                fileIsOpen = false;

            }

        }


        /// <summary>
        /// Sets the logfile to be used.
        /// A new file is automatically created if it didn't exist.
        /// </summary>
        /// <param name="file">Path to file</param>
        /// <param name="autoOpen">Let the Logger automatically open and close the file when nessesairy. Otherwise use <see cref="OpenFile"/> and <see cref="CloseFile"/> to open and close the file manually.</param>
        public static void SetFile(string file, bool autoOpen = true)
        {
            CreateDirIfNotExists(Path.GetDirectoryName(file));
            Logger.file = file;
            Logger.autoOpen = autoOpen;
        }

        /// <summary>
        /// Opens the file in order to write to the file with <see cref="WriteLine(string, string)"/>
        /// No need to use this if the <see cref="SetFile(string, bool)"/> with autoOpen = true, was used.
        /// </summary>
        public static void OpenFile()
        {
            Console.WriteLine("OpenLOG");
            string path = Path.GetDirectoryName(file);
            if (path != "")
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            lock (Logger.Instance.writerLock)
            {
                Logger.Instance.writer = new StreamWriter(File.Open(file, FileMode.Append, FileAccess.Write));
                Logger.Instance.fileIsOpen = true;
            }
        }

        /// <summary>
        /// Closes the file after lines has been written to the file with <see cref="WriteLine(string, string)"/>
        /// No need to use this if the <see cref="SetFile(string, bool)"/> with autoOpen = true, was used
        /// </summary>
        public static void CloseFile()
        {
            Console.WriteLine("CloseLOG");
            Instance.CloseFileInt();
        }



        /// <summary>
        /// Write a string to the logfile that was set with <see cref="SetFile(string, bool)"/>
        /// </summary>
        /// <param name="message">The message string to be written.</param>
        /// <param name="memberName">The name of the calling function.</param>
        public static void WriteLine(string message, [CallerMemberName] string memberName = "")
        {
            lock (Logger.Instance.writerLock)
            {
                if (autoOpen)
                {
                    Instance.closeFileTimer.Stop();
                    if (!Logger.Instance.fileIsOpen)
                        OpenFile();
                }
                string timestamp = DateTime.Now.ToString(DateTimeFormat);

                Logger.Instance.writer.WriteLine($"{timestamp}, {memberName}, {Regex.Escape(message)}");
                Console.WriteLine($"Log {memberName}: '{message}'");
                if (autoOpen)
                    Instance.closeFileTimer.Start();

            }
        }

        /// <summary>
        /// Write a string without anything appended.
        /// The string will be appended to the file "as is"
        /// </summary>
        /// <param name="message"></param>
        public static void WriteRawLine(string message)
        {
            lock (Logger.Instance.writerLock)
            {
                if (autoOpen)
                {
                    Instance.closeFileTimer.Stop();
                    if (!Logger.Instance.fileIsOpen)
                        OpenFile();
                }
                Logger.Instance.writer.WriteLine(message);
                Console.WriteLine($"Log 'message'");
                if (autoOpen)
                    Instance.closeFileTimer.Start();

            }
        }


        private static void CreateDirIfNotExists(string dir)
        {
            if (dir != "")
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
            }
        }
    }
}