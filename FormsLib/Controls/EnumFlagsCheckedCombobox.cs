using FormsLib.Extentions;
using System;
using System.Windows.Forms;


namespace FormsLib.Controls
{
    public class EnumFlagsCheckedCombobox : UserControl
    {
        public event EventHandler<Enum> OnCheckedChanged;
        TextBox textbox = new TextBox();
        Button button = new Button();
        ToolStripDropDown dropDown;
        ToolStripControlHost host;
        CheckedListBox lb;
        Enum _value;
        public Enum Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                /*if (!_value.GetType().GetCustomAttributes(typeof(FlagsAttribute), true). .Any())
                    throw new Exception("Enum doenst support flags");*/
                textbox.Text = _value.ToString();
                lb.Items.Clear();
                foreach (Enum item in Enum.GetValues(_value.GetType()))
                {
                    if (item.ToInt() != 0)
                        lb.Items.Add(item, _value.HasFlag(item));
                }
            }
        }

        public EnumFlagsCheckedCombobox()
        {

            button.Size = new System.Drawing.Size(22, 22);
            button.Location = new System.Drawing.Point(Width - button.Width, -1);
            button.Text = "\u2BC6";
            button.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            button.Click += new EventHandler(Button_Click);
            Controls.Add(button);

            textbox.Location = new System.Drawing.Point(0, 0);
            textbox.Size = new System.Drawing.Size(Width - button.Width, 20);
            textbox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            Controls.Add(textbox);

            Height = 20;

            dropDown = new ToolStripDropDown();
            lb = new CheckedListBox();
            host = new ToolStripControlHost(lb);

            //dropDown.AutoSize = true;
            dropDown.Items.Add(host);
            lb.ItemCheck += Lb_ItemCheck;
        }

        private void Lb_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            Enum changedItem = (Enum)lb.Items[e.Index];
            if (e.NewValue == CheckState.Checked)
                _value = _value.SetFlags(changedItem);
            else
                _value = _value.ClearFlags(changedItem);
            OnCheckedChanged?.Invoke(this, changedItem);
            textbox.Text = _value.ToString();
        }

        void Button_Click(object sender, EventArgs e)
        {
            dropDown.Width = Width;
            lb.Width = Width;
            host.Width = Width;

            if (dropDown.Visible)
                dropDown.Close();
            else
                dropDown.Show(this, 0, Height);
        }
    }
}
