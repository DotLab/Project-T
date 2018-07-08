using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.Core.ScriptSystem.Event;

namespace GameLogic.Character
{

    public class Stunt : IIdentifiable, IProperty
    {
        protected string _description;
        protected readonly string _id;
        protected ICharacter _belong;
        protected List<ICommand> _commands;

        public Stunt(string id)
        {
            this._id = id;
        }

        public string Description { get => _description; set => _description = value; }
        public string ID => _id;
        public ICharacter Belong { get => _belong; set => _belong = value; }
        public List<ICommand> Commands => _commands;
        
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
