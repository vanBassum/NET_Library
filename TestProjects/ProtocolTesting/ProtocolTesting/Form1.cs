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
using FRMLib.Scope;
using FRMLib.Scope.Controls;
using FRMLib.Scope.MathFunctions;

namespace ProtocolTesting
{
    public partial class Form1 : Form
    {
        ScopeController scope = new ScopeController();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            scopeView1.DataSource = scope;
            traceView1.DataSource = scope;

            Trace t1 = new Trace();
            t1.Name = "TestTrace";

            for (int i = 0; i < 100; i++)
                t1.Points.Add((double)i, Math.Sin(2 * Math.PI * i / 100));

            scope.Traces.Add(t1);

            Trace t2 = new Trace();
            t2.Function = typeof(Invert);
            t2.Pen = Palettes.Yellow;
            scope.Traces.Add(t2);


            /*
            Trace t1 = new Trace();
            t1.Name = "TestTrace";

            for (int i = 0; i < 100; i++)
                t1.Points.Add((double)i, Math.Sin(2 * Math.PI * i / 100));




            scope.Traces.Add(t1);
            */
        }
    }





    

}
