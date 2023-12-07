using CoreLib.Misc;
using FormsLib.Scope.Controls;
using System.Numerics;

namespace FormsLib.Scope.Models
{
    public class GraphMarker 
    {
        private static int nextId = 0;
        private static int NextId => nextId++;

        public int ID { get; } = NextId;
        public Pen Pen { get; set; } = new Pen(Color.White) { DashPattern = new float[] { 4.0F, 4.0F, 8.0F, 4.0F } };
        public float X { get; set; } = 0;
    }
}

