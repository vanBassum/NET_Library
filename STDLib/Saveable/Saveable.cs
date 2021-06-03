using STDLib.Serializers;
using System;
using System.IO;
using System.Reflection;

namespace STDLib.Saveable
{
    public class Saveable : ISaveable
    {
        Serializer serializer;


        public Saveable()
        {
            this.serializer = new JSON();
        }

        public Saveable(Serializer serializer)
        {
            this.serializer = serializer;
        }


        public void Save(string file)
        {
            Save(new FileInfo(file));
        }

        public void Load(string file)
        {
            Load(new FileInfo(file));
        }


        public void Save(FileInfo file)
        {
            Directory.CreateDirectory(file.DirectoryName);
            using (Stream stream = file.Open(FileMode.Create, FileAccess.ReadWrite))
                Save(stream);
        }

        public void Load(FileInfo file)
        {
            if (!file.Exists)
            {
                using (Stream stream = file.Open(FileMode.Create, FileAccess.ReadWrite))
                    Save(stream);
            }
            else
            {
                using (Stream stream = file.Open(FileMode.Open, FileAccess.Read))
                    Load(stream);

            }
        }

        public void Save(Stream stream)
        {
            serializer.Serialize<Saveable>(this, stream);
        }

        public void Load(Stream stream)
        {
            Saveable deserializedObject = serializer.Deserialize<Saveable>(stream);

            foreach (PropertyInfo pi in deserializedObject.GetType().GetProperties())
                pi.SetValue(this, pi.GetValue(deserializedObject));

        }
    }
}
