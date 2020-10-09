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
    public partial class MarkerView : UserControl
    {
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
                    dataGridView.DataSource = dataSource.Markers;
                    dataSource.Traces.ListChanged += Traces_ListChanged;
                    dataSource.Markers.ListChanged += Markers_ListChanged;
                }
            }
        }

        

        public MarkerView()
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
        }

       

        private void Traces_ListChanged(object sender, ListChangedEventArgs e)
        {
            //TODO: There is a lot of optimalisation possible here!
            dataGridView.Columns.Clear();
            foreach (var pi in typeof(Marker).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
                DataGridViewColumn col;
                dataGridView.Columns.Add(col = new DataGridViewColumn(new DataGridViewTextBoxCell()));

                col.DataPropertyName = pi.Name;
                col.Name = pi.Name;
                col.HeaderText = attr.Text == null ? pi.Name : attr.Text;
                col.Width = attr.Width == 0 ? 100 : attr.Width;
                col.AutoSizeMode = attr.AutoSizeMode;
            }

            if(dataSource != null)
            {
                foreach (Trace t in dataSource.Traces)
                {
                    DataGridViewColumn col;
                    dataGridView.Columns.Add(col = new DataGridViewColumn(new DataGridViewTextBoxCell()));

                    col.Tag = t;
                    col.Name = t.Name;
                    col.HeaderText = t.Name;
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
                }
            }
            //dataGridView.AutoResizeColumns();
        }

        private void Markers_ListChanged(object sender, ListChangedEventArgs e)
        {
            dataGridView.Refresh();
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            Trace t = dgv.Columns[e.ColumnIndex].Tag as Trace;

            if(t!=null)
            {
                e.Value = t.ToHumanReadable(t.GetYValue(DataSource.Markers[e.RowIndex].X));
            }

            if(dgv.Columns[e.ColumnIndex].DataPropertyName == nameof(Marker.X))
            {
                e.Value = dataSource.HorizontalToHumanReadable((double)e.Value);
            }

        }
    }
}
