using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MasterLibrary.PropertySensitive;

namespace Oscilloscope
{


    public partial class MathControl : UserControl
    {
        DataGridView dataGridView = new DataGridView();

        ScopeControl scope;

        public BindingList<MathItem> MathItems { get; set; } = new BindingList<MathItem>();

        public MathControl(ScopeControl parent)
        {
            scope = parent;
            InitializeComponent();

            dataGridView.AutoGenerateColumns = false;
            dataGridView.Dock = DockStyle.Fill;
            this.Controls.Add(dataGridView);

            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView1";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView.Size = new System.Drawing.Size(397, 86);
            this.dataGridView.TabIndex = 0;
            this.dataGridView.CellFormatting += DataGridView_CellFormatting;

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "",
                ReadOnly = true,
                Resizable = System.Windows.Forms.DataGridViewTriState.False,
                DataPropertyName = nameof(MathItem.Colour),
                Width = 20,
            }) ;

            dataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Math",
                ReadOnly = false,
                DataPropertyName = nameof(MathItem.CalculationType),
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True },
                Resizable = System.Windows.Forms.DataGridViewTriState.True,
                DataSource = Enum.GetValues(typeof(CalculationType)),                
                Width = 75,
            }) ;
        
            dataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "Trace",
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True },
                Resizable = System.Windows.Forms.DataGridViewTriState.True,
                DataPropertyName = nameof(MathItem.Trace),
                DisplayMember = nameof(Trace.Name),
                ValueMember = nameof(Trace.Self),
                Width = 75,
                DataSource = scope.Scope.Traces,
            });

            dataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "M1",
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True },
                Resizable = System.Windows.Forms.DataGridViewTriState.True,
                DataPropertyName = nameof(MathItem.Marker1),
                DisplayMember = nameof(MarkerLine.ID),
                ValueMember = nameof(MarkerLine.Self),
                Width = 50,
                DataSource = scope.Scope.MarkerLines,
            });

            dataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                HeaderText = "M2",
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True },
                Resizable = System.Windows.Forms.DataGridViewTriState.True,
                DataPropertyName = nameof(MathItem.Marker2),
                DisplayMember = nameof(MarkerLine.ID),
                ValueMember = nameof(MarkerLine.Self),
                Width = 50,
                DataSource = scope.Scope.MarkerLines,
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Result",
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { WrapMode = DataGridViewTriState.True },
                Resizable = System.Windows.Forms.DataGridViewTriState.True,
                DataPropertyName = nameof(MathItem.Result),
                Width = 150,
                
                //DataSource = scope.Scope.MarkerLines,
            });


            dataGridView.DataSource = MathItems;

        }

        private void DataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.CellStyle.BackColor = (Color)e.Value;
                e.CellStyle.ForeColor = (Color)e.Value;
            }
        }
    }

    public enum CalculationType
    {
        None,
        Linear_regression,
    }

    public class MathItem : PropertySensitiveExternal
    {
        public Color Colour { get { return GetPar(Color.Violet); } set { SetPar(value); } }
        public Trace Trace { get { return GetPar<Trace>(null); } set { SetPar(value); } }
        public CalculationType CalculationType { get { return GetPar(CalculationType.None); } set { SetPar(value); } }
        public MarkerLine Marker1 { get { return GetPar<MarkerLine>(null); } set { SetPar(value); } }
        public MarkerLine Marker2 { get { return GetPar<MarkerLine>(null); } set { SetPar(value); } }
        public string Result { get { return GetPar(""); } set { SetPar(value); } }


        public virtual void SetResult(params object[] data)
        {
            if(CalculationType == CalculationType.Linear_regression)
            {
                Result = string.Format("{0}, {1}", data[0], data[1]);
            }
        }
    }
}



