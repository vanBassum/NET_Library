using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace FormsLib.Scope.Controls
{
    public partial class CursorsView : UserControl
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
                    dataSource.Markers.ListChanged += Cursors_ListChanged;
                }
            }
        }



        public CursorsView()
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
            ResetColumns();

        }

        private void Traces_ListChanged(object sender, ListChangedEventArgs e)
        {
            //TODO: There is a lot of optimalisation possible here!
            

            if (dataSource != null)
            {

                switch (e.ListChangedType)
                {
                    case ListChangedType.Reset:
                        ResetColumns();
                        break;
                    case ListChangedType.ItemAdded:
                        AddTrace(DataSource.Traces[e.NewIndex]);
                        break;
                    case ListChangedType.ItemChanged:
                        UpdateTrace(DataSource.Traces[e.NewIndex]);
                        break;
                    default:
                        throw new NotImplementedException();
                }

            }
            //dataGridView.AutoResizeColumns();
        }

        void ResetColumns()
        {
            dataGridView.Columns.Clear();
            foreach (var pi in typeof(Marker).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
                DataGridViewColumn col;
                dataGridView.Columns.Add(col = new DataGridViewColumn(new DataGridViewTextBoxCell()));

                col.DataPropertyName = pi.Name;
                col.Name = pi.Name;
                col.HeaderText = attr.HeaderText == null ? pi.Name : attr.HeaderText;
                col.Width = attr.Width == 0 ? 100 : attr.Width;
                col.AutoSizeMode = attr.AutoSizeMode;
            }
        }

        void AddTrace(Trace trace)
        {
            if (trace.Visible == false)
                return;

            DataGridViewColumn? col = dataGridView.Columns.Cast<DataGridViewColumn>().FirstOrDefault(column => column.Tag != null && column.Tag == trace);

            if (col == null)
            {
                col = new DataGridViewColumn(new DataGridViewTextBoxCell());
                col.Tag = trace;
                col.Name = trace.Name;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                // Get the index of the trace in the DataSource
                int traceIndex = DataSource.Traces.IndexOf(trace);

                // Calculate the index to insert the new column, skipping the first two columns
                int skipCount = 2;
                int index = dataGridView.Columns
                    .Cast<DataGridViewColumn>()
                    .Skip(skipCount) // Skip the first two columns
                    .TakeWhile(column => column.Tag is Trace existingTrace && DataSource.Traces.IndexOf((Trace)column.Tag) < traceIndex)
                    .Count() + skipCount; // Add skipCount to adjust for skipped columns

                // Insert the new column at the determined index
                if (index < dataGridView.Columns.Count)
                    dataGridView.Columns.Insert(index, col);
                else
                    dataGridView.Columns.Add(col);
                
            }

            col.HeaderText = trace.Name;
        }

        void UpdateTrace(Trace trace)
        {

            DataGridViewColumn? col = dataGridView.Columns.Cast<DataGridViewColumn>().FirstOrDefault(column => column.Tag != null && column.Tag == trace);
            if (col != null)
            {
                col.Tag = trace;
                col.Name = trace.Name;
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                col.HeaderText = trace.Name;

                if (trace.Visible == false)
                    dataGridView.Columns.Remove(col);
            }
            else
            {
                AddTrace(trace);
            }
        }





        private void Cursors_ListChanged(object sender, ListChangedEventArgs e)
        {
            dataGridView.Refresh();
        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            Trace t = dgv.Columns[e.ColumnIndex].Tag as Trace;

            if (t != null)
            {
                e.Value = t.ToHumanReadable(t.GetYValue(DataSource.Markers[e.RowIndex].X));
            }

            if (dgv.Columns[e.ColumnIndex].DataPropertyName == nameof(Scope.Marker.X))
            {
                e.Value = dataSource.Settings.HorizontalToHumanReadable((double)e.Value);
            }

        }
    }
}
