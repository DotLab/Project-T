using GameLogic.Core;
using GameLogic.Container.Story;
using System;
using System.Collections.Generic;
using System.Text;
using GameLogic.Core.Network;

namespace GameLogic.Client
{

    public sealed class TextBox
    {
        private readonly Connection _networkRef;

        public event EventHandler<TextSelectedMessage> SelectItem;

        public TextBox(Connection connection)
        {
            _networkRef = connection;
        }

        public void AppendParagraph(string text)
        {

        }

        public void AppendSelectableParagraph(string text, int selection)
        {

        }

        public void Clear()
        {

        }

        public void SetCharacterView(CharacterView view)
        {

        }

        public void SetCharacterViewEffect(CharacterViewEffect effect)
        {

        }

        public void SetPortraitStyle(PortraitStyle portrait)
        {

        }

    }

    public class StoryScene
    {
        protected readonly Connection _networkRef;
        protected readonly TextBox _textBox;

        public TextBox TextBox => _textBox;

        public event EventHandler<StoryboardObjectMessage> Interact;
        public event EventHandler<StoryboardObjectMessage> CreateAspect;
        public event EventHandler<StoryboardObjectMessage> Attack;
        public event EventHandler<StoryboardObjectMessage> Hinder;

        protected StoryScene(Connection connection)
        {
            _networkRef = connection;
            _textBox = new TextBox(connection);
        }

        public void Reset()
        {

        }

        public void AddObject(IStoryObject obj)
        {

        }

        public void RemoveObject(IStoryObject obj)
        {

        }

        public void TransformObject(IStoryObject obj, Layout to)
        {

        }

        public void ShowObject(IStoryObject obj)
        {

        }

        public void HideObject(IStoryObject obj)
        {

        }

        public void SetObjectViewEffect(IStoryObject obj, CharacterViewEffect effect)
        {

        }

        public void SetObjectPortraitStyle(IStoryObject obj, PortraitStyle portrait)
        {

        }

        public void TransformCamera(Layout to)
        {

        }

        public void SetCameraEffect(CameraEffect effect)
        {

        }

        public void PlayBGM(string id)
        {

        }

        public void StopBGM()
        {

        }

        public void PlaySE(string id)
        {

        }

    }

    public sealed class DMStoryScene : StoryScene
    {
        private readonly DMClient _gameClient;

        public DMClient GameClient => _gameClient;

        public DMStoryScene(Connection connection, DMClient parent) :
            base(connection)
        {
            _gameClient = parent;
        }

        public event EventHandler OnNextSceneAction;

    }

    public sealed class PlayerStoryScene : StoryScene
    {
        private readonly PlayerClient _gameClient;

        public PlayerClient GameClient => _gameClient;

        public PlayerStoryScene(Connection connection, PlayerClient parent) :
            base(connection)
        {
            _gameClient = parent;
        }



    }

}
