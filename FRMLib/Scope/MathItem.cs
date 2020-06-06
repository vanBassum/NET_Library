using FRMLib.Scope.Controls;
using FRMLib.Scope.MathTypes;
using STDLib.Misc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FRMLib.Scope
{
    public class MathItem : PropertySensitive
    {
        [TraceViewAttribute(Text = "M1", Width = 40)]
        public Marker Marker1 { get { return GetPar<Marker>(null); } set { SetPar(value); } }
        [TraceViewAttribute(Text = "M2", Width = 40)]
        public Marker Marker2 { get { return GetPar<Marker>(null); } set { SetPar(value); } }

        //[TraceViewAttribute()]
        //public MathType MathType { get; set; }


        
        private IMathType mathInstance = new DeltaX();

        [TraceViewAttribute()]
        public MathType MathType 
        {
            get 
            {
                return GetPar<MathType>();

                //return GetPar<MathType>(new MathType(mathInstance.GetType())); 
            }
            set 
            { 
                SetPar(value);
                mathInstance = (IMathType)Activator.CreateInstance(value.Type);
            } 
        }

        [TraceViewAttribute()]
        public object Value { get { return mathInstance.GetValue(); } }
        

    }


    public class MathType
    {
        public Type Type { get; set; }
        public MathType Self { get { return this; } }
        public string Name { get { return Type.Name; } }

        public MathType(Type type)
        {
            Type = type;
        }

    }
}
