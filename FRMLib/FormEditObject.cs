using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FRMLib
{
    public partial class FormEditObject : Form
    {
        public object Value { get { return propertyGrid1.SelectedObject; } set { propertyGrid1.SelectedObject = value; } }

        public FormEditObject()
        {
            InitializeComponent();
        }

        private void FormEditObject_Load(object sender, EventArgs e)
        {
            button1.Click += (s, e2) => DialogResult = DialogResult.OK;
            button2.Click += (s, e2) => DialogResult = DialogResult.Cancel;
        }
    }
}
