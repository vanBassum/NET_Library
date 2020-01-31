﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MasterLibrary.PropertySensitive
{
    public class PropertySensitiveExternal : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Dict fields = new Dict();

        protected virtual void Verify(string propertyName)
        {
            //Override this if parameters are depending on eachother
            //use SetParSilent when adjusting...
        }

        protected bool SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            fields[propertyName] = value;
            Verify(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
        protected T GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            object value = null;
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

        [Serializable]
        private class Dict : Dictionary<string, object>
        {
        }

        [Serializable]
        private class DictEntry
        {
            public string Key { get; set; }
            public object Value { get; set; }

            public DictEntry()
            {

            }
            public DictEntry(string key, object value)
            {
                Key = key;
                Value = value;
            }
        }
    }
}
