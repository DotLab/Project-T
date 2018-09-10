using GameServer.CharacterSystem;
using GameServer.Core;
using GameServer.Core.DataSystem;
using GameUtil;
using GameUtil.Network;
using GameUtil.Network.ClientMessages;
using GameUtil.Network.ServerMessages;
using GameUtil.Network.Streamable;
using System;
using System.Collections.Generic;

namespace GameServer.ClientComponents {
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
			connection.SetRequestHandler(GetDirectResistStuntsMessage.MESSAGE_TYPE, this);
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
							var character = CharacterManager.Instance.FindCharacterByID(aspectDataMessage.characterID);
							resp = GetAspectData(character.FindAspectByID(aspectDataMessage.aspectID));
						}
						break;
					case GetConsequenceDataMessage.MESSAGE_TYPE: {
							var consequenceDataMessage = (GetConsequenceDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(consequenceDataMessage.characterID);
							var consequence = character.FindConsequenceByID(consequenceDataMessage.consequenceID);
							if (consequence != null) resp = GetConsequenceData(consequence);
						}
						break;
					case GetSkillDataMessage.MESSAGE_TYPE: {
							var skillDataMessage = (GetSkillDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(skillDataMessage.characterID);
							resp = GetSkillData(character, SkillType.SkillTypes[skillDataMessage.skillTypeID]);
						}
						break;
					case GetStuntDataMessage.MESSAGE_TYPE: {
							var stuntDataMessage = (GetStuntDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(stuntDataMessage.characterID);
							var stunt = character.FindStuntByID(stuntDataMessage.stuntID);
							if (stunt != null) resp = GetStuntData(stunt);
						}
						break;
					case GetExtraDataMessage.MESSAGE_TYPE: {
							var extraItemDataMessage = (GetExtraDataMessage)request;
							var character = CharacterManager.Instance.FindCharacterByID(extraItemDataMessage.characterID);
							var extra = character.FindExtraByID(extraItemDataMessage.extraID);
							if (extra != null) resp = GetExtraData(extra);
						}
						break;
					case GetDirectResistSkillsMessage.MESSAGE_TYPE: {
							var req = (GetDirectResistSkillsMessage)request;
							resp = GetDirectResistSkills(SkillType.SkillTypes[req.initiativeSkillTypeID], req.actionType);
						}
						break;
					case GetDirectResistStuntsMessage.MESSAGE_TYPE: {
							var req = (GetDirectResistStuntsMessage)request;
							resp = GetDirectResistStunts(SkillType.SkillTypes[req.initiativeSkillTypeID], CharacterManager.Instance.FindCharacterByID(req.passiveCharacterID), req.actionType);
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
			message.describable = StreamableFactory.CreateDescribable(character);
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
					if (character.Consequences == null) {
						message.properties = new CharacterPropertyDescription[0];
					} else {
						message.properties = new CharacterPropertyDescription[character.Consequences.Count];
						for (int i = 0; i < character.Consequences.Count; ++i)
							message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Consequences[i]);
					}
					break;
				case GetCharacterDataMessage.DataType.EXTRAS:
					message = new CharacterExtrasDescriptionMessage();
					if (character.Extras == null) {
						message.properties = new CharacterPropertyDescription[0];
					} else {
						message.properties = new CharacterPropertyDescription[character.Extras.Count];
						for (int i = 0; i < character.Extras.Count; ++i)
							message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Extras[i]);
					}
					break;
				case GetCharacterDataMessage.DataType.STUNTS:
					message = new CharacterStuntsDescriptionMessage();
					if (character.Stunts == null) {
						message.properties = new CharacterPropertyDescription[0];
					} else {
						message.properties = new CharacterPropertyDescription[character.Stunts.Count];
						for (int i = 0; i < character.Stunts.Count; ++i)
							message.properties[i] = StreamableFactory.CreateCharacterPropertyDescription(character.Stunts[i]);
					}
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
			message.benefiterID = aspect.Benefiter == null ? "" : aspect.Benefiter.ID;
			message.benefitTimes = aspect.BenefitTimes;
			return message;
		}

		private Message GetConsequenceData(Consequence consequence) {
			var message = new ConsequenceDataMessage();
			message.characterID = consequence.Belong == null ? "" : consequence.Belong.ID;
			message.consequenceID = consequence.ID;
			message.persistenceType = (int)consequence.PersistenceType;
			message.benefitCharacterID = consequence.Benefiter == null ? "" : consequence.Benefiter.ID;
			message.benefitTimes = consequence.BenefitTimes;
			message.counteractLevel = consequence.CounteractLevel;
			message.mentalDamage = consequence.MentalDamage;
			return message;
		}

		private Message GetSkillData(Character character, SkillType skillType) {
			var message = new SkillDataMessage();
			var skill = character.GetSkill(skillType);
			message.characterID = character.ID;
			message.skillTypeID = skillType.ID;
			message.customName = skill.Name;
			message.level = skill.Level;
			return message;
		}

		private Message GetStuntData(Stunt stunt) {
			var message = new StuntDataMessage();
			message.characterID = stunt.Belong == null ? "" : stunt.Belong.ID;
			message.stuntID = stunt.ID;
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

		private Message GetDirectResistSkills(SkillType initiativeSkillType, CharacterAction action) {
			var message = new DirectResistSkillsListMessage();
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

		private Message GetDirectResistStunts(SkillType initiativeSkillType, Character passive, CharacterAction action) {
			var message = new DirectResistStuntsListMessage();
			List<Stunt> resistables = new List<Stunt>();
			if (passive.Stunts != null) {
				foreach (var stunt in passive.Stunts) {
					if (stunt.CanResistSkillWithoutDMCheck(initiativeSkillType, action)) {
						resistables.Add(stunt);
					}
				}
			}
			message.characterID = passive.ID;
			message.stunts = new CharacterPropertyDescription[resistables.Count];
			for (int i = 0; i < resistables.Count; ++i) {
				message.stunts[i] = StreamableFactory.CreateCharacterPropertyDescription(resistables[i]);
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
