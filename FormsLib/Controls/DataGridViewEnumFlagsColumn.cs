using System;
using System.Windows.Forms;

namespace FormsLib.Controls
{
    public partial class TraceView
    {
        public class DataGridViewEnumFlagsColumn : DataGridViewColumn
        {
            
            private Array _dataSource;
            public Array DataSource
            {
                get { return _dataSource; }
                set
                {
                    _dataSource = value;
                }
            }
            
            public DataGridViewEnumFlagsColumn()
            {
                this.CellTemplate = new DataGridViewEnumFlagsCell();
            }

            public override DataGridViewCell CellTemplate
            {
                get
                {
                    return base.CellTemplate;
                }
                set
                {
                    // Ensure that the cell used for the template is a CalendarCell.
                    if (value != null &&
                        !value.GetType().IsAssignableFrom(typeof(DataGridViewEnumFlagsCell)))
                    {
                        throw new InvalidCastException("Must be a " + nameof(DataGridViewEnumFlagsCell));
                    }
                    base.CellTemplate = value;
                }
            }
        }
    }
}
