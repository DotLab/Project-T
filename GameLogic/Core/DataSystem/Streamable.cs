using GameLogic.CharacterSystem;
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
        String ReadString(int length);
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

    public interface IStreamable
    {
        void WriteTo(IDataOutputStream stream);
        void ReadFrom(IDataInputStream stream);
    }

    public static class OutputStreamHelper
    {
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

        public static void WriteDescribable(IDataOutputStream stream, IDescribable describable)
        {
            stream.WriteInt32(describable.Name.Length);
            stream.WriteString(describable.Name);
            stream.WriteInt32(describable.Description.Length);
            stream.WriteString(describable.Description);
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

        public static Describable ReadDescribable(IDataInputStream stream)
        {
            Describable ret = new Describable();
            int length = stream.ReadInt32();
            ret.Name = stream.ReadString(length);
            length = stream.ReadInt32();
            ret.Description = stream.ReadString(length);
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
}
