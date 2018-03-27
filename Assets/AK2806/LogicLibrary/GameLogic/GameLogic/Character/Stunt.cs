using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    public interface IStunt : IGroupable, IProperty
    {
        List<ICommand> Commands { get; }
        List<ITrigger> Triggers { get; }
    }

    public class Stunt : IStunt
    {
        private string description;
        private string id;
        private string group;
        private BaseCharacter belong;
        private List<ICommand> commands;
        private List<ITrigger> triggers;

        public string Description { get => description; set => description = value; }
        public string ID { get => id; set => id = value; }
        public string Group { get => group; set => group = value; }
        public BaseCharacter Belong { get => belong; set => belong = value; }
        public List<ICommand> Commands { get => commands; set => commands = value; }
        public List<ITrigger> Triggers { get => triggers; set => triggers = value; }
    }
}
