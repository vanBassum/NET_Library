using System.IO;

namespace STDLib.Saveable
{
    public interface ISaveable
    {
        void Save(Stream stream);
        void Load(Stream stream);

    }
}
