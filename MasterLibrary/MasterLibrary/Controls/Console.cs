using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MasterLibrary.Controls
{
    public partial class Console : UserControl
    {
        Dictionary<string, Action<string>> commands = new Dictionary<string, Action<string>>();



        public Console()
        {
            InitializeComponent();
            commands.Add("help", new Action<string>( p=> PrintHelp(p)));
            commands.Add("cls", new Action<string>(p => ClearScreen(p)));
        }


        public void AddCommand(string cmd, Action<string> action)
        {
            if (commands.ContainsKey(cmd.ToLower()))
                throw new Exception(cmd + " already exists");
            commands[cmd.ToLower()] = action;
        }

        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                Match m = Regex.Match(textBox1.Text, "([^ ]+)");
                if(m.Success)
                {
                    if (commands.ContainsKey(m.Groups[1].Value.ToLower()))
                    {
                        try
                        {
                            commands[m.Groups[1].Value.ToLower()].Invoke(textBox1.Text.Substring(m.Length));
                        }
                        catch(Exception ex)
                        {
                            WriteLine(ex.Message);
                        }
                    }
                    else
                        WriteLine("Command '" + m.Groups[1].Value + "' not found. Type help for a list of valid commands");
                }
                textBox1.Text = "";
            }
        }

        public void WriteLine(string message)
        {
            richTextBox1.AppendText(message);
            richTextBox1.AppendText("\r\n");
        }

        public void PrintHelp(string args)
        {
            WriteLine("Supported commands:");
            foreach (var a in commands)
            {
                WriteLine("\t" + a.Key);
            }
        }

        public void ClearScreen(string args)
        {
            richTextBox1.Text = "";
        }

    }
}
