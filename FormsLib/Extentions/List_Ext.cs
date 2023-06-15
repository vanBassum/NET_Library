using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FormsLib.Extentions
{

    public static class List_Ext
    {

        public static void MySort<T>(this IList<T> list, Comparison<T> comparison)
        {
            bool busy = true;

            while (busy)
            {
                busy = false;

                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (comparison(list[i], list[i + 1]) < 0)
                    {
                        busy = true;
                        T temp = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = temp;
                    }
                }
            }
        }

        public static void OrderBy(this IList list, string propertyName, bool Ascending)
        {
            Type listElementType = list.GetType().GetGenericArguments().SingleOrDefault();
            if (listElementType != null)
            {
                PropertyInfo pi = listElementType.GetProperty(propertyName);
                if (pi != null)
                {
                    list.OrderBy(pi, Ascending);
                }
            }
        }

        public static void OrderBy(this IList list, PropertyInfo propertyInfo, bool Ascending)
        {
            bool busy = true;

            while (busy)
            {
                busy = false;

                for (int i = 0; i < list.Count - 1; i++)
                {
                    var a = propertyInfo.GetValue(list[i]);
                    var b = propertyInfo.GetValue(list[i + 1]);

                    if (a is IComparable aa)
                    {
                        if (b is IComparable bb)
                        {
                            if (aa.CompareTo(b) < 0 && !Ascending || aa.CompareTo(b) > 0 && Ascending)
                            {
                                busy = true;
                                object temp = list[i];
                                list[i] = list[i + 1];
                                list[i + 1] = temp;
                            }

                        }
                    }
                }
            }
        }

        /*
        


        public static void SortByProperty<T>(this IList<T> list, PropertyInfo property)
        {

            bool busy = true;

            Comparer<T> comparer = Comparer<T>.Default;

            while (busy)
            {
                busy = false;

                for (int i = 0; i < list.Count - 1; i++)
                {
                    if (comparer.Compare(list[i], list[i+1]) > 0)
                    {
                        busy = true;
                        T temp = list[i];
                        list[i] = list[i + 1];
                        list[i + 1] = temp;
                    }
                }
            }
        }
        */
    }




}
