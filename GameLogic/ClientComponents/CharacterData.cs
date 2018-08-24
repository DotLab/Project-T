using GameLogic.CharacterSystem;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using GameLogic.Utilities;
using GameLogic.Utilities.DataSystem;
using System;
using System.Collections.Generic;

namespace GameLogic.ClientComponents {
	public sealed class CharacterData : IRequestHandler {
		private readonly Connection _connection;
		private readonly User _owner;

		public CharacterData(Connection connection, User owner) {
			_connection = connection;
			_owner = owner;
			connection.SetRequestHandler(GetCharacterDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetAspectDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetConsequenceDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetSkillDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetStuntDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetExtraDataMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetDirectResistSkillsMessage.MESSAGE_TYPE, this);
			connection.SetRequestHandler(GetSkillTypeListMessage.MESSAGE_TYPE, this);
		}

		public Message MakeResponse(Message request) {
			try {
				Message resp = null;
				switch (request.MessageType) {
					case GetCharacterDataMessage.MESSAGE_TYPE: {
							var characterDataMessage = (GetCharacterDataMessage)request;
							resp = GetCharacterData(CharacterManager.Instance.FindCharacterByID(characterDataMessage.characterID), characterDataMessage.dataType);
						}
						break;
					case GetAspectDataMessage.MESSAGE_TYPE: {
							var aspectDataMessage = (GetAspectDataMessage)request;
							Character character = CharacterManager.Instance.FindCharacterByID(aspectDataMessage.characterID);
							string aspectID = aspectDataMessage.aspectID;
							foreach (Aspect aspect in character.Aspects) {
								if (aspect.ID == aspectID) {
									resp = GetAspectData(aspect);
									break;
								}
							}
						}
						break;
					case GetConsequenceDataMessage.MESSAGE_TYPE: {
							var consequenceDataMessage = (GetConsequenceDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(consequenceDataMessage.characterID);
							string consequenceID = consequenceDataMessage.consequenceID;
							foreach (Consequence consequence in character.Aspects) {
								if (consequence.ID == consequenceID) {
									resp = GetConsequenceData(consequence);
									break;
								}
							}
						}
						break;
					case GetSkillDataMessage.MESSAGE_TYPE: {
							var skillLevelDataMessage = (GetSkillDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(skillLevelDataMessage.characterID);
							resp = GetSkillData(character, SkillType.SkillTypes[skillLevelDataMessage.skillTypeID]);
						}
						break;
					case GetStuntDataMessage.MESSAGE_TYPE: {
							var stuntDataMessage = (GetStuntDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(stuntDataMessage.characterID);
							string stuntID = stuntDataMessage.stuntID;
							foreach (Stunt stunt in character.Stunts) {
								if (stunt.ID == stuntID) {
									resp = GetStuntData(stunt);
									break;
								}
							}
						}
						break;
					case GetExtraDataMessage.MESSAGE_TYPE: {
							var extraItemDataMessage = (GetExtraDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(extraItemDataMessage.characterID);
							string extraID = extraItemDataMessage.extraID;
							foreach (Extra extra in character.Extras) {
								if (extra.ID == extraID) {
									resp = GetExtraData(extra);
									break;
								}
							}
						}
						break;
					case GetDirectResistSkillsMessage.MESSAGE_TYPE: {
							var resistSkillsMessage = (GetDirectResistSkillsMessage)request;
							resp = GetDirectResistSkills(SkillType.SkillTypes[resistSkillsMessage.initiativeSkillTypeID], (SkillChecker.CharacterAction)resistSkillsMessage.actionType);
						}
						break;
					case GetSkillTypeListMessage.MESSAGE_TYPE: {
							var skillTypeListMessage = (GetSkillTypeListMessage)request;
							resp = GetSkillTypeList();
						}
						break;
					default:
						break;
				}
				return resp;
			} catch (Exception e) {
				Logger.WriteLine(e.Message);
				return null;
			}
		}

		private Message GetCharacterData(Character character, GetCharacterDataMessage.DataType dataType) {
			switch (dataType) {
				case GetCharacterDataMessage.DataType.INFO:
					return GetCharacterInfoData(character);
				case GetCharacterDataMessage.DataType.ASPECTS:
				case GetCharacterDataMessage.DataType.CONSEQUENCES:
				case GetCharacterDataMessage.DataType.EXTRAS:
				case GetCharacterDataMessage.DataType.SKILLS:
				case GetCharacterDataMessage.DataType.STUNTS:
					return GetCharacterPropertiesDescriptionData(character, dataType);
				case GetCharacterDataMessage.DataType.FATEPOINT:
					return GetCharacterFatePointData(character);
				case GetCharacterDataMessage.DataType.STRESS:
					return GetCharacterStressData(character);
				default:
					return null;
			}
		}

		private Message GetCharacterInfoData(Character character) {
			var message = new CharacterInfoDataMessage();
			message.characterID = character.ID;
			message.describable = new Describable(character);
			return message;
		}

		private Message GetCharacterFatePointData(Character character) {
			var message = new CharacterFatePointDataMessage();
			message.characterID = character.ID;
			message.fatePoint = character.FatePoint;
			message.refreshPoint = character.RefreshPoint;
			return message;
		}

		private Message GetCharacterStressData(Character character) {
			var message = new CharacterStressDataMessage();
			message.characterID = character.ID;
			message.physicsStress = character.PhysicsStress;
			message.physicsStressMax = character.PhysicsStressMax;
			message.mentalStress = character.MentalStress;
			message.mentalStressMax = character.MentalStressMax;
			return message;
		}

		private Message GetCharacterPropertiesDescriptionData(Character character, GetCharacterDataMessage.DataType dataType) {
			CharacterPropertiesDescriptionMessage message;
			switch (dataType) {
				case GetCharacterDataMessage.DataType.ASPECTS:
					message = new CharacterAspectsDescriptionMessage();
					message.properties = new CharacterPropertyDescription[character.Aspects.Count];
					for (int i = 0; i < character.Aspects.Count; ++i)
						message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Aspects[i]);
					break;
				case GetCharacterDataMessage.DataType.CONSEQUENCES:
					message = new CharacterConsequencesDescriptionMessage();
					message.properties = new CharacterPropertyDescription[character.Consequences.Count];
					for (int i = 0; i < character.Consequences.Count; ++i)
						message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Consequences[i]);
					break;
				case GetCharacterDataMessage.DataType.EXTRAS:
					message = new CharacterExtrasDescriptionMessage();
					message.properties = new CharacterPropertyDescription[character.Extras.Count];
					for (int i = 0; i < character.Extras.Count; ++i)
						message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Extras[i]);
					break;
				case GetCharacterDataMessage.DataType.SKILLS:
					message = new CharacterSkillsDescriptionMessage();
					message.properties = new CharacterPropertyDescription[character.ReadonlySkillList.Count];
					for (int i = 0; i < character.ReadonlySkillList.Count; ++i)
						message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.ReadonlySkillList[i]);
					break;
				case GetCharacterDataMessage.DataType.STUNTS:
					message = new CharacterStuntsDescriptionMessage();
					message.properties = new CharacterPropertyDescription[character.Stunts.Count];
					for (int i = 0; i < character.Stunts.Count; ++i)
						message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Stunts[i]);
					break;
				default:
					return null;
			}
			message.characterID = character.ID;
			return message;
		}

