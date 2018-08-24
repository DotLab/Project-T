namespace GameLogic.Core.Network.ServerMessages {
	public sealed class ServerReadyMessage : Message {
		public const int MESSAGE_TYPE = -1;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class StorySceneResetMessage : Message {
		public const int MESSAGE_TYPE = -2;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class StorySceneObjectAddMessage : Message {
		public const int MESSAGE_TYPE = -3;
		public override int MessageType => MESSAGE_TYPE;

		public string objID;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			OutputStreamHelper.WriteCharacterView(stream, view);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			view = InputStreamHelper.ReadCharacterView(stream);
		}
	}

	public sealed class StorySceneObjectRemoveMessage : Message {
		public const int MESSAGE_TYPE = -4;
		public override int MessageType => MESSAGE_TYPE;

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
		public override int MessageType => MESSAGE_TYPE;

		public string objID;
		public Layout to;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			OutputStreamHelper.WriteLayout(stream, to);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			to = InputStreamHelper.ReadLayout(stream);
		}
	}

	public sealed class StorySceneObjectViewEffectMessage : Message {
		public const int MESSAGE_TYPE = -6;
		public override int MessageType => MESSAGE_TYPE;

		public string objID;
		public CharacterViewEffect effect;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			OutputStreamHelper.WriteCharacterViewEffect(stream, effect);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			effect = InputStreamHelper.ReadCharacterViewEffect(stream);
		}
	}

	public sealed class StorySceneObjectPortraitStyleMessage : Message {
		public const int MESSAGE_TYPE = -7;
		public override int MessageType => MESSAGE_TYPE;

		public string objID;
		public PortraitStyle portrait;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(objID);
			OutputStreamHelper.WritePortraitStyle(stream, portrait);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objID = stream.ReadString();
			portrait = InputStreamHelper.ReadPortraitStyle(stream);
		}
	}

	public sealed class StorySceneCameraTransformMessage : Message {
		public const int MESSAGE_TYPE = -8;
		public override int MessageType => MESSAGE_TYPE;

		public Layout to;

		public override void WriteTo(IDataOutputStream stream) {
			OutputStreamHelper.WriteLayout(stream, to);
		}

		public override void ReadFrom(IDataInputStream stream) {
			to = InputStreamHelper.ReadLayout(stream);
		}
	}

	public sealed class StorySceneCameraEffectMessage : Message {
		public const int MESSAGE_TYPE = -9;
		public override int MessageType => MESSAGE_TYPE;

		public CameraEffect effect;

		public override void WriteTo(IDataOutputStream stream) {
			OutputStreamHelper.WriteCameraEffect(stream, effect);
		}

		public override void ReadFrom(IDataInputStream stream) {
			effect = InputStreamHelper.ReadCameraEffect(stream);
		}
	}

	public sealed class PlayBGMMessage : Message {
		public const int MESSAGE_TYPE = -10;
		public override int MessageType => MESSAGE_TYPE;

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
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class PlaySEMessage : Message {
		public const int MESSAGE_TYPE = -12;
		public override int MessageType => MESSAGE_TYPE;

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
		public override int MessageType => MESSAGE_TYPE;

		public int sceneType;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(sceneType);
		}

		public override void ReadFrom(IDataInputStream stream) {
			sceneType = stream.ReadInt32();
		}
	}

	public sealed class TextBoxAddParagraphMessage : Message {
		public const int MESSAGE_TYPE = -14;
		public override int MessageType => MESSAGE_TYPE;

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
		public override int MessageType => MESSAGE_TYPE;

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
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class TextBoxSetPortraitMessage : Message {
		public const int MESSAGE_TYPE = -17;
		public override int MessageType => MESSAGE_TYPE;

		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			OutputStreamHelper.WriteCharacterView(stream, view);
		}

		public override void ReadFrom(IDataInputStream stream) {
			view = InputStreamHelper.ReadCharacterView(stream);
		}
	}

	public sealed class TextBoxPortraitStyleMessage : Message {
		public const int MESSAGE_TYPE = -18;
		public override int MessageType => MESSAGE_TYPE;

		public PortraitStyle style;

		public override void WriteTo(IDataOutputStream stream) {
			OutputStreamHelper.WritePortraitStyle(stream, style);
		}

		public override void ReadFrom(IDataInputStream stream) {
			style = InputStreamHelper.ReadPortraitStyle(stream);
		}
	}

	public sealed class TextBoxPortraitEffectMessage : Message {
		public const int MESSAGE_TYPE = -19;
		public override int MessageType => MESSAGE_TYPE;

		public CharacterViewEffect effect;

		public override void WriteTo(IDataOutputStream stream) {
			OutputStreamHelper.WriteCharacterViewEffect(stream, effect);
		}

		public override void ReadFrom(IDataInputStream stream) {
			effect = InputStreamHelper.ReadCharacterViewEffect(stream);
		}
	}

	public sealed class CharacterInfoDataMessage : Message {
		public const int MESSAGE_TYPE = -20;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class CharacterSkillsDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -21;
		public override int MessageType => MESSAGE_TYPE;
	}

	public sealed class CharacterAspectsDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -22;
		public override int MessageType => MESSAGE_TYPE;
	}

	public sealed class CharacterStuntsDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -23;
		public override int MessageType => MESSAGE_TYPE;
	}

	public sealed class CharacterExtrasDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -24;
		public override int MessageType => MESSAGE_TYPE;
	}

	public sealed class CharacterConsequencesDescriptionMessage : CharacterPropertiesDescriptionMessage {
		public const int MESSAGE_TYPE = -25;
		public override int MessageType => MESSAGE_TYPE;
	}

	public sealed class CharacterStressDataMessage : Message {
		public const int MESSAGE_TYPE = -26;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class CharacterFatePointDataMessage : Message {
		public const int MESSAGE_TYPE = -27;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class AspectDataMessage : Message {
		public const int MESSAGE_TYPE = -28;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string aspectID;
		public int persistenceType;
		public string benefitCharacterID;
		public int benefitTimes;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(aspectID);
			stream.WriteInt32(persistenceType);
			stream.WriteString(benefitCharacterID);
			stream.WriteInt32(benefitTimes);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			aspectID = stream.ReadString();
			persistenceType = stream.ReadInt32();
			benefitCharacterID = stream.ReadString();
			benefitTimes = stream.ReadInt32();
		}
	}

	public sealed class ConsequenceDataMessage : Message {
		public const int MESSAGE_TYPE = -29;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string consequenceID;
		public int persistenceType;
		public string benefitCharacterID;
		public int benefitTimes;
		public int counteractLevel;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(consequenceID);
			stream.WriteInt32(persistenceType);
			stream.WriteString(benefitCharacterID);
			stream.WriteInt32(benefitTimes);
			stream.WriteInt32(counteractLevel);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			consequenceID = stream.ReadString();
			persistenceType = stream.ReadInt32();
			benefitCharacterID = stream.ReadString();
			benefitTimes = stream.ReadInt32();
			counteractLevel = stream.ReadInt32();
		}
	}

	public sealed class SkillDataMessage : Message {
		public const int MESSAGE_TYPE = -30;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string skillTypeID;
		public SkillProperty skillProperty;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(skillTypeID);
			OutputStreamHelper.WriteSkillProperty(stream, skillProperty);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			skillTypeID = stream.ReadString();
			skillProperty = InputStreamHelper.ReadSkillProperty(stream);
		}
	}

	public sealed class StuntDataMessage : Message {
		public const int MESSAGE_TYPE = -31;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string stuntID;
		public string boundSkillTypeID;
		public bool needDMCheck;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(stuntID);
			stream.WriteString(boundSkillTypeID);
			stream.WriteBoolean(needDMCheck);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			stuntID = stream.ReadString();
			boundSkillTypeID = stream.ReadString();
			needDMCheck = stream.ReadBoolean();
		}
	}

	public sealed class ExtraDataMessage : Message {
		public const int MESSAGE_TYPE = -32;
		public override int MessageType => MESSAGE_TYPE;

		public string characterID;
		public string extraID;
		public string itemID;
		public bool isTool;
		public bool isLongRangeWeapon;
		public bool isVehicle;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(characterID);
			stream.WriteString(extraID);
			stream.WriteString(itemID);
			stream.WriteBoolean(isTool);
			stream.WriteBoolean(isLongRangeWeapon);
			stream.WriteBoolean(isVehicle);
		}

		public override void ReadFrom(IDataInputStream stream) {
			characterID = stream.ReadString();
			extraID = stream.ReadString();
			itemID = stream.ReadString();
			isTool = stream.ReadBoolean();
			isLongRangeWeapon = stream.ReadBoolean();
			isVehicle = stream.ReadBoolean();
		}
	}

	public sealed class DirectResistSkillsDataMessage : Message {
		public const int MESSAGE_TYPE = -33;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class SkillTypeListDataMessage : Message {
		public const int MESSAGE_TYPE = -34;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class SkillCheckPanelShowMessage : Message {
		public const int MESSAGE_TYPE = -35;
		public override int MessageType => MESSAGE_TYPE;

		public string initiativeCharacterID;
		public CharacterView initiativeView;
		public string passiveCharacterID;
		public CharacterView passiveView;
		public int playerState;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(initiativeCharacterID);
			OutputStreamHelper.WriteCharacterView(stream, initiativeView);
			stream.WriteString(passiveCharacterID);
			OutputStreamHelper.WriteCharacterView(stream, passiveView);
			stream.WriteInt32(playerState);
		}

		public override void ReadFrom(IDataInputStream stream) {
			initiativeCharacterID = stream.ReadString();
			initiativeView = InputStreamHelper.ReadCharacterView(stream);
			passiveCharacterID = stream.ReadString();
			passiveView = InputStreamHelper.ReadCharacterView(stream);
			playerState = stream.ReadInt32();
		}
	}

	public sealed class SkillCheckPanelHideMessage : Message {
		public const int MESSAGE_TYPE = -36;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class DMCheckPanelShowMessage : Message {
		public const int MESSAGE_TYPE = -37;
		public override int MessageType => MESSAGE_TYPE;

		public string text;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(text);
		}

		public override void ReadFrom(IDataInputStream stream) {
			text = stream.ReadString();
		}
	}

	public sealed class DMCheckPanelHideMessage : Message {
		public const int MESSAGE_TYPE = -38;
		public override int MessageType => MESSAGE_TYPE;

		public override void ReadFrom(IDataInputStream stream) { }
		public override void WriteTo(IDataOutputStream stream) { }
	}

	public sealed class DisplayDicePointsMessage : Message {
		public const int MESSAGE_TYPE = -39;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage : Message {
		public const int MESSAGE_TYPE = -40;
		public override int MessageType => MESSAGE_TYPE;

		public string initiativeCharacterID;
		public string passiveCharacterID;
		public int action;

		public override void ReadFrom(IDataInputStream stream) {
			initiativeCharacterID = stream.ReadString();
			passiveCharacterID = stream.ReadString();
			action = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(initiativeCharacterID);
			stream.WriteString(passiveCharacterID);
			stream.WriteInt32(action);
		}
	}

	public sealed class StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage : Message {
		public const int MESSAGE_TYPE = -41;
		public override int MessageType => MESSAGE_TYPE;

		public string passiveCharacterID;
		public string initiativeCharacterID;
		public SkillTypeDescription initiativeSkillType;
		public int action;

		public override void ReadFrom(IDataInputStream stream) {
			passiveCharacterID = stream.ReadString();
			initiativeCharacterID = stream.ReadString();
			initiativeSkillType.ReadFrom(stream);
			action = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteString(passiveCharacterID);
			stream.WriteString(initiativeCharacterID);
			initiativeSkillType.WriteTo(stream);
			stream.WriteInt32(action);
		}
	}

	public sealed class CheckerSelectSkillOrStuntCompleteMessage : Message {
		public const int MESSAGE_TYPE = -42;
		public override int MessageType => MESSAGE_TYPE;

		public bool isInitiative;
		public bool failure;
		public string extraMessage;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			failure = stream.ReadBoolean();
			extraMessage = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteBoolean(failure);
			stream.WriteString(extraMessage);
		}
	}

	public sealed class StorySceneCheckerNotifySelectAspectMessage : Message {
		public const int MESSAGE_TYPE = -43;
		public override int MessageType => MESSAGE_TYPE;

		public bool isInitiative;
		public string characterID;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			characterID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteString(characterID);
		}
	}

	public sealed class CheckerSelectAspectCompleteMessage : Message {
		public const int MESSAGE_TYPE = -44;
		public override int MessageType => MESSAGE_TYPE;

		public bool over;
		public bool isInitiative;
		public bool failure;
		public string extraMessage;

		public override void ReadFrom(IDataInputStream stream) {
			over = stream.ReadBoolean();
			isInitiative = stream.ReadBoolean();
			failure = stream.ReadBoolean();
			extraMessage = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(over);
			stream.WriteBoolean(isInitiative);
			stream.WriteBoolean(failure);
			stream.WriteString(extraMessage);
		}
	}

	public sealed class StorySceneCheckerUpdateSumPointMessage : Message {
		public const int MESSAGE_TYPE = -45;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class StorySceneCheckerDisplaySkillReadyMessage : Message {
		public const int MESSAGE_TYPE = -46;
		public override int MessageType => MESSAGE_TYPE;

		public bool isInitiative;
		public string skillTypeID;
		public bool bigone;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			skillTypeID = stream.ReadString();
			bigone = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteString(skillTypeID);
			stream.WriteBoolean(bigone);
		}
	}

	public sealed class StorySceneCheckerDisplayUsingAspectMessage : Message {
		public const int MESSAGE_TYPE = -47;
		public override int MessageType => MESSAGE_TYPE;

		public bool isInitiative;
		public string characterID;
		public string aspectID;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			characterID = stream.ReadString();
			aspectID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			stream.WriteString(characterID);
			stream.WriteString(aspectID);
		}
	}

	public sealed class StorySceneAddPlayerCharacterMessage : Message {
		public const int MESSAGE_TYPE = -48;
		public override int MessageType => MESSAGE_TYPE;

		public int playerIndex;
		public string characterID;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(playerIndex);
			stream.WriteString(characterID);
			OutputStreamHelper.WriteCharacterView(stream, view);
		}

		public override void ReadFrom(IDataInputStream stream) {
			playerIndex = stream.ReadInt32();
			characterID = stream.ReadString();
			view = InputStreamHelper.ReadCharacterView(stream);
		}
	}

	public sealed class StorySceneRemovePlayerCharacterMessage : Message {
		public const int MESSAGE_TYPE = -49;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class BattleScenePushGridObjectMessage : Message {
		public const int MESSAGE_TYPE = -50;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneGridObjData objData;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			objData.WriteTo(stream);
			OutputStreamHelper.WriteCharacterView(stream, view);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objData.ReadFrom(stream);
			view = InputStreamHelper.ReadCharacterView(stream);
		}
	}

	public sealed class BattleSceneRemoveGridObjectMessage : Message {
		public const int MESSAGE_TYPE = -51;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj gridObj;

		public override void WriteTo(IDataOutputStream stream) {
			gridObj.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			gridObj.ReadFrom(stream);
		}
	}

	public sealed class BattleSceneAddLadderObjectMessage : Message {
		public const int MESSAGE_TYPE = -52;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneLadderObjData objData;
		public CharacterView view;

		public override void WriteTo(IDataOutputStream stream) {
			objData.WriteTo(stream);
			OutputStreamHelper.WriteCharacterView(stream, view);
		}

		public override void ReadFrom(IDataInputStream stream) {
			objData.ReadFrom(stream);
			view = InputStreamHelper.ReadCharacterView(stream);
		}
	}

	public sealed class BattleSceneRemoveLadderObjectMessage : Message {
		public const int MESSAGE_TYPE = -53;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj ladderObj;

		public override void WriteTo(IDataOutputStream stream) {
			ladderObj.WriteTo(stream);
		}

		public override void ReadFrom(IDataInputStream stream) {
			ladderObj.ReadFrom(stream);
		}
	}

	public sealed class BattleSceneResetMessage : Message {
		public const int MESSAGE_TYPE = -54;
		public override int MessageType => MESSAGE_TYPE;

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

	public sealed class BattleSceneSetActingOrderMessage : Message {
		public const int MESSAGE_TYPE = -55;
		public override int MessageType => MESSAGE_TYPE;
		
		public BattleSceneObj[] objOrder;

		public override void ReadFrom(IDataInputStream stream) {
			int length = stream.ReadInt32();
			objOrder = new BattleSceneObj[length];
			for (int i = 0; i < length; ++i) {
				objOrder[i].ReadFrom(stream);
			}
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteInt32(objOrder.Length);
			foreach (var obj in objOrder) {
				obj.WriteTo(stream);
			}
		}
	}

	public sealed class BattleSceneChangeTurnMessage : Message {
		public const int MESSAGE_TYPE = -56;
		public override int MessageType => MESSAGE_TYPE;

		public bool canOperate;
		public BattleSceneObj gridObj;

		public override void ReadFrom(IDataInputStream stream) {
			canOperate = stream.ReadBoolean();
			gridObj.ReadFrom(stream);
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(canOperate);
			gridObj.WriteTo(stream);
		}
	}

	public sealed class BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage : Message {
		public const int MESSAGE_TYPE = -57;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj passiveObj;
		public BattleSceneObj initiativeObj;
		public SkillTypeDescription initiativeSkillType;
		public int action;

		public override void ReadFrom(IDataInputStream stream) {
			passiveObj.ReadFrom(stream);
			initiativeObj.ReadFrom(stream);
			initiativeSkillType.ReadFrom(stream);
			action = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			passiveObj.WriteTo(stream);
			initiativeObj.WriteTo(stream);
			initiativeSkillType.WriteTo(stream);
			stream.WriteInt32(action);
		}
	}

	public sealed class BattleSceneCheckerNotifySelectAspectMessage : Message {
		public const int MESSAGE_TYPE = -58;
		public override int MessageType => MESSAGE_TYPE;

		public bool isInitiative;
		public BattleSceneObj obj;

		public override void ReadFrom(IDataInputStream stream) {
			isInitiative = stream.ReadBoolean();
			obj.ReadFrom(stream);
		}

		public override void WriteTo(IDataOutputStream stream) {
			stream.WriteBoolean(isInitiative);
			obj.WriteTo(stream);
		}
	}

	public sealed class BattleSceneCheckerUpdateSumPointMessage : Message {
		public const int MESSAGE_TYPE = -59;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj obj;
		public int point;

		public override void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			point = stream.ReadInt32();
		}

		public override void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteInt32(point);
		}
	}

	public sealed class BattleSceneCheckerDisplaySkillReadyMessage : Message {
		public const int MESSAGE_TYPE = -60;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj obj;
		public string skillTypeID;
		public bool bigone;

		public override void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			skillTypeID = stream.ReadString();
			bigone = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteString(skillTypeID);
			stream.WriteBoolean(bigone);
		}
	}

	public sealed class BattleSceneCheckerDisplayUsingAspectMessage : Message {
		public const int MESSAGE_TYPE = -61;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj userObj;
		public BattleSceneObj aspectOwnerObj;
		public string aspectID;

		public override void ReadFrom(IDataInputStream stream) {
			userObj.ReadFrom(stream);
			aspectOwnerObj.ReadFrom(stream);
			aspectID = stream.ReadString();
		}

		public override void WriteTo(IDataOutputStream stream) {
			userObj.WriteTo(stream);
			aspectOwnerObj.WriteTo(stream);
			stream.WriteString(aspectID);
		}
	}

	public sealed class BattleSceneMovePathInfoMessage : Message {
		public const int MESSAGE_TYPE = -62;
		public override int MessageType => MESSAGE_TYPE;

		public struct ReachableGrid : IStreamable {
			public int prevPlaceIndex;
			public int row;
			public int col;
			public bool highland;
			public int leftMovePoint;

			public void ReadFrom(IDataInputStream stream) {
				prevPlaceIndex = stream.ReadInt32();
				row = stream.ReadInt32();
				col = stream.ReadInt32();
				highland = stream.ReadBoolean();
				leftMovePoint = stream.ReadInt32();
			}

			public void WriteTo(IDataOutputStream stream) {
				stream.WriteInt32(prevPlaceIndex);
				stream.WriteInt32(row);
				stream.WriteInt32(col);
				stream.WriteBoolean(highland);
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

	public sealed class BattleSceneDisplayActableObjectMovingMessage : Message {
		public const int MESSAGE_TYPE = -63;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj obj;
		public int direction;
		public bool stairway;

		public override void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			direction = stream.ReadInt32();
			stairway = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteInt32(direction);
			stream.WriteBoolean(stairway);
		}
	}

	public sealed class BattleSceneGridObjectDataMessage : Message {
		public const int MESSAGE_TYPE = -64;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj obj;
		public int direction;
		public bool stairway;

		public override void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			direction = stream.ReadInt32();
			stairway = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteInt32(direction);
			stream.WriteBoolean(stairway);
		}
	}

	public sealed class BattleSceneLadderObjectDataMessage : Message {
		public const int MESSAGE_TYPE = -65;
		public override int MessageType => MESSAGE_TYPE;

		public BattleSceneObj obj;
		public int direction;
		public bool stairway;

		public override void ReadFrom(IDataInputStream stream) {
			obj.ReadFrom(stream);
			direction = stream.ReadInt32();
			stairway = stream.ReadBoolean();
		}

		public override void WriteTo(IDataOutputStream stream) {
			obj.WriteTo(stream);
			stream.WriteInt32(direction);
			stream.WriteBoolean(stairway);
		}
	}
}
