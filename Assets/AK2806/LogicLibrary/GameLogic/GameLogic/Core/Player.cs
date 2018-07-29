using GameLogic.CharacterSystem;
using GameLogic.Client;
using GameLogic.Core.Network;
using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core
{
    public sealed class Player : User
    {
        private readonly PlayerClient _client;
        private readonly List<Character> _characters;
        private readonly int _index;
        
        public PlayerClient Client => _client;
        public List<Character> Characters => _characters;
        public override int Index => _index;

        public Player(string id, string name, Connection connection, int index, IEnumerable<Character> characters) :
            base(id, name, false)
        {
            _index = index > 0 ? index : throw new ArgumentOutOfRangeException(nameof(index), "Player index is less than 1.");
            foreach (Character character in characters)
            {
                if (character == null) throw new ArgumentNullException(nameof(character));
            }
            _client = new PlayerClient(connection, this);
            _characters = new List<Character>(characters);
            foreach (Character character in characters)
            {
                character.Controller = this;
            }
        }
        
    }

    public sealed class DM : User
    {
        private readonly DMClient _client;
        
        public DMClient Client => _client;
        public override int Index => 0;

        public DM(string id, string name, Connection connection) :
            base(id, name, true)
        {
            _client = new DMClient(connection, this);
        }
    }

    public abstract class User
    {
        protected readonly bool _isDM;
        protected readonly string _id;
        protected readonly string _name;
        
        public bool IsDM => _isDM;
        public DM AsDM => (DM)this;
        public Player AsPlayer => (Player)this;
        public string Id => _id;
        public string Name => _name;
        public abstract int Index { get; }

        protected User(string id, string name, bool isDM)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _isDM = isDM;
        }

        public void UpdateClient()
        {
            if (_isDM)
            {
                this.AsDM.Client.Update();
            }
            else
            {
                this.AsPlayer.Client.Update();
            }
        }

    }

}
