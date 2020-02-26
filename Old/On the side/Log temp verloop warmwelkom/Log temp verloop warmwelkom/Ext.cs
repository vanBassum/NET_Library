using System;

namespace Log_temp_verloop_warmwelkom
{
    public static class Ext
    { 
        public static bool CompareBegin<T>(this T[] arr1, T[] arr2) where T : IComparable
        {
            int len = arr1.Length;
            if (arr2.Length < len)
                len = arr2.Length;

            for (int i = 0; i < len; i++)
                if (arr1[i].CompareTo(arr2[i]) != 0)
                    return false;
            return true;
        }
    }
}
