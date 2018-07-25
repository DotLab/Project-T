using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network.ServerMessages
{
    public sealed class StorySceneResetMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -1L;
        public override long MessageID => MESSAGE_ID;

    }

    public sealed class StorySceneObjectAddMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -2L;
        public override long MessageID => MESSAGE_ID;
        
        public string objID;
        public CharacterView view;
        
    }

    public sealed class StorySceneObjectRemoveMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -2L;
        public override long MessageID => MESSAGE_ID;

        public string objID;

    }

    public sealed class StorySceneObjectTransformMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -3L;
        public override long MessageID => MESSAGE_ID;

        public string objID;
        public Layout to;
    }

    public sealed class StorySceneObjectViewEffectMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -4L;
        public override long MessageID => MESSAGE_ID;

        public string objID;
        public CharacterViewEffect effect;

    }

    public sealed class StorySceneObjectPortraitStyleMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -5L;
        public override long MessageID => MESSAGE_ID;

        public string objID;
        public PortraitStyle portrait;
    }

    public sealed class StorySceneCameraTransformMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -6L;
        public override long MessageID => MESSAGE_ID;

        public Layout to;
    }

    public sealed class StorySceneCameraEffectMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -7L;
        public override long MessageID => MESSAGE_ID;

        public CameraEffect effect;

    }

    public sealed class PlayBGMMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -8L;
        public override long MessageID => MESSAGE_ID;

        public string bgmID;
    }

    public sealed class StopBGMMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -9L;
        public override long MessageID => MESSAGE_ID;
    }

    public sealed class PlaySEMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -10L;
        public override long MessageID => MESSAGE_ID;

        public string seID;
    }
    
    public sealed class ShowSceneMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -11L;
        public override long MessageID => MESSAGE_ID;

        public int sceneType;
    }

    public sealed class TextBoxAddParagraphMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -12L;
        public override long MessageID => MESSAGE_ID;

        public string text;
    }

    public sealed class TextBoxAddSelectionMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -13L;
        public override long MessageID => MESSAGE_ID;

        public string text;
        public int selectionCode;
    }

    public sealed class TextBoxClearMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -14L;
        public override long MessageID => MESSAGE_ID;
    }

    public sealed class TextBoxSetPortraitMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -15L;
        public override long MessageID => MESSAGE_ID;

        public CharacterView view;
    }

    public sealed class TextBoxPortraitStyleMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -16L;
        public override long MessageID => MESSAGE_ID;

        public PortraitStyle style;
    }

    public sealed class TextBoxPortraitEffectMessage : Streamable
    {
        public static readonly long MESSAGE_ID = -17L;
        public override long MessageID => MESSAGE_ID;

        public CharacterViewEffect effect;
    }


}
