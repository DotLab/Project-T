using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Framework.ScriptSystem;
using GameLogic.Framework.ScriptSystem.Event;

namespace GameLogic.Character
{
    public interface IStunt : IIdentifiable, IProperty
    {
        List<ICommand> Commands { get; }
    }

    public class Stunt : IStunt
    {
        protected string _description;
        protected string _id;
        protected BaseCharacter _belong;
        protected List<ICommand> _commands;

        public string Description { get => _description; set => _description = value; }
        public string ID => _id
        public BaseCharacter Belong { get => _belong; set => _belong = value; }
        public List<ICommand> Commands => _commands;
    }
}
