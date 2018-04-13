using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Utilities
{
    public sealed class IDSearchTable<T> : IIdentifiable, IEnumerable<T>, IEnumerable where T : IIdentifiable
    {
        private readonly string _id;
        private Dictionary<string, T> _container;

        public string ID => _id;
        public int Count => _container.Count;

        public T this[string id] => this._container[id];

        public IDSearchTable(IEnumerable<T> list) : this("", list) { }

        public IDSearchTable(string id, IEnumerable<T> list)
        {
            this._id = id;
            foreach (T e in list)
            {
                this._container.Add(e.ID, e);
            }
        }

        public bool Contains(string id)
        {
            return this._container.ContainsKey(id);
        }

        public bool Contains(T item)
        {
            return this.Contains(item.ID);
        }

        public void ForEach(Action<T> action)
        {
            foreach (T e in this._container.Values)
            {
                action(e);
            }
        }
        
        public T[] ToArray()
        {
            T[] ret = new T[this._container.Count];
            this._container.Values.CopyTo(ret, 0);
            return ret;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this._container.Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
