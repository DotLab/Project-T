using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network.ClientMessages
{
    public sealed class StorySceneObjectActionMessage : Message
    {
        public enum PlayerAction : Byte
        {
            INTERACT,
            CREATE_ASPECT,
            ATTACK,
            HINDER
        }

        public static readonly long MESSAGE_TYPE = 0L;
        public override long MessageType => MESSAGE_TYPE;

        public string objectID;
        public PlayerAction action;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objectID.Length);
            stream.WriteString(objectID);
            stream.WriteByte((Byte)action);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objectID = stream.ReadString(length);
            action = (PlayerAction)stream.ReadByte();
        }
    }

    public sealed class TextSelectedMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 1L;
        public override long MessageType => MESSAGE_TYPE;

        public int selection;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(selection);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            selection = stream.ReadInt32();
        }
    }

    public sealed class SkillSelectedMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 2L;
        public override long MessageType => MESSAGE_TYPE;

        public string skillTypeID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(skillTypeID.Length);
            stream.WriteString(skillTypeID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            skillTypeID = stream.ReadString(length);
        }
    }

    public sealed class AspectsSelectedMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 3L;
        public override long MessageType => MESSAGE_TYPE;

        public class AspectGroup : Streamable
        {
            public string characterID;
            public string[] aspectsID;

            public override void WriteTo(IDataOutputStream stream)
            {
                stream.WriteInt32(characterID.Length);
                stream.WriteString(characterID);
                stream.WriteInt32(aspectsID.Length);
                foreach (string aspectID in aspectsID)
                {
                    stream.WriteInt32(aspectID.Length);
                    stream.WriteString(aspectID);
                }
            }

            public override void ReadFrom(IDataInputStream stream)
            {
                int length = stream.ReadInt32();
                characterID = stream.ReadString(length);
                length = stream.ReadInt32();
                aspectsID = new string[length];
                for (int i = 0; i < length; ++i)
                {
                    int length2 = stream.ReadInt32();
                    aspectsID[i] = stream.ReadString(length2);
                }
            }

        }

        public AspectGroup[] aspectGroups;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(aspectGroups.Length);
            foreach (AspectGroup aspectGroup in aspectGroups)
            {
                aspectGroup.WriteTo(stream);
            }
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            aspectGroups = new AspectGroup[length];
            for (int i = 0; i < length; ++i)
            {
                aspectGroups[i] = new AspectGroup();
                aspectGroups[i].ReadFrom(stream);
            }
        }
    }

    public sealed class StuntSelectedMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 4L;
        public override long MessageType => MESSAGE_TYPE;

        public string stuntID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(stuntID.Length);
            stream.WriteString(stuntID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            stuntID = stream.ReadString(length);
        }
    }

    public sealed class StorySceneNextActionMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 5L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class ClientInitMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 6L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class GetCharacterDataMessage : Message
    {
        public static readonly long MESSAGE_TYPE = 7L;
        public override long MessageType => MESSAGE_TYPE;

        public enum Action : Byte
        {
            SKILLS,
            ASPECTS,
            STUNTS,
            EXTRAS,
            CONSEQUENCES,

        }

        public override void ReadFrom(IDataInputStream stream)
        {

        }
        public override void WriteTo(IDataOutputStream stream) { }
    }
    
}
