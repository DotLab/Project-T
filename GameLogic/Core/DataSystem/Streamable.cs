using GameLogic.CharacterSystem;
using GameLogic.Container.BattleComponent;
using System;
using System.Collections.Generic;
using System.Text;

using Bit = Networkf.BitHelper;

namespace GameLogic.Core.DataSystem
{
    public interface IDataOutputStream
    {
        void WriteBoolean(Boolean val);
        void WriteString(String val);
        void WriteByte(Byte val);
        void WriteInt32(Int32 val);
        void WriteSingle(Single val);
    }

    public interface IDataInputStream
    {
        Boolean ReadBoolean();
        String ReadString();
        Byte ReadByte();
        Int32 ReadInt32();
        Single ReadSingle();
    }

    public sealed class BitDataOutputStream : IDataOutputStream
    {
        public readonly byte[] bytes;
        public int start, i;

        public BitDataOutputStream(byte[] bytes, int i = 0)
        {
            this.bytes = bytes;
            this.start = this.i = i;
        }

        public void WriteBoolean(bool val)
        {
            Bit.WriteUInt8(bytes, ref i, (byte)(val ? 1 : 0));
        }

        public void WriteByte(byte val)
        {
            Bit.WriteUInt8(bytes, ref i, val);
        }

        public void WriteInt32(int val)
        {
            Bit.WriteInt32(bytes, ref i, val);
        }

        public void WriteSingle(float val)
        {
            Bit.WriteSingle(bytes, ref i, val);
        }

        public void WriteString(string val)
        {
            Bit.WriteUInt16(bytes, ref i, (ushort)Bit.GetStringByteCount(val));
            Bit.WriteString(bytes, ref i, val);
        }
    }

    public sealed class BitDataInputStream : IDataInputStream
    {
        public readonly byte[] bytes;
        public int start, i;

        public BitDataInputStream(byte[] bytes, int i = 0)
        {
            this.bytes = bytes;
            this.start = this.i = i;
        }

        public bool ReadBoolean()
        {
            byte val = Bit.ReadUInt8(bytes, ref i);
            return val != 0;
        }

        public byte ReadByte()
        {
            return Bit.ReadUInt8(bytes, ref i);
        }

        public int ReadInt32()
        {
            return Bit.ReadInt32(bytes, ref i);
        }

        public float ReadSingle()
        {
            return Bit.ReadSingle(bytes, ref i);
        }

        public string ReadString()
        {
            int byteCount = Bit.ReadUInt16(bytes, ref i);
            return Bit.ReadString(bytes, ref i, byteCount);
        }
    }

    public interface IStreamable
    {
        void WriteTo(IDataOutputStream stream);
        void ReadFrom(IDataInputStream stream);
    }

    public static class OutputStreamHelper
    {
        public static void WriteGuid(IDataOutputStream stream, Guid guid)
        {
            byte[] bs = guid.ToByteArray();
            stream.WriteByte((byte)bs.Length);
            foreach (var b in bs)
            {
                stream.WriteByte(b);
            }
        }

        public static void WriteLayout(IDataOutputStream stream, Layout val)
        {
            stream.WriteSingle(val.pos.X);
            stream.WriteSingle(val.pos.Y);
            stream.WriteSingle(val.pos.Z);
            stream.WriteSingle(val.rot.W);
            stream.WriteSingle(val.rot.X);
            stream.WriteSingle(val.rot.Y);
            stream.WriteSingle(val.rot.Z);
            stream.WriteSingle(val.sca.X);
            stream.WriteSingle(val.sca.Y);
            stream.WriteSingle(val.sca.Z);
        }

        public static void WriteRange(IDataOutputStream stream, Range val)
        {
            stream.WriteBoolean(val.lowOpen);
            stream.WriteSingle(val.low);
            stream.WriteBoolean(val.highOpen);
            stream.WriteSingle(val.high);
        }

        public static void WriteCharacterView(IDataOutputStream stream, CharacterView val)
        {
            throw new NotImplementedException();
        }

        public static void WriteCameraEffect(IDataOutputStream stream, CameraEffect val)
        {
            stream.WriteInt32((Int32)val.animation);
        }

        public static void WriteCharacterViewEffect(IDataOutputStream stream, CharacterViewEffect val)
        {
            throw new NotImplementedException();
        }

        public static void WritePortraitStyle(IDataOutputStream stream, PortraitStyle val)
        {
            stream.WriteInt32(val.action);
            stream.WriteInt32(val.emotion);
        }
        
        public static void WriteSkillProperty(IDataOutputStream stream, SkillProperty property)
        {
            stream.WriteInt32(property.level);
            stream.WriteBoolean(property.canAttack);
            stream.WriteBoolean(property.canDefend);
            stream.WriteBoolean(property.canMove);
        }
    }

