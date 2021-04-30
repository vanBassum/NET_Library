using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FRMLib.Scope;


namespace scopeviewtest
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

            Trace t1 = new SampleTrace();

            for(int i=0; i<100; i++)
            {
                double y = Math.Sin(Math.PI * 2 * i / 100);

                t1.Points.Add(i, y);
            }

            scope.Traces.Add(t1);



        }
    }
}
