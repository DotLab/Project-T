using System;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic.Utilities {
	public interface IReadonlyIdentifiedObjList<T> : IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> {
		T this[string id] { get; }
		bool Contains(string id);
		bool Contains(T item);
		bool TryGetValue(string id, out T value);
	}

	public sealed class IdentifiedObjList<T> : IEnumerable<T>, IEnumerable, IReadonlyIdentifiedObjList<T> where T : IIdentifiable {
		private Dictionary<string, T> _table;

		public int Count => _table.Count;

		public T this[string id] => _table[id];

		public IdentifiedObjList() {
			_table = new Dictionary<string, T>();
		}

		public IdentifiedObjList(IEnumerable<T> list) :
			this() {
			foreach (T e in list) {
				_table.Add(e.ID, e);
			}
		}

		public void Clear() {
			_table.Clear();
		}

		public void Add(T obj) {
			_table.Add(obj.ID, obj);
		}

		public bool Remove(T obj) {
			return _table.Remove(obj.ID);
		}

		public bool Remove(string id) {
			return _table.Remove(id);
		}

		public bool Contains(string id) {
			return _table.ContainsKey(id);
		}

		public bool Contains(T item) {
			return this.Contains(item.ID);
		}

		public void ForEach(Action<T> action) {
			foreach (T e in _table.Values) {
				action(e);
			}
		}

		public bool TryGetValue(string id, out T value) {
			return _table.TryGetValue(id, out value);
		}

		public T[] ToArray() {
			T[] ret = new T[_table.Count];
			_table.Values.CopyTo(ret, 0);
			return ret;
		}

		public IEnumerator<T> GetEnumerator() {
			return ((IEnumerable<T>)_table.Values).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}

	}
}