    public static class InputStreamHelper
    {
        public static Guid ReadGuid(IDataInputStream stream)
        {
            byte length = stream.ReadByte();
            byte[] bs = new byte[length];
            for (int i = 0; i < length; ++i)
            {
                bs[i] = stream.ReadByte();
            }
            return new Guid(bs);
        }

        public static Layout ReadLayout(IDataInputStream stream)
        {
            Layout ret = new Layout();
            ret.pos.X = stream.ReadSingle();
            ret.pos.Y = stream.ReadSingle();
            ret.pos.Z = stream.ReadSingle();
            ret.rot.W = stream.ReadSingle();
            ret.rot.X = stream.ReadSingle();
            ret.rot.Y = stream.ReadSingle();
            ret.rot.Z = stream.ReadSingle();
            ret.sca.X = stream.ReadSingle();
            ret.sca.Y = stream.ReadSingle();
            ret.sca.Z = stream.ReadSingle();
            return ret;
        }

        public static Range ReadRange(IDataInputStream stream)
        {
            Range ret = new Range();
            ret.lowOpen = stream.ReadBoolean();
            ret.low = stream.ReadSingle();
            ret.highOpen = stream.ReadBoolean();
            ret.high = stream.ReadSingle();
            return ret;
        }

        public static CharacterView ReadCharacterView(IDataInputStream stream)
        {
            throw new NotImplementedException();
        }

        public static CameraEffect ReadCameraEffect(IDataInputStream stream)
        {
            CameraEffect ret = new CameraEffect();
            ret.animation = (CameraEffect.AnimateType)stream.ReadInt32();
            return ret;
        }
        
        public static CharacterViewEffect ReadCharacterViewEffect(IDataInputStream stream)
        {
            throw new NotImplementedException();
        }

        public static PortraitStyle ReadPortraitStyle(IDataInputStream stream)
        {
            PortraitStyle ret = new PortraitStyle();
            ret.action = stream.ReadInt32();
            ret.emotion = stream.ReadInt32();
            return ret;
        }
        
        public static SkillProperty ReadSkillProperty(IDataInputStream stream)
        {
            SkillProperty ret = new SkillProperty();
            ret.level = stream.ReadInt32();
            ret.canAttack = stream.ReadBoolean();
            ret.canDefend = stream.ReadBoolean();
            ret.canMove = stream.ReadBoolean();
            return ret;
        }
    }

    public struct Describable : IStreamable
    {
        public string name;
        public string description;

        public Describable(IDescribable describable)
        {
            name = describable.Name;
            description = describable.Description;
        }

        public void ReadFrom(IDataInputStream stream)
        {
            name = stream.ReadString();
            description = stream.ReadString();
        }

        public void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(name);
            stream.WriteString(description);
        }
    }

    public struct SkillTypeDescription : IStreamable
    {
        public string id;
        public string name;

        public SkillTypeDescription(SkillType skillType)
        {
            id = skillType.ID;
            name = skillType.Name;
        }

        public void ReadFrom(IDataInputStream stream)
        {
            id = stream.ReadString();
            name = stream.ReadString();
        }

        public void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(id);
            stream.WriteString(name);
        }
    }

    public struct CharacterPropertyDescription : IStreamable
    {
        public string propertyID;
        public Describable describable;

        public void ReadFrom(IDataInputStream stream)
        {
            propertyID = stream.ReadString();
            describable.ReadFrom(stream);
        }

        public void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(propertyID);
            describable.WriteTo(stream);
        }

        public CharacterPropertyDescription(ICharacterProperty characterProperty)
        {
            propertyID = characterProperty.ID;
            describable = new Describable(characterProperty);
        }

        public CharacterPropertyDescription(Skill skill)
        {
            propertyID = skill.SkillType.ID;
            describable = new Describable(skill);
        }
    }

    public struct BattleSceneObj : IStreamable
    {
        public string objID;
        public int row;
        public int col;

        public void ReadFrom(IDataInputStream stream)
        {
            objID = stream.ReadString();
            row = stream.ReadInt32();
            col = stream.ReadInt32();
        }

        public void WriteTo(IDataOutputStream stream)
        {
            stream.WriteString(objID);
            stream.WriteInt32(row);
            stream.WriteInt32(col);
        }

        public BattleSceneObj(GridObject gridObject)
        {
            row = gridObject.GridRef.PosRow;
            col = gridObject.GridRef.PosCol;
            objID = gridObject.ID;
        }

        public BattleSceneObj(LadderObject ladderObject)
        {
            row = ladderObject.GridRef.PosRow;
            col = ladderObject.GridRef.PosCol;
            objID = ladderObject.ID;
        }
    }

}
