using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Framework;

namespace GameLogic.Character
{
    interface IStunt : ICommand, IGroupable, IProperty
    {

    }

    class Stunt : Command, IStunt
    {
        private string description;
        private string id;
        private string group;
        private BaseCharacter belong;

        public string Description { get => description; set => description = value; }
        public string ID { get => id; set => id = value; }
        public string Group { get => group; set => group = value; }
        public BaseCharacter Belong { get => belong; set => belong = value; }
    }
}
