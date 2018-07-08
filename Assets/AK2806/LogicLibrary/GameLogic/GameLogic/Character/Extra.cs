using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem.Event;

namespace GameLogic.Character
{

    public class Extra : IProperty, IIdentifiable
    {
        protected PropertyList<ICharacter> _items;
        protected List<Trigger> _triggers;
        protected readonly string _id;
        protected string _description;
        protected ICharacter _belong;

        public Extra(string id)
        {
            this._id = id;
        }

        public string ID => _id;
        public string Description { get => _description; set => _description = value; }
        public PropertyList<ICharacter> Items => _items;
        public List<Trigger> Triggers => _triggers;
        public ICharacter Belong { get => _belong; set => _belong = value; }

        public virtual object GetContext()
        {
            throw new NotImplementedException();
        }

        public virtual void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }
}
