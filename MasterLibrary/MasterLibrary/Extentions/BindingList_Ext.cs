using System.ComponentModel;
using System.Text;

namespace MasterLibrary.Extentions
{
    public static class BindingList_Ext
    {
        public static string ListAsString(this BindingList<string> list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string s in list)
                sb.Append("'" + s + "', ");

            sb = sb.Remove(sb.Length - 2, 1);
            return sb.ToString();
        }
    }
}
