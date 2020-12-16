using System;
using System.Windows.Forms;

namespace FRMLib.Scope.Controls
{
    public partial class TraceView
    {
        public class MyColumn : DataGridViewColumn
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
            
            public MyColumn()
            {
                this.CellTemplate = new MyCell();
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
                        !value.GetType().IsAssignableFrom(typeof(MyCell)))
                    {
                        throw new InvalidCastException("Must be a " + nameof(MyCell));
                    }
                    base.CellTemplate = value;
                }
            }
        }
    }
}
