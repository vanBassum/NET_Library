using System.Collections;
using System.Collections.Generic;

namespace FormsLib.Misc
{
    /// <summary>
    /// https://stackoverflow.com/questions/10966331/two-way-bidirectional-dictionary-in-c/10966684
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

        public Map()
        {
            Forward = new Indexer<T1, T2>(_forward);
            Reverse = new Indexer<T2, T1>(_reverse);
        }

        public class Indexer<T3, T4>
        {
            private Dictionary<T3, T4> _dictionary;
            public Indexer(Dictionary<T3, T4> dictionary)
            {
                _dictionary = dictionary;
            }
            public T4 this[T3 index]
            {
                get { return _dictionary[index]; }
                set { _dictionary[index] = value; }
            }
        }

        public void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _reverse.Add(t2, t1);
        }

        public Indexer<T1, T2> Forward { get; private set; }
        public Indexer<T2, T1> Reverse { get; private set; }

        public bool TryForward(T1 t1, out T2 t2)
        {
            if (_forward.ContainsKey(t1))
            {
                t2 = Forward[t1];
                return true;
            }
            t2 = default;
            return false;
        }

        public bool TryReverse(T2 t2, out T1 t1)
        {
            if (_reverse.ContainsKey(t2))
            {
                t1 = Reverse[t2];
                return true;
            }
            t1 = default;
            return false;
        }



        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return _forward.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}