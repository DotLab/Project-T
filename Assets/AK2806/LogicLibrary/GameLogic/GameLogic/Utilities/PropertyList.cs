using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Character;

namespace GameLogic.Utilities
{
    public class PropertyList<T> where T:IProperty
    {
        private BaseCharacter owner;
        private List<T> container;
        
        public BaseCharacter Owner => owner;
        public int Count => container.Count;

        public T this[int i] { get => this.container[i]; set => this.container[i] = value; }
        
        public PropertyList(BaseCharacter owner)
        {
            this.owner = owner;
            this.container = new List<T>();
        }

        public void Add(T item)
        {
            this.container.Add(item);
            item.Belong = this.Owner;
        }

        public void Clear()
        {
            foreach (IProperty item in this.container)
            {
                item.Belong = null;
            }
            this.container.Clear();
        }

        public bool Contains(T item)
        {
            return this.container.Contains(item);
        }

        public void ForEach(Action<T> action)
        {
            this.container.ForEach(action);
        }

        public int IndexOf(T item, int index = 0, int count = -1)
        {
            if (count >= 0) return this.container.IndexOf(item, index, count);
            else return this.container.IndexOf(item, index);
        }

        public void Insert(int index, T item)
        {
            this.container.Insert(index, item);
            item.Belong = this.Owner;
        }

        public int LastIndexOf(T item, int index = 0, int count = -1)
        {
            if (count >= 0) return this.container.LastIndexOf(item, index, count);
            else return this.container.LastIndexOf(item, index);
        }

        public bool Remove(T item)
        {
            bool ret = this.container.Remove(item);
            if (ret) item.Belong = null;
            return ret;
        }

        public void RemoveAt(int index)
        {
            this.container[index].Belong = null;
            this.container.RemoveAt(index);
        }

        public void Reverse()
        {
            this.container.Reverse();
        }
        
        public T[] ToArray()
        {
            return this.container.ToArray();
        }
    }
}
