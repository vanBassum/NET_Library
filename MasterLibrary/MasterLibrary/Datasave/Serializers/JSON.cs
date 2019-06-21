using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Datasave.Serializers
{
    public class JSON : Serializer
    {
        public override T Deserialize<T>(Stream data)
        {
            using (StreamReader sr = new StreamReader(data))
            {
                int length = int.Parse(sr.ReadLine());
                char[] buffer = new char[length];
                sr.ReadBlock(buffer, 0, length);

                string serial = new string(buffer);

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

                sr.WriteLine(serial.Length.ToString());
                sr.WriteLine(serial);
            }
        }
    }
}
