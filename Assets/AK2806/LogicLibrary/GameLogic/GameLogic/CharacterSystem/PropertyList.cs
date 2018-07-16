using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameLogic.CharacterSystem;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public sealed class ReadonlyPropertyList<T> : PropertyList<T> where T : class, IProperty
    {
        public ReadonlyPropertyList(Character owner, IEnumerable<T> properties) :
            base(owner)
        {
            if (properties != null)
            {
                _container.AddRange(properties);
            }
        }

        public override void Add(T item) { }
        public override void Clear() { }
        public override void Insert(int index, T item) { }
        public override bool Remove(T item) { return false; }
        public override void RemoveAt(int index) { }
        public override void Reverse() { }
    } 

    public class PropertyList<T> : IEnumerable<T>, IEnumerable, IJSContextProvider where T : class, IProperty
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly PropertyList<T> _outer;

            public API(PropertyList<T> outer)
            {
                _outer = outer;
            }

            public IJSAPI getOwner()
            {
                try
                {
                    return (IJSAPI)_outer.Owner.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public int getCount()
            {
                try
                {
                    return _outer.Count;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public IJSAPI get(int index)
            {
                try
                {
                    return (IJSAPI)_outer[index].GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void set(int index, IJSAPI item)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    _outer[index] = originItem;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void add(IJSAPI item)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    _outer.Add(originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void clear()
            {
                try
                {
                    _outer.Clear();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public bool contains(IJSAPI item)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.Contains(originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public void forEach(Action<T> action)
            {
                try
                {
                    _outer.ForEach(action);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int indexOf(IJSAPI item, int index = 0, int count = -1)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.IndexOf(originItem, index, count);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void insert(int index, IJSAPI item)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    _outer.Insert(index, originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int lastIndexOf(IJSAPI item, int index = 0, int count = -1)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.LastIndexOf(originItem, index, count);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public bool remove(IJSAPI item)
            {
                try
                {
                    T originItem = (T)JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.Remove(originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public void removeAt(int index)
            {
                try
                {
                    _outer.RemoveAt(index);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void reverse()
            {
                try
                {
                    _outer.Reverse();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSAPI[] toArray()
            {
                try
                {
                    T[] origins = _outer.ToArray();
                    IJSAPI[] ret = new IJSAPI[origins.Length];
                    for (int i = 0; i < ret.Length; ++i)
                    {
                        ret[i] = (IJSAPI)origins[i].GetContext();
                    }
                    return ret;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSContextProvider Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        protected readonly Character _owner;
        protected readonly List<T> _container;
        
        public Character Owner => _owner;
        public int Count => _container.Count;

        public T this[int i] { get => _container[i]; set => _container[i] = value; }
        
        public PropertyList(Character owner)
        {
            _apiObj = new API(this);
            _owner = owner;
            _container = new List<T>();
        }

        public virtual void Add(T item)
        {
            _container.Add(item);
            item.Belong = _owner;
        }

        public virtual void Clear()
        {
            foreach (IProperty item in _container)
            {
                item.Belong = null;
            }
            _container.Clear();
        }

        public bool Contains(T item)
        {
            return _container.Contains(item);
        }

        public void ForEach(Action<T> action)
        {
            _container.ForEach(action);
        }

        public int IndexOf(T item, int index = 0, int count = -1)
        {
            if (count >= 0) return _container.IndexOf(item, index, count);
            else return _container.IndexOf(item, index);
        }

        public virtual void Insert(int index, T item)
        {
            _container.Insert(index, item);
            item.Belong = _owner;
        }

        public int LastIndexOf(T item, int index = 0, int count = -1)
        {
            if (count >= 0) return _container.LastIndexOf(item, index, count);
            else return _container.LastIndexOf(item, index);
        }

        public virtual bool Remove(T item)
        {
            bool ret = _container.Remove(item);
            if (ret) item.Belong = null;
            return ret;
        }

        public virtual void RemoveAt(int index)
        {
            _container[index].Belong = null;
            _container.RemoveAt(index);
        }

        public virtual void Reverse()
        {
            _container.Reverse();
        }
        
        public T[] ToArray()
        {
            return _container.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)_container).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}
