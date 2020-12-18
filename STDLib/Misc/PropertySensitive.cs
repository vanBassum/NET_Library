using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;



namespace STDLib.Misc
{
    /// <summary>
    /// A helperclass to help implement the INotifyPropertyChanged.
    /// <code>
    /// class Someclass : PropertySensitive
    /// { 
    ///     public int SomeProp { get { return GetPar(5); } set { SetPar(value); } }
    /// }
    /// </code>
    /// </summary>
    public abstract class PropertySensitive : INotifyPropertyChanged
    {
        public bool NotifyOnChange { get; set; } = true;
        /// <summary>
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private readonly Dictionary<string, object> fields = new Dictionary<string, object>();

        /// <summary>
        /// Use this in the setter of a property to set the value
        /// <code>
        /// public int SomeProp { set { SetPar(value); } }
        /// </code>
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="value">Value to witch the property will be set.</param>
        /// <param name="propertyName">The name of the property.</param>
        protected void SetPar<T>(T value, [CallerMemberName] string propertyName = null)
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
        protected T GetPar<T>(T defVal = default(T), [CallerMemberName] string propertyName = null)
        {
            if (!fields.ContainsKey(propertyName))
                fields[propertyName] = defVal;
            return (T)fields[propertyName];
        }
    }
}