using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Datasave.Serializers
{
    public class JSONIgnore : Serializer
    {
        public override T Deserialize<T>(Stream data)
        {
            using (StreamReader sr = new StreamReader(data))
            { 
                string serial = sr.ReadToEnd();

                return JsonConvert.DeserializeObject<T>(serial, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

            }
                
        }
        public override void Serialize<T>(T obj, Stream stream)
        {
            using (StreamWriter sr = new StreamWriter(stream))
            {
                string serial = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

                sr.WriteLine(serial);
            }
        }

        public T Deserialize<T>(byte[] data)
        {

            string serial = Encoding.ASCII.GetString(data);

            return JsonConvert.DeserializeObject<T>(serial, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });

            

        }
        public byte[] Serialize<T>(T obj)
        {

            string serial = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.Indented
            });

            return Encoding.ASCII.GetBytes(serial);
        }


        public object Deserialize(byte[] data)
        {

            string serial = Encoding.ASCII.GetString(data);

            return JsonConvert.DeserializeObject(serial, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });



        }
    }


}
