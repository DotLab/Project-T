using GameLogic.Core.DataSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network.ClientMessages
{
    public sealed class ClientInitMessage : Message
    {
        public const int MESSAGE_TYPE = 1;
        public override int MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class StorySceneObjectActionMessage : Message
    {
        public enum PlayerAction : byte
        {
            INTERACT,
            CREATE_ASPECT,
            ATTACK,
            HINDER
        }

        public const int MESSAGE_TYPE = 2;
        public override int MessageType => MESSAGE_TYPE;

        public string objID;
        public PlayerAction action;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(objID);
            stream.WriteByte((byte)action);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            objID = stream.ReadString();
            action = (PlayerAction)stream.ReadByte();
        }
    }

    public sealed class TextSelectedMessage : Message
    {
        public const int MESSAGE_TYPE = 3;
        public override int MessageType => MESSAGE_TYPE;

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

    public sealed class StorySceneNextActionMessage : Message
    {
        public const int MESSAGE_TYPE = 4;
        public override int MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class CheckerSkillSelectedMessage : Message
    {
        public const int MESSAGE_TYPE = 5;
        public override int MessageType => MESSAGE_TYPE;

        public string skillTypeID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(skillTypeID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            skillTypeID = stream.ReadString();
        }
    }
    
    public sealed class CheckerAspectSelectedMessage : Message
    {
        public const int MESSAGE_TYPE = 6;
        public override int MessageType => MESSAGE_TYPE;

        public string characterID;
        public string aspectID;
        public bool reroll;
        
        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteString(aspectID);
            stream.WriteBoolean(reroll);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            aspectID = stream.ReadString();
            reroll = stream.ReadBoolean();
        }
    }

    public sealed class CheckerStuntSelectedMessage : Message
    {
        public const int MESSAGE_TYPE = 7;
        public override int MessageType => MESSAGE_TYPE;

        public string stuntID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(stuntID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            stuntID = stream.ReadString();
        }
    }
    
    public sealed class GetCharacterDataMessage : Message
    {
        public const int MESSAGE_TYPE = 8;
        public override int MessageType => MESSAGE_TYPE;

        public enum DataType : byte
        {
            INFO,
            SKILLS,
            ASPECTS,
            STUNTS,
            EXTRAS,
            CONSEQUENCES,
            STRESS,
            FATEPOINT
        }

        public string characterID;
        public DataType dataType;

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            dataType = (DataType)stream.ReadByte();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteByte((byte)dataType);
        }
    }

    public sealed class GetAspectDataMessage : Message
    {
        public const int MESSAGE_TYPE = 9;
        public override int MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string aspectID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteString(aspectID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            aspectID = stream.ReadString();
        }
    }

    public sealed class GetConsequenceDataMessage : Message
    {
        public const int MESSAGE_TYPE = 10;
        public override int MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string consequenceID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteString(consequenceID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            consequenceID = stream.ReadString();
        }
    }

    public sealed class GetSkillDataMessage : Message
    {
        public const int MESSAGE_TYPE = 11;
        public override int MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string skillTypeID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteString(skillTypeID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            skillTypeID = stream.ReadString();
        }
    }

    public sealed class GetStuntDataMessage : Message
    {
        public const int MESSAGE_TYPE = 12;
        public override int MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string stuntID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteString(stuntID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            stuntID = stream.ReadString();
        }
    }

    public sealed class GetExtraDataMessage : Message
    {
        public const int MESSAGE_TYPE = 13;
        public override int MessageType => MESSAGE_TYPE;
        
        public string characterID;
        public string extraID;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(characterID);
            stream.WriteString(extraID);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            characterID = stream.ReadString();
            extraID = stream.ReadString();
        }
    }

    public sealed class GetDirectResistSkillsMessage : Message
    {
        public const int MESSAGE_TYPE = 14;
        public override int MessageType => MESSAGE_TYPE;

        public string initiativeSkillTypeID;
        public int actionType;

        public override void ReadFrom(IDataInputStream stream)
        {
            initiativeSkillTypeID = stream.ReadString();
            actionType = stream.ReadInt32();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(initiativeSkillTypeID);
            stream.WriteInt32(actionType);
        }
    }

    public sealed class GetSkillTypeListMessage : Message
    {
        public const int MESSAGE_TYPE = 15;
        public override int MessageType => MESSAGE_TYPE;

        public override void ReadFrom(IDataInputStream stream) { }
        public override void WriteTo(IDataOutputStream stream) { }
    }

    public sealed class DMCheckResultMessage : Message
    {
        public const int MESSAGE_TYPE = 16;
        public override int MessageType => MESSAGE_TYPE;

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
    
    public sealed class BattleSceneSetSkipSelectAspectMessage : Message
    {
        public const int MESSAGE_TYPE = 17;
        public override int MessageType => MESSAGE_TYPE;

        public bool val;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteBoolean(val);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            val = stream.ReadBoolean();
        }
    }

    public sealed class SelectAspectOverMessage : Message
    {
        public const int MESSAGE_TYPE = 18;
        public override int MessageType => MESSAGE_TYPE;

        public override void WriteTo(IDataOutputStream stream) { }
        public override void ReadFrom(IDataInputStream stream) { }
    }

    public sealed class BattleSceneGetActableObjectMovePathInfoMessage : Message
    {
        public const int MESSAGE_TYPE = 19;
        public override int MessageType => MESSAGE_TYPE;

        public override void WriteTo(IDataOutputStream stream) { }
        public override void ReadFrom(IDataInputStream stream) { }
    }

    public sealed class BattleSceneActableObjectMoveMessage : Message
    {
        public const int MESSAGE_TYPE = 20;
        public override int MessageType => MESSAGE_TYPE;
        
        public int dstRow;
        public int dstCol;
        public bool dstHighland;

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(dstRow);
            stream.WriteInt32(dstCol);
            stream.WriteBoolean(dstHighland);
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            dstRow = stream.ReadInt32();
            dstCol = stream.ReadInt32();
            dstHighland = stream.ReadBoolean();
        }
    }
    
    public sealed class BattleSceneActableObjectDoActionMessage : Message
    {
        public const int MESSAGE_TYPE = 21;
        public override int MessageType => MESSAGE_TYPE;

        public string skillTypeOrStuntID;
        public bool isStunt;
        public int action;
        public int dstRow;
        public int dstCol;
        public BattleSceneObj[] targets;

        public override void ReadFrom(IDataInputStream stream)
        {
            skillTypeOrStuntID = stream.ReadString();
            isStunt = stream.ReadBoolean();
            action = stream.ReadInt32();
            dstRow = stream.ReadInt32();
            dstCol = stream.ReadInt32();
            int length = stream.ReadInt32();
            targets = new BattleSceneObj[length];
            for (int i = 0; i < length; ++i)
            {
                targets[i].ReadFrom(stream);
            }
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(skillTypeOrStuntID);
            stream.WriteBoolean(isStunt);
            stream.WriteInt32(action);
            stream.WriteInt32(dstRow);
            stream.WriteInt32(dstCol);
            stream.WriteInt32(targets.Length);
            foreach (var target in targets)
            {
                target.WriteTo(stream);
            }
        }
    }
    
    public sealed class BattleSceneActableObjectDoSpecialActionMessage : Message
    {
        public const int MESSAGE_TYPE = 22;
        public override int MessageType => MESSAGE_TYPE;

        public string skillTypeOrStuntID;
        public bool isStunt;
        public int dstRow;
        public int dstCol;
        public BattleSceneObj[] targets;

        public override void ReadFrom(IDataInputStream stream)
        {
            skillTypeOrStuntID = stream.ReadString();
            isStunt = stream.ReadBoolean();
            dstRow = stream.ReadInt32();
            dstCol = stream.ReadInt32();
            int length = stream.ReadInt32();
            targets = new BattleSceneObj[length];
            for (int i = 0; i < length; ++i)
            {
                targets[i].ReadFrom(stream);
            }
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(skillTypeOrStuntID);
            stream.WriteBoolean(isStunt);
            stream.WriteInt32(dstRow);
            stream.WriteInt32(dstCol);
            stream.WriteInt32(targets.Length);
            foreach (var target in targets)
            {
                target.WriteTo(stream);
            }
        }
    }
    
}
