using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace STDLib.Serializers
{
    public class JSON : Serializer
    {
        public JsonSerializerSettings Settings { get; set; } = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,

            //TypeNameHandling = TypeNameHandling.Objects,
            //TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple
            //ObjectCreationHandling = ObjectCreationHandling.Replace,         //https://stackoverflow.com/questions/13394401/json-net-deserializing-list-gives-duplicate-items
        };

        public override T Deserialize<T>(string serial)
        {
            return JsonConvert.DeserializeObject<T>(serial, Settings);
        }

        public override void PopulateObject<T>(string serial, T obj)
        {
            JsonConvert.PopulateObject(serial, obj);
        }

        public override string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }
    }


}
