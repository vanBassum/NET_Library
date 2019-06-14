using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datasave.Serializers
{
    public abstract class Serializer
    {
        public abstract void Serialize<T>(T obj, Stream stream);
        public abstract T Deserialize<T>(Stream stream);

        public void Serialize<T>(T obj, string filename)
        {
            using (Stream s = File.Open(filename, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                Serialize<T>(obj, s);
            }
        }

        public T Deserialize<T>(string filename)
        {
            using (Stream s = File.Open(filename, FileMode.Open, FileAccess.ReadWrite))
            {
                return Deserialize<T>(s);
            }
        }

    }
}
