using System.Collections.Generic;

namespace Oscilloscope
{
    public class MarkerLine
    {
        public int ID { get; /*private set;*/ } = nextId++;
        public bool IsDate { get; set; } = false;
        public double X { get; set; } = 0;
        public string Text { get { return ScopeControl.ToHumanReadable(X, 3, IsDate); } }
        public MarkerLine Self { get { return this; } }

        private static int nextId = 0;

        ScopeClass parentScope;

        public List<MarkerValue> Values 
        { 
            get 
            {
                List<MarkerValue> values = new List<MarkerValue>();

                foreach (Trace t in parentScope.Traces)
                    values.Add(new MarkerValue { 
                        Colour = t.Colour, 
                        Value = ScopeControl.ToHumanReadable(t.GetYValue(X), 3) + " " + t.Unit,
                        Name = t.Name});


                return values;
            } 
        }


        public MarkerLine(ScopeClass parentScope)
        {
            this.parentScope = parentScope;
        }


        //public Color Colour { get; set; } = Color.Red;
        //public string Name { get; set; } = "Trace";
    }

}
