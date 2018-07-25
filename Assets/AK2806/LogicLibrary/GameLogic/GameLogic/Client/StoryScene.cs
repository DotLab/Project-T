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

    public sealed class TextBox : ClientComponent, IMessageReceiver
    {
        public void MessageReceived(long timestamp, Streamable message)
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
            _connectionRef.AddMessageReceiver(TextSelectedMessage.MESSAGE_ID, this);
        }

        public void AppendParagraph(string text)
        {
            TextBoxAddParagraphMessage message = new TextBoxAddParagraphMessage();
            message.text = text;
            _connectionRef.SendMessage(message);
        }

        public void AppendSelectableParagraph(string text, int selection)
        {
            TextBoxAddSelectionMessage message = new TextBoxAddSelectionMessage();
            message.text = text;
            message.selectionCode = selection;
            _connectionRef.SendMessage(message);
        }

        public void Clear()
        {
            TextBoxClearMessage message = new TextBoxClearMessage();
            _connectionRef.SendMessage(message);
        }

        public void SetCharacterView(CharacterView view)
        {
            TextBoxSetPortraitMessage message = new TextBoxSetPortraitMessage();
            message.view = view;
            _connectionRef.SendMessage(message);
        }

        public void SetCharacterViewEffect(CharacterViewEffect effect)
        {
            TextBoxPortraitEffectMessage message = new TextBoxPortraitEffectMessage();
            message.effect = effect;
            _connectionRef.SendMessage(message);
        }

        public void SetPortraitStyle(PortraitStyle portrait)
        {
            TextBoxPortraitStyleMessage messsage = new TextBoxPortraitStyleMessage();
            messsage.style = portrait;
            _connectionRef.SendMessage(messsage);
        }

    }

    public class StoryScene : ClientComponent
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
            _connectionRef.SendMessage(message);
        }

        public void AddObject(IStoryObject obj)
        {
            StorySceneObjectAddMessage message = new StorySceneObjectAddMessage();
            message.objID = obj.ID;
            message.view = obj.CharacterRef.View;
            _connectionRef.SendMessage(message);
        }

        public void RemoveObject(IStoryObject obj)
        {
            StorySceneObjectRemoveMessage message = new StorySceneObjectRemoveMessage();
            message.objID = obj.ID;
            _connectionRef.SendMessage(message);
        }

        public void TransformObject(IStoryObject obj, Layout to)
        {
            StorySceneObjectTransformMessage message = new StorySceneObjectTransformMessage();
            message.objID = obj.ID;
            message.to = to;
            _connectionRef.SendMessage(message);
        }
        
        public void SetObjectViewEffect(IStoryObject obj, CharacterViewEffect effect)
        {
            StorySceneObjectViewEffectMessage message = new StorySceneObjectViewEffectMessage();
            message.objID = obj.ID;
            message.effect = effect;
            _connectionRef.SendMessage(message);
        }

        public void SetObjectPortraitStyle(IStoryObject obj, PortraitStyle portrait)
        {
            StorySceneObjectPortraitStyleMessage message = new StorySceneObjectPortraitStyleMessage();
            message.objID = obj.ID;
            message.portrait = portrait;
            _connectionRef.SendMessage(message);
        }

        public void TransformCamera(Layout to)
        {
            StorySceneCameraTransformMessage message = new StorySceneCameraTransformMessage();
            message.to = to;
            _connectionRef.SendMessage(message);
        }

        public void SetCameraEffect(CameraEffect effect)
        {
            StorySceneCameraEffectMessage message = new StorySceneCameraEffectMessage();
            message.effect = effect;
            _connectionRef.SendMessage(message);
        }

    }

    public sealed class DMStoryScene : StoryScene, IMessageReceiver
    {
        public DMStoryScene(Connection connection, User owner) :
            base(connection, owner)
        {
            _connectionRef.AddMessageReceiver(StorySceneNextActionMessage.MESSAGE_ID, this);
        }

        public void MessageReceived(long timestamp, Streamable message)
        {
            this.OnNextAction();
        }

        private void OnNextAction()
        {

        }

    }

    public sealed class PlayerStoryScene : StoryScene, IMessageReceiver
    {
        public PlayerStoryScene(Connection connection, User owner) :
            base(connection, owner)
        {
            _connectionRef.AddMessageReceiver(StorySceneObjectActionMessage.MESSAGE_ID, this);
        }
        
        public void MessageReceived(long timestamp, Streamable message)
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
