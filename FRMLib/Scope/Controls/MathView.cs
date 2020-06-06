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
using FRMLib.Scope.MathTypes;

namespace FRMLib.Scope.Controls
{
    public partial class MathView : UserControl
    {
        DataGridViewComboBoxColumn markerColumn1 = new DataGridViewComboBoxColumn();
        DataGridViewComboBoxColumn markerColumn2 = new DataGridViewComboBoxColumn();
        private static BindingList<MathType> mathTypes = new BindingList<MathType>();
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
                    markerColumn1.DataSource = dataSource.Markers;
                    markerColumn2.DataSource = dataSource.Markers;
                }
            }
        }



        public MathView()
        {
            InitializeComponent();
            if (mathTypes.Count == 0)
            {
                var vs = AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(s => s.GetTypes())
                            .Where(p => typeof(IMathType).IsAssignableFrom(p) && p.IsClass).ToArray();

                foreach(var v in vs)
                    mathTypes.Add(new MathType(v));
            }

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Dock = DockStyle.Fill;
            Controls.Add(dataGridView);
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AllowUserToResizeRows = false;
            dataGridView.RowHeadersVisible = false;
            dataGridView.CellFormatting += DataGridView_CellFormatting;

            foreach (var pi in typeof(MathItem).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
                DataGridViewColumn col;

                if (pi.Name == nameof(MathItem.MathType))
                {
                    DataGridViewComboBoxColumn ccol = new DataGridViewComboBoxColumn();
                    ccol.DataSource = mathTypes;
                    Type t = typeof(int);

                    ccol.ValueMember = nameof(IMathType.Self);
                    ccol.DisplayMember = nameof(IMathType.Name);

                    col = ccol;
                }
                else if (pi.Name == nameof(MathItem.Marker1))
                {
                    markerColumn1.DisplayMember = nameof(Marker.ID);
                    markerColumn1.ValueMember = nameof(Marker.Self);
                    col = markerColumn1;
                }
                else if (pi.Name == nameof(MathItem.Marker2))
                {
                    markerColumn2.DisplayMember = nameof(Marker.ID);
                    markerColumn2.ValueMember = nameof(Marker.Self);
                    col = markerColumn2;
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
