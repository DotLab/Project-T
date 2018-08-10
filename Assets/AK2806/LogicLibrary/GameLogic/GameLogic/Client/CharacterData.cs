using GameLogic.CharacterSystem;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;

namespace GameLogic.Client
{
    public class CharacterData : ClientComponent
    {
        public CharacterData(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(GetCharacterDataMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(GetAspectDataMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(GetConsequenceDataMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(GetSkillDataMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(GetStuntDataMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(GetExtraDataMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(ulong timestamp, Message message)
        {
            switch (message.MessageType)
            {
                case GetCharacterDataMessage.MESSAGE_TYPE:
                    {
                        GetCharacterDataMessage characterDataMessage = (GetCharacterDataMessage)message;
                        this.SendCharacterData(CharacterManager.Instance.FindCharacterByID(characterDataMessage.characterID), characterDataMessage.dataType);
                    }
                    break;
                case GetAspectDataMessage.MESSAGE_TYPE:
                    {
                        GetAspectDataMessage aspectDataMessage = (GetAspectDataMessage)message;
                        Character character = CharacterManager.Instance.FindCharacterByID(aspectDataMessage.characterID);
                        string aspectID = aspectDataMessage.aspectID;
                        foreach (Aspect aspect in character.Aspects)
                        {
                            if (aspect.ID == aspectID)
                            {
                                this.SendAspectData(aspect);
                                break;
                            }
                        }
                    }
                    break;
                case GetConsequenceDataMessage.MESSAGE_TYPE:
                    {
                        GetConsequenceDataMessage consequenceDataMessage = (GetConsequenceDataMessage)message;
                        Character character = CharacterManager.Instance.FindCharacterByID(consequenceDataMessage.characterID);
                        string consequenceID = consequenceDataMessage.consequenceID;
                        foreach (Consequence consequence in character.Aspects)
                        {
                            if (consequence.ID == consequenceID)
                            {
                                this.SendConsequenceData(consequence);
                                break;
                            }
                        }
                    }
                    break;
                case GetSkillDataMessage.MESSAGE_TYPE:
                    {
                        GetSkillDataMessage skillLevelDataMessage = (GetSkillDataMessage)message;
                        Character character = CharacterManager.Instance.FindCharacterByID(skillLevelDataMessage.characterID);
                        this.SendSkillData(character, SkillType.SkillTypes[skillLevelDataMessage.skillTypeID]);
                    }
                    break;
                case GetStuntDataMessage.MESSAGE_TYPE:
                    {
                        GetStuntDataMessage stuntDataMessage = (GetStuntDataMessage)message;
                        Character character = CharacterManager.Instance.FindCharacterByID(stuntDataMessage.characterID);
                        string stuntID = stuntDataMessage.stuntID;
                        foreach (Stunt stunt in character.Stunts)
                        {
                            if (stunt.ID == stuntID)
                            {
                                this.SendStuntData(stunt);
                                break;
                            }
                        }
                    }
                    break;
                case GetExtraDataMessage.MESSAGE_TYPE:
                    {
                        GetExtraDataMessage extraItemDataMessage = (GetExtraDataMessage)message;
                        Character character = CharacterManager.Instance.FindCharacterByID(extraItemDataMessage.characterID);
                        string extraID = extraItemDataMessage.extraID;
                        foreach (Extra extra in character.Extras)
                        {
                            if (extra.ID == extraID)
                            {
                                this.SendExtraData(extra);
                                break;
                            }
                        }
                    }
                    break;
                default:
                    return;
            }
        }

        private void SendCharacterData(Character character, GetCharacterDataMessage.DataType dataType)
        {
            switch (dataType)
            {
                case GetCharacterDataMessage.DataType.INFO:
                    this.SendCharacterInfoData(character);
                    break;
                case GetCharacterDataMessage.DataType.ASPECTS:
                case GetCharacterDataMessage.DataType.CONSEQUENCES:
                case GetCharacterDataMessage.DataType.EXTRAS:
                case GetCharacterDataMessage.DataType.SKILLS:
                case GetCharacterDataMessage.DataType.STUNTS:
                    this.SendCharacterPropertiesDescriptionData(character, dataType);
                    break;
                case GetCharacterDataMessage.DataType.FATEPOINT:
                    this.SendCharacterFatePointData(character);
                    break;
                case GetCharacterDataMessage.DataType.STRESS:
                    this.SendCharacterStressData(character);
                    break;
                case GetCharacterDataMessage.DataType.ALL:
                    this.SendCharacterInfoData(character);
                    this.SendCharacterFatePointData(character);
                    this.SendCharacterStressData(character);
                    this.SendCharacterPropertiesDescriptionData(character, GetCharacterDataMessage.DataType.ASPECTS);
                    this.SendCharacterPropertiesDescriptionData(character, GetCharacterDataMessage.DataType.CONSEQUENCES);
                    this.SendCharacterPropertiesDescriptionData(character, GetCharacterDataMessage.DataType.EXTRAS);
                    this.SendCharacterPropertiesDescriptionData(character, GetCharacterDataMessage.DataType.SKILLS);
                    this.SendCharacterPropertiesDescriptionData(character, GetCharacterDataMessage.DataType.STUNTS);
                    break;
                default:
                    return;
            }
        }

        private void SendCharacterInfoData(Character character)
        {
            CharacterInfoDataMessage message = new CharacterInfoDataMessage();
            message.characterID = character.ID;
            message.describable = character;
            _connection.SendMessage(message);
        }

        private void SendCharacterFatePointData(Character character)
        {
            CharacterFatePointDataMessage message = new CharacterFatePointDataMessage();
            message.characterID = character.ID;
            message.fatePoint = character.FatePoint;
            message.refreshPoint = character.RefreshPoint;
            _connection.SendMessage(message);
        }

        private void SendCharacterStressData(Character character)
        {
            CharacterStressDataMessage message = new CharacterStressDataMessage();
            message.characterID = character.ID;
            message.physicsStress = character.PhysicsStress;
            message.physicsStressMax = character.PhysicsStressMax;
            message.mentalStress = character.MentalStress;
            message.mentalStressMax = character.MentalStressMax;
            _connection.SendMessage(message);
        }

        private void SendCharacterPropertiesDescriptionData(Character character, GetCharacterDataMessage.DataType dataType)
        {
            CharacterPropertiesDescriptionMessage message;
            switch (dataType)
            {
                case GetCharacterDataMessage.DataType.ASPECTS:
                    message = new CharacterAspectsDescriptionMessage();
                    message.properties = new CharacterPropertiesDescriptionMessage.Property[character.Aspects.Count];
                    for (int i = 0; i < character.Aspects.Count; ++i)
                    {
                        message.properties[i] = new CharacterPropertiesDescriptionMessage.Property();
                        message.properties[i].propertyID = character.Aspects[i].ID;
                        message.properties[i].describable = character.Aspects[i];
                    }
                    break;
                case GetCharacterDataMessage.DataType.CONSEQUENCES:
                    message = new CharacterConsequencesDescriptionMessage();
                    message.properties = new CharacterPropertiesDescriptionMessage.Property[character.Consequences.Count];
                    for (int i = 0; i < character.Consequences.Count; ++i)
                    {
                        message.properties[i] = new CharacterPropertiesDescriptionMessage.Property();
                        message.properties[i].propertyID = character.Consequences[i].ID;
                        message.properties[i].describable = character.Consequences[i];
                    }
                    break;
                case GetCharacterDataMessage.DataType.EXTRAS:
                    message = new CharacterExtrasDescriptionMessage();
                    message.properties = new CharacterPropertiesDescriptionMessage.Property[character.Extras.Count];
                    for (int i = 0; i < character.Extras.Count; ++i)
                    {
                        message.properties[i] = new CharacterPropertiesDescriptionMessage.Property();
                        message.properties[i].propertyID = character.Extras[i].ID;
                        message.properties[i].describable = character.Extras[i];
                    }
                    break;
                case GetCharacterDataMessage.DataType.SKILLS:
                    message = new CharacterSkillsDescriptionMessage();
                    message.properties = new CharacterPropertiesDescriptionMessage.Property[character.ReadonlySkillList.Count];
                    for (int i = 0; i < character.ReadonlySkillList.Count; ++i)
                    {
                        message.properties[i] = new CharacterPropertiesDescriptionMessage.Property();
                        message.properties[i].propertyID = character.ReadonlySkillList[i].SkillType.ID;
                        message.properties[i].describable = character.ReadonlySkillList[i];
                    }
                    break;
                case GetCharacterDataMessage.DataType.STUNTS:
                    message = new CharacterStuntsDescriptionMessage();
                    message.properties = new CharacterPropertiesDescriptionMessage.Property[character.Stunts.Count];
                    for (int i = 0; i < character.Stunts.Count; ++i)
                    {
                        message.properties[i] = new CharacterPropertiesDescriptionMessage.Property();
                        message.properties[i].propertyID = character.Stunts[i].ID;
                        message.properties[i].describable = character.Stunts[i];
                    }
                    break;
                default:
                    return;
            }
            message.characterID = character.ID;
            _connection.SendMessage(message);
        }

        private void SendAspectData(Aspect aspect)
        {
            AspectDataMessage message = new AspectDataMessage();
            message.characterID = aspect.Belong == null ? "" : aspect.Belong.ID;
            message.aspectID = aspect.ID;
            message.persistenceType = (int)aspect.PersistenceType;
            message.benefitCharacterID = aspect.Benefit == null ? "" : aspect.Benefit.ID;
            message.benefitTimes = aspect.BenefitTimes;
            _connection.SendMessage(message);
        }

        private void SendConsequenceData(Consequence consequence)
        {
            ConsequenceDataMessage message = new ConsequenceDataMessage();
            message.characterID = consequence.Belong == null ? "" : consequence.Belong.ID;
            message.consequenceID = consequence.ID;
            message.persistenceType = (int)consequence.PersistenceType;
            message.benefitCharacterID = consequence.Benefit == null ? "" : consequence.Benefit.ID;
            message.benefitTimes = consequence.BenefitTimes;
            message.counteractLevel = consequence.CounteractLevel;
            _connection.SendMessage(message);
        }

        private void SendSkillData(Character character, SkillType skillType)
        {
            SkillDataMessage message = new SkillDataMessage();
            message.characterID = character.ID;
            message.skillTypeID = skillType.ID;
            message.skillProperty = character.GetSkillProperty(skillType);
            _connection.SendMessage(message);
        }

        private void SendStuntData(Stunt stunt)
        {
            StuntDataMessage message = new StuntDataMessage();
            message.characterID = stunt.Belong == null ? "" : stunt.Belong.ID;
            message.stuntID = stunt.ID;
            message.boundSkillTypeID = stunt.BoundSkillType.ID;
            message.needDMCheck = stunt.NeedDMCheck;
            _connection.SendMessage(message);
        }

        private void SendExtraData(Extra extra)
        {
            ExtraDataMessage message = new ExtraDataMessage();
            message.characterID = extra.Belong == null ? "" : extra.Belong.ID;
            message.extraID = extra.ID;
            message.itemID = extra.Item.ID;
            message.isTool = extra.IsTool;
            message.isLongRangeWeapon = extra.IsLongRangeWeapon;
            message.isVehicle = extra.IsVehicle;
            _connection.SendMessage(message);
        }
    }
}
