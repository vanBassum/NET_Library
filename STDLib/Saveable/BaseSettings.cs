using STDLib.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace STDLib.Saveable
{
    public class BaseSettings
    {
        private static BaseSettings instance;
        private static BaseSettings Instance { get { lock (instance) { return instance; }; } }

        public static readonly string defaultSettingsFile = $"/data/{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}/settings.json";

        private Dictionary<string, object> fields = new Dictionary<string, object>();
        Serializer serializer;

        protected BaseSettings()
        {
            this.serializer = new JSON();
            instance = (BaseSettings)Activator.CreateInstance(this.GetType());
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
            else
                GenerateSettings(file);
        }

        public static void Save(string file)
        {
            BaseSettings.Instance._Save(file);
        }

        public static void GenerateSettings(string file)
        {
            Instance._GenerateSettings(file);
        }

        virtual public void _GenerateSettings(string file)
        {

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

