using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Serializers
{
    public class JSON : Serializer
    {
        bool writeInfo = false;


        public JSON(bool writeInfo = false)
        {
            this.writeInfo = writeInfo;
        }


        public T Deserialize<T>(byte[] data)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(data));
        }

        public byte[] Serialize<T>(T obj)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
        }



        public override T Deserialize<T>(Stream data)
        {
            using (StreamReader sr = new StreamReader(data))
            {
                string serial;
                if (writeInfo)
                {
                    string info = sr.ReadLine();

                    SerialInfo si = JsonConvert.DeserializeObject<SerialInfo>(info, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                    });
                    char[] buffer = new char[si.Size];
                    sr.ReadBlock(buffer, 0, buffer.Length);
                    serial = new string(buffer);
                }
                else
                    serial = sr.ReadToEnd();
               

                return JsonConvert.DeserializeObject<T>(serial, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
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
                    TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                });


                if (writeInfo)
                {

                    SerialInfo si = new SerialInfo();
                    si.Size = serial.Length;
                    SerializerVersionAttribute attr = obj.GetType().GetCustomAttribute(typeof(SerializerVersionAttribute)) as SerializerVersionAttribute;
                    if (attr != null)
                        si.Version = attr.Version;

                    string serialInfo = JsonConvert.SerializeObject(si, Formatting.None, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects,
                        TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
                    });

                    sr.WriteLine(serialInfo);
                }
                sr.WriteLine(serial);
            }
        }

        //Used to store information about the serialization
        private class SerialInfo
        {
            public string Version { get; set; } = "";
            public int Size { get; set; }

        }

       
    }

}
