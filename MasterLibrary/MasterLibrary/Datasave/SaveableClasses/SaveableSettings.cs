using MasterLibrary.Datasave.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.Datasave.SaveableClasses
{
    public class SaveableSettings : Saveable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        Serializer serializer;

        public SaveableSettings()
        {
            this.serializer = new JSON();
        }

        public SaveableSettings(Serializer serializer)
        {
            this.serializer = serializer;
        }

        public void Save(Stream stream)
        {
            serializer.Serialize<SaveableSettings>(this, stream);
        }

        public void Load(Stream stream)
        {
            SaveableSettings deserializedObject = serializer.Deserialize<SaveableSettings>(stream);

            foreach (PropertyInfo pi in deserializedObject.GetType().GetProperties())
                pi.SetValue(this, pi.GetValue(deserializedObject));

        }

        protected bool SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(GetPar<T>(propertyName), value))
                return false;

            PropertyInfo pi = this.GetType().GetProperty(propertyName);

            if (pi == null)
                throw new Exception("Property " + propertyName + " not found");

            pi.SetValue(this, value);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected T GetPar<T>([CallerMemberName] string propertyName = null)
        {
            PropertyInfo pi = this.GetType().GetProperty(propertyName);

            if(pi == null)
                return default(T);

            return (T)pi.GetValue(this);
        }
    }
}
