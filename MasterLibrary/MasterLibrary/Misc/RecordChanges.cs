using MasterLibrary.Misc;
using System.Collections.Generic;
using System.ComponentModel;

namespace MasterLibrary.Misc
{
    public abstract class RecordChanges : INotifyPropertyChanged
    {
        private object SendParLock = new object();
        private Dictionary<string, object> changedPars = new Dictionary<string, object>();

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public void StartRecordingChanges()
        {
            PropertyChanged += RecordChanges_PropertyChanged;
        }

        public void StopRecordingChanges()
        {
            PropertyChanged -= RecordChanges_PropertyChanged;
        }

        private void RecordChanges_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            changedPars[e.PropertyName] = this.GetType().GetProperty(e.PropertyName).GetValue(this);
        }

        public Dictionary<string, object> GetAndClearChangedPars()
        {
            Dictionary<string, object> cpy = new Dictionary<string, object>();
            lock (SendParLock)
            {
                foreach (KeyValuePair<string, object> kvp in changedPars)
                    cpy[kvp.Key] = kvp.Value;
                changedPars.Clear();
            }
            return cpy;
        }

        public bool HasChanges
        {
            get
            {
                using (var dictionaryEnum = changedPars.GetEnumerator())
                {
                    return dictionaryEnum.MoveNext();
                }
            }
        }
    }

}
