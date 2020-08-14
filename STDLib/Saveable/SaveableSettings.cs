using STDLib.Serializers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace STDLib.Saveable
{
    [Obsolete("This is now obsolete, use BaseSettings instead.")]
    public class SaveableSettings : ISaveable
    {
        Serializer serializer;
        

        public SaveableSettings()
        {
            this.serializer = new JSON();
        }

        public SaveableSettings(Serializer serializer)
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
            serializer.Serialize<SaveableSettings>(this, stream);
        }

        public void Load(Stream stream)
        {
            SaveableSettings deserializedObject = serializer.Deserialize<SaveableSettings>(stream);

            foreach (PropertyInfo pi in deserializedObject.GetType().GetProperties())
                pi.SetValue(this, pi.GetValue(deserializedObject));

        }
    }
}
