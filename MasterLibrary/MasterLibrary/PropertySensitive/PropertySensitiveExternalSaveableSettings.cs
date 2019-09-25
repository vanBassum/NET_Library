using MasterLibrary.Saveable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MasterLibrary.PropertySensitive
{
    public class PropertySensitiveExternalSaveableSettings : SaveableSettings, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, object> fields = new Dictionary<string, object>();

        protected virtual void Verify(string propertyName)
        {
            //Override this if parameters are depending on eachother
            //use SetParSilent when adjusting...
        }
        protected bool SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(GetPar<T>(propertyName), value))
                return false;
            fields[propertyName] = value;
            Verify(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected T GetPar<T>(T defVal, [CallerMemberName] string propertyName = null)
        {
            object value = defVal;
            if (fields.TryGetValue(propertyName, out value))
            {
                if (value is T)
                {
                    return (T)value;
                }
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return defVal;
                }
            }

            return defVal;
        }

        protected T GetPar<T>([CallerMemberName] string propertyName = null)
        {
            return GetPar(default(T), propertyName);
        }       
    }
}
