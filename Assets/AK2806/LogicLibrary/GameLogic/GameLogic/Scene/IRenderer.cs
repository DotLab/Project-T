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

    public struct AtomicAction {
        public AtomicActionType actionType;
        public ArrayList parameters;
    }

    public interface IRenderer
    {
        void Render(List<AtomicAction> actions);
    }
}
