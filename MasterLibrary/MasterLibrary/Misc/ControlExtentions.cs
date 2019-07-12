using System;
using System.Windows.Forms;

namespace ProjectCopier
{
    public static class ControlExtentions
    {
        private delegate void SafeCallDelegate(Control c, Predicate<Control> action);
        public static void InvokeIfRequired(this Control c, Predicate<Control> action)
        {
            if (c.InvokeRequired)
            {
                c.Invoke(new SafeCallDelegate(InvokeIfRequired), new object[] {c,  action });
            }
            else
            {
                action(c);
            }
        }
    }
}
