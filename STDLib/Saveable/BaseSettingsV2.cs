using STDLib.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace STDLib.Saveable
{



    public class BaseSettingsV2<T1> where T1 : BaseSettingsV2<T1>
    {
        public static readonly string defaultDataFolder = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? Path.Combine("/data", "vanBassum", System.Reflection.Assembly.GetEntryAssembly().GetName().Name) : "data";
        private Dictionary<string, object> fields = new Dictionary<string, object>();
        private static readonly Serializer serializer = new JSON();
        public static FileInfo SettingsFile = new FileInfo( Path.Combine(defaultDataFolder, $"{typeof(T1).Name}.json"));

        public static T1 Items { get; set; }

        public static void Save()
        {
            Directory.CreateDirectory(SettingsFile.DirectoryName);
            using (Stream stream = SettingsFile.Open(FileMode.Create, FileAccess.ReadWrite))
                serializer.Serialize(Items, stream);
        }

        public static void Load()
        {
            if (!SettingsFile.Exists)
            {
                Items = Activator.CreateInstance<T1>();
                Save();
            }
            else
            {
                Items = serializer.Deserialize<T1>(SettingsFile);
            }
        }

        protected void SetPar<T2>(T2 value, [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                fields[propertyName] = value;
            }
        }

        protected T2 GetPar<T2>(T2 defVal = default(T2), [CallerMemberName] string propertyName = null)
        {

            T2 val = defVal;
#if !DEBUG
            try
            {
#endif
            lock (fields)
            {
                if (!fields.ContainsKey(propertyName))
                    fields[propertyName] = defVal;
                val = (T2)fields[propertyName];
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
#endif
            return val;
        }
    }
}

