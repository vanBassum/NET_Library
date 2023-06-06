using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace CoreLib.Misc
{    /// <summary>
     /// A helperclass to help implement the INotifyPropertyChanged.
     /// <code>
     /// class Someclass : PropertySensitive
     /// { 
     ///     public int SomeProp { get { return GetPar(5); } set { SetPar(value); } }
     /// }
     /// </code>
     /// </summary>
     /// 
    public abstract class PropertySensitive : INotifyPropertyChanged
    {
        [Browsable(false)]
        public bool NotifyOnChange { get; set; } = true;
        /// <summary>
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();


        public void InvokePropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Use this in the setter of a property to set the value
        /// <code>
        /// public int SomeProp { set { SetPar(value); } }
        /// </code>
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="value">Value to witch the property will be set.</param>
        /// <param name="propertyName">The name of the property.</param>
        public void SetPar<T>(T value, [CallerMemberName] string propertyName = null)
        {
            fields[propertyName] = value;
            if(NotifyOnChange)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Use this in the getter of a property to retrieve the value.
        /// <code>
        /// public int SomeProp { get { return GetPar(5); }}
        /// </code> 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defVal"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public T GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            if (!fields.ContainsKey(propertyName))
                fields[propertyName] = defVal;
            return (T)fields[propertyName];
        }

        public IEnumerable<KeyValuePair<string, object>> GetFields()
        {
            foreach (var kvp in fields)
                yield return kvp;
        }
    }
}