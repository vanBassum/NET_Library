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
        
    }
}
