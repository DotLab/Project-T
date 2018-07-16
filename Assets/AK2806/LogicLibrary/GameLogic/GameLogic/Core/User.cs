using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Client;

namespace GameLogic.Core
{
    public sealed class User : IJSContextProvider
    {
        private sealed class API : IJSAPI
        {
            private readonly User _outer;

            public API(User outer)
            {
                _outer = outer;
            }

            public IJSContextProvider Origin(JSContextHelper proof)
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

        private API _apiObj;

        private readonly GameClient _client;
        private readonly string _id;
        private readonly string _name;
        private readonly int _level;

        public GameClient Client => _client;
        public string Id => _id;
        public string Name => _name;
        public int Level => _level;

        public User(string id, string name, int level)
        {
            _apiObj = new API(this);
            _client = new GameClient();
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _level = level;
        }
        
        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}
