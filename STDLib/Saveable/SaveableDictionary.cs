using STDLib.Serializers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace STDLib.Saveable
{
    public class SaveableDictionary<T1, T2> : Dictionary<T1, T2>
    {
        public static string DefaultDataFolder { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Path.Combine("/data", "vanBassum", System.Reflection.Assembly.GetEntryAssembly().GetName().Name) : "data";
        private readonly Serializer serializer = new JSON();
        FileInfo File { get; set; } = new FileInfo(Path.Combine(DefaultDataFolder, $"{typeof(T1).Name}.json"));


        public SaveableDictionary()
        {
            this.serializer = new JSON();
        }

        public SaveableDictionary(Serializer serializer)
        {
            this.serializer = serializer;
        }

        public SaveableDictionary(string file)
        {
            this.serializer = new JSON();
            this.File = new FileInfo(file);
            Load();
        }

        public void Save()
        {
            Directory.CreateDirectory(File.DirectoryName);
            using (Stream stream = File.Open(FileMode.Create, FileAccess.ReadWrite))
                serializer.Serialize(this, stream);
        }

        public void Load()
        {
            if (!File.Exists)
            {
                Save();
            }
            else
            {
                SaveableDictionary<T1, T2> items = serializer.Deserialize<SaveableDictionary<T1, T2>>(File);
                Clear();
                foreach (var itm in items)
                    this.Add(itm.Key, itm.Value);
            }
        }
    }
}