using STDLib.Ethernet;
using STDLib.JBVProtocol;
using STDLib.JBVProtocol.Connections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRMLib;
using System.Runtime.CompilerServices;
using STDLib.Saveable;
using STDLib.Misc;

namespace ProtocolTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Settings.Load("Settings.json", true);
            Logger.SetFile("Log.txt");
            Logger.WriteLine("test");
        }
    }


    public class SubSettings
    {
        public int Someting { get; set; } = 7;
    }


    public class Settings : BaseSettings<Settings>
    {
        public static int SomeSetting { get { return GetPar(0); } set { SetPar(value); } }
        public static string AnotherSetting { get { return GetPar("bla"); } set { SetPar(value); } }
        public static SubSettings SubSet { get { return GetPar<SubSettings>(new SubSettings()); } set { SetPar(value); } }
    }





    

}
