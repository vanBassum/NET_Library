using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace CoreLib.Misc
{

    public static class BindingList_EXT
    {
        public static IEnumerable<T> Where<T>(this IBindingList list, Predicate<T> predicate)
        {
            foreach(var item in list)
            {
                if (item is T t)
                {
                    if (predicate(t))
                        yield return t;
                }
            }
        }
    }



    public class FilteredBindingList<T> : IBindingList
    {
        IBindingList _DataSource;

        public event ListChangedEventHandler ListChanged;

        Predicate<T> _Filter { get; set; } = (a) => true;
        public Predicate<T> Filter
        {
            get => _Filter;
            set
            {
                _Filter = value;
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        public IBindingList DataSource
        {
            get => _DataSource;
            set
            {
                if (_DataSource != null)
                    _DataSource.ListChanged -= _DataSource_ListChanged;
                _DataSource = value;
                _DataSource.ListChanged += _DataSource_ListChanged;
                ListChanged?.Invoke(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        private void _DataSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            ListChanged?.Invoke(this, e);
        }

        public FilteredBindingList()
        {
        }

        public FilteredBindingList(BindingList<T> dataSource)
        {
            DataSource = dataSource;
        }

        public bool AllowEdit => throw new NotImplementedException();

        public bool AllowNew => throw new NotImplementedException();

        public bool AllowRemove => throw new NotImplementedException();

        public bool IsSorted => throw new NotImplementedException();

        public ListSortDirection SortDirection => throw new NotImplementedException();

        public PropertyDescriptor SortProperty => throw new NotImplementedException();

        public bool SupportsChangeNotification => true;

        public bool SupportsSearching => throw new NotImplementedException();

        public bool SupportsSorting => throw new NotImplementedException();

        public bool IsFixedSize => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

        public int Count => DataSource.Where<T>(a=>Filter(a)).Count();

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public object this[int index] 
        { 
            get
            {
                var v = DataSource.Where<T>(a => Filter(a)).ToArray();
                return v[index];
            }
            set
            {
                var v = DataSource.Where<T>(a => Filter(a)).ToArray();
                if(value is T t)
                    v[index] = t;
            }
        }





        public void AddIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public object AddNew()
        {
            throw new NotImplementedException();
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }

        public int Add(object value)
        {
            return DataSource.Add(value);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object value)
        {
            DataSource.Remove(value);
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            return DataSource.Where<T>(a => Filter(a)).GetEnumerator();
        }
    }



}