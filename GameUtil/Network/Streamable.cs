using System;

namespace GameLib.Utilities.Network.Streamable {
	public interface IDataOutputStream {
		void WriteBoolean(Boolean val);
		void WriteString(String val);
		void WriteByte(Byte val);
		void WriteInt32(Int32 val);
		void WriteSingle(Single val);
	}

	public interface IDataInputStream {
		Boolean ReadBoolean();
		String ReadString();
		Byte ReadByte();
		Int32 ReadInt32();
		Single ReadSingle();
	}

	public interface IStreamable {
		void WriteTo(IDataOutputStream stream);
		void ReadFrom(IDataInputStream stream);
	}

	public static class OutputStreamHelper {
		public static void WriteGuid(IDataOutputStream stream, Guid guid) {
			byte[] bs = guid.ToByteArray();
			stream.WriteByte((byte)bs.Length);
			foreach (var b in bs) {
				stream.WriteByte(b);
			}
		}

		public static void WriteLayout(IDataOutputStream stream, Layout val) {
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

		public static void WriteRange(IDataOutputStream stream, Range val) {
			stream.WriteBoolean(val.lowOpen);
			stream.WriteSingle(val.low);
			stream.WriteBoolean(val.highOpen);
			stream.WriteSingle(val.high);
		}

		public static void WriteCharacterView(IDataOutputStream stream, CharacterView val) {
			stream.WriteString(val.id);
			stream.WriteString(val.story);
			stream.WriteString(val.battle);
		}

		public static void WriteCameraEffect(IDataOutputStream stream, CameraEffect val) {
			stream.WriteByte((byte)val.animation);
		}

		public static void WriteCharacterViewEffect(IDataOutputStream stream, CharacterViewEffect val) {
			stream.WriteByte((byte)val.animation);
			stream.WriteSingle(val.tint.W);
			stream.WriteSingle(val.tint.X);
			stream.WriteSingle(val.tint.Y);
			stream.WriteSingle(val.tint.Z);
		}

		public static void WritePortraitStyle(IDataOutputStream stream, PortraitStyle val) {
			stream.WriteInt32(val.action);
			stream.WriteInt32(val.emotion);
		}

		public static void WriteSkillProperty(IDataOutputStream stream, SkillProperty property) {
			stream.WriteInt32(property.level);
			stream.WriteBoolean(property.canAttack);
			stream.WriteBoolean(property.damageMental);
			stream.WriteBoolean(property.canDefend);
			stream.WriteInt32(property.actionPointCost);
			WriteRange(stream, property.useRange);
			stream.WriteBoolean(property.islinearUse);
			stream.WriteByte((byte)property.linearUseDirection);
			WriteRange(stream, property.affectRange);
			stream.WriteBoolean(property.islinearAffect);
			stream.WriteByte((byte)property.linearAffectDirection);
			stream.WriteInt32(property.targetCount);
		}
	}

	public static class InputStreamHelper {
		public static Guid ReadGuid(IDataInputStream stream) {
			byte length = stream.ReadByte();
			byte[] bs = new byte[length];
			for (int i = 0; i < length; ++i) {
				bs[i] = stream.ReadByte();
			}
			return new Guid(bs);
		}

		public static Layout ReadLayout(IDataInputStream stream) {
			var ret = new Layout();
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

		public static Range ReadRange(IDataInputStream stream) {
			var ret = new Range();
			ret.lowOpen = stream.ReadBoolean();
			ret.low = stream.ReadSingle();
			ret.highOpen = stream.ReadBoolean();
			ret.high = stream.ReadSingle();
			return ret;
		}

		public static CharacterView ReadCharacterView(IDataInputStream stream) {
			var ret = new CharacterView();
			ret.id = stream.ReadString();
			ret.story = stream.ReadString();
			ret.battle = stream.ReadString();
			return ret;
		}

		public static CameraEffect ReadCameraEffect(IDataInputStream stream) {
			var ret = new CameraEffect();
			ret.animation = (CameraEffect.AnimateType)stream.ReadByte();
			return ret;
		}

		public static CharacterViewEffect ReadCharacterViewEffect(IDataInputStream stream) {
			var ret = new CharacterViewEffect();
			ret.animation = (CharacterViewEffect.AnimateType)stream.ReadByte();
			ret.tint.W = stream.ReadSingle();
			ret.tint.X = stream.ReadSingle();
			ret.tint.Y = stream.ReadSingle();
			ret.tint.Z = stream.ReadSingle();
			return ret;
		}

		public static PortraitStyle ReadPortraitStyle(IDataInputStream stream) {
			var ret = new PortraitStyle();
			ret.action = stream.ReadInt32();
			ret.emotion = stream.ReadInt32();
			return ret;
		}

		public static SkillProperty ReadSkillProperty(IDataInputStream stream) {
			var ret = new SkillProperty();
			ret.level = stream.ReadInt32();
			ret.canAttack = stream.ReadBoolean();
			ret.damageMental = stream.ReadBoolean();
			ret.canDefend = stream.ReadBoolean();
			ret.actionPointCost = stream.ReadInt32();
			ret.useRange = ReadRange(stream);
			ret.islinearUse = stream.ReadBoolean();
			ret.linearUseDirection = (BattleMapDirection)stream.ReadByte();
			ret.affectRange = ReadRange(stream);
			ret.islinearAffect = stream.ReadBoolean();
			ret.linearAffectDirection = (BattleMapDirection)stream.ReadByte();
			ret.targetCount = stream.ReadInt32();
			return ret;
		}
	}

