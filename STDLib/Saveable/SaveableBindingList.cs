using STDLib.Serializers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace STDLib.Saveable
{
    public class SaveableBindingList<T> : BindingList<T>, ISaveable
    {
        Serializer serializer;
        string file = null;

        private bool saveOnDestruction = false;
        public bool SaveOnDestruction { get { return saveOnDestruction; } set { saveOnDestruction = value; if (file == null) throw new Exception("Can't save on destruction if file is unknown"); } }

        public SaveableBindingList()
        {
            this.serializer = new JSON();
        }

        public SaveableBindingList(Serializer serializer)
        {
            this.serializer = serializer;
        }

        public SaveableBindingList(string file)
        {
            this.serializer = new JSON();
            this.file = file;
            Load();
        }

        ~SaveableBindingList()
        {
            if (SaveOnDestruction && file != null)
                Save();
        }

        public void SortBy<Tkey>(Func<T, Tkey> predicate)
        {
            var sortedListInstance = new BindingList<T>(this.OrderBy(predicate).ToList());
            this.Clear();
            foreach (var v in sortedListInstance)
                this.Add(v);
        }

        public void Save()
        {
            if (file == null)
                throw new Exception("Wrong initializer used, use SaveableBindingList(string file)");
            Save(file);
        }

        public void Load()
        {
            if (file == null)
                throw new Exception("Wrong initializer used, use SaveableBindingList(string file)");
            if (!File.Exists(file))
                return;
            Load(file);
        }

        public void Save(string file)
        {
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
                Save(stream);
        }

        public void Load(string file)
        {
            if (!File.Exists(file))
                return;
            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                Load(stream);
        }

        public void Save(Stream stream)
        {
            serializer.Serialize<BindingList<T>>(this, stream);
        }


        public void Load(Stream stream)
        {
            BindingList<T> deserializedObject = serializer.Deserialize<BindingList<T>>(stream);

            this.Clear();
            foreach (T i in deserializedObject)
                this.Add(i);
        }

        public class TypeNameSerializationBinder : SerializationBinder
        {
            public string TypeFormat { get; private set; }

            public TypeNameSerializationBinder(string typeFormat)
            {
                TypeFormat = typeFormat;
            }

            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                assemblyName = null;
                typeName = serializedType.Name;
            }

            public override Type BindToType(string assemblyName, string typeName)
            {
                var resolvedTypeName = string.Format(TypeFormat, typeName);
                return Type.GetType(resolvedTypeName, true);
            }
        }
    }
}