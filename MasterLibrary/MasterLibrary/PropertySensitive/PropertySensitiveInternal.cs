using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MasterLibrary.PropertySensitive
{

    public class PropertySensitiveInternal : RecordChanges, INotifyPropertyChanged
    {
        public override event PropertyChangedEventHandler PropertyChanged;

        protected bool SetPar<T>(T obj, T value, [CallerMemberName] string propertyName = null)
        {
            obj = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        protected T GetPar<T>(T obj, [CallerMemberName] string propertyName = null)
        {
            return obj;
        }

    }
}
