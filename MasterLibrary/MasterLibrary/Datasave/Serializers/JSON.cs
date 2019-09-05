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
    public class JSON : Serializer
    {
        bool ignoreLength = false;

        public override T Deserialize<T>(Stream data)
        {
            using (StreamReader sr = new StreamReader(data))
            {

                string info = sr.ReadLine();

                SerialInfo si = JsonConvert.DeserializeObject<SerialInfo>(info, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

                ignoreLength = si.IgnoreLength;

                /*
                T a = Activator.CreateInstance<T>();

                SerializerVersionAttribute attr = typeof(T).GetCustomAttribute(typeof(SerializerVersionAttribute)) as SerializerVersionAttribute;
                if (attr != null)
                {
                    if (si.Version != attr.Version)
                        throw new Exception("Deserialize version mismatch");
                }
                */

                string serial;

                if (si.IgnoreLength)
                {
                    serial = sr.ReadToEnd();
                }
                else
                {
                    char[] buffer = new char[si.Size];
                    sr.ReadBlock(buffer, 0, buffer.Length);
                    serial = new string(buffer);
                }
                

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
                SerialInfo si = new SerialInfo();
                si.Size = serial.Length;
                si.IgnoreLength = ignoreLength;

                SerializerVersionAttribute attr = obj.GetType().GetCustomAttribute(typeof(SerializerVersionAttribute)) as SerializerVersionAttribute;
                if (attr != null)
                    si.Version = attr.Version;

                string serialInfo = JsonConvert.SerializeObject(si, Formatting.None, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

                sr.WriteLine(serialInfo);
                sr.WriteLine(serial);
            }
        }

        //Used to store information about the serialization
        private class SerialInfo
        {
            public string Version { get; set; } = "";
            public int Size { get; set; }
            public bool IgnoreLength { get; set; } = false;

        }

       
    }

}
