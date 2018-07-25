using GameLogic.Core.ScriptSystem;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic.Core
{
    public interface IAttachable<T> : IJSContextProvider where T : class, IJSContextProvider
    {
        T Belong { get; set; }
    }
    
    public class AttachableList<TOwner, TItem> : IEnumerable<TItem>, IEnumerable, IJSContextProvider
        where TOwner : class, IJSContextProvider
        where TItem : class, IAttachable<TOwner>
    {
        #region Javascript API class
        protected class API : IJSAPI<AttachableList<TOwner, TItem>>
        {
            private readonly AttachableList<TOwner, TItem> _outer;

            public API(AttachableList<TOwner, TItem> outer)
            {
                _outer = outer;
            }

            public IJSAPI<TOwner> getOwner()
            {
                try
                {
                    return (IJSAPI<TOwner>)_outer.Owner.GetContext();
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

            public IJSAPI<TItem> get(int index)
            {
                try
                {
                    return (IJSAPI<TItem>)_outer[index].GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void set(int index, IJSAPI<TItem> item)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
                    _outer[index] = originItem;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void add(IJSAPI<TItem> item)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
                    _outer.Add(originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void addRange(IJSAPI<TItem>[] items)
            {
                try
                {
                    TItem[] originItems = new TItem[items.Length];
                    for (int i = 0; i < items.Length; ++i)
                    {
                        originItems[i] = JSContextHelper.Instance.GetAPIOrigin(items[i]);
                    }
                    _outer.AddRange(originItems);
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

            public bool contains(IJSAPI<TItem> item)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.Contains(originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public void forEach(Action<TItem> action)
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

            public int indexOf(IJSAPI<TItem> item, int index = 0, int count = -1)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.IndexOf(originItem, index, count);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void insert(int index, IJSAPI<TItem> item)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
                    _outer.Insert(index, originItem);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int lastIndexOf(IJSAPI<TItem> item, int index = 0, int count = -1)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
                    return _outer.LastIndexOf(originItem, index, count);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public bool remove(IJSAPI<TItem> item)
            {
                try
                {
                    TItem originItem = JSContextHelper.Instance.GetAPIOrigin(item);
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

            public IJSAPI<TItem>[] toArray()
            {
                try
                {
                    TItem[] origins = _outer.ToArray();
                    IJSAPI<TItem>[] ret = new IJSAPI<TItem>[origins.Length];
                    for (int i = 0; i < ret.Length; ++i)
                    {
                        ret[i] = (IJSAPI<TItem>)origins[i].GetContext();
                    }
                    return ret;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public AttachableList<TOwner, TItem> Origin(JSContextHelper proof)
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

        protected readonly TOwner _owner;
        protected readonly List<TItem> _container;
        
        public TOwner Owner => _owner;
        public int Count => _container.Count;

        public TItem this[int i]
        {
            get => _container[i];
            set
            {
                if (value.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(value));
                _container[i].Belong = null;
                _container[i] = value;
                value.Belong = _owner;
            }
        }
        
        public AttachableList(TOwner owner)
        {
            _apiObj = new API(this);
            _owner = owner;
            _container = new List<TItem>();
        }
        
        public virtual void Add(TItem item)
        {
            if (item.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(item));
            _container.Add(item);
            item.Belong = _owner;
        }

        public virtual void AddRange(IEnumerable<TItem> items)
        {
            foreach (TItem item in items)
            {
                if (item.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(item));
            }
            _container.AddRange(items);
            foreach (TItem item in items)
            {
                item.Belong = _owner;
            }
        }
        
        public virtual void Clear()
        {
            foreach (TItem item in _container)
            {
                item.Belong = null;
            }
            _container.Clear();
        }

        public bool Contains(TItem item)
        {
            return _container.Contains(item);
        }

        public void ForEach(Action<TItem> action)
        {
            _container.ForEach(action);
        }

        public int IndexOf(TItem item, int index = 0, int count = -1)
        {
            if (count >= 0) return _container.IndexOf(item, index, count);
            else return _container.IndexOf(item, index);
        }

        public virtual void Insert(int index, TItem item)
        {
            if (item.Belong != null) throw new ArgumentException("This item has already been bound.", nameof(item));
            _container.Insert(index, item);
            item.Belong = _owner;
        }

        public int LastIndexOf(TItem item, int index = 0, int count = -1)
        {
            if (count >= 0) return _container.LastIndexOf(item, index, count);
            else return _container.LastIndexOf(item, index);
        }

        public virtual bool Remove(TItem item)
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
        
        public TItem[] ToArray()
        {
            return _container.ToArray();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return ((IEnumerable<TItem>)_container).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
        
    }
}
