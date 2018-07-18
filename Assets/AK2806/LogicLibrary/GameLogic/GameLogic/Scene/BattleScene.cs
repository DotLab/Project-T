using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Scene
{
    public class BattleScene : IJSContextProvider
    {
        private sealed class API : IJSAPI
        {
            private readonly BattleScene _outer;

            public API(BattleScene outer)
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

        private readonly API _apiObj;

        private static readonly BattleScene _instance = new BattleScene();
        public static BattleScene Instance => _instance;

        public BattleScene()
        {
            _apiObj = new API(this);
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
