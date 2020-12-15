using System;
using System.Windows.Forms;

namespace FRMLib
{
    public partial class ObjectEditDialog : Form
    {
        public object Value { get { return propertyGrid1.SelectedObject; } set { propertyGrid1.SelectedObject = value; } }

        public ObjectEditDialog()
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