	public struct Describable : IStreamable {
		public string name;
		public string description;

		public Describable(IDescribable describable) {
			name = describable.Name;
			description = describable.Description;
		}

		public void ReadFrom(IDataInputStream stream) {
			name = stream.ReadString();
			description = stream.ReadString();
		}

		public void WriteTo(IDataOutputStream stream) {
			stream.WriteString(name);
			stream.WriteString(description);
		}
	}

	public struct SkillTypeDescription : IStreamable {
		public string id;
		public string name;

		public void ReadFrom(IDataInputStream stream) {
			id = stream.ReadString();
			name = stream.ReadString();
		}

		public void WriteTo(IDataOutputStream stream) {
			stream.WriteString(id);
			stream.WriteString(name);
		}
	}

	public struct CharacterPropertyDescription : IStreamable {
		public string propertyID;
		public Describable describable;

		public void ReadFrom(IDataInputStream stream) {
			propertyID = stream.ReadString();
			describable.ReadFrom(stream);
		}

		public void WriteTo(IDataOutputStream stream) {
			stream.WriteString(propertyID);
			describable.WriteTo(stream);
		}
	}

	public struct BattleSceneObj : IStreamable {
		public string id;
		public int row;
		public int col;

		public void ReadFrom(IDataInputStream stream) {
			id = stream.ReadString();
			row = stream.ReadInt32();
			col = stream.ReadInt32();
		}

		public void WriteTo(IDataOutputStream stream) {
			stream.WriteString(id);
			stream.WriteInt32(row);
			stream.WriteInt32(col);
		}
	}

	public struct GridObjectData : IStreamable {
		public struct ActableObjectData : IStreamable {
			public int actionPoint;
			public int actionPointMax;
			public bool movable;
			public int movePoint;

			public void ReadFrom(IDataInputStream stream) {
				actionPoint = stream.ReadInt32();
				actionPointMax = stream.ReadInt32();
				movable = stream.ReadBoolean();
				movePoint = stream.ReadInt32();
			}

			public void WriteTo(IDataOutputStream stream) {
				stream.WriteInt32(actionPoint);
				stream.WriteInt32(actionPointMax);
				stream.WriteBoolean(movable);
				stream.WriteInt32(movePoint);
			}
		}

		public BattleSceneObj obj;
		public bool obstacle;
		public bool highland;
		public int stagnate;
		public bool terrain;
		public BattleMapDirection direction;
		public bool actable;
		public ActableObjectData actableObjData;

		public void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			obstacle = stream.ReadBoolean();
			highland = stream.ReadBoolean();
			stagnate = stream.ReadInt32();
			terrain = stream.ReadBoolean();
			direction = (BattleMapDirection)stream.ReadByte();
			actable = stream.ReadBoolean();
			if (actable) {
				actableObjData.ReadFrom(stream);
			}
		}

		public void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteBoolean(obstacle);
			stream.WriteBoolean(highland);
			stream.WriteInt32(stagnate);
			stream.WriteBoolean(terrain);
			stream.WriteByte((byte)direction);
			stream.WriteBoolean(actable);
			if (actable) {
				actableObjData.WriteTo(stream);
			}
		}
	}

	public struct LadderObjectData : IStreamable {
		public BattleSceneObj obj;
		public int stagnate;
		public BattleMapDirection direction;

		public void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			stagnate = stream.ReadInt32();
			direction = (BattleMapDirection)stream.ReadByte();
		}

		public void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteInt32(stagnate);
			stream.WriteByte((byte)direction);
		}
	}
}