		private Message GetAspectData(Aspect aspect) {
			var message = new AspectDataMessage();
			message.characterID = aspect.Belong == null ? "" : aspect.Belong.ID;
			message.aspectID = aspect.ID;
			message.persistenceType = (int)aspect.PersistenceType;
			message.benefitCharacterID = aspect.Benefit == null ? "" : aspect.Benefit.ID;
			message.benefitTimes = aspect.BenefitTimes;
			return message;
		}

		private Message GetConsequenceData(Consequence consequence) {
			var message = new ConsequenceDataMessage();
			message.characterID = consequence.Belong == null ? "" : consequence.Belong.ID;
			message.consequenceID = consequence.ID;
			message.persistenceType = (int)consequence.PersistenceType;
			message.benefitCharacterID = consequence.Benefit == null ? "" : consequence.Benefit.ID;
			message.benefitTimes = consequence.BenefitTimes;
			message.counteractLevel = consequence.CounteractLevel;
			return message;
		}

		private Message GetSkillData(Character character, SkillType skillType) {
			var message = new SkillDataMessage();
			message.characterID = character.ID;
			message.skillTypeID = skillType.ID;
			message.skillProperty = character.GetSkillProperty(skillType);
			return message;
		}

		private Message GetStuntData(Stunt stunt) {
			var message = new StuntDataMessage();
			message.characterID = stunt.Belong == null ? "" : stunt.Belong.ID;
			message.stuntID = stunt.ID;
			message.boundSkillTypeID = stunt.BoundSkillType.ID;
			message.needDMCheck = stunt.NeedDMCheck;
			return message;
		}

		private Message GetExtraData(Extra extra) {
			var message = new ExtraDataMessage();
			message.characterID = extra.Belong == null ? "" : extra.Belong.ID;
			message.extraID = extra.ID;
			message.itemID = extra.Item.ID;
			message.isTool = extra.IsTool;
			message.isLongRangeWeapon = extra.IsLongRangeWeapon;
			message.isVehicle = extra.IsVehicle;
			return message;
		}

		private Message GetDirectResistSkills(SkillType initiativeSkillType, SkillChecker.CharacterAction action) {
			var message = new DirectResistSkillsDataMessage();
			List<SkillType> resistables = new List<SkillType>();
			foreach (var skillType in SkillType.SkillTypes) {
				if (SkillChecker.CanResistSkillWithoutDMCheck(initiativeSkillType, skillType.Value, action)) {
					resistables.Add(skillType.Value);
				}
			}
			message.skillTypes = new SkillTypeDescription[resistables.Count];
			for (int i = 0; i < resistables.Count; ++i) {
				message.skillTypes[i] = StreamableFactory.CreateSkillTypeDescription(resistables[i]);
			}
			return message;
		}

		private Message GetSkillTypeList() {
			var message = new SkillTypeListDataMessage();
			message.skillTypes = new SkillTypeDescription[SkillType.SkillTypes.Count];
			int i = 0;
			foreach (var skillType in SkillType.SkillTypes) {
				message.skillTypes[i++] = StreamableFactory.CreateSkillTypeDescription(skillType.Value);
			}
			return message;
		}
	}
}
