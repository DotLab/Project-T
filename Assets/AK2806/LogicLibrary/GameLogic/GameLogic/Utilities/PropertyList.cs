using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Character;

namespace GameLogic.Utilities
{
    public sealed class PropertyList<T> where T:IProperty
    {
        private BaseCharacter _owner;
        private List<T> _container;
        
        public BaseCharacter Owner => _owner;
        public int Count => _container.Count;

        public T this[int i] { get => this._container[i]; set => this._container[i] = value; }
        
        public PropertyList(BaseCharacter owner)
        {
            this._owner = owner;
            this._container = new List<T>();
        }

        public void Add(T item)
        {
            this._container.Add(item);
            item.Belong = this.Owner;
        }

        public void Clear()
        {
            foreach (IProperty item in this._container)
            {
                item.Belong = null;
            }
            this._container.Clear();
        }

        public bool Contains(T item)
        {
            return this._container.Contains(item);
        }

        public void ForEach(Action<T> action)
        {
            this._container.ForEach(action);
        }

        public int IndexOf(T item, int index = 0, int count = -1)
        {
            if (count >= 0) return this._container.IndexOf(item, index, count);
            else return this._container.IndexOf(item, index);
        }

        public void Insert(int index, T item)
        {
            this._container.Insert(index, item);
            item.Belong = this.Owner;
        }

        public int LastIndexOf(T item, int index = 0, int count = -1)
        {
            if (count >= 0) return this._container.LastIndexOf(item, index, count);
            else return this._container.LastIndexOf(item, index);
        }

        public bool Remove(T item)
        {
            bool ret = this._container.Remove(item);
            if (ret) item.Belong = null;
            return ret;
        }

        public void RemoveAt(int index)
        {
            this._container[index].Belong = null;
            this._container.RemoveAt(index);
        }

        public void Reverse()
        {
            this._container.Reverse();
        }
        
        public T[] ToArray()
        {
            return this._container.ToArray();
        }
    }
}
