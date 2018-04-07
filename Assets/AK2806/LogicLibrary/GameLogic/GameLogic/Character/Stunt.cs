using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;
using GameLogic.Framework.ScriptSystem;
using GameLogic.Framework.ScriptSystem.Event;

namespace GameLogic.Character
{
    public interface IStunt : IGroupable, IProperty
    {
        List<ICommand> Commands { get; }
        List<ITrigger> Triggers { get; }
    }

    public class Stunt : IStunt
    {
        protected string _description;
        protected string _id;
        protected string _group;
        protected BaseCharacter _belong;
        protected List<ICommand> _commands;
        protected List<ITrigger> _triggers;

        public string Description { get => _description; set => _description = value; }
        public string ID { get => _id; set => _id = value; }
        public string Group { get => _group; set => _group = value; }
        public BaseCharacter Belong { get => _belong; set => _belong = value; }
        public List<ICommand> Commands { get => _commands; set => _commands = value; }
        public List<ITrigger> Triggers { get => _triggers; set => _triggers = value; }
    }
}
