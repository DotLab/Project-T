using GameLogic.Core.DataSystem;
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

        public const long MESSAGE_TYPE = 0L;
        public override long MessageType => MESSAGE_TYPE;

        public string objID;
        public PlayerAction action;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(objID.Length);
            stream.WriteString(objID);
            stream.WriteByte((Byte)action);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            objID = stream.ReadString(length);
            action = (PlayerAction)stream.ReadByte();
        }
    }

    public sealed class TextSelectedMessage : Message
    {
        public const long MESSAGE_TYPE = 1L;
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
        public const long MESSAGE_TYPE = 2L;
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
    
    public sealed class AspectSelectedMessage : Message
    {
        public const long MESSAGE_TYPE = 3L;
        public override long MessageType => MESSAGE_TYPE;

        public string characterID;
        public string aspectID;
        public bool reroll;
        
        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(aspectID.Length);
            stream.WriteString(aspectID);
            stream.WriteBoolean(reroll);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            aspectID = stream.ReadString(length);
            reroll = stream.ReadBoolean();
        }
    }

    public sealed class StuntSelectedMessage : Message
    {
        public const long MESSAGE_TYPE = 4L;
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
        public const long MESSAGE_TYPE = 5L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class ClientInitMessage : Message
    {
        public const long MESSAGE_TYPE = 6L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class GetCharacterDataMessage : Message
    {
        public const long MESSAGE_TYPE = 7L;
        public override long MessageType => MESSAGE_TYPE;

        public enum DataType : Byte
        {
            INFO,
            SKILLS,
            ASPECTS,
            STUNTS,
            EXTRAS,
            CONSEQUENCES,
            STRESS,
            FATEPOINT,
            ALL
        }

        public string characterID;
        public DataType dataType;

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            dataType = (DataType)stream.ReadByte();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteByte((Byte)dataType);
        }
    }

    public sealed class GetAspectDataMessage : Message
    {
        public const long MESSAGE_TYPE = 8L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string aspectID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(aspectID.Length);
            stream.WriteString(aspectID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            aspectID = stream.ReadString(length);
        }
    }

    public sealed class GetConsequenceDataMessage : Message
    {
        public const long MESSAGE_TYPE = 9L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string consequenceID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(consequenceID.Length);
            stream.WriteString(consequenceID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            consequenceID = stream.ReadString(length);
        }
    }

    public sealed class GetSkillDataMessage : Message
    {
        public const long MESSAGE_TYPE = 10L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string skillTypeID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(skillTypeID.Length);
            stream.WriteString(skillTypeID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            skillTypeID = stream.ReadString(length);
        }
    }

    public sealed class GetStuntDataMessage : Message
    {
        public const long MESSAGE_TYPE = 11L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string stuntID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(stuntID.Length);
            stream.WriteString(stuntID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            stuntID = stream.ReadString(length);
        }
    }

    public sealed class GetExtraDataMessage : Message
    {
        public const long MESSAGE_TYPE = 12L;
        public override long MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string extraID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(characterID.Length);
            stream.WriteString(characterID);
            stream.WriteInt32(extraID.Length);
            stream.WriteString(extraID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int length = stream.ReadInt32();
            characterID = stream.ReadString(length);
            length = stream.ReadInt32();
            extraID = stream.ReadString(length);
        }
    }

    public sealed class DMCheckResultMessage : Message
    {
        public const long MESSAGE_TYPE = 13L;
        public override long MessageType => MESSAGE_TYPE;

        public bool result;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteBoolean(result);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            result = stream.ReadBoolean();
        }
    }
    
    public sealed class CancelSkillCheckMessage : Message
    {
        public const long MESSAGE_TYPE = 14L;
        public override long MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

}
