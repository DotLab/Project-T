using System;
using System.Collections;
using System.Collections.Generic;

namespace GameLogic.CharacterSystem
{
    public enum RenderActionType {
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
        public string target;
        public double value;
    }

    public struct RenderAction {
        public RenderActionType actionType;
        public ArrayList parameters;
    }

    public sealed class Viewable
    {
        public string battle;
        public string story;
    }
}
