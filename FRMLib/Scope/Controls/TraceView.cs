using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static FRMLib.Controls.TraceView;

namespace FRMLib.Scope.Controls
{
    public partial class TraceView : UserControl
    {
        DataGridView dataGridView1 = new DataGridView();
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

            this.Controls.Add(dataGridView1);
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            

            foreach (var pi in typeof(Trace).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
                DataGridViewColumn col;

                if (pi.PropertyType == typeof(bool))
                {
                    col = new DataGridViewCheckBoxColumn();
                }
                else if (pi.PropertyType.IsEnum)
                {
                    if (pi.PropertyType.GetCustomAttributes<FlagsAttribute>().Any())
                    {

                        //https://www.codeproject.com/Articles/24614/How-to-Host-a-Color-Picker-Combobox-in-Windows-For
                        DataGridViewEnumFlagsColumn ccol = new DataGridViewEnumFlagsColumn();
                        ccol.DataSource = Enum.GetValues(pi.PropertyType);
                        col = ccol;
                    }
                    else
                    {
                        DataGridViewComboBoxColumn ccol = new DataGridViewComboBoxColumn();
                        ccol.DataSource = Enum.GetValues(pi.PropertyType);
                        col = ccol;
                    }

                }
                else
                {
                    col = new DataGridViewTextBoxColumn();
                }


                dataGridView1.Columns.Add(col);
                col.DataPropertyName = pi.Name;
                col.Name = pi.Name;
                col.HeaderText = attr.Text == null ? pi.Name : attr.Text;
                col.Width = attr.Width == 0 ? 100 : attr.Width;
                
            }
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
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
        public DataGridViewAutoSizeColumnMode AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
    }
}
