using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datasave.Serializers
{
    public class JSON : Serializer
    {
        public override T Deserialize<T>(Stream data)
        {
            using (StreamReader sr = new StreamReader(data))
                return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
        }
        public override void Serialize<T>(T obj, Stream stream)
        {
            using (StreamWriter sr = new StreamWriter(stream))
                sr.WriteLine(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}
