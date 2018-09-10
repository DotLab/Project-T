using GameUtil.Network.Streamable;

namespace GameUtil.Network.ClientMessages {
	public sealed class ClientInitMessage : Message {
		public const int MESSAGE_TYPE = 1;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class StorySceneObjectActionMessage : Message {
		public enum PlayerAction {
			INTERACT,
			CREATE_ASPECT,
			ATTACK,
			HINDER
		}

		public const int MESSAGE_TYPE = 2;
		public override int MessageType => MESSAGE_TYPE;

		public string objID;
		public PlayerAction action;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			stream.WriteByte((byte)action);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			action = (PlayerAction)stream.ReadByte();
		}
	}

	public sealed class TextSelectedMessage : Message {
		public const int MESSAGE_TYPE = 3;
		public override int MessageType => MESSAGE_TYPE;

		public int selection;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(selection);
		}

		public override void ReadFrom(IDataInputStream stream) {
			selection = stream.ReadInt32();
		}
	}

	public sealed class StorySceneNextActionMessage : Message {
		public const int MESSAGE_TYPE = 4;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class CheckerSkillSelectedMessage : Message {
		public const int MESSAGE_TYPE = 5;
		public override int MessageType => MESSAGE_TYPE;

		public string skillTypeID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(skillTypeID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			skillTypeID = stream.ReadString();
		}
	}

	public sealed class CheckerAspectSelectedMessage : Message {
		public const int MESSAGE_TYPE = 6;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string aspectID;
		public bool reroll;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(aspectID);
			stream.WriteBoolean(reroll);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			aspectID = stream.ReadString();
			reroll = stream.ReadBoolean();
		}
	}

	public sealed class CheckerStuntSelectedMessage : Message {
		public const int MESSAGE_TYPE = 7;
		public override int MessageType => MESSAGE_TYPE;

		public string stuntID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(stuntID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			stuntID = stream.ReadString();
		}
	}

	public sealed class GetCharacterDataMessage : Message {
		public const int MESSAGE_TYPE = 8;
		public override int MessageType => MESSAGE_TYPE;

		public enum DataType {
			INFO,
			ASPECTS,
			STUNTS,
			EXTRAS,
			CONSEQUENCES,
			STRESS,
			FATEPOINT
		}

		public string characterID;
		public DataType dataType;

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			dataType = (DataType)stream.ReadByte();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteByte((byte)dataType);
		}
	}

	public sealed class GetAspectDataMessage : Message {
		public const int MESSAGE_TYPE = 9;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string aspectID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(aspectID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			aspectID = stream.ReadString();
		}
	}

	public sealed class GetConsequenceDataMessage : Message {
		public const int MESSAGE_TYPE = 10;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string consequenceID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(consequenceID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			consequenceID = stream.ReadString();
		}
	}

	public sealed class GetSkillDataMessage : Message {
		public const int MESSAGE_TYPE = 11;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string skillTypeID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(skillTypeID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			skillTypeID = stream.ReadString();
		}
	}

	public sealed class GetStuntDataMessage : Message {
		public const int MESSAGE_TYPE = 12;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string stuntID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(stuntID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			stuntID = stream.ReadString();
		}
	}

	public sealed class GetExtraDataMessage : Message {
		public const int MESSAGE_TYPE = 13;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string extraID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(extraID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			extraID = stream.ReadString();
		}
	}

	public sealed class GetDirectResistSkillsMessage : Message {
		public const int MESSAGE_TYPE = 14;
		public override int MessageType => MESSAGE_TYPE;

		public string initiativeSkillTypeID;
		public CharacterAction actionType;

		public override void ReadFrom(IDataInputStream stream) {
			initiativeSkillTypeID = stream.ReadString();
			actionType = (CharacterAction)stream.ReadByte();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(initiativeSkillTypeID);
			stream.WriteByte((byte)actionType);
		}
	}

	public sealed class GetSkillTypeListMessage : Message {
		public const int MESSAGE_TYPE = 15;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class DMCheckResultMessage : Message {
		public const int MESSAGE_TYPE = 16;
		public override int MessageType => MESSAGE_TYPE;

		public bool result;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(result);
		}

		public override void ReadFrom(IDataInputStream stream) {
			result = stream.ReadBoolean();
		}
	}

	public sealed class BattleSceneSetSkipSelectAspectMessage : Message {
		public const int MESSAGE_TYPE = 17;
		public override int MessageType => MESSAGE_TYPE;

		public bool val;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(val);
		}

		public override void ReadFrom(IDataInputStream stream) {
			val = stream.ReadBoolean();
		}
	}

	public sealed class SelectAspectOverMessage : Message {
		public const int MESSAGE_TYPE = 18;
		public override int MessageType => MESSAGE_TYPE;

		public override void WriteTo(IDataOutputStream stream) { }
		public override void ReadFrom(IDataInputStream stream) { }
	}

	public sealed class BattleSceneGetMovePathInfoMessage : Message {
		public const int MESSAGE_TYPE = 19;
		public override int MessageType => MESSAGE_TYPE;

		public override void WriteTo(IDataOutputStream stream) { }
		public override void ReadFrom(IDataInputStream stream) { }
	}

	public sealed class BattleSceneActableObjectMoveMessage : Message {
		public const int MESSAGE_TYPE = 20;
		public override int MessageType => MESSAGE_TYPE;

		public GridPos dst;

		public override void WriteTo(IDataOutputStream stream) {
			dst.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			dst.ReadFrom(stream);
		}
	}

	public sealed class BattleSceneActableObjectDoActionMessage : Message {
		public const int MESSAGE_TYPE = 21;
		public override int MessageType => MESSAGE_TYPE;

		public string skillTypeOrStuntID;
		public bool isStunt;
		public CharacterAction action;
		public GridPos dstCenter;
		public string[] targets;

		public override void ReadFrom(IDataInputStream stream) {
			skillTypeOrStuntID = stream.ReadString();
			isStunt = stream.ReadBoolean();
			action = (CharacterAction)stream.ReadByte();
			dstCenter.ReadFrom(stream);
			int length = stream.ReadInt32();
			targets = new string[length];
			for (int i = 0; i < length; ++i) {
				targets[i] = stream.ReadString();
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(skillTypeOrStuntID);
			stream.WriteBoolean(isStunt);
			stream.WriteByte((byte)action);
			dstCenter.WriteTo(stream);
			stream.WriteInt32(targets.Length);
			foreach (var target in targets) {
				stream.WriteString(target);
			}
		}
	}

	public sealed class BattleSceneActableObjectDoInteractMessage : Message {
		public const int MESSAGE_TYPE = 22;
		public override int MessageType => MESSAGE_TYPE;

		public string skillTypeOrStuntID;
		public bool isStunt;
		public GridPos dstCenter;
		public string[] targets;

		public override void ReadFrom(IDataInputStream stream) {
			skillTypeOrStuntID = stream.ReadString();
			isStunt = stream.ReadBoolean();
			dstCenter.ReadFrom(stream);
			int length = stream.ReadInt32();
			targets = new string[length];
			for (int i = 0; i < length; ++i) {
				targets[i] = stream.ReadString();
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(skillTypeOrStuntID);
			stream.WriteBoolean(isStunt);
			dstCenter.WriteTo(stream);
			stream.WriteInt32(targets.Length);
			foreach (var target in targets) {
				stream.WriteString(target);
			}
		}
	}

	public sealed class BattleSceneTakeExtraMovePointMessage : Message {
		public const int MESSAGE_TYPE = 23;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}
	
	public sealed class BattleSceneGetGridObjectDataMessage : Message {
		public const int MESSAGE_TYPE = 24;
		public override int MessageType => MESSAGE_TYPE;

		public string objID;

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
		}
	}

	public sealed class BattleSceneGetLadderObjectDataMessage : Message {
		public const int MESSAGE_TYPE = 25;
		public override int MessageType => MESSAGE_TYPE;

		public string objID;

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
		}
	}

	public sealed class BattleSceneGetInitiativeUsableSkillOrStuntListMessage : Message {
		public const int MESSAGE_TYPE = 26;
		public override int MessageType => MESSAGE_TYPE;

		public CharacterAction action;
		public bool stunt;

		public override void ReadFrom(IDataInputStream stream) {
			action = (CharacterAction)stream.ReadByte();
			stunt = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteByte((byte)action);
			stream.WriteBoolean(stunt);
		}
	}

	public sealed class BattleSceneGetPassiveUsableSkillOrStuntListMessage : Message {
		public const int MESSAGE_TYPE = 27;
		public override int MessageType => MESSAGE_TYPE;
		
		public bool stunt;

		public override void ReadFrom(IDataInputStream stream) {
			stunt = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(stunt);
		}
	}

	public sealed class BattleSceneGetCanExtraMoveMessage : Message {
		public const int MESSAGE_TYPE = 28;
		public override int MessageType => MESSAGE_TYPE;
		
		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class BattleSceneTurnOverMessage : Message {
		public const int MESSAGE_TYPE = 29;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class UserDeterminResultMessage : Message {
		public const int MESSAGE_TYPE = 30;
		public override int MessageType => MESSAGE_TYPE;

		public int result;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(result);
		}

		public override void ReadFrom(IDataInputStream stream) {
			result = stream.ReadInt32();
		}
	}

	public sealed class GetStuntTargetSelectableMessage : Message {
		public const int MESSAGE_TYPE = 31;
		public override int MessageType => MESSAGE_TYPE;

		public string targetID;
		public string stuntID;
		public CharacterAction action;
		public bool isInteract;

		public override void ReadFrom(IDataInputStream stream) {
			targetID = stream.ReadString();
			stuntID = stream.ReadString();
			action = (CharacterAction)stream.ReadByte();
			isInteract = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(targetID);
			stream.WriteString(stuntID);
			stream.WriteByte((byte)action);
			stream.WriteBoolean(isInteract);
		}
	}
	
	public sealed class BattleSceneGetActionAffectableAreasMessage : Message {
		public const int MESSAGE_TYPE = 32;
		public override int MessageType => MESSAGE_TYPE;

		public string skillTypeOrStuntID;
		public bool isStunt;

		public override void ReadFrom(IDataInputStream stream) {
			skillTypeOrStuntID = stream.ReadString();
			isStunt = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(skillTypeOrStuntID);
			stream.WriteBoolean(isStunt);
		}
	}

	public sealed class BattleSceneGetActionTargetCountMessage : Message {
		public const int MESSAGE_TYPE = 33;
		public override int MessageType => MESSAGE_TYPE;

		public string skillTypeOrStuntID;
		public bool isStunt;

		public override void ReadFrom(IDataInputStream stream) {
			skillTypeOrStuntID = stream.ReadString();
			isStunt = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(skillTypeOrStuntID);
			stream.WriteBoolean(isStunt);
		}
	}

	public sealed class GetDirectResistStuntsMessage : Message {
		public const int MESSAGE_TYPE = 34;
		public override int MessageType => MESSAGE_TYPE;

		public string initiativeSkillTypeID;
		public string passiveCharacterID;
		public CharacterAction actionType;

		public override void ReadFrom(IDataInputStream stream) {
			initiativeSkillTypeID = stream.ReadString();
			passiveCharacterID = stream.ReadString();
			actionType = (CharacterAction)stream.ReadByte();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(initiativeSkillTypeID);
			stream.WriteString(passiveCharacterID);
			stream.WriteByte((byte)actionType);
		}
	}
	
	public sealed class BattleSceneGetInitiativeUsableStuntListOnInteractMessage : Message {
		public const int MESSAGE_TYPE = 35;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

}
