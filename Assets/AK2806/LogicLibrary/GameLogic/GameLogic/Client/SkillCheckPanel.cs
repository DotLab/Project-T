using GameLogic.Campaign;
using GameLogic.CharacterSystem;
using GameLogic.Container;
using GameLogic.Container.Story;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public sealed class SkillCheckPanel : ClientComponent
    {
        public SkillCheckPanel(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(SkillSelectedMessage.MESSAGE_TYPE, this);
            _connection.AddMessageReceiver(AspectsSelectedMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(ulong timestamp, Message message)
        {
            if (message.MessageType == SkillSelectedMessage.MESSAGE_TYPE)
            {
                SkillSelectedMessage skillSelectedMessage = (SkillSelectedMessage)message;
                if (SkillType.SkillTypes.TryGetValue(skillSelectedMessage.skillTypeID, out SkillType skillType))
                {
                    this.OnSelectSkill(skillType);
                }
            }
            else if (message.MessageType == StuntSelectedMessage.MESSAGE_TYPE)
            {
                StuntSelectedMessage stuntSelectedMessage = (StuntSelectedMessage)message;
                //stuntSelectedMessage.StuntID;
            }
            else if (message.MessageType == AspectsSelectedMessage.MESSAGE_TYPE)
            {
                List<Aspect> result = new List<Aspect>();
                switch (CampaignManager.Instance.CurrentShot.Type)
                {
                    case ShotType.Battle:
                        {

                        }
                        break;
                    case ShotType.Story:
                    case ShotType.Map:
                        {
                            AspectsSelectedMessage aspectsSelectedMessage = (AspectsSelectedMessage)message;
                            IdentifiedObjList<IStoryObject> storyObjects = StorySceneContainer.Instance.ObjList;

                            foreach (AspectsSelectedMessage.AspectGroup aspectGroup in aspectsSelectedMessage.aspectGroups)
                            {
                                if (storyObjects.TryGetValue(aspectGroup.characterID, out IStoryObject storyObject))
                                {
                                    this.RetrieveAspects(aspectGroup.aspectsID, storyObject.CharacterRef, result);
                                }
                                else
                                {
                                    foreach (IStoryObject obj in storyObjects)
                                    {
                                        if (this.FindAllAspects(aspectGroup.characterID, aspectGroup.aspectsID, obj.CharacterRef, result)) break;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        return;
                }
                this.OnSelectAspects(result);
            }
        }

        private void RetrieveAspects(IEnumerable<string> aspectsID, Character character, List<Aspect> result)
        {
            foreach (string aspectID in aspectsID)
            {
                foreach (Aspect aspect in character.Aspects)
                {
                    if (aspect.ID == aspectID)
                    {
                        result.Add(aspect);
                        break;
                    }
                }
            }
        }

        private bool FindAllAspects(string ownerCharacterID, IEnumerable<string> aspectsID, Character rootCharacter, List<Aspect> result)
        {
            if (rootCharacter.ID == ownerCharacterID)
            {
                this.RetrieveAspects(aspectsID, rootCharacter, result);
                return true;
            }
            foreach (Extra extra in rootCharacter.Extras)
            {
                if (this.FindAllAspects(ownerCharacterID, aspectsID, extra.Item, result)) return true;
            }
            return false;
        }

        private void OnSelectStunt(Stunt stunt)
        {

        }

        private void OnSelectSkill(SkillType skillType)
        {

        }

        private void OnSelectAspects(IEnumerable<Aspect> aspects)
        {

        }

        public void Show(Character initiative, Character passive)
        {
            SkillCheckPanelShowMessage message = new SkillCheckPanelShowMessage();
            message.initiativeCharacterID = initiative.ID;
            message.initiativeView = initiative.View;
            message.passiveCharacterID = passive.ID;
            message.passiveView = passive.View;
            _connection.SendMessage(message);
        }
        
        public void Hide()
        {
            SkillCheckPanelHideMessage message = new SkillCheckPanelHideMessage();
            _connection.SendMessage(message);
        }

        public void DisplayDicePoint(bool isInitiative, int[] dicePoints)
        {

        }
        
        public void DisplaySkill(bool isInitiative, SkillType skillType)
        {

        }

        public void DisplayAspects(bool isInitiative, IEnumerable<Aspect> aspect)
        {

        }
        
        public void DisplayResult(SkillChecker.CheckResult checkResult)
        {

        }

    }
}
