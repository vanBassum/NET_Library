using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Windows.Forms;

namespace ClassLibrary3.Palettes
{
    public interface IPalette
    {
        Color this[int index] { get; set; }
    }
}
