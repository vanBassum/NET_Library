using STDLib.Serializers;
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
            serializer.Serialize<Saveable>(this, stream);
        }

        public void Load(Stream stream)
        {
            serializer.PopulateObject(stream, this);
        }
    }
}
