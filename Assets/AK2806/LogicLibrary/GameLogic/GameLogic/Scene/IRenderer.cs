using System;
using System.Collections;
using System.Collections.Generic;
namespace GameLogic.Scene
{
    public enum AtomicActionType {
        PutViewable, EditViewable, DeleteViewable,
        Tween, Cutscene,
        PlayBGM, StopBGM, PlaySE,
        ShowText, Pause, RunBattle
    }

    public class TweenParameter {
        public struct Tween {
            public struct Effect {
                public string target;
                public string function;
                public long time;
                public Object param;
            }

            public int layer;
            public List<Effect> effect;
        }

        public List<Tween> tween;
    }

    public class RenderAttribute {
        
    }

    public struct AtomicAction {
        public AtomicActionType actionType;
        public ArrayList parameters;
    }

    public interface IRenderer
    {
        void Render(List<AtomicAction> actions);
    }
}
