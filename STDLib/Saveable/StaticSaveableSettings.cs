using STDLib.Serializers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace STDLib.Saveable
{


    public class StaticSaveableSettingsInstance
    {
        private static Dictionary<string, object> fields = new Dictionary<string, object>();
        Serializer serializer;

        public StaticSaveableSettingsInstance()
        {
            this.serializer = new JSON();
        }

        public StaticSaveableSettingsInstance(Serializer serializer)
        {
            this.serializer = serializer;
        }

        public void Save(string file)
        {
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
                Save(stream);
        }

        public void Load(string file)
        {
            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                Load(stream);
            
        }

        public void Save(Stream stream)
        {
            serializer.Serialize(fields, stream);
        }

        public void Load(Stream stream)
        {
            fields = serializer.Deserialize<Dictionary<string, object>>(stream);
        }

        public void SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                fields[propertyName] = value;
            }

        }

        public T GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                if (!fields.ContainsKey(propertyName))
                    fields[propertyName] = defVal;
                return (T)fields[propertyName];
            }
        }
    }

    public class StaticSaveableSettings
    {
        private static readonly StaticSaveableSettingsInstance instance = new StaticSaveableSettingsInstance();

        protected StaticSaveableSettings()
        {
            throw new System.Exception("Forbidden!");
        }

        protected static void SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            instance.SetPar<T>(value, propertyName);
        }

        protected static T GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            return instance.GetPar<T>(defVal, propertyName);
        }

        public static void Load(string file)
        {
            if (File.Exists(file))
                instance.Load(file);
        }

        public static void Save(string file)
        {
            instance.Save(file);
        }
    }
}

