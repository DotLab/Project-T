using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GameLogic.CharacterSystem;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.CharacterSystem
{
    public interface IProperty : IDescribable, IAttachable<Character> { }

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

    public class PropertyList<T> : AttachableList<Character, T> where T : class, IProperty
    {
        public PropertyList(Character owner) :
            base(owner)
        {
        }
    }
}
