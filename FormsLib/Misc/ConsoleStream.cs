using System;
using System.Collections.Generic;
using System.Text;

namespace FormsLib.Misc
{
    public class ConsoleStream
    {
        public event EventHandler? OnRecieved;
        Queue<string> x = new Queue<string>();

        public string ReadAll()
        {
            StringBuilder res = new StringBuilder();
            while (x.Count > 0)
                res.Append(x.Dequeue());
            return res.ToString();
        }

        public void Write(string text)
        {
            x.Enqueue(text);
            OnRecieved?.Invoke(this, EventArgs.Empty);
        }
    }
}
