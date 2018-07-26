using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network.ServerMessages
{
    public sealed class StorySceneResetMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -1L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class StorySceneObjectAddMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -2L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string objID;
        public CharacterView view;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WriteCharacterView(stream, view);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            view = InputStreamHelper.ReadCharacterView(stream);
        }
    }

    public sealed class StorySceneObjectRemoveMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -2L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
        }
    }

    public sealed class StorySceneObjectTransformMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -3L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public Layout to;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WriteLayout(stream, to);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            to = InputStreamHelper.ReadLayout(stream);
        }
    }

    public sealed class StorySceneObjectViewEffectMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -4L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public CharacterViewEffect effect;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WriteCharacterViewEffect(stream, effect);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            effect = InputStreamHelper.ReadCharacterViewEffect(stream);
        }
    }

    public sealed class StorySceneObjectPortraitStyleMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -5L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public PortraitStyle portrait;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            OutputStreamHelper.WritePortraitStyle(stream, portrait);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            portrait = InputStreamHelper.ReadPortraitStyle(stream);
        }
    }

    public sealed class StorySceneCameraTransformMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -6L;
        public override long MessageType => MESSAGE_TYPE;

        public Layout to;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteLayout(stream, to);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            to = InputStreamHelper.ReadLayout(stream);
        }
    }

    public sealed class StorySceneCameraEffectMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -7L;
        public override long MessageType => MESSAGE_TYPE;

        public CameraEffect effect;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteCameraEffect(stream, effect);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            effect = InputStreamHelper.ReadCameraEffect(stream);
        }
    }

    public sealed class PlayBGMMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -8L;
        public override long MessageType => MESSAGE_TYPE;

        public string bgmID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(bgmID.Length);
            stream.WriteString(bgmID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            bgmID = stream.ReadString(length);
        }
    }

    public sealed class StopBGMMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -9L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class PlaySEMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -10L;
        public override long MessageType => MESSAGE_TYPE;

        public string seID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(seID.Length);
            stream.WriteString(seID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            seID = stream.ReadString(length);
        }
    }
    
    public sealed class ShowSceneMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -11L;
        public override long MessageType => MESSAGE_TYPE;

        public int sceneType;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(sceneType);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            sceneType = stream.ReadInt32();
        }
    }

    public sealed class TextBoxAddParagraphMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -12L;
        public override long MessageType => MESSAGE_TYPE;

        public string text;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(text.Length);
            stream.WriteString(text);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            text = stream.ReadString(length);
        }
    }

    public sealed class TextBoxAddSelectionMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -13L;
        public override long MessageType => MESSAGE_TYPE;

        public string text;
        public int selectionCode;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(text.Length);
            stream.WriteString(text);
            stream.WriteInt32(selectionCode);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            text = stream.ReadString(length);
            selectionCode = stream.ReadInt32();
        }
    }

    public sealed class TextBoxClearMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -14L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class TextBoxSetPortraitMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -15L;
        public override long MessageType => MESSAGE_TYPE;

        public CharacterView view;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteCharacterView(stream, view);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            view = InputStreamHelper.ReadCharacterView(stream);
        }
    }

    public sealed class TextBoxPortraitStyleMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -16L;
        public override long MessageType => MESSAGE_TYPE;

        public PortraitStyle style;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WritePortraitStyle(stream, style);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            style = InputStreamHelper.ReadPortraitStyle(stream);
        }
    }

    public sealed class TextBoxPortraitEffectMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -17L;
        public override long MessageType => MESSAGE_TYPE;

        public CharacterViewEffect effect;

        public override void WriteTo(IDataOutputStream stream)
        {
            OutputStreamHelper.WriteCharacterViewEffect(stream, effect);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            effect = InputStreamHelper.ReadCharacterViewEffect(stream);
        }
    }

    public sealed class SkillCheckPanelShowMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -18L;
        public override long MessageType => MESSAGE_TYPE;

        public string initiativeCharacterID;
        public CharacterView initiativeView;
        public string passiveCharacterID;
        public CharacterView passiveView;

        public override void WriteTo(IDataOutputStream stream)
        {
            
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            throw new NotImplementedException();
        }
    }
    
    public sealed class SkillCheckPanelHideMessage : Message
    {
        public static readonly long MESSAGE_TYPE = -19L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream)
        {
            throw new NotImplementedException();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            throw new NotImplementedException();
        }
    }
}
