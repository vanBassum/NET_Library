using System;
using System.Drawing;
using System.Windows.Forms;

namespace FormsLib.Controls
{
    public partial class TraceView
    {
        public class DataGridViewEnumFlagsCell :  DataGridViewCell
        {
            public DataGridViewEnumFlagsCell()
            {
                
            }

            protected override void OnClick(DataGridViewCellEventArgs e)
            {
                
                this.DataGridView.CurrentCell = this;
                this.DataGridView.BeginEdit(true);
                base.OnClick(e);
            }

            public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
            {
                base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
                DataGridViewEnumFlagsEditControl ctl = DataGridView.EditingControl as DataGridViewEnumFlagsEditControl;
                ctl.Dock = DockStyle.Fill;
                ctl.Value = (Enum)this.Value;
            }


            public override Type EditType { get { return typeof(DataGridViewEnumFlagsEditControl); } }
            public override Type ValueType { get { return typeof(Enum); } }
            

            protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
            {
                using (
                    Brush gridBrush = new SolidBrush(Color.Gray),
                    backColorBrush = new SolidBrush(cellStyle.BackColor),
                    foreColorBrush = new SolidBrush(cellStyle.ForeColor))
                {
                    using (Pen gridLinePen = new Pen(gridBrush),
                        foreGroundPen = new Pen(cellStyle.ForeColor))
                    {
                        Rectangle size = cellBounds;

                        // Erase the cell.
                        g.FillRectangle(backColorBrush, size);

                        // Draw border.
                        //g.DrawRectangle(gridLinePen, size);
                        g.DrawLine(gridLinePen, size.Left, size.Bottom - 1, size.Right - 1, size.Bottom - 1);
                        g.DrawLine(gridLinePen, size.Right - 1, size.Top, size.Right - 1, size.Bottom);

                        g.DrawString(formattedValue.ToString(), cellStyle.Font, foreColorBrush, size);
                    }
                }
            }
        }
    }
}
