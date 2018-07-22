using GameLogic.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public interface IPlayerClientEventHandler
    {
        void BindHandler(PlayerClient client);
    }

    public sealed class PlayerClient
    {
        public enum PlayerClientScene
        {
            STORYBOARD,
            BATTLEGROUND
        }
        
        private readonly PlayerStoryboard _storyboard;
        private readonly PlayerBattleground _battleground;
        
        public PlayerStoryboard Storyboard => _storyboard;
        public PlayerBattleground Battleground => _battleground;

        public PlayerClient()
        {
            _storyboard = new PlayerStoryboard(this);
            _battleground = new PlayerBattleground(this);
        }

        public void AddClientEventHandler(IPlayerClientEventHandler handler)
        {
            handler.BindHandler(this);
        }

        public void UseScene(PlayerClientScene scene)
        {
            switch (scene)
            {
                case PlayerClientScene.STORYBOARD:
                    break;
                case PlayerClientScene.BATTLEGROUND:
                    break;
                default:
                    break;
            }
        }

    }

    public sealed class PlayerStoryboard : Storyboard
    {
        private readonly PlayerClient _gameClient;

        public PlayerClient GameClient => _gameClient;

        public PlayerStoryboard(PlayerClient parent)
        {
            _gameClient = parent;
        }
        
        public event EventHandler<SceneObjectEventArgs> OnObjInteract;
        public event EventHandler<SceneObjectEventArgs> OnObjCreateAspect;
        public event EventHandler<SceneObjectEventArgs> OnObjAttack;
        public event EventHandler<SceneObjectEventArgs> OnObjSupport;

    }

    public sealed class PlayerBattleground : Battleground
    {
        private readonly PlayerClient _gameClient;

        public PlayerClient GameClient => _gameClient;

        public PlayerBattleground(PlayerClient parent)
        {
            _gameClient = parent;
        }

    }

}
