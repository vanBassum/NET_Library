namespace FormsLib.Scope.MathFunctions
{
    public class DiffX : MathFunction
    {
        public override object Calculate(MathItem mathItem)
        {
            return mathItem.Marker2.X - mathItem.Marker1.X;
        }
    }



}
