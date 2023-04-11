using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsLib.Controls
{
    public partial class CollectionEditControl : UserControl
    {
        public object SelectedObject { get; private set; }
        public event EventHandler<object> SelectedItemChanged;
        public event EventHandler<object> ObjectChanged;
        bool objectIsNew = false;
        object? editObject = null;
        IBindingList? _datasource;
        public Func<object> CreateObject { get; set; }
        public IBindingList? DataSource { get { return _datasource; } set { _datasource = value; listBox1.DataSource = _datasource; } }
        public string DisplayMember { get { return listBox1.DisplayMember; } set { listBox1.DisplayMember = value; } }



        public CollectionEditControl()
        {
            InitializeComponent();
        }

        private void CollectionEditControl_Load(object sender, EventArgs e)
        {
            StopEditMode();

        }


        object CreateNewObject()
        {
            if (CreateObject == null)
            {
                if (DataSource == null)
                {
                    return null;
                }
                else
                {
                    Type type = DataSource.GetType().GetGenericArguments()[0];
                    return Activator.CreateInstance(type);
                }
            }
            else
            {
                return CreateObject();
            }
        }


        private void btn_New_Click(object sender, EventArgs e)
        {
            objectIsNew = true;
            editObject = CreateNewObject();
            propertyGrid1.SelectedObject = editObject;
            StartEditMode();
        }

        private void btn_Edit_Click(object sender, EventArgs e)
        {
            objectIsNew = false;
            editObject = listBox1.SelectedItem;
            propertyGrid1.SelectedObject = editObject;
            StartEditMode();
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            DataSource.Remove(propertyGrid1.SelectedObject);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (objectIsNew)
                DataSource.Add(editObject);
            StopEditMode();
            ObjectChanged?.Invoke(this, editObject);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            while (changes.Any())
            {
                PropertyValueChangedEventArgs change = changes.Pop();
                change.ChangedItem.PropertyDescriptor.SetValue(editObject, change.OldValue);
            }
            StopEditMode();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                propertyGrid1.SelectedObject = listBox1.SelectedItem;
                SelectedObject = listBox1.SelectedItem;
                SelectedItemChanged?.Invoke(this, listBox1.SelectedItem);
            }
        }



        private void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            changes.Push(e);
        }

        void StartEditMode()
        {
            splitContainer1.Panel1.Enabled = false;
            splitContainer1.Panel2.Enabled = true;
            changes.Clear();
        }

        void StopEditMode()
        {
            splitContainer1.Panel1.Enabled = true;
            splitContainer1.Panel2.Enabled = false;
            listBox1.SelectedItem = editObject;
            propertyGrid1.Refresh();
            listBox1.Refresh();

        }

        Stack<PropertyValueChangedEventArgs> changes = new Stack<PropertyValueChangedEventArgs>();
    }
}
