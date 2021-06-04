using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;



namespace STDLib.Misc
{
    public abstract class PropertySensitiveTypeDescriptor : PropertySensitive, ICustomTypeDescriptor
    {



        #region ICustomTypeDescriptor
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }


        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();

            //Add all the properties that are hardcoded
            foreach (var pi in this.GetType().GetProperties())
            {
                IEnumerable<Attribute> attrs = new Attribute[] {
                    //new CategoryAttribute("Design"),
                }.Concat(pi.GetCustomAttributes());

                
                SimplePropertyDescriptor pd = new SimplePropertyDescriptor(pi.Name, attrs.ToArray(), pi.PropertyType);
                pd.Getter = () => GetPar<object>(null, pi.Name);
                pd.Setter = (a) => SetPar(a, pi.Name);
                descriptors.Add(pd);
            }

            //Add all properties that are fields
            foreach (var field in GetFields())
            {
                string fieldName = field.Key;
                object value = field.Value;

                if (!descriptors.Any(a => a.Name == fieldName))
                {
                    IEnumerable<Attribute> attrs = new Attribute[] {
                        new CategoryAttribute("Properties"),
                    };

                    SimplePropertyDescriptor pd = new SimplePropertyDescriptor(fieldName, attrs.ToArray(), value.GetType());
                    pd.Getter = () => GetPar<object>(null, fieldName);
                    pd.Setter = (a) => SetPar(a, fieldName);
                    descriptors.Add(pd);
                }
            }
            return new PropertyDescriptorCollection(descriptors.ToArray());
        }

        #endregion
    }
}
