using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace AutoUpdaterTest
{
    public partial class Form1 : Form
    {
        Updator updator = new Updator();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            updator.CloseApplication += Updator_CloseApplication;
        }

        private void Updator_CloseApplication()
        {
            this.Close();
        }

        private void Button1_Click(object sender, EventArgs e)
        {

            updator.DoUpdate(@"C:\a\Updater");
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (updator.DoUpdate(@"C:\a\Updater"))
                timer1.Stop();

            int i;
            if (int.TryParse(this.Text, out i))
                this.Text = (i + 1).ToString();
            else
                this.Text = "0";
        }
    }
}
