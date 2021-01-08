using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using STDLib.Misc;
using STDLib.Serializers;

namespace STDLib.Saveable
{
    public class LogList<T>
    {
        public static readonly string defaultDataFolder = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Path.Combine("/data", "vanBassum", System.Reflection.Assembly.GetEntryAssembly().GetName().Name) : "data";
        public string LogFile { get; set; } = Path.Combine(defaultDataFolder, $"LogList.json");

        private readonly JSON json = new JSON();
        private Stream fileStream = null;
        private System.Timers.Timer closeFileTimer = new System.Timers.Timer(1000);
        bool fileIsOpen = false;
        object streamLock = new object();


        public LogList()
        {
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            closeFileTimer.Elapsed += CloseFileTimer_Elapsed;
            json.Settings.Formatting = Newtonsoft.Json.Formatting.None;
        }

        public LogList(string filename)
        {
            LogFile = Path.Combine(defaultDataFolder, filename);
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            closeFileTimer.Elapsed += CloseFileTimer_Elapsed;
            json.Settings.Formatting = Newtonsoft.Json.Formatting.None;
        }

        private void CloseFileTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            closeFileTimer.Stop();
            CloseFile();
        }

        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            CloseFile();
        }

        private void OpenFile()
        {
            string path = Path.GetDirectoryName(LogFile);
            if (path != "")
            {
                Directory.CreateDirectory(path);
            }
            lock (streamLock)
            {
                if(!fileIsOpen)
                    fileStream = File.Open(LogFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                fileIsOpen = true;
            }
        }

        private void CloseFile()
        {
            lock (streamLock)
            {
                fileStream?.Close();
                fileStream?.Dispose();
                fileIsOpen = false;
            }
        }
        

        public virtual void Add(T item)
        {
            lock (streamLock)
            {
                closeFileTimer.Stop();
                if (!fileIsOpen)
                    OpenFile();
                fileStream.Seek(0, SeekOrigin.End);

                byte[] data = json.SerializeToByteArray(item);
                fileStream.Write(data, 0, data.Length);
                fileStream.Write(new byte[] { (byte)'\n' }, 0, 1);
                fileStream.Flush();
                closeFileTimer.Start();
            }
        }



        public List<T> ReadAll()
        {
            List<T> list = new List<T>();
            closeFileTimer.Stop();
            if (!fileIsOpen)
                OpenFile();
            lock (streamLock)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
                StreamReader reader = new StreamReader(fileStream);
                while (!reader.EndOfStream)
                {
                    string line = Regex.Unescape( reader.ReadLine().Trim('\"'));
                    list.Add(json.Deserialize<T>(line));
                }

            }
            closeFileTimer.Start();
            return list;
        }

    }
}


