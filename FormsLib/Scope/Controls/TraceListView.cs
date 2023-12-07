using FormsLib.Extentions;
using FormsLib.Scope.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static FormsLib.Controls.TraceView;

namespace FormsLib.Scope.Controls
{
    public partial class TraceListView : UserControl
    {
        //DataGridView dataGridView1 = new DataGridView();
        //private GraphController dataSource;
        //public GraphController DataSource
        //{
        //    get { return dataSource; }
        //    set
        //    {
        //        dataSource = value;
        //        if (dataSource != null)
        //            dataGridView1.DataSource = dataSource.Traces;
        //    }
        //}

        /*
        public TraceListView()
        {
            InitializeComponent();

            this.Controls.Add(dataGridView1);
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            foreach (var pi in typeof(GraphTrace).GetProperties().Where(p => p.GetCustomAttribute<TraceViewAttribute>() != null))
            {
                TraceViewAttribute? attr = pi.GetCustomAttribute<TraceViewAttribute>();
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
                col.HeaderText = attr.HeaderText == null ? pi.Name : attr.HeaderText;
                col.Width = attr.Width == 0 ? 100 : attr.Width;
            }

            dataGridView1.CellFormatting += dataGridView1_CellFormatting;
            dataGridView1.KeyDown += DataGridView1_KeyDown;
        }

        private void DataGridView1_KeyDown(object? sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void TraceView_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = sender as DataGridView;

            PropertyInfo pi = typeof(GraphTrace).GetProperty(dgv.Columns[e.ColumnIndex].DataPropertyName);
            if (pi == null)
                return;
            TraceViewAttribute attr = pi.GetCustomAttribute<TraceViewAttribute>();
            if (attr == null)
                return;

            if (pi.Name == nameof(GraphTrace.Color))
            {
                if(e.Value is Color color)
                    e.CellStyle.BackColor = color;
            }

            switch (e.Value)
            {
                case Pen p:
                    e.CellStyle.BackColor = p.Color;
                    //e.CellStyle.SelectionBackColor =  ControlPaint.Dark(p.Color); Doenst work?
                    break;
            }

            if (attr.HideValue)
            {
                e.CellStyle.ForeColor = Color.Transparent;
                e.CellStyle.SelectionForeColor = Color.Transparent;
            }
        }
        */
    }

   //public class TraceViewAttribute : Attribute
   //{
   //    public bool Show { get; set; } = true;
   //    public string HeaderText { get; set; } = null;
   //    public bool HideValue { get; set; } = false;
   //    public int Width { get; set; } = 0;
   //    public DataGridViewAutoSizeColumnMode AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
   //
   //}



    //class TraceComparer : IComparer<Trace>
    //{
    //    public int Compare(Trace? trace1, Trace? trace2)
    //    {
    //        if(trace1== null || trace2==null) 
    //            return 0;
    //
    //        int orderCompare = trace1.Order - trace2.Order;
    //        if (orderCompare != 0)
    //            return orderCompare;
    //
    //        int compareResult = System.String.Compare(trace1.Name, trace2.Name);
    //
    //        return compareResult;
    //    }
    //}
}
