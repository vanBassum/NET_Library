using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace STDLib.Serializers
{
    public class JSON : Serializer
    {
        public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
        {
            //TypeNameHandling = TypeNameHandling.Objects,
            //TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            //ObjectCreationHandling = ObjectCreationHandling.Replace,         //https://stackoverflow.com/questions/13394401/json-net-deserializing-list-gives-duplicate-items
        };

        public override T Deserialize<T>(Stream data)
        {
            using (StreamReader sr = new StreamReader(data))
            {
                string serial = sr.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(serial, Settings);
            }
        }

        public override void Serialize<T>(T obj, Stream stream)
        {
            using (StreamWriter sr = new StreamWriter(stream))
            {
                string serial = JsonConvert.SerializeObject(obj, Formatting.Indented, Settings);
                sr.WriteLine(serial);
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            string serial = Encoding.ASCII.GetString(data);
            return JsonConvert.DeserializeObject<T>(serial, Settings);
        }

        public byte[] Serialize<T>(T obj)
        {
            string serial = JsonConvert.SerializeObject(obj, Formatting.Indented, Settings);
            return Encoding.ASCII.GetBytes(serial);
        }

        public object Deserialize(byte[] data)
        {
            string serial = Encoding.ASCII.GetString(data);
            return JsonConvert.DeserializeObject(serial, Settings);
        }
    }


}
