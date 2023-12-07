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
                    dataGridView.DataSource = dataSource.Cursors;
                    dataSource.Traces.ListChanged += Traces_ListChanged;
                    dataSource.Cursors.ListChanged += Cursors_ListChanged;
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

            foreach (var pi in typeof(Cursor).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
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

        private void Traces_ListChanged(object sender, ListChangedEventArgs e)
        {
            //TODO: There is a lot of optimalisation possible here!
            //dataGridView.Columns.Clear();
            

            if (dataSource != null)
            {
                foreach (Trace t in dataSource.Traces)
                {
                    DataGridViewColumn? col = dataGridView.Columns.Cast<DataGridViewColumn>().FirstOrDefault(column => column.Tag != null && column.Tag == t);

                    if (col == null)
                    {
                        col = new DataGridViewColumn(new DataGridViewTextBoxCell());
                        dataGridView.Columns.Add(col = new DataGridViewColumn(new DataGridViewTextBoxCell()));
                        col.Tag = t;
                        col.Name = t.Name;
                        col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    col.HeaderText = t.Name;
                }
            }
            //dataGridView.AutoResizeColumns();
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
                e.Value = t.ToHumanReadable(t.GetYValue(DataSource.Cursors[e.RowIndex].X));
            }

            if (dgv.Columns[e.ColumnIndex].DataPropertyName == nameof(Scope.Cursor.X))
            {
                e.Value = dataSource.Settings.HorizontalToHumanReadable((double)e.Value);
            }

        }
    }
}
