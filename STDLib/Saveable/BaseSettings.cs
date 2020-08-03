using STDLib.Serializers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace STDLib.Saveable
{
    public class BaseSettings
    {
        private static BaseSettings instance = new BaseSettings();
        private static BaseSettings Instance { get { lock (instance) { return instance; }; } }

        private Dictionary<string, object> fields = new Dictionary<string, object>();
        Serializer serializer;

        protected BaseSettings()
        {
            this.serializer = new JSON();
        }

      
        protected static void SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            BaseSettings.Instance._SetPar<T>(value, propertyName);
        }

        protected static T GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            return BaseSettings.Instance._GetPar<T>(defVal, propertyName);
        }

        public static void Load(string file)
        {
            if (File.Exists(file))
                BaseSettings.Instance._Load(file);
        }

        public static void Save(string file)
        {
            BaseSettings.Instance._Save(file);
        }


        void _Save(string file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
                _Save(stream);
        }

        void _Load(string file)
        {
            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                _Load(stream);

        }

        void _Save(Stream stream)
        {
            serializer.Serialize(fields, stream);
        }

        void _Load(Stream stream)
        {
            fields = serializer.Deserialize<Dictionary<string, object>>(stream);
        }

        void _SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                fields[propertyName] = value;
            }

        }

        T _GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            lock (fields)
            {
                if (!fields.ContainsKey(propertyName))
                    fields[propertyName] = defVal;
                return (T)fields[propertyName];
            }
        }
    }
}

