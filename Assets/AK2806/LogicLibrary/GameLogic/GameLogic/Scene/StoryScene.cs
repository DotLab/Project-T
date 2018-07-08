using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.Character;

namespace GameLogic.Scene
{
    public interface IStoryObject : IIdentifiable
    {
        void OnInteract();
        void OnCreateAdvantage();
        void OnAttack();
        void OnSupport();
        ICharacter Character { get; }
        void ChangeStyle(StyleParam param);
        void ApplyEffect(ViewEffect effect);
    }

    public class StoryObject : IStoryObject
    {
        protected readonly string _id;
        protected ICommand _interact;
        protected ICommand _createAdvantage;
        protected ICommand _attack;
        protected ICommand _support;
        protected ICharacter _characterRef;
        protected StyleParam _style;
        protected ViewEffect _effect;

        public ICommand Interact { get => _interact; set => _interact = value; }
        public ICommand CreateAdvantage { get => _createAdvantage; set => _createAdvantage = value; }
        public ICommand Attack { get => _attack; set => _attack = value; }
        public ICommand Support { get => _support; set => _support = value; }
        public ICharacter CharacterRef { get => _characterRef; set => _characterRef = value; }
        public StyleParam Style { get => _style; set => _style = value; }
        public ViewEffect Effect { get => _effect; set => _effect = value; }

        public string ID => _id;
        public ICharacter Character { get => _characterRef; set => _characterRef = value; }
        
        public virtual void ApplyEffect(ViewEffect effect)
        {
            throw new NotImplementedException();
        }

        public virtual void ChangeStyle(StyleParam param)
        {
            throw new NotImplementedException();
        }

        public object GetContext()
        {
            throw new NotImplementedException();
        }

        public virtual void OnAttack()
        {
            throw new NotImplementedException();
        }

        public virtual void OnCreateAdvantage()
        {
            throw new NotImplementedException();
        }

        public virtual void OnInteract()
        {
            throw new NotImplementedException();
        }

        public virtual void OnSupport()
        {
            throw new NotImplementedException();
        }

        public void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StoryCamera
    {
        private Vector3 _pos;
        private Quaternion _rot;
        private ViewEffect _effect;

        public Vector3 Pos { get => _pos; set => _pos = value; }
        public Quaternion Rot { get => _rot; set => _rot = value; }
        public ViewEffect Effect { get => _effect; set => _effect = value; }

        public void ApplyEffect(ViewEffect effect)
        {
            throw new NotImplementedException();
        }

    }

    public sealed class StoryScene : JSContext
    {
        private List<IStoryObject> _objList;
        private StoryCamera _camera;

        public List<IStoryObject> ObjList { get => _objList; set => _objList = value; }
        public StoryCamera Camera { get => _camera; set => _camera = value; }

        public object GetContext()
        {
            throw new NotImplementedException();
        }

        public void SetContext(object context)
        {
            throw new NotImplementedException();
        }
    }
}
