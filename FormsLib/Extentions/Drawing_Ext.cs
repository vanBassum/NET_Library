using System.Drawing;

namespace FormsLib.Extentions
{
    public static class Drawing_Ext
    {
        public static bool CheckIfPointIsWithin(this Rectangle rect, Point pt)
        {
            return pt.X > rect.X && pt.X < rect.X + rect.Width + rect.X && pt.Y > rect.Y && pt.Y < rect.Y + rect.Height;
        }
    }
}
