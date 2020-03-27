using MasterLibrary.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace STDLib.Saveable
{
    public class SaveableBindingList<T> : BindingList<T>, Saveable
    {
        Serializer serializer;

        public SaveableBindingList()
        {
            this.serializer = new JSON();
        }

        public SaveableBindingList(Serializer serializer)
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