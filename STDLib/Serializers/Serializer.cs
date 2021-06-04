using System;
using System.IO;
using System.Text;

namespace STDLib.Serializers
{
    public abstract class Serializer
    {
        public Encoding Encoding { get; set; } = Encoding.UTF8;

        // String
        public abstract string Serialize<T>(T obj);
        public abstract T Deserialize<T>(string serial);
        public abstract void PopulateObject<T>(string serial, T obj);

        // byte[]
        public virtual byte[] SerializeToByteArray<T>(T obj)
        {
            string serial = Serialize(obj);
            return Encoding.GetBytes(serial);
        }

        public virtual T Deserialize<T>(byte[] data)
        {
            string serial = Encoding.GetString(data);
            return Deserialize<T>(serial);
        }

        public virtual void PopulateObject<T>(byte[] data, T obj)
        {
            string serial = Encoding.GetString(data);
            PopulateObject<T>(serial, obj);
        }

        //stream
        public virtual void Serialize<T>(T obj, Stream stream)
        {
            StreamWriter sr = new StreamWriter(stream);
            string serial = Serialize(obj);
            sr.WriteLine(serial);
            sr.Flush();
        }

        public virtual T Deserialize<T>(Stream data)
        {
            StreamReader sr = new StreamReader(data);
            string serial = sr.ReadToEnd();
            return Deserialize<T>(serial);
        }

        public virtual void PopulateObject<T>(Stream data, T obj)
        {
            StreamReader sr = new StreamReader(data);
            string serial = sr.ReadToEnd();
            PopulateObject<T>(serial, obj);
        }

        //file
        public virtual void Serialize<T>(T obj, FileInfo file)
        {
            using (Stream stream = file.Open(FileMode.Create, FileAccess.Write))
            {
                Serialize(obj, stream);
            }
        }

        public virtual T Deserialize<T>(FileInfo file)
        {
            T result = default(T);
            using (Stream stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                result = Deserialize<T>(stream);
            }
            return result;
        }

        public virtual void PopulateObject<T>(FileInfo file, T obj)
        {
            using (Stream stream = file.Open(FileMode.Open, FileAccess.Read))
            {
                PopulateObject<T>(stream, obj);
            }
        }

    }
}
