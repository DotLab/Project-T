using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core
{
    public sealed class IdentifiedObjList<T> : IEnumerable<T>, IEnumerable where T : IIdentifiable
    {
        private Dictionary<string, T> _table;
        
        public int Count => _table.Count;

        public T this[string id] => this._table[id];
        
        public IdentifiedObjList()
        {
            this._table = new Dictionary<string, T>();
        }

        public IdentifiedObjList(IEnumerable<T> list) :
            this()
        {
            foreach (T e in list)
            {
                this._table.Add(e.ID, e);
            }
        }

        public void Clear()
        {
            this._table.Clear();
        }

        public void Add(T obj)
        {
            this._table.Add(obj.ID, obj);
        }
        
        public bool Remove(T obj)
        {
            return this._table.Remove(obj.ID);
        }

        public bool Remove(string id)
        {
            return this._table.Remove(id);
        }
        
        public bool Contains(string id)
        {
            return this._table.ContainsKey(id);
        }

        public bool Contains(T item)
        {
            return this.Contains(item.ID);
        }

        public void ForEach(Action<T> action)
        {
            foreach (T e in this._table.Values)
            {
                action(e);
            }
        }

        public T[] ToArray()
        {
            T[] ret = new T[this._table.Count];
            this._table.Values.CopyTo(ret, 0);
            return ret;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this._table.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

    }
}
