using GameLogic.CharacterSystem;
using GameLogic.Client;
using GameLogic.Core.Network;
using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core
{
    public sealed class Player
    {
        private readonly PlayerClient _client;
        private readonly User _user;
        private readonly List<Character> _characters;
        private readonly int _index;
        
        public PlayerClient Client => _client;
        public User User => _user;
        public List<Character> Characters => _characters;
        public int Index => _index;
        
        public Player(User user, Connection connection, int index)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _index = index > 0 ? index : throw new ArgumentOutOfRangeException(nameof(index), "Player index is less than 1.");
            _client = new PlayerClient(connection, user);
            _characters = new List<Character>();
        }
        
    }

    public sealed class DM
    {
        private readonly DMClient _client;
        private readonly User _user;
        
        public DMClient Client => _client;
        public User User => _user;

        public DM(User user, Connection connection)
        {
            _user = user ?? throw new ArgumentNullException(nameof(user));
            _client = new DMClient(connection, user);
        }
    }

    public sealed class User
    {
        public static User CreatePlayer(string id, string name, Connection connection, int index, IEnumerable<Character> characters)
        {
            foreach (Character character in characters)
            {
                if (character == null) throw new ArgumentNullException(nameof(character));
            }
            User ret = new User(id, name, false, connection, index);
            ret.AsPlayer.Characters.AddRange(characters);
            return ret;
        }

        public static User CreateDM(string id, string name, Connection connection)
        {
            return new User(id, name, true, connection, 0);
        }

        private readonly bool _isDM;
        private readonly DM _dm;
        private readonly Player _player;
        private readonly string _id;
        private readonly string _name;
        
        public bool IsDM => _isDM;
        public DM AsDM => _dm;
        public Player AsPlayer => _player;
        public string Id => _id;
        public string Name => _name;

        private User(string id, string name, bool isDM, Connection connection, int index)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _isDM = isDM;
            if (isDM)
            {
                _player = null;
                _dm = new DM(this, connection);
            }
            else
            {
                _player = new Player(this, connection, index);
                _dm = null;
            }
        }

        public void UpdateClient()
        {
            if (_isDM)
            {
                _dm.Client.Update();
            }
            else
            {
                _player.Client.Update();
            }
        }

    }

}
