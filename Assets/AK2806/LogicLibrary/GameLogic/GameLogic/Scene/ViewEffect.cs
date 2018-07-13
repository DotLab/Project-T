using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameLogic.Scene
{
    public enum AnimateType
    {
        None, Shake
    }

    public struct ViewEffect
    {
        public static readonly ViewEffect INIT = new ViewEffect(new Vector4(1, 1, 1, 1), AnimateType.None);

        public Vector4 tint;
        public AnimateType animation;

        public ViewEffect(Vector4 tint, AnimateType animation)
        {
            this.tint = tint;
            this.animation = animation;
        }
    }

    public struct Style
    {
        public static readonly Style INIT = new Style(0, 0);

        public int action;
        public int emotion;

        public Style(int action, int emotion)
        {
            this.action = action;
            this.emotion = emotion;
        }
    }
}
