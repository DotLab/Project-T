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

        public IReadonlyIdentifiedObjList<IStoryObject> ObjList => _objList;
        public IReadonlyIdentifiedObjList<Character> PlayerCharacters => _playerCharacters;
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

        private StoryObject CreateStoryObject(Character character)
        {
            StoryObject ret = new StoryObject(character);
            return ret;
        }

        public void AddPlayerCharacter(Character playerCharacter)
        {
            _playerCharacters.Add(playerCharacter);
            throw new NotImplementedException();
        }

        public bool RemovePlayerCharacter(string id)
        {
            bool ret = _playerCharacters.Remove(id);
            throw new NotImplementedException();
            return ret;
        }

        public bool RemovePlayerCharacter(Character playerCharacter)
        {
            return this.RemovePlayerCharacter(playerCharacter.ID);
        }
        
        public bool AddIntoScene(IStoryObject sceneObject)
        {
            _objList.Add(sceneObject);
            throw new NotImplementedException();
            return true;
        }

        public bool RemoveFromScene(string id)
        {
            bool ret = _objList.Remove(id);
            throw new NotImplementedException();
            return ret;
        }

        public bool RemoveFromScene(IStoryObject sceneObject)
        {
            return this.RemoveFromScene(sceneObject.ID);
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

        public void StartCheck(
            Character initiative, Character passive, SkillChecker.CharacterAction action,
            Action<SkillChecker.CheckResult> initiativeCallback, Action<SkillChecker.CheckResult> passiveCallback
            )
        {
            SkillChecker.Instance.StartCheck(initiative, passive, action, initiativeCallback, passiveCallback);
            foreach (Player player in MainLogic.Players)
            {
                if (initiative.Controller == player) player.Client.SkillCheckPanel.Show(initiative, passive, Client.SkillCheckPanel.ClientPosition.INITIATIVE);
                else if (passive.Controller == player) player.Client.SkillCheckPanel.Show(initiative, passive, Client.SkillCheckPanel.ClientPosition.PASSIVE);
                else player.Client.SkillCheckPanel.Show(initiative, passive, Client.SkillCheckPanel.ClientPosition.OBSERVER);
            }
            if (initiative.Controller == null) MainLogic.DM.Client.SkillCheckPanel.Show(initiative, passive, Client.SkillCheckPanel.ClientPosition.INITIATIVE);
            else if (passive.Controller == null) MainLogic.DM.Client.SkillCheckPanel.Show(initiative, passive, Client.SkillCheckPanel.ClientPosition.PASSIVE);
            else MainLogic.DM.Client.SkillCheckPanel.Show(initiative, passive, Client.SkillCheckPanel.ClientPosition.OBSERVER);
        }

        public void CancelCheck()
        {
            SkillChecker.Instance.CancelCheck();
            foreach (User user in MainLogic.Players)
            {
                user.AsPlayer.Client.SkillCheckPanel.Hide();
            }
            MainLogic.DM.AsDM.Client.SkillCheckPanel.Hide();
        }

        public void ForceEndCheck(SkillChecker.CheckResult initiativeResult, SkillChecker.CheckResult passiveResult)
        {
            SkillChecker.Instance.ForceEndCheck(initiativeResult, passiveResult);
            foreach (Player player in MainLogic.Players)
            {
                if (SkillChecker.Instance.Initiative.Controller == player) player.Client.SkillCheckPanel.Hide();
                else if (SkillChecker.Instance.Passive.Controller == player) player.Client.SkillCheckPanel.Hide();
                else player.Client.SkillCheckPanel.Hide();
            }
            if (SkillChecker.Instance.Initiative.Controller == null) MainLogic.DM.Client.SkillCheckPanel.Hide();
            else if (SkillChecker.Instance.Passive.Controller == null) MainLogic.DM.Client.SkillCheckPanel.Hide();
            else MainLogic.DM.Client.SkillCheckPanel.Hide();
        }

        public void EndCheck()
        {
            SkillChecker.Instance.EndCheck();
            foreach (Player player in MainLogic.Players)
            {
                if (SkillChecker.Instance.Initiative.Controller == player) player.Client.SkillCheckPanel.Hide();
                else if (SkillChecker.Instance.Passive.Controller == player) player.Client.SkillCheckPanel.Hide();
                else player.Client.SkillCheckPanel.Hide();
            }
            if (SkillChecker.Instance.Initiative.Controller == null) MainLogic.DM.Client.SkillCheckPanel.Hide();
            else if (SkillChecker.Instance.Passive.Controller == null) MainLogic.DM.Client.SkillCheckPanel.Hide();
            else MainLogic.DM.Client.SkillCheckPanel.Hide();
        }

        private void InitiativeSelectSkill(SkillType skillType, bool bigone, int[] fixedDicePoints)
        {
            SkillChecker.Instance.InitiativeSelectSkill(skillType);
            int[] dicePoints = SkillChecker.Instance.InitiativeRollDice(fixedDicePoints);
            SkillChecker.Instance.InitiativeSkillSelectionOver();
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckPanel.DisplayDicePoint(true, dicePoints);
                player.Client.SkillCheckPanel.DisplaySkillReady(true, skillType, bigone);
                player.Client.SkillCheckPanel.UpdateSumPoint(true, SkillChecker.Instance.InitiativePoint);
            }
            MainLogic.DM.Client.SkillCheckPanel.DisplayDicePoint(true, dicePoints);
            MainLogic.DM.Client.SkillCheckPanel.DisplaySkillReady(true, skillType, bigone);
            MainLogic.DM.Client.SkillCheckPanel.UpdateSumPoint(true, SkillChecker.Instance.InitiativePoint);
        }

        public void InitiativeUseSkill(SkillType skillType, bool force, bool bigone, int[] fixedDicePoints = null)
        {
            if (force || SkillChecker.Instance.Action == SkillChecker.CharacterAction.ATTACK)
            {
                this.InitiativeSelectSkill(skillType, bigone, fixedDicePoints);
            }
            else if (SkillChecker.Instance.Action == SkillChecker.CharacterAction.CREATE_ASPECT || SkillChecker.Instance.Action == SkillChecker.CharacterAction.HINDER)
            {
                MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Initiative.Name + "对" + SkillChecker.Instance.Passive.Name + "使用" + skillType.Name + ",可以吗？",
                    result => { if (result) this.InitiativeSelectSkill(skillType, bigone, fixedDicePoints); });
            }
        }

        public void InitiativeUseStunt(Stunt stunt)
        {
            if (stunt.Belong != SkillChecker.Instance.Initiative) throw new ArgumentException("This stunt is not belong to initiative character.", nameof(stunt));
            if (stunt.NeedDMCheck)
                MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Initiative.Name + "对" + SkillChecker.Instance.Passive.Name + "使用" + stunt.Name + ",可以吗？",
                    result => { if (result) stunt.InitiativeEffect.DoAction(); });
            else stunt.InitiativeEffect.DoAction();
        }

        private void PassiveSelectSkill(SkillType skillType, bool bigone, int[] fixedDicePoints)
        {
            SkillChecker.Instance.PassiveSelectSkill(skillType);
            int[] dicePoints = SkillChecker.Instance.PassiveRollDice(fixedDicePoints);
            SkillChecker.Instance.PassiveSkillSelectionOver();
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckPanel.DisplayDicePoint(false, dicePoints);
                player.Client.SkillCheckPanel.DisplaySkillReady(false, skillType, bigone);
                player.Client.SkillCheckPanel.UpdateSumPoint(false, SkillChecker.Instance.PassivePoint);
            }
            MainLogic.DM.Client.SkillCheckPanel.DisplayDicePoint(false, dicePoints);
            MainLogic.DM.Client.SkillCheckPanel.DisplaySkillReady(false, skillType, bigone);
            MainLogic.DM.Client.SkillCheckPanel.UpdateSumPoint(false, SkillChecker.Instance.PassivePoint);
        }

        public void PassiveUseSkill(SkillType skillType, bool force, bool bigone, int[] fixedDicePoints = null)
        {
            if (force)
            {
                this.PassiveSelectSkill(skillType, bigone, fixedDicePoints);
            }
            else
            {
                if (SkillChecker.CanResistSkillWithoutDMCheck(SkillChecker.Instance.InitiativeSkillType, skillType, SkillChecker.Instance.Action))
                {
                    this.PassiveSelectSkill(skillType, bigone, fixedDicePoints);
                }
                else
                {
                    MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Passive.Name + "对" + SkillChecker.Instance.Passive.Name + "使用" + skillType.Name + ",可以吗？",
                        result => { if (result) this.PassiveSelectSkill(skillType, bigone, fixedDicePoints); });
                }
            }
        }

        public void PassiveUseStunt(Stunt stunt)
        {
            if (stunt.Belong != SkillChecker.Instance.Passive) throw new ArgumentException("This stunt is not belong to passive character.", nameof(stunt));
            if (stunt.NeedDMCheck)
                MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Passive.Name + "对" + SkillChecker.Instance.Initiative.Name + "使用" + stunt.Name + ",可以吗？",
                    result => { if (result) stunt.InitiativeEffect.DoAction(); });
            else stunt.InitiativeEffect.DoAction();
        }

        public void InitiativeUseAspect(Aspect aspect, bool reroll)
        {
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckPanel.DisplayUsingAspect(true, aspect);
            }
            MainLogic.DM.Client.SkillCheckPanel.DisplayUsingAspect(true, aspect);
            MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Initiative.Name + "想使用" + aspect.Belong.Name + "的" + aspect.Name + "可以吗？",
                result =>
                {
                    if (result)
                    {
                        SkillChecker.Instance.InitiativeSelectAspect(aspect, reroll);
                        foreach (Player player in MainLogic.Players)
                        {
                            player.Client.SkillCheckPanel.UpdateSumPoint(true, SkillChecker.Instance.InitiativePoint);
                        }
                        MainLogic.DM.Client.SkillCheckPanel.UpdateSumPoint(true, SkillChecker.Instance.InitiativePoint);
                    }
                });
        }

        public void InitiativeSkipUsingAspect()
        {
            SkillChecker.Instance.InitiativeAspectSelectionOver();
        }

        public void PassiveUseAspect(Aspect aspect, bool reroll)
        {
            foreach (Player player in MainLogic.Players)
            {
                player.Client.SkillCheckPanel.DisplayUsingAspect(false, aspect);
            }
            MainLogic.DM.Client.SkillCheckPanel.DisplayUsingAspect(false, aspect);
            MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Passive.Name + "想使用" + aspect.Belong.Name + "的" + aspect.Name + "可以吗？",
                result =>
                {
                    if (result)
                    {
                        SkillChecker.Instance.PassiveSelectAspect(aspect, reroll);
                        foreach (Player player in MainLogic.Players)
                        {
                            player.Client.SkillCheckPanel.UpdateSumPoint(false, SkillChecker.Instance.PassivePoint);
                        }
                        MainLogic.DM.Client.SkillCheckPanel.UpdateSumPoint(false, SkillChecker.Instance.PassivePoint);
                    }
                });
        }

        public void PassiveSkipUsingAspect()
        {
            SkillChecker.Instance.PassiveAspectSelectionOver();
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
        bool IsSelection { get; }
        int SelectionCode { get; }
        string Text { get; }
    }

    public sealed class TextItem : ITextItem
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<TextItem>
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

        private string _text;

        public bool IsSelection => false;
        public int SelectionCode => -1;
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }

        public TextItem(string text)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _apiObj = new JSAPI(this);
        }
        
        public IJSContext GetContext()
        {
            return _apiObj;
        }
        
        public void SetContext(IJSContext context) { }
    }

    public sealed class SelectionItem : ITextItem
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<SelectionItem>
        {
            private readonly SelectionItem _outer;

            public JSAPI(SelectionItem outer)
            {
                _outer = outer;
            }

            public void setSelectionCode(int code)
            {
                try
                {
                    _outer.SelectionCode = code;
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

            public SelectionItem Origin(JSContextHelper proof)
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

        private string _text;
        private int _selectionCode;

        public bool IsSelection => true;
        public string Text { get => _text; set => _text = value ?? throw new ArgumentNullException(nameof(value)); }
        public int SelectionCode { get => _selectionCode; set => _selectionCode = value; }
        
        public SelectionItem(string text, int selectionCode)
        {
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _apiObj = new JSAPI(this);
        }
        
        public IJSContext GetContext()
        {
            return _apiObj;
        }
        
        public void SetContext(IJSContext context) { }
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

            public IJSAPI<TextItem> generateText(string text)
            {
                try
                {
                    ITextItem textItem = _outer.GenerateText(text);
                    return (IJSAPI<TextItem>)textItem.GetContext();
                }
                catch (Exception e)
                {
                    JSEngineManager.Engine.Log(e.Message);
                    return null;
                }
            }

            public IJSAPI<SelectionItem> generateSelection(string text, int selectionCode)
            {
                try
                {
                    ITextItem buttonItem = _outer.GenerateSelection(text, selectionCode);
                    return (IJSAPI<SelectionItem>)buttonItem.GetContext();
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

        public ITextItem GenerateText(string text)
        {
            TextItem ret = new TextItem(text);
            _textItems.Add(ret);
            return ret;
        }

        public ITextItem GenerateSelection(string text, int selectionCode)
        {
            SelectionItem ret = new SelectionItem(text, selectionCode);
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