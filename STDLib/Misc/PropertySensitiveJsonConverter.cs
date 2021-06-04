using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;



namespace STDLib.Misc
{
    public class PropertySensitiveJsonConverter : JsonConverter
    {
        //private readonly Type[] _types;

        public PropertySensitiveJsonConverter()
        {
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            if (value is PropertySensitive ps)
            {
                Dictionary<string, object> fields = new Dictionary<string, object>();

                foreach (var pi in ps.GetType().GetProperties())
                {
                    bool ignore = pi.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any();
                    if (!ignore)
                    {
                        fields.Add(pi.Name, pi.GetValue(ps));
                    }
                }
               
                foreach (var kvp in ps.GetFields())
                {
                    if (!fields.ContainsKey(kvp.Key))
                        fields.Add(kvp.Key, kvp.Value);
                }
                
                
                JToken t = JToken.FromObject(fields);
                t.WriteTo(writer);
            }
            else
                throw new Exception();

        }

        public override bool CanRead        { get => true; }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object result = existingValue ?? Activator.CreateInstance(objectType);

            if (result is PropertySensitive ps)
            {
                if (reader.TokenType == JsonToken.StartObject)
                {
                    JObject item = JObject.Load(reader);

                    foreach(JProperty prop in item.Properties())
                    {

                        var pi = ps.GetType().GetProperty(prop.Name);
                        if (pi != null)
                        {
                            bool ignore = pi.GetCustomAttributes(typeof(JsonIgnoreAttribute), true).Any();
                            if (!ignore)
                            {
                                var obj = prop.Value.ToObject(pi.PropertyType);
                                pi.SetValue(ps, obj);
                            }
                        }
                        else
                        {
                            var obj = prop.Value.ToObject(typeof(object));

                            //var obj = prop.ToObject(prop.Value.);
                            ps.SetPar(obj, prop.Name);
                        }
                    }
                }
                else
                {
                    JArray array = JArray.Load(reader);

                }
            }
            
            return result;
        }

        void SetPar(PropertySensitive ps, string name, object value)
        {
            
        }

        public override bool CanConvert(Type objectType)
        {
            bool result = false;
            try
            {
                object obj = Activator.CreateInstance(objectType);
                if (obj is PropertySensitive)
                    result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }
    }
}