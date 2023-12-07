using FormsLib.Scope.Enums;
using FormsLib.Scope.Helpers;
using System.Numerics;

namespace FormsLib.Scope.Models
{

    public class GraphTrace
    {
        public string Name { get; set; } = "New trace";
        public string Unit { get; set; } = "";
        public float Scale { get; set; } = 1f;
        public float Offset { get; set; } = 0f;
        public bool Visible { get; set; } = true;
        public int Layer { get; set; } = 0;
        public Color Color { get; set; } = Color.Aqua;
        public DrawStyles DrawStyle { get; set; } = DrawStyles.Lines;
        public DrawOptions DrawOptions { get; set; } = DrawOptions.None;
        public Func<float, string> ToHumanReadable { get; set; } = (value) => StringHelpers.FormatFloatWithUnits(value, 3);
        public List<Vector2> Points { get; } = new List<Vector2>();
        public List<TraceLabel> Labels { get; } = new List<TraceLabel>();
        public Dictionary<string, object> Tags { get; } = new ();
    }
}
