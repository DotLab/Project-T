using System;
using System.Collections.Generic;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using GameLogic.CharacterSystem;
using GameLogic.Container.StoryComponent;
using System.Numerics;

namespace GameLogic.Container
{
    public sealed class StorySceneContainer : IJSContextProvider
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<StorySceneContainer>
        {
            private readonly StorySceneContainer _outer;

            public JSAPI(StorySceneContainer outer)
            {
                _outer = outer;
            }

            public IJSAPI<StoryObject> createStoryObject(IJSAPI<Character> character)
            {
                try
                {
                    Character originCharacter = JSContextHelper.Instance.GetAPIOrigin(character);
                    StoryObject sceneObject = _outer.CreateStoryObject(originCharacter);
                    return (IJSAPI<StoryObject>)sceneObject.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public bool addToScene(IJSAPI<IStoryObject> sceneObj)
            {
                try
                {
                    IStoryObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
                    return _outer.AddIntoScene(originSceneObj);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public IJSAPI<IStoryObject> getObject(string id)
            {
                try
                {
                    IStoryObject sceneObject = _outer.ObjList[id];
                    return (IJSAPI<IStoryObject>)sceneObject.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public bool isInScene(IJSAPI<IStoryObject> sceneObj)
            {
                try
                {
                    IStoryObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
                    return _outer.ObjList.Contains(originSceneObj);
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
                    return _outer.ObjList.Contains(id);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public bool removeFromScene(IJSAPI<IStoryObject> sceneObj)
            {
                try
                {
                    IStoryObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj);
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
            
            public StorySceneContainer Origin(JSContextHelper proof)
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
        private readonly JSAPI _apiObj;

        private static readonly StorySceneContainer _instance = new StorySceneContainer();
        public static StorySceneContainer Instance => _instance;
        
        private readonly IdentifiedObjList<IStoryObject> _objList;
        private readonly IdentifiedObjList<Character> _playerCharacters;
        private readonly Camera _camera;
        private readonly List<TextBox> _textBoxes;

        public IdentifiedObjList<IStoryObject> ObjList => _objList;
        public IdentifiedObjList<Character> PlayerCharacters => _playerCharacters;
        public Camera Camera => _camera;
        public List<TextBox> TextBoxes => _textBoxes;
        
        public StorySceneContainer()
        {
            _objList = new IdentifiedObjList<IStoryObject>();
            _playerCharacters = new IdentifiedObjList<Character>();
            _camera = new Camera();
            _textBoxes = new List<TextBox>();
            _apiObj = new JSAPI(this);
        }

        public StoryObject CreateStoryObject(Character character)
        {
            StoryObject ret = new StoryObject(character);
            return ret;
        }

        public void AddPlayerCharacter(Character playerCharacter)
        {
            _playerCharacters.Add(playerCharacter);
        }

        public bool RemovePlayerCharacter(Character playerCharacter)
        {
            return this.RemovePlayerCharacter(playerCharacter.ID);
        }

        public bool RemovePlayerCharacter(string id)
        {
            bool ret = _playerCharacters.Remove(id);
            throw new NotImplementedException();
            return ret;
        }

        public bool AddIntoScene(IStoryObject sceneObject)
        {
            _objList.Add(sceneObject);
            throw new NotImplementedException();
            return true;
        }
        
        public bool RemoveFromScene(IStoryObject sceneObject)
        {
            return this.RemoveFromScene(sceneObject.ID);
        }

        public bool RemoveFromScene(string id)
        {
            bool ret = _objList.Remove(id);
            throw new NotImplementedException();
            return ret;
        }

        public void ResetScene()
        {
            _objList.Clear();
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

namespace GameLogic.Container.StoryComponent
{
    public interface IStoryObject : IIdentifiable
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
        void ApplyEffect(CharacterViewEffect effect);
    }

    public class StoryObject : IStoryObject
    {
        #region Javascript API class
        protected class JSAPI : IJSAPI<StoryObject>
        {
            private readonly StoryObject _outer;

            public JSAPI(StoryObject outer)
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

            public void applyEffect(CharacterViewEffect effect)
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

            public StoryObject Origin(JSContextHelper proof)
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
        private readonly JSAPI _apiObj;
        
        protected Command _interact;
        protected Command _createAspect;
        protected Command _attack;
        protected Command _hinder;
        protected readonly Character _characterRef;
        protected Layout _layout;
        protected PortraitStyle _style;
        protected CharacterViewEffect _effect;

        public string ID => _characterRef.ID;
        public string Name { get => _characterRef.Name; set { } }
        public string Description { get => _characterRef.Description; set { } }
        public Command Interact { get => _interact; set => _interact = value; }
        public Command CreateAspect { get => _createAspect; set => _createAspect = value; }
        public Command Attack { get => _attack; set => _attack = value; }
        public Command Hinder { get => _hinder; set => _hinder = value; }
        public Character CharacterRef => _characterRef;
        public Layout Layout => _layout;
        public PortraitStyle Style => _style;
        public CharacterViewEffect Effect => _effect;

        public StoryObject(Character character)
        {
            _characterRef = character ?? throw new ArgumentNullException(nameof(character));
            _layout = Layout.INIT;
            _style = PortraitStyle.INIT;
            _effect = CharacterViewEffect.INIT;
            _apiObj = new JSAPI(this);
        }

        public virtual void ApplyEffect(CharacterViewEffect effect)
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
            if (_createAspect != null)
            {
                _createAspect.DoAction();
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

    public class PictureObject : StoryObject
    {
        #region Javascript API class
        protected new class JSAPI : StoryObject.JSAPI, IJSAPI<PictureObject>
        {
            private readonly PictureObject _outer;

            public JSAPI(PictureObject outer) :
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
        private readonly JSAPI _apiObj;
        
        protected readonly Camera _cameraRef;
        
        public PictureObject(Character character) :
            base(character)
        {
            _apiObj = new JSAPI(this);
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
        private sealed class JSAPI : IJSAPI<Camera>
        {
            private readonly Camera _outer;

            public JSAPI(Camera outer)
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

            public void applyEffect(CharacterViewEffect effect)
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
        private readonly JSAPI _apiObj;

        private Layout _layout;
        private CharacterViewEffect _effect;

        public Layout Layout => _layout;
        public CharacterViewEffect Effect => _effect;

        public Camera()
        {
            _layout = Layout.INIT;
            _effect = CharacterViewEffect.INIT;
            _apiObj = new JSAPI(this);
        }

        public void TransTo(Layout layout)
        {
            _layout = layout;
            throw new NotImplementedException();
        }

        public void ApplyEffect(CharacterViewEffect effect)
        {
            _effect = effect;
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _layout = Layout.INIT;
            _effect = CharacterViewEffect.INIT;
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
        protected class JSAPI : IJSAPI<TextItem>
        {
            private readonly TextItem _outer;

            public JSAPI(TextItem outer)
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
        private readonly JSAPI _apiObj;

        protected string _text;

        public bool IsReactable => false;
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }

        public TextItem(string text = "")
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _apiObj = new JSAPI(this);
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
        protected class JSAPI : IJSAPI<ClickableItem>
        {
            private readonly ClickableItem _outer;

            public JSAPI(ClickableItem outer)
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
        private readonly JSAPI _apiObj;

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
            _apiObj = new JSAPI(this);
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
        private sealed class JSAPI : IJSAPI<TextBox>
        {
            private readonly TextBox _outer;

            public JSAPI(TextBox outer)
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
        private readonly JSAPI _apiObj;

        private readonly List<ITextItem> _textItems;
        private CharacterView _portrait;
        private PortraitStyle _portraitStyle;
        private readonly int _playerIndex;
        
        public int PlayerIndex => _playerIndex;
        public CharacterView Portrait => _portrait;
        public PortraitStyle PortraitStyle => _portraitStyle;

        public TextBox(int index)
        {
            _playerIndex = index >= 0 ? index : throw new ArgumentOutOfRangeException(nameof(index), "Player index is less than 0.");
            _textItems = new List<ITextItem>();
            _apiObj = new JSAPI(this);
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

        public void UpdatePortrait(CharacterView view)
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