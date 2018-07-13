using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.ScriptSystem;

namespace GameLogic.Core.DataSystem
{
    public sealed class GameObjectSearcher : IJSContextProvider
    {
        private class API
        {
            private GameObjectSearcher _outer;

            public API(GameObjectSearcher outer)
            {
                _outer = outer;
            }


        }

        private API _apiObj;

        private static GameObjectSearcher _instance = new GameObjectSearcher();
        public static GameObjectSearcher Instance => _instance;
        

        private GameObjectSearcher()
        {
            _apiObj = new API(this);
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context)
        {
            
        }
    }
}
