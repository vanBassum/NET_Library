using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System;

namespace FRMLib.Scope.Controls
{
    public partial class MathView : UserControl
    {

        DataGridViewComboBoxColumn markerColumn1 = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn markerColumn2 = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn traceColumn = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn functionColumn = new DataGridViewComboBoxColumn();

        DataGridView dataGridView = new DataGridView();
        private ScopeController dataSource;
        public ScopeController DataSource
        {
            get { return dataSource; }
            set
            {
                dataSource = value;
                if (dataSource != null)
                {
                    dataGridView.DataSource = dataSource.MathItems;
                    markerColumn1.DataSource = dataSource.Cursors;
                    markerColumn2.DataSource = dataSource.Cursors;
                    traceColumn.DataSource = DataSource.Traces;
                }
            }
        }
 


        public MathView()
        {
            InitializeComponent();

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Dock = DockStyle.Fill;
            Controls.Add(dataGridView);
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersVisible = false;
            dataGridView.CellFormatting += DataGridView_CellFormatting;

            try
            {
                foreach (var pi in typeof(MathItem).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
                {

                    TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
                    DataGridViewColumn col;

                    if (pi.Name == nameof(MathItem.Trace))
                    {
                        traceColumn.DisplayMember = nameof(Trace.Name);
                        traceColumn.ValueMember = nameof(Trace.Self);
                        col = traceColumn;
                    }
                    else if (pi.Name == nameof(MathItem.Marker1))
                    {
                        markerColumn1.DisplayMember = nameof(Scope.Cursor.ID);
                        markerColumn1.ValueMember = nameof(Scope.Cursor.Self);
                        col = markerColumn1;
                    }
                    else if (pi.Name == nameof(MathItem.Marker2))
                    {
                        markerColumn2.DisplayMember = nameof(Scope.Cursor.ID);
                        markerColumn2.ValueMember = nameof(Scope.Cursor.Self);
                        col = markerColumn2;
                    }
                    else if (pi.Name == nameof(MathItem.Function))
                    {
                        functionColumn.DataSource = MathItem.MathFunctions;
                        functionColumn.DisplayMember = nameof(MathFunction.Name);
                        functionColumn.ValueMember = nameof(MathFunction.Self);
                        col = functionColumn;
                    }
                    else
                    {
                        col = new DataGridViewTextBoxColumn();
                    }


                    dataGridView.Columns.Add(col);
                    col.DataPropertyName = pi.Name;
                    col.Name = pi.Name;
                    col.HeaderText = attr.Text == null ? pi.Name : attr.Text;
                    col.Width = attr.Width == 0 ? 100 : attr.Width;
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            /*
            DataGridView dgv = sender as DataGridView;

            PropertyInfo pi = typeof(Trace).GetProperty(dgv.Columns[e.ColumnIndex].DataPropertyName);
            if (pi == null)
                return;

            switch (e.Value)
            {
                case Type p:
                    e.text = p.Name;
                    break;
            }
            */
        }
    }




}
