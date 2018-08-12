using GameLogic.CharacterSystem;
using GameLogic.Container.BattleComponent;
using GameLogic.Core;
using GameLogic.Core.ScriptSystem;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Container
{
    public class BattleSceneContainer : IJSContextProvider
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<BattleSceneContainer>
        {
            private readonly BattleSceneContainer _outer;

            public JSAPI(BattleSceneContainer outer)
            {
                _outer = outer;
            }
            
            public BattleSceneContainer Origin(JSContextHelper proof)
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

        private static readonly BattleSceneContainer _instance = new BattleSceneContainer();
        public static BattleSceneContainer Instance => _instance;

        private readonly IdentifiedObjList<GridObject> _gridObjList;
        private readonly IdentifiedObjList<LadderObject> _ladderObjList;
        private readonly List<ActableGridObject> _actableObjList;
        private readonly BattleMap _battleMap;
        private ActableGridObject _currentActable;
        private int _roundCount;

        private GridObject _initiative;
        private SkillType _initiativeSkillType;
        private int _initiativeRollPoint;
        private SkillChecker.CharacterAction _action;
        private List<GridObject> _passives;
        private GridObject _currentPassive;

        public IReadonlyIdentifiedObjList<GridObject> GridObjList => _gridObjList;
        public IReadonlyIdentifiedObjList<LadderObject> LadderObjList => _ladderObjList;
        public IReadOnlyList<ActableGridObject> ActableObjList => _actableObjList;
        public BattleMap BattleMap => _battleMap;
        public ActableGridObject CurrentActable => _currentActable;
        public int RoundCount => _roundCount;

        public BattleSceneContainer()
        {
            _gridObjList = new IdentifiedObjList<GridObject>();
            _ladderObjList = new IdentifiedObjList<LadderObject>();
            _actableObjList = new List<ActableGridObject>();
            _battleMap = new BattleMap();
            _apiObj = new JSAPI(this);
        }
        
        public void Reset(int rows, int cols)
        {
            _battleMap.Reset(rows, cols);
            _gridObjList.Clear();
            _ladderObjList.Clear();
            _actableObjList.Clear();
            _currentActable = null;
            _roundCount = 0;
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.Reset(rows, cols);
            }
            MainLogic.DM.Client.BattleScene.Reset(rows, cols);
            this.Update();
        }

        public GridObject FindGridObject(string id)
        {
            if (_gridObjList.TryGetValue(id, out GridObject gridObject))
            {
                return gridObject;
            }
            return null;
        }

        public LadderObject FindLadderObject(string id)
        {
            if (_ladderObjList.TryGetValue(id, out LadderObject ladderObject))
            {
                return ladderObject;
            }
            return null;
        }

        public void PushGridObject(int row, int col, bool highland, GridObject gridObject)
        {
            if (_gridObjList.Contains(gridObject)) throw new ArgumentException("This object is already added to scene.", nameof(gridObject));
            List<GridObject> land;
            if (highland) land = (List<GridObject>)_battleMap[row, col].Highland;
            else land = (List<GridObject>)_battleMap[row, col].Lowland;
            land.Add(gridObject);
            gridObject.GridRef = _battleMap[row, col];
            gridObject.IsHighland = highland;
            _gridObjList.Add(gridObject);
            if (gridObject is ActableGridObject) _actableObjList.Add((ActableGridObject)gridObject);
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.PushGridObject(row, col, highland, gridObject);
            }
            MainLogic.DM.Client.BattleScene.PushGridObject(row, col, highland, gridObject);
        }

        public GridObject PopGridObject(int row, int col, bool highland)
        {
            GridObject ret = null;
            List<GridObject> land;
            if (highland) land = (List<GridObject>)_battleMap[row, col].Highland;
            else land = (List<GridObject>)_battleMap[row, col].Lowland;
            if (land.Count > 0)
            {
                ret = land[land.Count - 1];
                land.RemoveAt(land.Count - 1);
                ret.GridRef = null;
                _gridObjList.Remove(ret);
                if (ret is ActableGridObject) _actableObjList.Remove((ActableGridObject)ret);
            }
            if (ret != null)
            {
                foreach (Player player in MainLogic.Players)
                {
                    player.Client.BattleScene.RemoveGridObject(ret);
                }
                MainLogic.DM.Client.BattleScene.RemoveGridObject(ret);
            }
            return ret;
        }

        public bool RemoveGridObject(GridObject gridObject)
        {
            if (!_gridObjList.Contains(gridObject)) return false;
            if (gridObject.IsHighland) ((List<GridObject>)gridObject.GridRef.Highland).Remove(gridObject);
            else ((List<GridObject>)gridObject.GridRef.Lowland).Remove(gridObject);
            gridObject.GridRef = null;
            _gridObjList.Remove(gridObject);
            if (gridObject is ActableGridObject) _actableObjList.Remove((ActableGridObject)gridObject);
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.RemoveGridObject(gridObject);
            }
            MainLogic.DM.Client.BattleScene.RemoveGridObject(gridObject);
            return true;
        }

        public void AddLadderObject(int row, int col, Direction direction, LadderObject ladderObject)
        {
            if (_ladderObjList.Contains(ladderObject)) throw new ArgumentException("This object is already added to scene.", nameof(ladderObject));
            if (row == 0 && direction == Direction.NEGATIVE_ROW) throw new ArgumentOutOfRangeException(nameof(direction));
            if (row == _battleMap.Rows - 1 && direction == Direction.POSITIVE_ROW) throw new ArgumentOutOfRangeException(nameof(direction));
            if (col == 0 && direction == Direction.NEGATIVE_COL) throw new ArgumentOutOfRangeException(nameof(direction));
            if (col == _battleMap.Cols - 1 && direction == Direction.POSITIVE_COL) throw new ArgumentOutOfRangeException(nameof(direction));
            Grid grid = _battleMap[row, col];
            if (grid.GetLadder(direction) != null) throw new ArgumentException("This grid has already bound a ladder.", nameof(ladderObject));
            switch (direction)
            {
                case Direction.POSITIVE_ROW:
                    {
                        Grid anotherGrid = _battleMap[row + 1, col];
                        grid.PositiveRowLadder = ladderObject;
                        ladderObject.GridRef = grid;
                        anotherGrid.NegativeRowLadder = ladderObject;
                        ladderObject.AnotherGridRef = anotherGrid;
                    }
                    break;
                case Direction.POSITIVE_COL:
                    {
                        Grid anotherGrid = _battleMap[row, col + 1];
                        grid.PositiveColLadder = ladderObject;
                        ladderObject.GridRef = grid;
                        anotherGrid.NegativeColLadder = ladderObject;
                        ladderObject.AnotherGridRef = anotherGrid;
                    }
                    break;
                case Direction.NEGATIVE_ROW:
                    {
                        Grid anotherGrid = _battleMap[row - 1, col];
                        grid.NegativeRowLadder = ladderObject;
                        ladderObject.GridRef = grid;
                        anotherGrid.PositiveRowLadder = ladderObject;
                        ladderObject.AnotherGridRef = anotherGrid;
                    }
                    break;
                case Direction.NEGATIVE_COL:
                    {
                        Grid anotherGrid = _battleMap[row, col - 1];
                        grid.NegativeColLadder = ladderObject;
                        ladderObject.GridRef = grid;
                        anotherGrid.PositiveColLadder = ladderObject;
                        ladderObject.AnotherGridRef = anotherGrid;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
            _ladderObjList.Add(ladderObject);
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.AddLadderObject(row, col, direction, ladderObject);
            }
            MainLogic.DM.Client.BattleScene.AddLadderObject(row, col, direction, ladderObject);
        }

        public LadderObject RemoveLadderObject(int row, int col, Direction direction)
        {
            if (row == 0 && direction == Direction.NEGATIVE_ROW) return null;
            if (row == _battleMap.Rows - 1 && direction == Direction.POSITIVE_ROW) return null;
            if (col == 0 && direction == Direction.NEGATIVE_COL) return null;
            if (col == _battleMap.Cols - 1 && direction == Direction.POSITIVE_COL) return null;
            Grid grid = _battleMap[row, col];
            if (grid.GetLadder(direction) == null) return null;
            LadderObject ladderObject;
            switch (direction)
            {
                case Direction.POSITIVE_ROW:
                    {
                        Grid anotherGrid = _battleMap[row + 1, col];
                        ladderObject = grid.PositiveRowLadder;
                        ladderObject.GridRef = null;
                        ladderObject.AnotherGridRef = null;
                        grid.PositiveRowLadder = null;
                        anotherGrid.NegativeRowLadder = null;
                    }
                    break;
                case Direction.POSITIVE_COL:
                    {
                        Grid anotherGrid = _battleMap[row, col + 1];
                        ladderObject = grid.PositiveColLadder;
                        ladderObject.GridRef = null;
                        ladderObject.AnotherGridRef = null;
                        grid.PositiveColLadder = null;
                        anotherGrid.NegativeColLadder = null;
                    }
                    break;
                case Direction.NEGATIVE_ROW:
                    {
                        Grid anotherGrid = _battleMap[row - 1, col];
                        ladderObject = grid.NegativeRowLadder;
                        ladderObject.GridRef = null;
                        ladderObject.AnotherGridRef = null;
                        grid.NegativeRowLadder = null;
                        anotherGrid.PositiveRowLadder = null;
                    }
                    break;
                case Direction.NEGATIVE_COL:
                    {
                        Grid anotherGrid = _battleMap[row, col - 1];
                        ladderObject = grid.NegativeColLadder;
                        ladderObject.GridRef = null;
                        ladderObject.AnotherGridRef = null;
                        grid.NegativeColLadder = null;
                        anotherGrid.PositiveColLadder = null;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
            _ladderObjList.Remove(ladderObject);
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.RemoveLadderObject(ladderObject);
            }
            MainLogic.DM.Client.BattleScene.RemoveLadderObject(ladderObject);
            return ladderObject;
        }

        public bool RemoveLadderObject(LadderObject ladderObject)
        {
            if (!_ladderObjList.Contains(ladderObject)) return false;
            if (ladderObject.GridRef.PositiveRowLadder == ladderObject) ladderObject.GridRef.PositiveRowLadder = null;
            else if (ladderObject.GridRef.PositiveColLadder == ladderObject) ladderObject.GridRef.PositiveColLadder = null;
            else if (ladderObject.GridRef.NegativeRowLadder == ladderObject) ladderObject.GridRef.NegativeRowLadder = null;
            else if (ladderObject.GridRef.NegativeColLadder == ladderObject) ladderObject.GridRef.NegativeColLadder = null;
            else return false;
            if (ladderObject.AnotherGridRef.PositiveRowLadder == ladderObject) ladderObject.AnotherGridRef.PositiveRowLadder = null;
            else if (ladderObject.AnotherGridRef.PositiveColLadder == ladderObject) ladderObject.AnotherGridRef.PositiveColLadder = null;
            else if (ladderObject.AnotherGridRef.NegativeRowLadder == ladderObject) ladderObject.AnotherGridRef.NegativeRowLadder = null;
            else if (ladderObject.AnotherGridRef.NegativeColLadder == ladderObject) ladderObject.AnotherGridRef.NegativeColLadder = null;
            else return false;
            ladderObject.GridRef = null;
            ladderObject.AnotherGridRef = null;
            _ladderObjList.Remove(ladderObject);
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.RemoveLadderObject(ladderObject);
            }
            MainLogic.DM.Client.BattleScene.RemoveLadderObject(ladderObject);
            return true;
        }
        
        public void NewRound()
        {
            foreach (ActableGridObject actableObject in _actableObjList)
            {
                SkillProperty noticeProperty = actableObject.CharacterRef.GetSkillProperty(SkillType.Notice);
                SkillProperty athleticsProperty = actableObject.CharacterRef.GetSkillProperty(SkillType.Athletics);
                int[] dicePoints = FateDice.Roll();
                if (actableObject.CharacterRef.Controller != null)
                {
                    actableObject.CharacterRef.Controller.Client.BattleScene.DisplayDicePoints(dicePoints);
                }
                int sumPoint = 0;
                foreach (int point in dicePoints) sumPoint += point;
                actableObject.ActionTurn = noticeProperty.level + sumPoint;
                actableObject.ActionPoint = 1 + athleticsProperty.level >= 1 ? (athleticsProperty.level - 1) / 2 : 0;
                actableObject.MovePoint = 0;
            }
            _actableObjList.Sort((ActableGridObject a, ActableGridObject b) => { return b.ActionTurn - a.ActionTurn; });
            foreach (Player player in MainLogic.Players)
            {
                player.Client.BattleScene.SetActingOrder(_actableObjList);
            }
            MainLogic.DM.Client.BattleScene.SetActingOrder(_actableObjList);
            if (_actableObjList.Count > 0) _currentActable = _actableObjList[0];
            else _currentActable = null;
            ++_roundCount;
        }
        
        public void CurrentActionOver()
        {
            int next = _actableObjList.IndexOf(_currentActable) + 1;
            if (next >= _actableObjList.Count) // new round
            {
                this.NewRound();
            }
            else
            {
                _currentActable = _actableObjList[next];
            }
            _currentActable.AddMovePoint();
            this.Update();
        }
        
        public void Update()
        {

        }

        public void StartCheck(
            GridObject initiative, IEnumerable<GridObject> passives,
            SkillChecker.CharacterAction action, SkillType initiativeSkillType
            )
        {
            _initiative = initiative;
            _passives = new List<GridObject>(passives);
            _initiativeSkillType = initiativeSkillType;
            _action = action;
            int[] dicePoints = FateDice.Roll();
            _initiativeRollPoint = 0;
            foreach (int point in dicePoints) _initiativeRollPoint += point;
            // ...
            _currentPassive = null;
        }

        public void InitiativeResult(SkillChecker.CheckResult result)
        {

        }

        public void PassiveResult(SkillChecker.CheckResult result)
        {

        }

        public bool NextPassiveCheck()
        {
            if (_passives.Count <= 0) return false;
            _currentPassive = _passives[_passives.Count - 1];
            _passives.RemoveAt(_passives.Count - 1);
            SkillChecker.Instance.StartCheck(_initiative.CharacterRef, _currentPassive.CharacterRef, _action, this.InitiativeResult, this.PassiveResult);
            SkillChecker.Instance.InitiativeSelectSkill(_initiativeSkillType);
            int[] point = { _initiativeRollPoint };
            SkillChecker.Instance.InitiativeRollDice(point);
            SkillChecker.Instance.InitiativeSkillSelectionOver();
            return true;
        }

        private void CurrentPassiveSelectSkill(SkillType skillType, bool bigone, int[] fixedDicePoints)
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

        public void CurrentPassiveUseSkill(SkillType skillType, bool force, bool bigone, int[] fixedDicePoints = null)
        {
            if (force)
            {
                this.CurrentPassiveSelectSkill(skillType, bigone, fixedDicePoints);
            }
            else
            {
                if (SkillChecker.CanResistSkillWithoutDMCheck(SkillChecker.Instance.InitiativeSkillType, skillType, SkillChecker.Instance.Action))
                {
                    this.CurrentPassiveSelectSkill(skillType, bigone, fixedDicePoints);
                }
                else
                {
                    MainLogic.DM.Client.DMCheckDialog.RequestCheck(SkillChecker.Instance.Passive.Name + "对" + SkillChecker.Instance.Passive.Name + "使用" + skillType.Name + ",可以吗？",
                        result => { if (result) this.CurrentPassiveSelectSkill(skillType, bigone, fixedDicePoints); });
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

namespace GameLogic.Container.BattleComponent
{
    public enum Direction
    {
        POSITIVE_ROW = 0b0001,
        POSITIVE_COL = 0b0010,
        NEGATIVE_ROW = 0b0100,
        NEGATIVE_COL = 0b1000
    }

    public class GridObject : IIdentifiable
    {
        #region Javascript API class
        protected class JSAPI : IJSAPI<GridObject>
        {
            private readonly GridObject _outer;

            public JSAPI(GridObject outer)
            {
                _outer = outer;
            }

            public GridObject Origin(JSContextHelper proof)
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

        protected readonly Character _characterRef;
        protected bool _isObstacle;
        protected Grid _gridRef;
        protected bool _isHighland;
        protected int _stagnate;
        protected readonly bool _isTerrain;
        protected Direction _direction;
        
        public string ID => _characterRef.ID;

        public string Name { get => _characterRef.Name; set { } }
        public string Description { get => _characterRef.Description; set { } }

        public Character CharacterRef => _characterRef;
        public bool IsObstacle { get => _isObstacle; set => _isObstacle = value; }
        public Grid GridRef { get => _gridRef; set => _gridRef = value; }
        public bool IsHighland { get => _isHighland; set => _isHighland = value; }
        public int Stagnate { get => _stagnate; set => _stagnate = value; }
        public bool IsTerrain => _isTerrain;
        public Direction Direction { get => _direction; set => _direction = value; }

        public GridObject(Character characterRef, bool isTerrian)
        {
            _characterRef = characterRef ?? throw new ArgumentNullException(nameof(characterRef));
            _isTerrain = isTerrian;
            _apiObj = new JSAPI(this);
        }

        public virtual IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

    public sealed class LadderObject : IIdentifiable
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<LadderObject>
        {
            private readonly LadderObject _outer;

            public JSAPI(LadderObject outer)
            {
                _outer = outer;
            }

            public LadderObject Origin(JSContextHelper proof)
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

        private readonly Character _characterRef;
        private Grid _gridRef;
        private Grid _anotherGridRef;
        private int _stagnate;
        private Direction _direction;
        
        public string ID => _characterRef.ID;

        public string Name { get => _characterRef.Name; set { } }
        public string Description { get => _characterRef.Description; set { } }

        public Character CharacterRef => _characterRef;
        public Grid GridRef { get => _gridRef; set => _gridRef = value; }
        public Grid AnotherGridRef { get => _anotherGridRef; set => _anotherGridRef = value; }
        public int Stagnate { get => _stagnate; set => _stagnate = value; }
        public Direction Direction { get => _direction; set => _direction = value; }

        public LadderObject(Character characterRef)
        {
            _characterRef = characterRef ?? throw new ArgumentNullException(nameof(characterRef));
            _apiObj = new JSAPI(this);
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

    public sealed class ActableGridObject : GridObject
    {
        #region Javascript API class
        private new class JSAPI : GridObject.JSAPI, IJSAPI<ActableGridObject>
        {
            private readonly ActableGridObject _outer;

            public JSAPI(ActableGridObject outer) : base(outer)
            {
                _outer = outer;
            }

            ActableGridObject IJSAPI<ActableGridObject>.Origin(JSContextHelper proof)
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

        private int _actionTurn;
        private int _actionPoint;
        private bool _movable;
        private int _movePoint;

        public int ActionTurn { get => _actionTurn; set => _actionTurn = value; }
        public int ActionPoint { get => _actionPoint; set => _actionPoint = value; }
        public bool Movable { get => _movable; set => _movable = value; }
        public int MovePoint { get => _movePoint; set => _movePoint = value; }

        public ActableGridObject(Character characterRef) :
            base (characterRef, false)
        {
            _apiObj = new JSAPI(this);
        }

        public void AddMovePoint()
        {
            int[] dicePoints = FateDice.Roll();
            int sumPoint = 0;
            foreach (int point in dicePoints) sumPoint += point;
            SkillProperty athleticsProperty = this.CharacterRef.GetSkillProperty(SkillType.Athletics);
            this.MovePoint += athleticsProperty.level + sumPoint;
        }
        
        public void TakeExtraMove()
        {
            int leftActionPoint = this.ActionPoint;
            if (--leftActionPoint < 0) throw new InvalidOperationException("Action point is not enough.");
            this.ActionPoint = leftActionPoint;
            this.AddMovePoint();
        }

        public bool CanMove(Direction direction, bool stairway)
        {
            int srcRow = this.GridRef.PosRow;
            int srcCol = this.GridRef.PosCol;
            int dstRow = srcRow;
            int dstCol = srcCol;
            switch (direction)
            {
                case Direction.POSITIVE_ROW:
                    ++dstRow;
                    break;
                case Direction.POSITIVE_COL:
                    ++dstCol;
                    break;
                case Direction.NEGATIVE_ROW:
                    --dstRow;
                    break;
                case Direction.NEGATIVE_COL:
                    --dstCol;
                    break;
                default:
                    return false;
            }
            Grid dstGrid = BattleSceneContainer.Instance.BattleMap[dstRow, dstCol];
            bool dstHighland = stairway ^ this.IsHighland;
            IReadOnlyList<GridObject> land = dstHighland ? dstGrid.Highland : dstGrid.Lowland;
            if (land.Count <= 0 || land[land.Count - 1].IsObstacle) return false;
            int leftMovePoint = this.MovePoint;
            if (stairway)
            {
                LadderObject ladderObject = this.GridRef.GetLadder(direction);
                leftMovePoint -= ladderObject.Stagnate;
            }
            GridObject terrian = null;
            for (int i = land.Count - 1; i >= 0; --i)
            {
                if (land[i].IsTerrain)
                {
                    terrian = land[i];
                    break;
                }
            }
            if (terrian == null) return false;
            leftMovePoint -= terrian.Stagnate;
            if (leftMovePoint < 0) return false;
            return true;
        }

        public void Move(Direction direction, bool stairway)
        {
            int srcRow = this.GridRef.PosRow;
            int srcCol = this.GridRef.PosCol;
            int dstRow = srcRow;
            int dstCol = srcCol;
            switch (direction)
            {
                case Direction.POSITIVE_ROW:
                    ++dstRow;
                    break;
                case Direction.POSITIVE_COL:
                    ++dstCol;
                    break;
                case Direction.NEGATIVE_ROW:
                    --dstRow;
                    break;
                case Direction.NEGATIVE_COL:
                    --dstCol;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
            Grid dstGrid = BattleSceneContainer.Instance.BattleMap[dstRow, dstCol];
            bool dstHighland = stairway ^ this.IsHighland;
            IReadOnlyList<GridObject> land = dstHighland ? dstGrid.Highland : dstGrid.Lowland;
            if (land.Count <= 0 || land[land.Count - 1].IsObstacle) throw new ArgumentOutOfRangeException(nameof(direction));
            int leftMovePoint = this.MovePoint;
            if (stairway)
            {
                LadderObject ladderObject = this.GridRef.GetLadder(direction);
                leftMovePoint -= ladderObject.Stagnate;
            }
            GridObject terrian = null;
            for (int i = land.Count - 1; i >= 0; --i)
            {
                if (land[i].IsTerrain)
                {
                    terrian = land[i];
                    break;
                }
            }
            leftMovePoint -= terrian.Stagnate;
            if (leftMovePoint < 0) throw new ArgumentOutOfRangeException(nameof(direction));
            this.MovePoint = leftMovePoint;
            BattleSceneContainer.Instance.BattleMap.MoveStack(srcRow, srcCol, this.IsHighland, dstRow, dstCol, dstHighland);
        }

        public bool CanUseSkill(SkillType skillType, int row, int col)
        {
            throw new NotImplementedException();
        }

        public void UseSkill(SkillType skillType, int row, int col)
        {
            SkillProperty skillProperty = this.CharacterRef.GetSkillProperty(skillType);
            if (skillProperty.islinearUse)
            {
                do
                {
                    if ((skillProperty.linearUseDirection & Direction.POSITIVE_ROW) != 0 && col == this.GridRef.PosCol && skillProperty.useRange.InRange(row - this.GridRef.PosRow)) break;
                    if ((skillProperty.linearUseDirection & Direction.NEGATIVE_ROW) != 0 && col == this.GridRef.PosCol && skillProperty.useRange.InRange(this.GridRef.PosRow - row)) break;
                    if ((skillProperty.linearUseDirection & Direction.POSITIVE_COL) != 0 && row == this.GridRef.PosRow && skillProperty.useRange.InRange(col - this.GridRef.PosCol)) break;
                    if ((skillProperty.linearUseDirection & Direction.NEGATIVE_COL) != 0 && row == this.GridRef.PosRow && skillProperty.useRange.InRange(this.GridRef.PosCol - col)) break;
                    throw new InvalidOperationException("Cannot use this skill at the specified position.");
                } while (false);
            }
            else if(skillProperty.useRange.OutOfRange(Math.Abs(row - this.GridRef.PosRow) + Math.Abs(col - this.GridRef.PosCol))) throw new InvalidOperationException("Cannot use this skill at the specified position.");
            if (skillProperty.targetCount == -1)
            {

            }
            else
            {

            }
        }

        public bool CanUseStunt()
        {
            throw new NotImplementedException();
        }

        public void UseStunt(Stunt stunt, int row, int col)
        {
            if (stunt.Belong != this.CharacterRef) throw new ArgumentException("This stunt is not belong to the character.", nameof(stunt));
            
        }
        
        public override IJSContext GetContext()
        {
            return _apiObj;
        }
    }

    public sealed class Grid : IJSContextProvider
    {
        #region Javascript API class
        private sealed class JSAPI : IJSAPI<Grid>
        {
            private readonly Grid _outer;

            public JSAPI(Grid outer)
            {
                _outer = outer;
            }

            public Grid Origin(JSContextHelper proof)
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

        private readonly List<GridObject> _highland;
        private readonly List<GridObject> _lowland;
        private LadderObject _positiveRowLadder = null;
        private LadderObject _positiveColLadder = null;
        private LadderObject _negativeRowLadder = null;
        private LadderObject _negativeColLadder = null;
        private bool _isMiddleLand = false;
        private readonly int _posRow;
        private readonly int _posCol;

        public IReadOnlyList<GridObject> Highland => _highland;
        public IReadOnlyList<GridObject> Lowland => _lowland;
        public LadderObject PositiveRowLadder { get => _positiveRowLadder; set => _positiveRowLadder = value; }
        public LadderObject PositiveColLadder { get => _positiveColLadder; set => _positiveColLadder = value; }
        public LadderObject NegativeRowLadder { get => _negativeRowLadder; set => _negativeRowLadder = value; }
        public LadderObject NegativeColLadder { get => _negativeColLadder; set => _negativeColLadder = value; }
        public bool IsMiddleLand { get => _isMiddleLand; set => _isMiddleLand = value; }
        public int PosRow => _posRow;
        public int PosCol => _posCol;

        public Grid(int row, int col)
        {
            _posRow = row;
            _posCol = col;
            _highland = new List<GridObject>();
            _lowland = new List<GridObject>();
            _apiObj = new JSAPI(this);
        }

        public LadderObject GetLadder(Direction direction)
        {
            switch (direction)
            {
                case Direction.POSITIVE_ROW:
                    return _positiveRowLadder;
                case Direction.POSITIVE_COL:
                    return _positiveColLadder;
                case Direction.NEGATIVE_ROW:
                    return _negativeRowLadder;
                case Direction.NEGATIVE_COL:
                    return _negativeColLadder;
                default:
                    return null;
            }
        }

        public IJSContext GetContext()
        {
            return _apiObj;
        }

        public void SetContext(IJSContext context) { }
    }

    public sealed class BattleMap
    {
        private Grid[,] _grids;
        private int _rows;
        private int _cols;
        
        public int Rows => _rows;
        public int Cols => _cols;
        public Grid this[int row, int col] => _grids[row, col];

        public BattleMap()
        {
            this.Reset(0, 0);
        }
        
        public void Reset(int rows, int cols)
        {
            _rows = rows;
            _cols = cols;
            _grids = new Grid[_rows, _cols];
            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _cols; ++j)
                {
                    _grids[i, j] = new Grid(i, j);
                }
            }
        }

        public void MoveStack(int srcRow, int srcCol, bool srcHighland, int dstRow, int dstCol, bool dstHighland, int count = 1)
        {
            Grid srcGrid = _grids[srcRow, srcCol];
            Grid dstGrid = _grids[dstRow, dstCol];
            List<GridObject> srcLand, dstLand;
            if (srcHighland) srcLand = (List<GridObject>)srcGrid.Highland;
            else srcLand = (List<GridObject>)srcGrid.Lowland;
            if (dstHighland) dstLand = (List<GridObject>)dstGrid.Highland;
            else dstLand = (List<GridObject>)dstGrid.Lowland;
            int lastIndex = dstLand.Count;
            for (int i = srcLand.Count - 1; srcLand.Count - i <= count; --i)
            {
                GridObject gridObject = srcLand[i];
                gridObject.GridRef = dstGrid;
                gridObject.IsHighland = dstHighland;
                dstLand.Insert(lastIndex, gridObject);
                srcLand.RemoveAt(i);
            }
        }
        
    }
}