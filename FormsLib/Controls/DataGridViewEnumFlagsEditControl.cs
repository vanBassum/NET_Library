using FormsLib.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace FormsLib.Controls
{
    public partial class TraceView
    {
        public class DataGridViewEnumFlagsEditControl : EnumFlagsCheckedCombobox, IDataGridViewEditingControl
        {

            public DataGridViewEnumFlagsEditControl()
            {
                OnCheckedChanged += MyEditControl_OnCheckedChanged;
            }

            private void MyEditControl_OnCheckedChanged(object sender, Enum e)
            {
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            }

            /*---------------------------------------------------------------------------------------------------*/
            /*---------------------------------------------------------------------------------------------------*/
            /*---------------------------------------------------------------------------------------------------*/


            DataGridView dataGridView;
            int rowIndex;
            bool valueChanged = false;
            public DataGridView EditingControlDataGridView { get => dataGridView; set => dataGridView = value; }
            //public object EditingControlFormattedValue { get => ""; set { } }
            public object EditingControlFormattedValue { get => Value; set => throw new NotImplementedException(); }
            public int EditingControlRowIndex { get => rowIndex; set => rowIndex = value; }
            public bool EditingControlValueChanged { get => valueChanged; set => valueChanged = value; }
            public System.Windows.Forms.Cursor EditingPanelCursor => base.Cursor;
            public bool RepositionEditingControlOnValueChange => false;

            public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
            {
                this.Font = dataGridViewCellStyle.Font;
            }

            public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                    default:
                        return !dataGridViewWantsInputKey;
                }
            }

            public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            {
                return EditingControlFormattedValue;
            }

            public void PrepareEditingControlForEdit(bool selectAll)
            {
            }
        }
    }
}
