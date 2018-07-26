using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.Network
{
    public interface IDataOutputStream
    {
        void WriteBoolean(Boolean val);
        void WriteChar(Char val);
        void WriteString(String val);
        void WriteByte(Byte val);
        void WriteSByte(SByte val);
        void WriteInt16(Int16 val);
        void WriteInt32(Int32 val);
        void WriteInt24(Int32 val);
        void WriteInt64(Int64 val);
        void WriteUInt16(UInt16 val);
        void WriteUInt32(UInt32 val);
        void WriteUInt24(UInt32 val);
        void WriteUInt64(UInt64 val);
        void WriteSingle(Single val);
        void WriteDouble(Double val);
    }

    public interface IDataInputStream
    {
        bool IsEnd();
        void Skip(int val);
        Boolean ReadBoolean();
        Char ReadChar();
        String ReadString(int length);
        Byte ReadByte();
        SByte ReadSByte();
        Int16 ReadInt16();
        Int32 ReadInt32();
        Int32 ReadInt24();
        Int64 ReadInt64();
        UInt16 ReadUInt16();
        UInt32 ReadUInt32();
        UInt32 ReadUInt24();
        UInt64 ReadUInt64();
        Single ReadSingle();
        Double ReadDouble();
    }

    public abstract class Streamable
    {
        public abstract void WriteTo(IDataOutputStream stream);
        public abstract void ReadFrom(IDataInputStream stream);
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
    }
}
