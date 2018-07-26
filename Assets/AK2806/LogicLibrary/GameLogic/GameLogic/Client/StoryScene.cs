using GameLogic.Core;
using GameLogic.Container.Story;
using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;

namespace GameLogic.Client
{

    public sealed class TextBox : ClientComponent
    {
        public override void MessageReceived(ulong timestamp, Message message)
        {
            TextSelectedMessage selectedMessage = (TextSelectedMessage)message;
            this.OnSelectItem(selectedMessage.selection);
        }

        private void OnSelectItem(int selection)
        {

        }

        public TextBox(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(TextSelectedMessage.MESSAGE_TYPE, this);
        }

        public void AppendParagraph(string text)
        {
            TextBoxAddParagraphMessage message = new TextBoxAddParagraphMessage();
            message.text = text;
            _connection.SendMessage(message);
        }

        public void AppendSelectableParagraph(string text, int selection)
        {
            TextBoxAddSelectionMessage message = new TextBoxAddSelectionMessage();
            message.text = text;
            message.selectionCode = selection;
            _connection.SendMessage(message);
        }

        public void Clear()
        {
            TextBoxClearMessage message = new TextBoxClearMessage();
            _connection.SendMessage(message);
        }

        public void SetCharacterView(CharacterView view)
        {
            TextBoxSetPortraitMessage message = new TextBoxSetPortraitMessage();
            message.view = view;
            _connection.SendMessage(message);
        }

        public void SetCharacterViewEffect(CharacterViewEffect effect)
        {
            TextBoxPortraitEffectMessage message = new TextBoxPortraitEffectMessage();
            message.effect = effect;
            _connection.SendMessage(message);
        }

        public void SetPortraitStyle(PortraitStyle portrait)
        {
            TextBoxPortraitStyleMessage messsage = new TextBoxPortraitStyleMessage();
            messsage.style = portrait;
            _connection.SendMessage(messsage);
        }

    }

    public abstract class StoryScene : ClientComponent
    {
        protected readonly TextBox _textBox;

        public TextBox TextBox => _textBox;
        
        protected StoryScene(Connection connection, User owner) :
            base(connection, owner)
        {
            _textBox = new TextBox(connection, owner);
        }

        public void Reset()
        {
            StorySceneResetMessage message = new StorySceneResetMessage();
            _connection.SendMessage(message);
        }

        public void AddObject(IStoryObject obj)
        {
            StorySceneObjectAddMessage message = new StorySceneObjectAddMessage();
            message.objID = obj.ID;
            message.view = obj.CharacterRef.View;
            _connection.SendMessage(message);
        }

        public void RemoveObject(IStoryObject obj)
        {
            StorySceneObjectRemoveMessage message = new StorySceneObjectRemoveMessage();
            message.objID = obj.ID;
            _connection.SendMessage(message);
        }

        public void TransformObject(IStoryObject obj, Layout to)
        {
            StorySceneObjectTransformMessage message = new StorySceneObjectTransformMessage();
            message.objID = obj.ID;
            message.to = to;
            _connection.SendMessage(message);
        }
        
        public void SetObjectViewEffect(IStoryObject obj, CharacterViewEffect effect)
        {
            StorySceneObjectViewEffectMessage message = new StorySceneObjectViewEffectMessage();
            message.objID = obj.ID;
            message.effect = effect;
            _connection.SendMessage(message);
        }

        public void SetObjectPortraitStyle(IStoryObject obj, PortraitStyle portrait)
        {
            StorySceneObjectPortraitStyleMessage message = new StorySceneObjectPortraitStyleMessage();
            message.objID = obj.ID;
            message.portrait = portrait;
            _connection.SendMessage(message);
        }

        public void TransformCamera(Layout to)
        {
            StorySceneCameraTransformMessage message = new StorySceneCameraTransformMessage();
            message.to = to;
            _connection.SendMessage(message);
        }

        public void SetCameraEffect(CameraEffect effect)
        {
            StorySceneCameraEffectMessage message = new StorySceneCameraEffectMessage();
            message.effect = effect;
            _connection.SendMessage(message);
        }

    }

    public sealed class DMStoryScene : StoryScene
    {
        public DMStoryScene(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(StorySceneNextActionMessage.MESSAGE_TYPE, this);
        }

        public override void MessageReceived(ulong timestamp, Message message)
        {
            this.OnNextAction();
        }

        private void OnNextAction()
        {

        }

    }

    public sealed class PlayerStoryScene : StoryScene
    {
        public PlayerStoryScene(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(StorySceneObjectActionMessage.MESSAGE_TYPE, this);
        }
        
        public override void MessageReceived(ulong timestamp, Message message)
        {
            StorySceneObjectActionMessage objectMessage = (StorySceneObjectActionMessage)message;
            switch (objectMessage.action)
            {
                case StorySceneObjectActionMessage.PlayerAction.INTERACT:
                    this.OnInteract(objectMessage.objectID);
                    break;
                case StorySceneObjectActionMessage.PlayerAction.CREATE_ASPECT:
                    this.OnCreateAspect(objectMessage.objectID);
                    break;
                case StorySceneObjectActionMessage.PlayerAction.ATTACK:
                    this.OnAttack(objectMessage.objectID);
                    break;
                case StorySceneObjectActionMessage.PlayerAction.HINDER:
                    this.OnHinder(objectMessage.objectID);
                    break;
                default:
                    break;
            }
        }

        private void OnInteract(string objID)
        {

        }

        private void OnCreateAspect(string objID)
        {

        }

        private void OnAttack(string objID)
        {

        }

        private void OnHinder(string objID)
        {

        }


    }

}
