using FormsLib.Scope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BasLibrary.Scope.Controls
{
    public class StyleView : UserControl
    {
        private Style? dataSource;
        public Style? DataSource { get => dataSource; set { dataSource = value; propertyGrid.SelectedObject = value; } }

        private PropertyGrid propertyGrid;

        public StyleView()
        {
            propertyGrid = new PropertyGrid()
            { 
                Dock = DockStyle.Fill
            };
            this.Controls.Add(propertyGrid);
        }
    }
}
