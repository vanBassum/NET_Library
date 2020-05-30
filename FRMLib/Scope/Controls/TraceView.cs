using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace FRMLib.Scope.Controls
{
    public partial class TraceView : UserControl
    {
        private ScopeController dataSource;
        public ScopeController DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                if (dataSource != null)
                    dataGridView1.DataSource = dataSource.Traces;
            }
        }


        public TraceView()
        {
            InitializeComponent();

            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;


            foreach (var pi in typeof(Trace).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();


                DataGridViewColumn col;
                if (pi.PropertyType == typeof(bool))
                    dataGridView1.Columns.Add(col = new DataGridViewColumn(new DataGridViewCheckBoxCell()));
                else
                    dataGridView1.Columns.Add(col = new DataGridViewColumn(new DataGridViewTextBoxCell()));

                col.DataPropertyName = pi.Name;
                col.Name = pi.Name;
                col.HeaderText = attr.Text == null ? pi.Name : attr.Text;
                col.Width = attr.Width == 0 ? 100 : attr.Width;

            }
            dataGridView1.ClearSelection();
        }



        private void TraceView_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

            DataGridView dgv = sender as DataGridView;

            PropertyInfo pi = typeof(Trace).GetProperty(dgv.Columns[e.ColumnIndex].DataPropertyName);
            if (pi == null)
                return;
            TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
            if (attr == null)
                return;


            switch (e.Value)
            {
                case Pen p:
                    e.CellStyle.BackColor = p.Color;
                    e.CellStyle.ForeColor = p.Color;
                    break;
            }
        }
    }

    public class TraceViewAttribute : Attribute
    {
        public bool Show { get; set; } = true;
        public string Text { get; set; } = null;
        public int Width { get; set; } = 0;
    }
}
