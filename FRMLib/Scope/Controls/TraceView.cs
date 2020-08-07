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
using STDLib.Misc;

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
                {
                    dataGridView1.DataSource = dataSource.Traces;
                    IEnumerable<Type> exporters = typeof(Trace)
                        .Assembly.GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(Trace)) && !t.IsAbstract)
                        .Select(t => t);

                    foreach (var v in exporters)
                    {
                        test(v);
                    }
                }
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

            test(typeof(Trace));
            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
        }


        void test(Type type)
        {
            foreach (var pi in type.GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
                DataGridViewColumn col;

                if (dataGridView1.Columns[pi.Name] == null)
                {

                    if (pi.PropertyType == typeof(bool))
                    {
                        col = new DataGridViewCheckBoxColumn();
                    }
                    else if (pi.PropertyType == typeof(Trace))
                    {
                        DataGridViewComboBoxColumn ccol = new DataGridViewComboBoxColumn();

                        ccol.DataSource = DataSource.Traces;
                        ccol.DisplayMember = nameof(Trace.Name);
                        ccol.ValueMember = nameof(Trace.Self);
                        col = ccol;
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        if (pi.PropertyType.GetCustomAttributes<FlagsAttribute>().Any())
                        {
                            //https://www.codeproject.com/Articles/24614/How-to-Host-a-Color-Picker-Combobox-in-Windows-For
                            DataGridViewComboBoxColumn ccol = new DataGridViewComboBoxColumn();
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
                    /*else if (pi.Name == nameof(Trace.Function))
                    {
                        functionColumn.DataSource = Trace.TraceTypes;
                        //functionColumn.DisplayMember = nameof(Trace.Function);
                        //functionColumn.ValueMember = nameof(Trace.Function);
                        col = functionColumn;
                    }*/
                    
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
            }
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
