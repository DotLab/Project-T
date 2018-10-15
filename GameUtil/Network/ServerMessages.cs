using GameUtil.Network.Streamable;

namespace GameUtil.Network.ServerMessages {
	public sealed class ServerReadyMessage : Message {
		public const int MESSAGE_TYPE = -1;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class UserDecidingMessage : Message {
		public const int MESSAGE_TYPE = -74;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string promptText;
		public string[] selections;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(promptText);
			stream.WriteInt32(selections.Length);
			foreach (string selection in selections) {
				stream.WriteString(selection);
			}
		}

		public override void ReadFrom(IDataInputStream stream) {
			promptText = stream.ReadString();
			int length = stream.ReadInt32();
			selections = new string[length];
			for (int i = 0; i < length; ++i) {
				selections[i] = stream.ReadString();
			}
		}
	}

	public sealed class WaitUserDecisionMessage : Message {
		public const int MESSAGE_TYPE = -83;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool enabled;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(enabled);
		}

		public override void ReadFrom(IDataInputStream stream) {
			enabled = stream.ReadBoolean();
		}
	}

	public sealed class CheckerStartCheckMessage : Message {
		public const int MESSAGE_TYPE = -71;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string initiativeID;
		public string initiativeSkillTypeID;
		public CharacterAction action;
		public string[] targetsID;

		public override void ReadFrom(IDataInputStream stream) {
			initiativeID = stream.ReadString();
			initiativeSkillTypeID = stream.ReadString();
			action = (CharacterAction)stream.ReadByte();
			int length = stream.ReadInt32();
			targetsID = new string[length];
			for (int i = 0; i < length; ++i) {
				targetsID[i] = stream.ReadString();
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(initiativeID);
			stream.WriteString(initiativeSkillTypeID);
			stream.WriteByte((byte)action);
			stream.WriteInt32(targetsID.Length);
			foreach (var targetID in targetsID) {
				stream.WriteString(targetID);
			}
		}
	}

	public sealed class CheckerCheckNextOneMessage : Message {
		public const int MESSAGE_TYPE = -72;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string nextOneID;

		public override void ReadFrom(IDataInputStream stream) {
			nextOneID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(nextOneID);
		}
	}

	public sealed class CheckerEndCheckMessage : Message {
		public const int MESSAGE_TYPE = -73;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class CheckerCheckResultMessage : Message {
		public const int MESSAGE_TYPE = -75;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public CheckResult initiative;
		public CheckResult passive;
		public int delta;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteByte((byte)initiative);
			stream.WriteByte((byte)passive);
			stream.WriteInt32(delta);
		}

		public override void ReadFrom(IDataInputStream stream) {
			initiative = (CheckResult)stream.ReadByte();
			passive = (CheckResult)stream.ReadByte();
			delta = stream.ReadInt32();
		}
	}

	public sealed class CheckerNotifyPassiveSelectActionMessage : Message {
		public const int MESSAGE_TYPE = -55;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class CheckerNotifySelectAspectMessage : Message {
		public const int MESSAGE_TYPE = -56;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool isInitiative;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
		}
	}

	public sealed class CheckerCharacterActionResponseMessage : Message {
		public const int MESSAGE_TYPE = -40;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool isInitiative;
		public bool failure;
		public string failureDescription;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			failure = stream.ReadBoolean();
			failureDescription = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteBoolean(failure);
			stream.WriteString(failureDescription);
		}
	}

	public sealed class CheckerSelectAspectResponseMessage : Message {
		public const int MESSAGE_TYPE = -42;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool selectionOver;
		public bool isInitiative;
		public bool failure;
		public string failureDescription;

		public override void ReadFrom(IDataInputStream stream) {
			selectionOver = stream.ReadBoolean();
			isInitiative = stream.ReadBoolean();
			failure = stream.ReadBoolean();
			failureDescription = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(selectionOver);
			stream.WriteBoolean(isInitiative);
			stream.WriteBoolean(failure);
			stream.WriteString(failureDescription);
		}
	}

	public sealed class CheckerCanInitiativeUseSkillMessage : Message {
		public const int MESSAGE_TYPE = -66;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool result;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(result);
		}

		public override void ReadFrom(IDataInputStream stream) {
			result = stream.ReadBoolean();
		}
	}

	public sealed class CheckerCanInitiativeUseStuntMessage : Message {
		public const int MESSAGE_TYPE = -66;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool result;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(result);
		}

		public override void ReadFrom(IDataInputStream stream) {
			result = stream.ReadBoolean();
		}
	}

	public sealed class CheckerPassiveUsableActionListMessage : Message {
		public const int MESSAGE_TYPE = -67;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string[] skillTypesID;
		public string[] stuntsID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(skillTypesID.Length);
			foreach (var skillTypeID in skillTypesID) {
				stream.WriteString(skillTypeID);
			}
			stream.WriteInt32(stuntsID.Length);
			foreach (var stuntID in stuntsID) {
				stream.WriteString(stuntID);
			}
		}

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			skillTypesID = new string[length];
			for (int i = 0; i < length; ++i) {
				skillTypesID[i] = stream.ReadString();
			}
			length = stream.ReadInt32();
			stuntsID = new string[length];
			for (int i = 0; i < length; ++i) {
				stuntsID[i] = stream.ReadString();
			}
		}
	}

	public sealed class CheckerStuntTargetValidityMessage : Message {
		public const int MESSAGE_TYPE = -76;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool result;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(result);
		}

		public override void ReadFrom(IDataInputStream stream) {
			result = stream.ReadBoolean();
		}
	}

	public sealed class CheckerUpdateSumPointMessage : Message {
		public const int MESSAGE_TYPE = -57;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool isInitiative;
		public int point;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			point = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteInt32(point);
		}
	}

	public sealed class CheckerDisplayUsingSkillMessage : Message {
		public const int MESSAGE_TYPE = -58;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool isInitiative;
		public string skillTypeID;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			skillTypeID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteString(skillTypeID);
		}
	}

	public sealed class CheckerDisplayUsingStuntMessage : Message {
		public const int MESSAGE_TYPE = -81;
		public override int MessageType { get { return MESSAGE_TYPE; } }

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

	public sealed class CheckerDisplayUsingAspectMessage : Message {
		public const int MESSAGE_TYPE = -59;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool isInitiative;
		public string aspectOwnerID;
		public string aspectID;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			aspectOwnerID = stream.ReadString();
			aspectID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteString(aspectOwnerID);
			stream.WriteString(aspectID);
		}
	}

	public sealed class CharacterInfoDataMessage : Message {
		public const int MESSAGE_TYPE = -20;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public Describable describable;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			describable.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			describable.ReadFrom(stream);
		}
	}

	public abstract class CharacterPropertiesDescriptionMessage : Message {
		public string characterID;
		public CharacterPropertyDescription[] properties;

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			int length = stream.ReadInt32();
			properties = new CharacterPropertyDescription[length];
			for (int i = 0; i < length; ++i) {
				properties[i].ReadFrom(stream);
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteInt32(properties.Length);
			foreach (CharacterPropertyDescription property in properties) {
				property.WriteTo(stream);
			}
		}
	}

	public sealed class CharacterAspectsDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -21;
		public override int MessageType { get { return MESSAGE_TYPE; } }
	}

	public sealed class CharacterStuntsDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -22;
		public override int MessageType { get { return MESSAGE_TYPE; } }
	}

	public sealed class CharacterExtrasDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -23;
		public override int MessageType { get { return MESSAGE_TYPE; } }
	}

	public sealed class CharacterConsequencesDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -24;
		public override int MessageType { get { return MESSAGE_TYPE; } }
	}

	public sealed class CharacterFatePointDataMessage : Message {
		public const int MESSAGE_TYPE = -26;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public int fatePoint;
		public int refreshPoint;

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			fatePoint = stream.ReadInt32();
			refreshPoint = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteInt32(fatePoint);
			stream.WriteInt32(refreshPoint);
		}
	}

	public sealed class CharacterStressDataMessage : Message {
		public const int MESSAGE_TYPE = -25;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public int physicsStress;
		public int physicsStressMax;
		public int mentalStress;
		public int mentalStressMax;

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			physicsStress = stream.ReadInt32();
			physicsStressMax = stream.ReadInt32();
			mentalStress = stream.ReadInt32();
			mentalStressMax = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteInt32(physicsStress);
			stream.WriteInt32(physicsStressMax);
			stream.WriteInt32(mentalStress);
			stream.WriteInt32(mentalStressMax);
		}
	}

	public sealed class CharacterGroupDataMessage : Message {
		public const int MESSAGE_TYPE = -84;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public CharacterToken token;
		public string controlUserID;
		public int groupID;
		public string[] membersID;

		public override void ReadFrom(IDataInputStream stream) {
			token = (CharacterToken)stream.ReadByte();
			controlUserID = stream.ReadString();
			groupID = stream.ReadInt32();
			int length = stream.ReadInt32();
			membersID = new string[length];
			for (int i = 0; i < length; ++i) {
				membersID[i] = stream.ReadString();
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteByte((byte)token);
			stream.WriteString(controlUserID);
			stream.WriteInt32(groupID);
			stream.WriteInt32(membersID.Length);
			foreach (string characterID in membersID) {
				stream.WriteString(characterID);
			}
		}
	}

	public sealed class AspectDataMessage : Message {
		public const int MESSAGE_TYPE = -27;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public string aspectID;
		public int persistenceType;
		public string benefiterID;
		public int benefitTimes;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(aspectID);
			stream.WriteInt32(persistenceType);
			stream.WriteString(benefiterID);
			stream.WriteInt32(benefitTimes);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			aspectID = stream.ReadString();
			persistenceType = stream.ReadInt32();
			benefiterID = stream.ReadString();
			benefitTimes = stream.ReadInt32();
		}
	}

	public sealed class ConsequenceDataMessage : Message {
		public const int MESSAGE_TYPE = -28;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public string consequenceID;
		public int persistenceType;
		public string benefitCharacterID;
		public int benefitTimes;
		public int counteractLevel;
		public bool mentalDamage;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(consequenceID);
			stream.WriteInt32(persistenceType);
			stream.WriteString(benefitCharacterID);
			stream.WriteInt32(benefitTimes);
			stream.WriteInt32(counteractLevel);
			stream.WriteBoolean(mentalDamage);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			consequenceID = stream.ReadString();
			persistenceType = stream.ReadInt32();
			benefitCharacterID = stream.ReadString();
			benefitTimes = stream.ReadInt32();
			counteractLevel = stream.ReadInt32();
			mentalDamage = stream.ReadBoolean();
		}
	}

	public sealed class SkillDataMessage : Message {
		public const int MESSAGE_TYPE = -29;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public string skillTypeID;
		public string customName;
		public int level;
		public int targetMaxCount;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(skillTypeID);
			stream.WriteString(customName);
			stream.WriteInt32(level);
			stream.WriteInt32(targetMaxCount);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			skillTypeID = stream.ReadString();
			customName = stream.ReadString();
			level = stream.ReadInt32();
			targetMaxCount = stream.ReadInt32();
		}
	}

	public sealed class StuntDataMessage : Message {
		public const int MESSAGE_TYPE = -30;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public string stuntID;
		public int targetMaxCount;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(stuntID);
			stream.WriteInt32(targetMaxCount);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			stuntID = stream.ReadString();
			targetMaxCount = stream.ReadString();
		}
	}

	public sealed class ExtraDataMessage : Message {
		public const int MESSAGE_TYPE = -31;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public string extraID;
		public bool isLongRangeWeapon;
		public bool isVehicle;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(extraID);
			stream.WriteBoolean(isLongRangeWeapon);
			stream.WriteBoolean(isVehicle);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			extraID = stream.ReadString();
			isLongRangeWeapon = stream.ReadBoolean();
			isVehicle = stream.ReadBoolean();
		}
	}

	public sealed class DirectResistableSkillsListMessage : Message {
		public const int MESSAGE_TYPE = -32;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string[] skillTypesID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(skillTypesID.Length);
			foreach (string skillTypeID in skillTypesID) {
				stream.WriteString(skillTypeID);
			}
		}

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			skillTypesID = new string[length];
			for (int i = 0; i < length; ++i) {
				skillTypesID[i] = stream.ReadString();
			}
		}
	}

	public sealed class DirectResistableStuntsListMessage : Message {
		public const int MESSAGE_TYPE = -79;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;
		public string[] stuntsID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteInt32(stuntsID.Length);
			foreach (var stuntID in stuntsID) {
				stream.WriteString(stuntID);
			}
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			int length = stream.ReadInt32();
			stuntsID = new string[length];
			for (int i = 0; i < length; ++i) {
				stuntsID[i] = stream.ReadString();
			}
		}
	}

	public sealed class SkillTypeListDataMessage : Message {
		public const int MESSAGE_TYPE = -33;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public SkillTypeDescription[] skillTypes;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(skillTypes.Length);
			foreach (var skillType in skillTypes) {
				skillType.WriteTo(stream);
			}
		}

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			skillTypes = new SkillTypeDescription[length];
			for (int i = 0; i < length; ++i) {
				skillTypes[i].ReadFrom(stream);
			}
		}
	}

	public sealed class DisplayDicePointsMessage : Message {
		public const int MESSAGE_TYPE = -37;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string userID;
		public int[] dicePoints;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(userID);
			stream.WriteInt32(dicePoints.Length);
			foreach (int point in dicePoints) {
				stream.WriteInt32(point);
			}
		}

		public override void ReadFrom(IDataInputStream stream) {
			userID = stream.ReadString();
			int length = stream.ReadInt32();
			dicePoints = new int[length];
			for (int i = 0; i < length; ++i) {
				dicePoints[i] = stream.ReadInt32();
			}
		}
	}

	public sealed class PlayBGMMessage : Message {
		public const int MESSAGE_TYPE = -10;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string bgmID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(bgmID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			bgmID = stream.ReadString();
		}
	}

	public sealed class StopBGMMessage : Message {
		public const int MESSAGE_TYPE = -11;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class PlaySEMessage : Message {
		public const int MESSAGE_TYPE = -12;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string seID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(seID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			seID = stream.ReadString();
		}
	}

	public sealed class ShowSceneMessage : Message {
		public const int MESSAGE_TYPE = -13;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public byte sceneType;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteByte(sceneType);
		}

		public override void ReadFrom(IDataInputStream stream) {
			sceneType = stream.ReadByte();
		}
	}

	public sealed class StorySceneResetMessage : Message {
		public const int MESSAGE_TYPE = -2;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class StorySceneAddPlayerCharacterMessage : Message {
		public const int MESSAGE_TYPE = -46;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public int playerIndex;
		public string characterID;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(playerIndex);
			stream.WriteString(characterID);
			view.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			playerIndex = stream.ReadInt32();
			characterID = stream.ReadString();
			view = new CharacterView();
			view.ReadFrom(stream);
		}
	}

	public sealed class StorySceneRemovePlayerCharacterMessage : Message {
		public const int MESSAGE_TYPE = -47;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public int playerIndex;
		public string characterID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(playerIndex);
			stream.WriteString(characterID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			playerIndex = stream.ReadInt32();
			characterID = stream.ReadString();
		}
	}

	public sealed class StorySceneObjectAddMessage : Message {
		public const int MESSAGE_TYPE = -3;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			view.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			view = new CharacterView();
			view.ReadFrom(stream);
		}
	}

	public sealed class StorySceneObjectRemoveMessage : Message {
		public const int MESSAGE_TYPE = -4;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
		}
	}

	public sealed class StorySceneObjectTransformMessage : Message {
		public const int MESSAGE_TYPE = -5;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public Layout to;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			to.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			to.ReadFrom(stream);
		}
	}

	public sealed class StorySceneObjectViewEffectMessage : Message {
		public const int MESSAGE_TYPE = -6;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public CharacterViewEffect effect;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			effect.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			effect.ReadFrom(stream);
		}
	}

	public sealed class StorySceneObjectPortraitStyleMessage : Message {
		public const int MESSAGE_TYPE = -7;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public PortraitStyle portrait;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			portrait.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			portrait.ReadFrom(stream);
		}
	}

	public sealed class StorySceneCameraTransformMessage : Message {
		public const int MESSAGE_TYPE = -8;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public Layout to;

		public override void WriteTo(IDataOutputStream stream) {
			to.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			to.ReadFrom(stream);
		}
	}

	public sealed class StorySceneCameraEffectMessage : Message {
		public const int MESSAGE_TYPE = -9;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public CameraEffect effect;

		public override void WriteTo(IDataOutputStream stream) {
			effect.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			effect.ReadFrom(stream);
		}
	}

	public sealed class TextBoxAddParagraphMessage : Message {
		public const int MESSAGE_TYPE = -14;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string text;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(text);
		}

		public override void ReadFrom(IDataInputStream stream) {
			text = stream.ReadString();
		}
	}

	public sealed class TextBoxAddSelectionMessage : Message {
		public const int MESSAGE_TYPE = -15;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string text;
		public int selectionCode;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(text);
			stream.WriteInt32(selectionCode);
		}

		public override void ReadFrom(IDataInputStream stream) {
			text = stream.ReadString();
			selectionCode = stream.ReadInt32();
		}
	}

	public sealed class TextBoxClearMessage : Message {
		public const int MESSAGE_TYPE = -16;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class TextBoxSetPortraitMessage : Message {
		public const int MESSAGE_TYPE = -17;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			view.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			view = new CharacterView();
			view.ReadFrom(stream);
		}
	}

	public sealed class TextBoxPortraitStyleMessage : Message {
		public const int MESSAGE_TYPE = -18;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public PortraitStyle style;

		public override void WriteTo(IDataOutputStream stream) {
			style.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			style.ReadFrom(stream);
		}
	}

	public sealed class TextBoxPortraitEffectMessage : Message {
		public const int MESSAGE_TYPE = -19;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public CharacterViewEffect effect;

		public override void WriteTo(IDataOutputStream stream) {
			effect.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			effect.ReadFrom(stream);
		}
	}

	public sealed class BattleScenePushGridObjectMessage : Message {
		public const int MESSAGE_TYPE = -48;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public GridObjectData objData;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			objData.WriteTo(stream);
			view.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objData.ReadFrom(stream);
			view = new CharacterView();
			view.ReadFrom(stream);
		}
	}

	public sealed class BattleSceneRemoveGridObjectMessage : Message {
		public const int MESSAGE_TYPE = -49;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string gridObjID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(gridObjID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			gridObjID = stream.ReadString();
		}
	}

	public sealed class BattleSceneAddLadderObjectMessage : Message {
		public const int MESSAGE_TYPE = -50;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public LadderObjectData objData;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			objData.WriteTo(stream);
			view.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objData.ReadFrom(stream);
			view = new CharacterView();
			view.ReadFrom(stream);
		}
	}

	public sealed class BattleSceneRemoveLadderObjectMessage : Message {
		public const int MESSAGE_TYPE = -51;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string ladderObjID;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(ladderObjID);
		}

		public override void ReadFrom(IDataInputStream stream) {
			ladderObjID = stream.ReadString();
		}
	}

	public sealed class BattleSceneResetMessage : Message {
		public const int MESSAGE_TYPE = -52;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public int rows;
		public int cols;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(rows);
			stream.WriteInt32(cols);
		}

		public override void ReadFrom(IDataInputStream stream) {
			rows = stream.ReadInt32();
			cols = stream.ReadInt32();
		}
	}

	public sealed class BattleSceneGridDataMessage : Message {
		public const int MESSAGE_TYPE = -69;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public int row;
		public int col;
		public bool isMiddleLand;

		public override void ReadFrom(IDataInputStream stream) {
			row = stream.ReadInt32();
			col = stream.ReadInt32();
			isMiddleLand = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(row);
			stream.WriteInt32(col);
			stream.WriteBoolean(isMiddleLand);
		}
	}

	public sealed class BattleSceneGridObjectDataMessage : Message {
		public const int MESSAGE_TYPE = -62;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public GridObjectData objData;

		public override void ReadFrom(IDataInputStream stream) {
			objData.ReadFrom(stream);
		}

		public override void WriteTo(IDataOutputStream stream) {
			objData.WriteTo(stream);
		}
	}

	public sealed class BattleSceneLadderObjectDataMessage : Message {
		public const int MESSAGE_TYPE = -63;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public LadderObjectData objData;

		public override void ReadFrom(IDataInputStream stream) {
			objData.ReadFrom(stream);
		}

		public override void WriteTo(IDataOutputStream stream) {
			objData.WriteTo(stream);
		}
	}

	public sealed class BattleSceneStartBattleMessage : Message {
		public const int MESSAGE_TYPE = -86;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class BattleSceneUpdateTurnOrderMessage : Message {
		public const int MESSAGE_TYPE = -53;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string[] objsIDOrdered;

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			objsIDOrdered = new string[length];
			for (int i = 0; i < length; ++i) {
				objsIDOrdered[i] = stream.ReadString();
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(objsIDOrdered.Length);
			foreach (var objID in objsIDOrdered) {
				stream.WriteString(objID);
			}
		}
	}

	public sealed class BattleSceneNewTurnMessage : Message {
		public const int MESSAGE_TYPE = -54;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objIDWhoseTurn;
		public bool canOperate;
		
		public override void ReadFrom(IDataInputStream stream) {
			objIDWhoseTurn = stream.ReadString();
			canOperate = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objIDWhoseTurn);
			stream.WriteBoolean(canOperate);
		}
	}

	public sealed class BattleSceneMovePathInfoMessage : Message {
		public const int MESSAGE_TYPE = -60;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public struct ReachableGrid : IStreamable {
			public int prevPlaceIndex;
			public GridPos pos;
			public int leftMovePoint;

			public void ReadFrom(IDataInputStream stream) {
				prevPlaceIndex = stream.ReadInt32();
				pos.ReadFrom(stream);
				leftMovePoint = stream.ReadInt32();
			}

			public void WriteTo(IDataOutputStream stream) {
				stream.WriteInt32(prevPlaceIndex);
				pos.WriteTo(stream);
				stream.WriteInt32(leftMovePoint);
			}
		}

		public ReachableGrid[] pathInfo;

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			pathInfo = new ReachableGrid[length];
			for (int i = 0; i < length; ++i) {
				pathInfo[i].ReadFrom(stream);
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(pathInfo.Length);
			foreach (var grid in pathInfo) {
				grid.WriteTo(stream);
			}
		}
	}

	public sealed class BattleSceneCanObjectTakeExtraMoveMessage : Message {
		public const int MESSAGE_TYPE = -68;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public bool result;

		public override void ReadFrom(IDataInputStream stream) {
			result = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(result);
		}
	}

	public sealed class BattleSceneDisplayObjectMovingMessage : Message {
		public const int MESSAGE_TYPE = -61;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public BattleMapDirection direction;
		public bool stairway;

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			direction = (BattleMapDirection)stream.ReadByte();
			stairway = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			stream.WriteByte((byte)direction);
			stream.WriteBoolean(stairway);
		}
	}

	public sealed class BattleSceneDisplayObjectTakeExtraMovePointMessage : Message {
		public const int MESSAGE_TYPE = -64;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public string moveSkillTypeID;
		public int newMovePoint;

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			moveSkillTypeID = stream.ReadString();
			newMovePoint = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			stream.WriteString(moveSkillTypeID);
			stream.WriteInt32(newMovePoint);
		}
	}

	public sealed class BattleSceneUpdateActionPointMessage : Message {
		public const int MESSAGE_TYPE = -65;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public int newActionPoint;

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			newActionPoint = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			stream.WriteInt32(newActionPoint);
		}
	}

	public sealed class BattleSceneUpdateMovePointMessage : Message {
		public const int MESSAGE_TYPE = -70;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string objID;
		public int newMovePoint;

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			newMovePoint = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			stream.WriteInt32(newMovePoint);
		}
	}

	public sealed class BattleSceneActionAffectableAreasMessage : Message {
		public const int MESSAGE_TYPE = -77;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public GridPos[] centers;
		public GridPos[][] areas;

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			centers = new GridPos[length];
			for (int i = 0; i < length; ++i) {
				centers[i].ReadFrom(stream);
			}
			areas = new GridPos[length][];
			for (int i = 0; i < length; ++i) {
				int gridCount = stream.ReadInt32();
				areas[i] = new GridPos[gridCount];
				for (int j = 0; j < gridCount; ++j) {
					areas[i][j].ReadFrom(stream);
				}
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(centers.Length);
			foreach (var center in centers) {
				center.WriteTo(stream);
			}
			foreach (var area in areas) {
				stream.WriteInt32(area.Length);
				foreach (var grid in area) {
					grid.WriteTo(stream);
				}
			}
		}
	}
	
	public sealed class AllPartyListMessage : Message {
		public const int MESSAGE_TYPE = -85;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string[][] parties;

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			parties = new string[length][];
			for (int i = 0; i < length; ++i) {
				int memberCount = stream.ReadInt32();
				parties[i] = new string[memberCount];
				for (int j = 0; j < memberCount; ++j) {
					parties[i][j] = stream.ReadString();
				}
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(parties.Length);
			foreach (string[] charactersID in parties) {
				stream.WriteInt32(charactersID.Length);
				foreach (string characterID in charactersID) {
					stream.WriteString(characterID);
				}
			}
		}
	}

	public sealed class PlayerCharacterMessage : Message {
		public const int MESSAGE_TYPE = -87;
		public override int MessageType { get { return MESSAGE_TYPE; } }

		public string characterID;

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
		}
	}
	
}
