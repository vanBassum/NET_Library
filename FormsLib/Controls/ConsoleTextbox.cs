using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FormsLib.Extentions;
using FormsLib.Misc;

namespace FormsLib.Controls
{
    public partial class ConsoleTextbox : RichTextBox
    {
        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);
        public event EventHandler<CMDArgs> OnCommand;
        public BindingList<string> History { get; } = new BindingList<string>();
        const string seperator = ">>";
        int startpos = 0;

        public ConsoleTextbox()
        {
            this.KeyDown += ConsoleTextbox_KeyDown;
            this.SelectionChanged += ConsoleTextbox_SelectionChanged;
            this.MouseDown += ConsoleTextbox_MouseDown;
        }

        private void ConsoleTextbox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                if(SelectionLength == 0)
                {
                    Text = Text.Insert(SelectionStart, Clipboard.GetText(TextDataFormat.Text));
                }
                else
                {
                    Clipboard.SetText(SelectedText);
                }
            }
        }

        private void ConsoleTextbox_SelectionChanged(object sender, EventArgs e)
        {
            //if (SelectionStart < startpos && Text.Length >= startpos)
            //    SetCaret(startpos);
        }

        void SetCaret(int pos)
        {
            SelectionStart = pos;
            //SelectionLength = 0;
        }

        CancellationTokenSource cts;
        CMDArgs args;
        int histPtr = 0;

        public async void ExecuteCommand(string cmd)
        {
            if (cts == null)
            {
                cts = new CancellationTokenSource();
                if (History.LastOrDefault() != cmd)
                    History.Add(cmd);
                histPtr = History.Count;
                args = new CMDArgs(cmd);
                args.CancellationToken = cts.Token;
                args.OutputStream.OnRecieved += (object s, EventArgs a) =>
                {
                    if (s is ConsoleStream cs)
                    {
                        this.Invoke(new Action(() => { this.AppendText(cs.ReadAll()); }));
                    }
                };
                Task t = new Task(() => { OnCommand?.Invoke(this, args); });
                AppendText("\r\n");
                t.Start();
                await t;
                t.Dispose();
                cts.Dispose();
                cts = null;
                AppendText("\r\n");
                Start();
            }
        }






       


        public void Start()
        {
            this.AppendText(seperator);
            startpos = Text.Length;
        }

        private async void ConsoleTextbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (SelectionStart < startpos && Text.Length >= startpos)
                SetCaret(Text.Length);

            if (cts != null)
            {
                if (e.KeyData.HasFlag(Keys.Control, Keys.C))
                {
                    cts.Cancel();
                }
                else
                {
                    //args.InputStream.Write();
                }

                e.SuppressKeyPress = true;
            }
            else
            {
                if (e.KeyData == Keys.Enter)
                {
                    string cmd = this.Text.Substring(startpos);
                    e.SuppressKeyPress = true;
                    ExecuteCommand(cmd);
                }
                else if (e.KeyData.HasFlag(Keys.Control, Keys.V))
                {
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Back)
                {
                    if (SelectionStart <= startpos && Text.Length >= startpos)
                        e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Up)
                {
                    histPtr--;

                    if (histPtr < 0)
                        histPtr = 0;

                    if (histPtr < History.Count)
                    {
                        if(startpos < Text.Length)
                            Text = Text.Remove(startpos);

                        AppendText(History[histPtr]);
                        e.SuppressKeyPress = true;
                    }
                }
                else if(e.KeyData == Keys.Down)
                {
                    histPtr++;

                    if (histPtr >= History.Count)
                        histPtr = History.Count - 1;

                    if (histPtr < History.Count)
                    {
                        if (startpos < Text.Length)
                            Text = Text.Remove(startpos);

                        AppendText(History[histPtr]);
                        e.SuppressKeyPress = true;
                    }
                }
            }
        }
    }

    

    public class CMDArgs
    {
        public string RawCommand { get; set; }  //The raw input of the user
        public byte[] Command { get; private set; }  //The raw input of the user
        public CancellationToken CancellationToken { get; set; }
        public ConsoleStream OutputStream { get; set; } = new ConsoleStream();
        public LogLevel LogLevel { get; set; } = LogLevel.INFO;

        public CMDArgs(string rawCmd)
        {
            RawCommand = rawCmd;
            ParseCommandASCII(rawCmd);
        }


        public static string HelpMessage { get { return @"
The commandline supports the following escape characters:
\x  Change to hexadecimal mode
\a  Change to ascii mode
\\  Write '\' character
\l  Change logging niveau, last occurance is leading
    \lv = verbose
    \ld = debug
    \li = info
    \le = error
    \ln = none

Examples of some commands:

    Command                          ASCII          HEX
    'CTES\x68656c6c6f0d0a'  => Sends 'CTEShello'    '4354455368656c6c6f'
    '\x4354455368656c6c6f'  => Sends 'CTEShello'    '4354455368656c6c6f'
    'CTES\\hello'           => Sends 'CTES\hello'   '435445535c68656c6c6f'
    'CTES\x6865\allo'       => Sends 'CTEShello'    '4354455368656c6c6f'
"; } }




        enum Mode
        {
            ASCII,      // Default and \a 
            HEX,        // \x
            //DECIMAL,  // Maybe \dl for little endian @TODO
        }


        void ParseCommandASCII(string cmd)
        {
            Command = Encoding.ASCII.GetBytes(cmd);
        }

        void ParseCommand(string cmd)
        {
            List<byte> res = new List<byte>();
            int rdptr = 0;
            Mode mode = Mode.ASCII;

            while (rdptr < cmd.Length)
            {
                int ind = cmd.IndexOf('\\', rdptr);
                int len = 0;

                if (ind >= 0)
                    len = ind;
                else if (ind < 0)
                    len = cmd.Length - rdptr;

                if (len > 0)
                {
                    string substr = cmd.Substring(rdptr, len);
                    rdptr += len;
                    byte[] buf;
                    switch (mode)
                    {
                        case Mode.ASCII:
                            buf = Encoding.ASCII.GetBytes(substr);
                            res.AddRange(buf);
                            break;
                        case Mode.HEX:
                            buf = StringToByteArray(substr.Replace(" ", ""));
                            res.AddRange(buf);

                            break;
                    }
                }
                if (ind >= 0)
                {
                    rdptr++;
                    if (rdptr < cmd.Length)
                    {
                        char escapedChar = cmd[rdptr];

                        switch (escapedChar)
                        {
                            case '\\':
                                switch (mode)
                                {
                                    case Mode.ASCII:
                                        byte[] r = Encoding.ASCII.GetBytes("\\");
                                        res.AddRange(r);
                                        rdptr += r.Length;
                                        break;
                                }
                                break;

                            case 'x':
                                mode = Mode.HEX;
                                break;

                            case 'a':
                                mode = Mode.HEX;
                                break;

                            case 'l':
                                rdptr++;
                                switch (cmd[rdptr])
                                {
                                    case 'v': LogLevel = LogLevel.VERBOSE; break;
                                    case 'd': LogLevel = LogLevel.DEBUG; break;
                                    case 'i': LogLevel = LogLevel.INFO; break;
                                    case 'e': LogLevel = LogLevel.ERROR; break;
                                    case 'n': LogLevel = LogLevel.NONE; break;
                                }
                                break;
                        }
                        rdptr++;
                    }
                }
            }
            Command = res.ToArray();
        }



        public static byte[] StringToByteArray(string hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

    }

    
}
