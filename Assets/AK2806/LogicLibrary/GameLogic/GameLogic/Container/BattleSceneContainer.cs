using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Container
{
    public class BattleSceneContainer : IJSContextProvider
    {
        private sealed class JSAPI : IJSAPI<BattleSceneContainer>
        {
            private readonly BattleSceneContainer _outer;

            public JSAPI(BattleSceneContainer outer)
            {
                _outer = outer;
            }
            
            public BattleSceneContainer Origin(JSContextHelper proof)
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

        private readonly JSAPI _apiObj;

        private static readonly BattleSceneContainer _instance = new BattleSceneContainer();
        public static BattleSceneContainer Instance => _instance;

        public BattleSceneContainer()
        {
            _apiObj = new JSAPI(this);
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}
