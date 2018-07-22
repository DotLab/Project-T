using GameLogic.CharacterSystem;
using GameLogic.Client;
using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core
{
    public sealed class Player : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI<Player>
        {
            private readonly Player _outer;

            public API(Player outer)
            {
                _outer = outer;
            }

            public IJSAPI<Character> getCharacter()
            {
                try
                {
                    return (IJSAPI<Character>)_outer.Character.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public int getIndex()
            {
                try
                {
                    return _outer.Index;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public Player Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;
        
        private readonly PlayerClient _client;
        private readonly Character _character;
        private readonly int _index;
        
        public PlayerClient Client => _client;
        public Character Character => _character;
        public int Index => _index;

        public Player(Character character, int index)
        {
            _apiObj = new API(this);
            _client = new PlayerClient();
            _character = character ?? throw new ArgumentNullException(nameof(character));
            _index = index;
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

    public sealed class DM
    {
        private readonly DMClient _client;
        
        public DMClient Client => _client;
        public int Index => 0;

        public DM()
        {
            _client = new DMClient();
        }
    }

    public sealed class User
    {
        private readonly bool _isDM;
        private readonly DM _dm;
        private readonly Player _player;
        private readonly string _id;
        private readonly string _name;
        
        public bool IsDM => _isDM;
        public DM DM => _dm;
        public Player Player => _player;
        public string Id => _id;
        public string Name => _name;
        
        public User(string id, string name, bool isDM, Player player, DM dm)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _isDM = isDM;
            if (isDM)
            {
                _player = null;
                _dm = dm ?? throw new ArgumentNullException(nameof(dm));
            }
            else
            {
                _player = player ?? throw new ArgumentNullException(nameof(player));
                _dm = null;
            }
        }

    }

}
