using System;
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

        static public int FindIndex<T>(this BindingList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }
            return -1;
        }

        static public void RemoveWhere<T>(this BindingList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    list.RemoveAt(i--);
            }
        }
        static public bool Exists<T>(this BindingList<T> list, Predicate<T> predicate)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return true;
            }
            return false;
        }
    }
}
