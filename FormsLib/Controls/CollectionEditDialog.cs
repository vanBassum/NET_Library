using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsLib.Controls
{
    public partial class CollectionEditDialog : Form
    {
        public IBindingList? DataSource { get => collectionEditControl1.DataSource; set => collectionEditControl1.DataSource = value; }
        public string DisplayMember { get => collectionEditControl1.DisplayMember; set => collectionEditControl1.DisplayMember = value; }

        public Func<object> CreateObject { get => collectionEditControl1.CreateObject; set => collectionEditControl1.CreateObject = value; }

        public CollectionEditDialog()
        {
            InitializeComponent();
        }

        private void CollectionEditDialog_Load(object sender, EventArgs e)
        {

        }
    }
}
