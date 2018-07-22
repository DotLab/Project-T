﻿using System;
using System.Collections.Generic;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.CharacterSystem;
using GameLogic.Scene.Story;
using GameLogic.Campaign;
using System.Numerics;

namespace GameLogic.Scene
{
    public sealed class StoryScene : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI<StoryScene>
        {
            private readonly StoryScene _outer;

            public API(StoryScene outer)
            {
                _outer = outer;
            }

            public IJSAPI<SceneObject> createSceneObject(string id, IJSAPI<Character> character)
            {
                try
                {
                    Character originCharacter = (Character)JSContextHelper.Instance.GetAPIOrigin(character);
                    SceneObject sceneObject = _outer.CreateSceneObject(id, originCharacter);
                    return (IJSAPI<SceneObject>)sceneObject.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public bool addToScene(IJSAPI<ISceneObject> sceneObj)
            {
                try
                {
                    ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
                    return _outer.AddIntoScene(originSceneObj);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public IJSAPI<ISceneObject> getObject(string id)
            {
                try
                {
                    ISceneObject sceneObject = _outer.ObjList[id];
                    return (IJSAPI<ISceneObject>)sceneObject.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public bool isInScene(IJSAPI<ISceneObject> sceneObj)
            {
                try
                {
                    ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
                    return _outer.ObjInSceneList.Contains(originSceneObj);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public bool isInScene(string id)
            {
                try
                {
                    return _outer.ObjInSceneList.Contains(id);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public bool removeFromScene(IJSAPI<ISceneObject> sceneObj)
            {
                try
                {
                    ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
                    return _outer.RemoveFromScene(originSceneObj);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public bool removeFromScene(string id)
            {
                try
                {
                    return _outer.RemoveFromScene(id);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public void resetScene()
            {
                try
                {
                    _outer.ResetScene();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }
            
            public IJSAPI<Camera> getCamera()
            {
                try
                {
                    return (IJSAPI<Camera>)_outer.Camera.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<TextBox> getTextBox(int index)
            {
                try
                {
                    return (IJSAPI<TextBox>)_outer.TextBoxes[index].GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<Player> getPlayer(int index)
            {
                try
                {
                    return (IJSAPI<Player>)_outer.Players[index].GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public StoryScene Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        private static readonly StoryScene _instance = new StoryScene();
        public static StoryScene Instance => _instance;
        
        private readonly IdentifiedObjList<ISceneObject> _objList;
        private readonly IdentifiedObjList<ISceneObject> _objInSceneList;
        private readonly Camera _camera;
        private readonly List<TextBox> _textBoxes;
        private readonly List<Player> _players;

        public IdentifiedObjList<ISceneObject> ObjList => _objList;
        public IdentifiedObjList<ISceneObject> ObjInSceneList => _objInSceneList;
        public Camera Camera => _camera;
        public List<TextBox> TextBoxes => _textBoxes;
        public List<Player> Players => _players;

        public StoryScene()
        {
            _objList = new IdentifiedObjList<ISceneObject>();
            _objInSceneList = new IdentifiedObjList<ISceneObject>();
            _camera = new Camera();
            _textBoxes = new List<TextBox>();
            _players = new List<Player>();
            _apiObj = new API(this);
        }

        public SceneObject CreateSceneObject(string id, Character character)
        {
            SceneObject ret = new SceneObject(id, character);
            _objList.Add(ret);
            return ret;
        }

        public bool AddIntoScene(ISceneObject sceneObject)
        {
            if (!_objList.Contains(sceneObject)) throw new ArgumentException("Scene object is not created by the container.", nameof(sceneObject));
            _objInSceneList.Add(sceneObject);
            throw new NotImplementedException();
            return true;
        }
        
        public bool RemoveFromScene(ISceneObject sceneObject)
        {
            return this.RemoveFromScene(sceneObject.ID);
        }

        public bool RemoveFromScene(string id)
        {
            bool ret = _objInSceneList.Remove(id);
            throw new NotImplementedException();
            return ret;
        }

        public void ResetScene()
        {
            _objList.Clear();
            _objInSceneList.Clear();
            _camera.Reset();
            foreach (TextBox box in _textBoxes)
            {
                box.Clear();
                box.DisplayText();
            }
            throw new NotImplementedException();
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
}

namespace GameLogic.Scene.Story
{
    public interface ISceneObject : IIdentifiable
    {
        void OnInteract();
        void OnCreateAspect();
        void OnAttack();
        void OnHinder();
        Character CharacterRef { get; }
        Layout Layout { get; }
        PortraitStyle Style { get; }
        void TransTo(Layout layout);
        void ChangeStyle(PortraitStyle style);
        void ApplyEffect(StoryViewEffect effect);
    }

    public class SceneObject : ISceneObject
    {
        #region Javascript API class
        protected class API : IJSAPI<SceneObject>
        {
            private readonly SceneObject _outer;

            public API(SceneObject outer)
            {
                _outer = outer;
            }

            public string getID()
            {
                try
                {
                    return _outer.ID;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void setInteractReaction(Action callback)
            {
                try
                {
                    _outer.Interact = new Command(callback);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setCreateAspectReaction(Action callback)
            {
                try
                {
                    _outer.CreateAspect = new Command(callback);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setAttackReaction(Action callback)
            {
                try
                {
                    _outer.Attack = new Command(callback);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setHinderReaction(Action callback)
            {
                try
                {
                    _outer.Hinder = new Command(callback);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSAPI<Character> getCharacter()
            {
                try
                {
                    return (IJSAPI<Character>)_outer.CharacterRef.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public void transTo(Layout layout)
            {
                try
                {
                    _outer.TransTo(layout);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public Vector3 getPosition()
            {
                try
                {
                    return _outer.Layout.pos;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return new Vector3();
                }
            }

            public Quaternion getRotation()
            {
                try
                {
                    return _outer.Layout.rot;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return new Quaternion();
                }
            }

            public Vector3 getScale()
            {
                try
                {
                    return _outer.Layout.sca;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return new Vector3();
                }
            }

            public void changeStyle(PortraitStyle style)
            {
                try
                {
                    _outer.ChangeStyle(style);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public int getAction()
            {
                try
                {
                    return _outer.Style.action;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public int getEmotion()
            {
                try
                {
                    return _outer.Style.emotion;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void applyEffect(StoryViewEffect effect)
            {
                try
                {
                    _outer.ApplyEffect(effect);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public SceneObject Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        protected readonly string _id;
        protected Command _interact;
        protected Command _createAdvantage;
        protected Command _attack;
        protected Command _hinder;
        protected readonly Character _characterRef;
        protected Layout _layout;
        protected PortraitStyle _style;
        protected StoryViewEffect _effect;

        public string ID => _id;
        public Command Interact { get => _interact; set => _interact = value; }
        public Command CreateAspect { get => _createAdvantage; set => _createAdvantage = value; }
        public Command Attack { get => _attack; set => _attack = value; }
        public Command Hinder { get => _hinder; set => _hinder = value; }
        public Character CharacterRef => _characterRef;
        public Layout Layout => _layout;
        public PortraitStyle Style => _style;
        public StoryViewEffect Effect => _effect;
        
        public SceneObject(string id, Character character)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _characterRef = character ?? throw new ArgumentNullException(nameof(character));
            _layout = Layout.INIT;
            _style = PortraitStyle.INIT;
            _effect = StoryViewEffect.INIT;
            _apiObj = new API(this);
        }

        public virtual void ApplyEffect(StoryViewEffect effect)
        {
            _effect = effect;
            throw new NotImplementedException();
        }
        
        public virtual void TransTo(Layout layout)
        {
            _layout = layout;
            throw new NotImplementedException();
        }

        public virtual void ChangeStyle(PortraitStyle style)
        {
            _style = style;
            throw new NotImplementedException();
        }

        public virtual void OnAttack()
        {
            if (_attack != null)
            {
                _attack.DoAction();
            }
        }

        public virtual void OnCreateAspect()
        {
            if (_createAdvantage != null)
            {
                _createAdvantage.DoAction();
            }
        }

        public virtual void OnInteract()
        {
            if (_interact != null)
            {
                _interact.DoAction();
            }
        }

        public virtual void OnHinder()
        {
            if (_hinder != null)
            {
                _hinder.DoAction();
            }
        }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

    public class PictureObject : SceneObject
    {
        #region Javascript API class
        protected new class API : SceneObject.API, IJSAPI<PictureObject>
        {
            private readonly PictureObject _outer;

            public API(PictureObject outer) :
                base(outer)
            {
                _outer = outer;
            }

            PictureObject IJSAPI<PictureObject>.Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;
        
        protected readonly Camera _cameraRef;
        
        public PictureObject(string id, Character character) :
            base(id, character)
        {
        }
        
        public override void TransTo(Layout layout)
        {
            _layout = layout;
            throw new NotImplementedException();
        }
        
        public override IJSContext GetContext()
        {
            return _apiObj;
        }
        
    }

    public sealed class Camera : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI<Camera>
        {
            private readonly Camera _outer;

            public API(Camera outer)
            {
                _outer = outer;
            }

            public void TransTo(Layout layout)
            {
                try
                {
                    _outer.TransTo(layout);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }
            
            public Vector3 getPosition()
            {
                try
                {
                    return _outer.Layout.pos;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return new Vector3();
                }
            }

            public Quaternion getRotation()
            {
                try
                {
                    return _outer.Layout.rot;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return new Quaternion();
                }
            }

            public Vector3 getScale()
            {
                try
                {
                    return _outer.Layout.sca;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return new Vector3();
                }
            }

            public void applyEffect(StoryViewEffect effect)
            {
                try
                {
                    _outer.ApplyEffect(effect);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void reset()
            {
                try
                {
                    _outer.Reset();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public Camera Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        private Layout _layout;
        private StoryViewEffect _effect;

        public Layout Layout => _layout;
        public StoryViewEffect Effect => _effect;

        public Camera()
        {
            _layout = Layout.INIT;
            _effect = StoryViewEffect.INIT;
            _apiObj = new API(this);
        }

        public void TransTo(Layout layout)
        {
            _layout = layout;
            throw new NotImplementedException();
        }

        public void ApplyEffect(StoryViewEffect effect)
        {
            _effect = effect;
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _layout = Layout.INIT;
            _effect = StoryViewEffect.INIT;
            throw new NotImplementedException();
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

    public interface ITextItem : IJSContextProvider
    {
        bool IsReactable { get; }
        void React();
        string Text { get; }
    }

    public class TextItem : ITextItem
    {
        #region Javascript API class
        protected class API : IJSAPI<TextItem>
        {
            private readonly TextItem _outer;

            public API(TextItem outer)
            {
                _outer = outer;
            }

            public void setText(string text)
            {
                try
                {
                    _outer.Text = text;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public string getText()
            {
                try
                {
                    return _outer.Text;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public TextItem Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        protected string _text;

        public bool IsReactable => false;
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }

        public TextItem(string text = "")
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _apiObj = new API(this);
        }

        public virtual void React() { }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }
        
        public virtual void SetContext(IJSContext context) { }
    }

    public class ClickableItem : ITextItem
    {
        #region Javascript API class
        protected class API : IJSAPI<ClickableItem>
        {
            private readonly ClickableItem _outer;

            public API(ClickableItem outer)
            {
                _outer = outer;
            }

            public void setAction(Action action)
            {
                try
                {
                    _outer.Action = new Command(action);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setText(string text)
            {
                try
                {
                    _outer.Text = text;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public string getText()
            {
                try
                {
                    return _outer.Text;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public ClickableItem Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        protected string _text;
        protected Command _action;

        public bool IsReactable => true;
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }
        public Command Action { get => _action; set => _action = value; }

        public ClickableItem(string text = "", string actionCode = null)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            if (actionCode != null)
            {
                _action = new Command(actionCode);
            }
            else
            {
                _action = null;
            }
            _apiObj = new API(this);
        }
        
        public virtual void React()
        {
            if (_action != null)
            {
                _action.DoAction();
            }
        }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }
        
        public virtual void SetContext(IJSContext context) { }
    }

    public sealed class TextBox : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI<TextBox>
        {
            private readonly TextBox _outer;

            public API(TextBox outer)
            {
                _outer = outer;
            }

            public IJSAPI<TextItem> generateText()
            {
                try
                {
                    ITextItem textItem = _outer.GenerateText();
                    return (IJSAPI<TextItem>)textItem.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<ClickableItem> generateButton()
            {
                try
                {
                    ITextItem buttonItem = _outer.GenerateButton();
                    return (IJSAPI<ClickableItem>)buttonItem.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public int getPlayerIndex()
            {
                try
                {
                    return _outer.PlayerIndex;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return -1;
                }
            }

            public void clear()
            {
                try
                {
                    _outer.Clear();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void display()
            {
                try
                {
                    _outer.DisplayText();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public TextBox Origin(JSContextHelper proof)
            {
                try
                {
                    if (proof == JSContextHelper.Instance)
                    {
                        return _outer;
                    }
                    return null;
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        #endregion
        private readonly API _apiObj;

        private readonly List<ITextItem> _textItems;
        private View _portrait;
        private PortraitStyle _portraitStyle;
        private readonly int _playerIndex;
        
        public int PlayerIndex => _playerIndex;
        public View Portrait => _portrait;
        public PortraitStyle PortraitStyle => _portraitStyle;

        public TextBox(int index)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Player index is less than 0.");
            _textItems = new List<ITextItem>();
            _playerIndex = index;
            _apiObj = new API(this);
        }

        public ITextItem GenerateText()
        {
            TextItem ret = new TextItem();
            _textItems.Add(ret);
            return ret;
        }

        public ITextItem GenerateButton()
        {
            ClickableItem ret = new ClickableItem();
            _textItems.Add(ret);
            return ret;
        }

        public void Clear()
        {
            _textItems.Clear();
        }

        public void DisplayText()
        {
            throw new NotImplementedException();
        }

        public void UpdatePortrait(View view)
        {
            _portrait = view;
            throw new NotImplementedException();
        }

        public void UpdatePortraitStyle(PortraitStyle style)
        {
            _portraitStyle = style;
            throw new NotImplementedException();
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }
    
}