using System;
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
        private sealed class API : IJSAPI
        {
            private readonly StoryScene _outer;

            public API(StoryScene outer)
            {
                _outer = outer;
            }

            public IJSAPI createObject(string id, IJSAPI character)
            {
                try
                {
                    Character originCharacter = JSContextHelper.Instance.GetAPIOrigin(character) as Character;
                    if (originCharacter != null)
                    {
                        ISceneObject sceneObject = _outer.CreateSceneObject(id, originCharacter);
                        return (IJSAPI)sceneObject.GetContext();
                    }
                    return null;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public bool addToScene(IJSAPI sceneObj)
            {
                try
                {
                    ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj) as ISceneObject;
                    if (originSceneObj != null)
                    {
                        return _outer.AddIntoScene(originSceneObj);
                    }
                    return false;
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return false;
                }
            }

            public IJSAPI getObject(string id)
            {
                try
                {
                    ISceneObject sceneObject = _outer.ObjList[id];
                    return (IJSAPI)sceneObject.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }
            
            public bool isInScene(IJSAPI sceneObj)
            {
                try
                {
                    ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj) as ISceneObject;
                    if (originSceneObj != null)
                    {
                        return _outer.ObjInSceneList.Contains(originSceneObj);
                    }
                    return false;
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

            public bool removeFromScene(IJSAPI sceneObj)
            {
                try
                {
                    ISceneObject originSceneObj = JSContextHelper.Instance.GetAPIOrigin(sceneObj) as ISceneObject;
                    if (originSceneObj != null)
                    {
                        return _outer.RemoveFromScene(originSceneObj);
                    }
                    return false;
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
            
            public IJSAPI getCamera()
            {
                try
                {
                    return (IJSAPI)_outer.Camera.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getTextBox(int index)
            {
                try
                {
                    return (IJSAPI)_outer.TextBoxes[index].GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI getPlayer(int index)
            {
                try
                {
                    return (IJSAPI)_outer.Players[index].GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSContextProvider Origin(JSContextHelper proof)
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

        public ISceneObject CreateSceneObject(string id, Character character)
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
                box.Display();
            }
            throw new NotImplementedException();
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
}

namespace GameLogic.Scene.Story
{
    public interface ISceneObject : IIdentifiable
    {
        void OnInteract();
        void OnCreateAdvantage();
        void OnAttack();
        void OnSupport();
        Character CharacterRef { get; }
        Layout Layout { get; }
        Style Style { get; }
        void TransTo(Layout layout);
        void ChangeStyle(Style style);
        void ApplyEffect(ViewEffect effect);
    }

    public class SceneObject : ISceneObject
    {
        #region Javascript API class
        private sealed class API : IJSAPI
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

            public void setInteractCode(string jscode)
            {
                try
                {
                    _outer.Interact = new Command(jscode);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setCreateAdvantageCode(string jscode)
            {
                try
                {
                    _outer.CreateAdvantage = new Command(jscode);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setAttackCode(string jscode)
            {
                try
                {
                    _outer.Attack = new Command(jscode);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public void setSupportCode(string jscode)
            {
                try
                {
                    _outer.Support = new Command(jscode);
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSAPI getCharacter()
            {
                try
                {
                    return (IJSAPI)_outer.CharacterRef.GetContext();
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

            public void changeStyle(Style style)
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

            public void applyEffect(ViewEffect effect)
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

            public IJSContextProvider Origin(JSContextHelper proof)
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
        protected ICommand _interact;
        protected ICommand _createAdvantage;
        protected ICommand _attack;
        protected ICommand _support;
        protected Character _characterRef;
        protected Layout _layout;
        protected Style _style;
        protected ViewEffect _effect;

        public string ID => _id;
        public ICommand Interact { get => _interact; set => _interact = value; }
        public ICommand CreateAdvantage { get => _createAdvantage; set => _createAdvantage = value; }
        public ICommand Attack { get => _attack; set => _attack = value; }
        public ICommand Support { get => _support; set => _support = value; }
        public Character CharacterRef { get => _characterRef; set => _characterRef = value ?? throw new ArgumentNullException(nameof(CharacterRef)); }
        public Layout Layout => _layout;
        public Style Style => _style;
        public ViewEffect Effect => _effect;
        
        public SceneObject(string id, Character character)
        {
            _id = id ?? throw new ArgumentNullException(nameof(id));
            _characterRef = character ?? throw new ArgumentNullException(nameof(character));
            _layout = Layout.INIT;
            _style = Style.INIT;
            _effect = ViewEffect.INIT;
            _apiObj = new API(this);
        }

        public virtual void ApplyEffect(ViewEffect effect)
        {
            _effect = effect;
            throw new NotImplementedException();
        }
        
        public virtual void TransTo(Layout layout)
        {
            _layout = layout;
            throw new NotImplementedException();
        }

        public virtual void ChangeStyle(Style style)
        {
            _style = style;
            throw new NotImplementedException();
        }

        public virtual void OnAttack()
        {
            if (_attack != null)
            {
                JSEngineManager.Run(_attack);
            }
        }

        public virtual void OnCreateAdvantage()
        {
            if (_createAdvantage != null)
            {
                JSEngineManager.Run(_createAdvantage);
            }
        }

        public virtual void OnInteract()
        {
            if (_interact != null)
            {
                JSEngineManager.Run(_interact);
            }
        }

        public virtual void OnSupport()
        {
            if (_support != null)
            {
                JSEngineManager.Run(_support);
            }
        }

        public virtual object GetContext()
        {
            return _apiObj;
        }

        public virtual void SetContext(object context) { }
    }

    public sealed class Camera : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI
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

            public void applyEffect(ViewEffect effect)
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

            public IJSContextProvider Origin(JSContextHelper proof)
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
        private ViewEffect _effect;

        public Layout Layout => _layout;
        public ViewEffect Effect => _effect;

        public Camera()
        {
            _layout = Layout.INIT;
            _effect = ViewEffect.INIT;
            _apiObj = new API(this);
        }

        public void TransTo(Layout layout)
        {
            _layout = layout;
            throw new NotImplementedException();
        }

        public void ApplyEffect(ViewEffect effect)
        {
            _effect = effect;
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _layout = Layout.INIT;
            _effect = ViewEffect.INIT;
            throw new NotImplementedException();
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
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
        private sealed class API : IJSAPI
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

            public IJSContextProvider Origin(JSContextHelper proof)
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
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(Text)); }

        public TextItem(string text = "")
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _apiObj = new API(this);
        }

        public virtual void React() { }

        public virtual object GetContext()
        {
            return _apiObj;
        }
        
        public virtual void SetContext(object context) { }
    }

    public class SelectionItem : ITextItem
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly SelectionItem _outer;

            public API(SelectionItem outer)
            {
                _outer = outer;
            }

            public void setActionCode(string jscode)
            {
                try
                {
                    _outer.Action = new Command(jscode);
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

            public IJSContextProvider Origin(JSContextHelper proof)
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
        protected ICommand _action;

        public bool IsReactable => true;
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(Text)); }
        public ICommand Action { get => _action; set => _action = value; }

        public SelectionItem(string text = "", string actionCode = null)
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
                JSEngineManager.Run(_action);
            }
        }

        public virtual object GetContext()
        {
            return _apiObj;
        }
        
        public virtual void SetContext(object context) { }
    }

    public sealed class TextBox : IJSContextProvider
    {
        #region Javascript API class
        private sealed class API : IJSAPI
        {
            private readonly TextBox _outer;

            public API(TextBox outer)
            {
                _outer = outer;
            }

            public IJSAPI generateText()
            {
                try
                {
                    ITextItem textItem = _outer.GenerateText();
                    return (IJSAPI)textItem.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI generateSelection()
            {
                try
                {
                    ITextItem selectionItem = _outer.GenerateSelection();
                    return (IJSAPI)selectionItem.GetContext();
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
                    _outer.Display();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                }
            }

            public IJSContextProvider Origin(JSContextHelper proof)
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
        private int _playerIndex;

        public List<ITextItem> TextItems => _textItems;
        public int PlayerIndex { get => _playerIndex; set => _playerIndex = value; }

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

        public ITextItem GenerateSelection()
        {
            SelectionItem ret = new SelectionItem();
            _textItems.Add(ret);
            return ret;
        }

        public void Clear()
        {
            _textItems.Clear();
        }

        public void Display()
        {
            throw new NotImplementedException();
        }

        public object GetContext()
        {
            return _apiObj;
        }

        public void SetContext(object context) { }
    }
    
}