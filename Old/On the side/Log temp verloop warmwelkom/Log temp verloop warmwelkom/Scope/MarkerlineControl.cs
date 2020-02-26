using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Oscilloscope
{
    public class MarkerlineControl : UserControl
    {
        DataGridView dataGridView = new DataGridView();

        public MarkerlineControl()
        {
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
            //this.dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //this.dataGridView.ColumnHeadersHeight = 50;
            //this.dataGridView.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            // this.dataGridView.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValidated);
            //this.dataGridView.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);

            //dataGridView.DataSource = scope.MarkerLines;

        }

        public void DoTheThing(BindingList<MarkerLine> markerLines)
        {
            dataGridView.Columns.Clear();

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                HeaderText = "",
                ReadOnly = true,
                Resizable = System.Windows.Forms.DataGridViewTriState.False,
                Width = 20,
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Timestamp",
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle {  WrapMode = DataGridViewTriState.True },
                Resizable = System.Windows.Forms.DataGridViewTriState.True,
                Width = 75,
            });


            for (int i = 0; i< markerLines.Count; i++)
            {
                if(i == 0)
                {
                    foreach (MarkerValue mv in markerLines[i].Values)
                    {
                        dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                        {
                            HeaderText = mv.Name,
                            ReadOnly = true,
                            Resizable = System.Windows.Forms.DataGridViewTriState.True,
                            Width = 50,
                            DefaultCellStyle = new DataGridViewCellStyle { BackColor = mv.Colour }
                        });
                    }
                }


                DataGridViewRow row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell { Value = markerLines[i].ID.ToString() });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = ScopeControl.ToHumanReadable(markerLines[i].X, 3, markerLines[i].IsDate) });
                row.MinimumHeight = 30;
                foreach (MarkerValue mv in markerLines[i].Values)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell { Value = mv.ToString() });
                }

                dataGridView.Rows.Add(row);

            }
        }


    }

    
    public class MarkerValue
    {
        public string Name { get; set; } = "";
        public string Value { get; set; } = "";

        public Color Colour = Color.Red;

        public override string ToString()
        {
            return Value;
        }
    }

}
