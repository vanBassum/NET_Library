using System;
using System.ComponentModel;



namespace STDLib.Misc
{
    public class SimplePropertyDescriptor : PropertyDescriptor
    {
        Type type;
        public Action<object> Setter;
        public Func<object> Getter;

        public override Type ComponentType => type;
        public override bool IsReadOnly => false;
        public override Type PropertyType => type;

        public SimplePropertyDescriptor(string name, Type type) : base(name, null)
        {
            this.type = type;
        }

        public SimplePropertyDescriptor(string name, Attribute[] attributes, Type type) : base(name, attributes)
        {
            this.type = type;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return Getter();
        }

        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object component, object value)
        {
            Setter(value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
