﻿using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace MasterLibrary.Controls
{
    public static class Ext
    {
        private delegate void SafeCallDelegate<T>(T c, Action<T> action);

        public static void InvokeIfRequired<T>(this T c, Action<T> action)
            where T : Control
        {
            if (c.InvokeRequired)
            {
                c.Invoke(new SafeCallDelegate<T>(InvokeIfRequired), new object[] { c, action });
            }
            else
            {
                action(c);
            }
        }

        public static bool ContainsAny(this string s, string[] filter, bool ignoreCase = false)
        {
            if(ignoreCase)
            {
                string ss = s.ToLower();
                foreach (string f in filter)
                    if (ss.Contains(f.ToLower()))
                        return true;
            }
            else
            {
                foreach (string f in filter)
                    if (s.Contains(f))
                        return true;
            }
            return false;
        }

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
