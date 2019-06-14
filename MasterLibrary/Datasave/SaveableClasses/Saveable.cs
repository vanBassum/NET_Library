using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datasave
{
    public interface Saveable
    {
        void Save(Stream stream);

        void Load(Stream stream);

    }
}
