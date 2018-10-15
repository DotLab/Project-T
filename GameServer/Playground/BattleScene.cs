using GameServer.CharacterComponents;
using GameServer.Client;
using GameServer.Playground.BattleComponent;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameServer.Playground {
	public sealed class BattleScene : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<BattleScene> {
			private readonly BattleScene _outer;

			public JSAPI(BattleScene outer) {
				_outer = outer;
			}

			public IJSAPI<SceneObject> findObject(string id) {
				try {
					var obj = _outer.FindObject(id);
					return obj == null ? null : (IJSAPI<SceneObject>)obj.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}
			
			public BattleScene Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private static readonly BattleScene _instance = new BattleScene();
		public static BattleScene Instance => _instance;

		private readonly IdentifiedObjectList<SceneObject> _objectList;
		private readonly List<ActableGridObject> _actableObjectList;
		private readonly BattleMap _battleMap;
		private ActableGridObject _currentActable = null;
		private int _roundCount = 0;
		private bool _isPlaying = false;

		public IReadonlyIdentifiedObjectList<SceneObject> ObjectList => _objectList;
		public IReadOnlyList<ActableGridObject> ActableObjectList => _actableObjectList;
		public BattleMap BattleMap => _battleMap;
		public ActableGridObject CurrentActable => _currentActable;
		public int RoundCount => _roundCount;
		
		public BattleScene() {
			_objectList = new IdentifiedObjectList<SceneObject>();
			_actableObjectList = new List<ActableGridObject>();
			_battleMap = new BattleMap();
			_apiObj = new JSAPI(this);
		}

		public void ClientSynchronizeData(BattleSceneProxy battleScene) {
			battleScene.Reset(_battleMap.Rows, _battleMap.Cols);
			for (int row = 0; row < _battleMap.Rows; ++row) {
				for (int col = 0; col < _battleMap.Cols; ++col) {
					var grid = _battleMap[row, col];
					battleScene.UpdateGridData(grid);
					foreach (var lowlandObj in grid.LowlandObjects) {
						battleScene.PushGridObject(lowlandObj);
					}
					foreach (var highlandObj in grid.HighlandObjects) {
						battleScene.PushGridObject(highlandObj);
					}
					if (grid.PositiveRowLadder != null) battleScene.AddLadderObject(grid.PositiveRowLadder);
					if (grid.PositiveColLadder != null) battleScene.AddLadderObject(grid.PositiveColLadder);
					if (grid.NegativeRowLadder != null) battleScene.AddLadderObject(grid.NegativeRowLadder);
					if (grid.NegativeColLadder != null) battleScene.AddLadderObject(grid.NegativeColLadder);
				}
			}
			if (_isPlaying) {
				battleScene.StartBattle();
				battleScene.UpdateTurnOrder(_actableObjectList);
				if (_currentActable != null) {
					battleScene.NewTurn(_currentActable);
					battleScene.UpdateActionPoint(_currentActable);
				}
			}
		}

		public void ClientSynchronizeState(BattleSceneProxy battleScene) {
			battleScene.UpdateTurnOrder(_actableObjectList);
			if (_currentActable != null) {
				battleScene.NewTurn(_currentActable);
				battleScene.UpdateActionPoint(_currentActable);
				battleScene.DisplayTakeExtraMovePoint(_currentActable, SkillType.Athletics);
				if (SkillChecker.Instance.IsChecking) {

				}
			}
		}

		public void Reset(int rows, int cols) {
			_battleMap.Reset(rows, cols);
			_objectList.Clear();
			_actableObjectList.Clear();
			_currentActable = null;
			_roundCount = 0;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.Reset(rows, cols);
			}
			Game.DM.Client.BattleScene.Reset(rows, cols);
			Update();
			_isPlaying = false;
		}

		public SceneObject FindObject(string id) {
			if (_objectList.TryGetValue(id, out SceneObject gridObject)) return gridObject;
			return null;
		}

		public void PushGridObject(int row, int col, bool highland, GridObject gridObject) {
			if (_objectList.Contains(gridObject)) throw new ArgumentException("This object is already added to scene.", nameof(gridObject));
			List<GridObject> land;
			if (highland) land = (List<GridObject>)_battleMap[row, col].HighlandObjects;
			else land = (List<GridObject>)_battleMap[row, col].LowlandObjects;
			land.Add(gridObject);
			gridObject.SetGridRef(_battleMap[row, col]);
			gridObject.SetHighland(highland);
			_objectList.Add(gridObject);
			if (gridObject is ActableGridObject) {
				_actableObjectList.Add((ActableGridObject)gridObject);
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
				}
				Game.DM.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
			}
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.PushGridObject(gridObject);
			}
			Game.DM.Client.BattleScene.PushGridObject(gridObject);
		}

		public GridObject PopGridObject(int row, int col, bool highland) {
			GridObject ret = null;
			List<GridObject> land;
			if (highland) land = (List<GridObject>)_battleMap[row, col].HighlandObjects;
			else land = (List<GridObject>)_battleMap[row, col].LowlandObjects;
			if (land.Count > 0) {
				ret = land[land.Count - 1];
				land.RemoveAt(land.Count - 1);
				_objectList.Remove(ret);
				if (ret is ActableGridObject) {
					_actableObjectList.Remove((ActableGridObject)ret);
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
					}
					Game.DM.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
				}
			}
			if (ret != null) {
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.RemoveGridObject(ret);
				}
				Game.DM.Client.BattleScene.RemoveGridObject(ret);
				ret.SetGridRef(null);
			}
			return ret;
		}

		public bool RemoveGridObject(GridObject gridObject) {
			if (!_objectList.Contains(gridObject)) return false;
			if (gridObject.Highland) ((List<GridObject>)gridObject.GridRef.HighlandObjects).Remove(gridObject);
			else ((List<GridObject>)gridObject.GridRef.LowlandObjects).Remove(gridObject);
			_objectList.Remove(gridObject);
			if (gridObject is ActableGridObject) {
				_actableObjectList.Remove((ActableGridObject)gridObject);
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
				}
				Game.DM.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
			}
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.RemoveGridObject(gridObject);
			}
			Game.DM.Client.BattleScene.RemoveGridObject(gridObject);
			gridObject.SetGridRef(null);
			return true;
		}

		public void AddLadderObject(int row, int col, BattleMapDirection direction, LadderObject ladderObject) {
			if (_objectList.Contains(ladderObject)) throw new ArgumentException("This object is already added to scene.", nameof(ladderObject));
			if (row == 0 && direction == BattleMapDirection.NEGATIVE_ROW) throw new ArgumentOutOfRangeException(nameof(direction));
			if (row == _battleMap.Rows - 1 && direction == BattleMapDirection.POSITIVE_ROW) throw new ArgumentOutOfRangeException(nameof(direction));
			if (col == 0 && direction == BattleMapDirection.NEGATIVE_COL) throw new ArgumentOutOfRangeException(nameof(direction));
			if (col == _battleMap.Cols - 1 && direction == BattleMapDirection.POSITIVE_COL) throw new ArgumentOutOfRangeException(nameof(direction));
			Grid grid = _battleMap[row, col];
			if (grid.GetLadder(direction) != null) throw new ArgumentException("This grid has already bound a ladder.", nameof(ladderObject));
			switch (direction) {
				case BattleMapDirection.POSITIVE_ROW: {
						Grid anotherGrid = _battleMap[row + 1, col];
						grid.SetLadderRef(BattleMapDirection.POSITIVE_ROW, ladderObject);
						ladderObject.SetFirstGridRef(grid);
						anotherGrid.SetLadderRef(BattleMapDirection.NEGATIVE_ROW, ladderObject);
						ladderObject.SetSecondGridRef(anotherGrid);
					}
					break;
				case BattleMapDirection.POSITIVE_COL: {
						Grid anotherGrid = _battleMap[row, col + 1];
						grid.SetLadderRef(BattleMapDirection.POSITIVE_COL, ladderObject);
						ladderObject.SetFirstGridRef(grid);
						anotherGrid.SetLadderRef(BattleMapDirection.NEGATIVE_COL, ladderObject);
						ladderObject.SetSecondGridRef(anotherGrid);
					}
					break;
				case BattleMapDirection.NEGATIVE_ROW: {
						Grid anotherGrid = _battleMap[row - 1, col];
						grid.SetLadderRef(BattleMapDirection.NEGATIVE_ROW, ladderObject);
						ladderObject.SetFirstGridRef(grid);
						anotherGrid.SetLadderRef(BattleMapDirection.POSITIVE_ROW, ladderObject);
						ladderObject.SetSecondGridRef(anotherGrid);
					}
					break;
				case BattleMapDirection.NEGATIVE_COL: {
						Grid anotherGrid = _battleMap[row, col - 1];
						grid.SetLadderRef(BattleMapDirection.NEGATIVE_COL, ladderObject);
						ladderObject.SetFirstGridRef(grid);
						anotherGrid.SetLadderRef(BattleMapDirection.POSITIVE_COL, ladderObject);
						ladderObject.SetSecondGridRef(anotherGrid);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction));
			}
			ladderObject.SetDirectionOnFirstGrid(direction);
			_objectList.Add(ladderObject);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.AddLadderObject(ladderObject);
			}
			Game.DM.Client.BattleScene.AddLadderObject(ladderObject);
		}

		public LadderObject RemoveLadderObject(int row, int col, BattleMapDirection direction) {
			if (row == 0 && direction == BattleMapDirection.NEGATIVE_ROW) return null;
			if (row == _battleMap.Rows - 1 && direction == BattleMapDirection.POSITIVE_ROW) return null;
			if (col == 0 && direction == BattleMapDirection.NEGATIVE_COL) return null;
			if (col == _battleMap.Cols - 1 && direction == BattleMapDirection.POSITIVE_COL) return null;
			Grid grid = _battleMap[row, col];
			if (grid.GetLadder(direction) == null) return null;
			LadderObject ladderObject;
			switch (direction) {
				case BattleMapDirection.POSITIVE_ROW: {
						Grid anotherGrid = _battleMap[row + 1, col];
						ladderObject = grid.PositiveRowLadder;
						ladderObject.SetFirstGridRef(null);
						ladderObject.SetSecondGridRef(null);
						grid.SetLadderRef(BattleMapDirection.POSITIVE_ROW, null);
						anotherGrid.SetLadderRef(BattleMapDirection.NEGATIVE_ROW, null);
					}
					break;
				case BattleMapDirection.POSITIVE_COL: {
						Grid anotherGrid = _battleMap[row, col + 1];
						ladderObject = grid.PositiveColLadder;
						ladderObject.SetFirstGridRef(null);
						ladderObject.SetSecondGridRef(null);
						grid.SetLadderRef(BattleMapDirection.POSITIVE_COL, null);
						anotherGrid.SetLadderRef(BattleMapDirection.NEGATIVE_COL, null);
					}
					break;
				case BattleMapDirection.NEGATIVE_ROW: {
						Grid anotherGrid = _battleMap[row - 1, col];
						ladderObject = grid.NegativeRowLadder;
						ladderObject.SetFirstGridRef(null);
						ladderObject.SetSecondGridRef(null);
						grid.SetLadderRef(BattleMapDirection.NEGATIVE_ROW, null);
						anotherGrid.SetLadderRef(BattleMapDirection.POSITIVE_ROW, null);
					}
					break;
				case BattleMapDirection.NEGATIVE_COL: {
						Grid anotherGrid = _battleMap[row, col - 1];
						ladderObject = grid.NegativeColLadder;
						ladderObject.SetFirstGridRef(null);
						ladderObject.SetSecondGridRef(null);
						grid.SetLadderRef(BattleMapDirection.NEGATIVE_COL, null);
						anotherGrid.SetLadderRef(BattleMapDirection.POSITIVE_COL, null);
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(direction));
			}
			_objectList.Remove(ladderObject);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.RemoveLadderObject(ladderObject);
			}
			Game.DM.Client.BattleScene.RemoveLadderObject(ladderObject);
			return ladderObject;
		}

		public bool RemoveLadderObject(LadderObject ladderObject) {
			if (ladderObject == null) throw new ArgumentNullException(nameof(ladderObject));
			if (!_objectList.Contains(ladderObject)) return false;
			if (ladderObject.GridRef.PositiveRowLadder == ladderObject) ladderObject.GridRef.SetLadderRef(BattleMapDirection.POSITIVE_ROW, null);
			else if (ladderObject.GridRef.PositiveColLadder == ladderObject) ladderObject.GridRef.SetLadderRef(BattleMapDirection.POSITIVE_COL, null);
			else if (ladderObject.GridRef.NegativeRowLadder == ladderObject) ladderObject.GridRef.SetLadderRef(BattleMapDirection.NEGATIVE_ROW, null);
			else if (ladderObject.GridRef.NegativeColLadder == ladderObject) ladderObject.GridRef.SetLadderRef(BattleMapDirection.POSITIVE_COL, null);
			else return false;
			if (ladderObject.SecondGridRef.PositiveRowLadder == ladderObject) ladderObject.SecondGridRef.SetLadderRef(BattleMapDirection.POSITIVE_ROW, null);
			else if (ladderObject.SecondGridRef.PositiveColLadder == ladderObject) ladderObject.SecondGridRef.SetLadderRef(BattleMapDirection.POSITIVE_COL, null);
			else if (ladderObject.SecondGridRef.NegativeRowLadder == ladderObject) ladderObject.SecondGridRef.SetLadderRef(BattleMapDirection.NEGATIVE_ROW, null);
			else if (ladderObject.SecondGridRef.NegativeColLadder == ladderObject) ladderObject.SecondGridRef.SetLadderRef(BattleMapDirection.NEGATIVE_COL, null);
			else return false;
			_objectList.Remove(ladderObject);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.RemoveLadderObject(ladderObject);
			}
			Game.DM.Client.BattleScene.RemoveLadderObject(ladderObject);
			ladderObject.SetFirstGridRef(null);
			ladderObject.SetSecondGridRef(null);
			return true;
		}

		public void StartBattle() {
			Update();
			_isPlaying = true;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.StartBattle();
			}
			Game.DM.Client.BattleScene.StartBattle();
			NewRound();
		}

		public void NewRound() {
			foreach (ActableGridObject actableObject in _actableObjectList) {
				var notice = actableObject.CharacterRef.GetSkill(SkillType.Notice);
				var athletics = actableObject.CharacterRef.GetSkill(SkillType.Athletics);
				int[] dicePoints = FateDice.Roll();
				if (actableObject.CharacterRef.ControlPlayer != null) {
					actableObject.CharacterRef.ControlPlayer.Client.DisplayDicePoints(actableObject.CharacterRef.ControlPlayer, dicePoints);
				}
				int sumPoint = 0;
				foreach (int point in dicePoints) sumPoint += point;
				actableObject.ActionPriority = notice.Level + sumPoint;
				int ap = 1 + (athletics.Level >= 1 ? (athletics.Level - 1) / 2 : 0);
				actableObject.ActionPoint = actableObject.ActionPoint >= 0 ? ap : ap + actableObject.ActionPoint;
				actableObject.ActionPointMax = ap;
				actableObject.MovePoint = 0;
			}
			_actableObjectList.Sort((x, y) => { return y.ActionPriority - x.ActionPriority; });
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
			}
			Game.DM.Client.BattleScene.UpdateTurnOrder(_actableObjectList);
			if (_actableObjectList.Count > 0) {
				_currentActable = _actableObjectList[0];
				_currentActable.ResetHinderingRecords();
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.NewTurn(_currentActable);
					player.Client.BattleScene.UpdateActionPoint(_currentActable);
				}
				Game.DM.Client.BattleScene.NewTurn(_currentActable);
				Game.DM.Client.BattleScene.UpdateActionPoint(_currentActable);
				_currentActable.AddMovePoint();
			} else _currentActable = null;
			++_roundCount;
			Update();
		}

		public void CurrentTurnOver() {
			if (_currentActable == null) throw new InvalidOperationException("Current acting character is null.");
			int next = _actableObjectList.IndexOf(_currentActable) + 1;
			if (next >= _actableObjectList.Count) {
				NewRound();
			} else {
				_currentActable = _actableObjectList[next];
				_currentActable.ResetHinderingRecords();
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.NewTurn(_currentActable);
					player.Client.BattleScene.UpdateActionPoint(_currentActable);
				}
				Game.DM.Client.BattleScene.NewTurn(_currentActable);
				Game.DM.Client.BattleScene.UpdateActionPoint(_currentActable);
				_currentActable.AddMovePoint();
			}
			Update();
		}

		public void Update() {
			foreach (var obj in _objectList) {
				if (!obj.CharacterRef.PhysicsInvincible && obj.CharacterRef.PhysicsStress >= obj.CharacterRef.PhysicsStressMax) {
					obj.MarkDestroyed();
				}
				if (!obj.CharacterRef.MentalInvincible && obj.CharacterRef.MentalStress >= obj.CharacterRef.MentalStressMax) {
					obj.MarkDestroyed();
				}
			}
			List<SceneObject> removal = new List<SceneObject>();
			for (int row = 0; row < _battleMap.Rows; ++row) {
				for (int col = 0; col < _battleMap.Cols; ++col) {
					var grid = _battleMap[row, col];
					foreach (var lowlandObj in grid.LowlandObjects) {
						if (lowlandObj.Destroyed) removal.Add(lowlandObj);
					}
					foreach (var highlandObj in grid.HighlandObjects) {
						if (highlandObj.Destroyed) removal.Add(highlandObj);
					}
					if (grid.PositiveRowLadder != null && grid.PositiveRowLadder.Destroyed && !removal.Contains(grid.PositiveRowLadder)) removal.Add(grid.PositiveRowLadder);
					if (grid.PositiveColLadder != null && grid.PositiveColLadder.Destroyed && !removal.Contains(grid.PositiveColLadder)) removal.Add(grid.PositiveColLadder);
					if (grid.NegativeRowLadder != null && grid.NegativeRowLadder.Destroyed && !removal.Contains(grid.NegativeRowLadder)) removal.Add(grid.NegativeRowLadder);
					if (grid.NegativeColLadder != null && grid.NegativeColLadder.Destroyed && !removal.Contains(grid.NegativeColLadder)) removal.Add(grid.NegativeColLadder);
				}
			}
			foreach (var obj in removal) {
				var asGridObject = obj as GridObject;
				var asLadderObject = obj as LadderObject;
				if (asGridObject != null) {
					RemoveGridObject(asGridObject);
				}
				if (asLadderObject != null) {
					RemoveLadderObject(asLadderObject);
				}
			}
			// ...
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}

namespace GameServer.Playground.BattleComponent {
	public class SceneObject : IIdentifiable {
		#region Javascript API class
		protected class JSAPI : IJSAPI<SceneObject> {
			private readonly SceneObject _outer;

			public JSAPI(SceneObject outer) {
				_outer = outer;
			}

			public string getID() {
				try {
					return _outer.ID;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public string getName() {
				try {
					return _outer.Name;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setName(string val) {
				try {
					_outer.Name = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public string getDescription() {
				try {
					return _outer.Description;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public void setDescription(string val) {
				try {
					_outer.Description = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public IJSAPI<Character> getCharacterRef() {
				try {
					return (IJSAPI<Character>)_outer.CharacterRef.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public IJSAPI<Grid> getGridRef() {
				try {
					if (_outer.GridRef == null) return null;
					return (IJSAPI<Grid>)_outer.GridRef.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public int getStagnate() {
				try {
					return _outer.Stagnate;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setStagnate(int val) {
				try {
					_outer.Stagnate = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public bool isDestroyed() {
				try {
					return _outer.Destroyed;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public void markDestroyed() {
				try {
					_outer.MarkDestroyed();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public SceneObject Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		protected readonly Character _characterRef;
		protected Grid _gridRef;
		protected int _stagnate;

		protected SceneObject(Character characterRef) {
			_characterRef = characterRef ?? throw new ArgumentNullException(nameof(characterRef));
			_apiObj = new JSAPI(this);
		}

		public string ID => _characterRef.ID;
		public string Name { get => _characterRef.Name; set => _characterRef.Name = value; }
		public string Description { get => _characterRef.Description; set => _characterRef.Description = value; }
		public Character CharacterRef => _characterRef;
		public Grid GridRef => _gridRef;
		public int Stagnate { get => _stagnate; set => _stagnate = value; }
		public bool Destroyed => _characterRef.Destroyed;

		public void MarkDestroyed() {
			_characterRef.MarkDestroyed();
		}

		public Situation GetStuntSituationForPassive(SceneObject initiative, SkillType initiativeSkillType, Stunt stunt, CharacterAction checkingAction) {
			if (initiative == null) throw new ArgumentNullException(nameof(initiative));
			if (initiativeSkillType == null) throw new ArgumentNullException(nameof(initiativeSkillType));
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			var ret = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = false,
				InitiativeSS = null, InitiativeBS = initiative,
				PassivesSS = null, PassivesBS = new SceneObject[] { this },
				Action = checkingAction,
				InitiativeSkillType = initiativeSkillType
			};
			return ret;
		}

		public virtual IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public class GridObject : SceneObject {
		#region Javascript API class
		protected new class JSAPI : SceneObject.JSAPI, IJSAPI<GridObject> {
			private readonly GridObject _outer;

			public JSAPI(GridObject outer) : base(outer) {
				_outer = outer;
			}

			public bool isObstacle() {
				try {
					return _outer.Obstacle;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return true;
				}
			}

			public void setObstacle(bool val) {
				try {
					_outer.Obstacle = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public bool isHighland() {
				try {
					return _outer.Highland;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public int getDirection() {
				try {
					return (int)_outer.Direction;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setDirection(int val) {
				try {
					_outer.Direction = (BattleMapDirection)val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			GridObject IJSAPI<GridObject>.Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		protected bool _obstacle;
		protected bool _highland;
		protected BattleMapDirection _direction;

		public bool Obstacle { get => _obstacle; set => _obstacle = value; }
		public bool Highland => _highland;
		public BattleMapDirection Direction { get => _direction; set => _direction = value; }

		public GridObject(Character characterRef) :
			base(characterRef) {
			_apiObj = new JSAPI(this);
		}

		public void SetHighland(bool highland) {
			_highland = highland;
		}

		public void SetGridRef(Grid gridRef) {
			_gridRef = gridRef;
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}

	public sealed class LadderObject : SceneObject {
		#region Javascript API class
		private sealed new class JSAPI : SceneObject.JSAPI, IJSAPI<LadderObject> {
			private readonly LadderObject _outer;

			public JSAPI(LadderObject outer) : base(outer) {
				_outer = outer;
			}

			public IJSAPI<Grid> getSecondGridRef() {
				try {
					if (_outer.SecondGridRef == null) return null;
					return (IJSAPI<Grid>)_outer.SecondGridRef.GetContext();
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return null;
				}
			}

			public int getDirectionOnFirstGrid() {
				try {
					return (int)_outer.DirectionOnFirstGrid;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			LadderObject IJSAPI<LadderObject>.Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private Grid _secondGridRef;
		private BattleMapDirection _directionOnFirstGrid;

		public Grid SecondGridRef => _secondGridRef;
		public BattleMapDirection DirectionOnFirstGrid => _directionOnFirstGrid;

		public LadderObject(Character characterRef) :
			base(characterRef) {
			_apiObj = new JSAPI(this);
		}

		public void SetFirstGridRef(Grid gridRef) {
			_gridRef = gridRef;
		}

		public void SetSecondGridRef(Grid gridRef) {
			_secondGridRef = gridRef;
		}

		public void SetDirectionOnFirstGrid(BattleMapDirection direction) {
			_directionOnFirstGrid = direction;
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}

	public sealed class ActableGridObject : GridObject {
		#region Javascript API class
		private sealed new class JSAPI : GridObject.JSAPI, IJSAPI<ActableGridObject> {
			private readonly ActableGridObject _outer;

			public JSAPI(ActableGridObject outer) : base(outer) {
				_outer = outer;
			}

			public int getActionPriority() {
				try {
					return _outer.ActionPriority;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return 0;
				}
			}

			public void setActionPriority(int val) {
				try {
					_outer.ActionPriority = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getActionPoint() {
				try {
					return _outer.ActionPoint;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setActionPoint(int val) {
				try {
					_outer.ActionPoint = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getActionPointMax() {
				try {
					return _outer.ActionPointMax;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setActionPointMax(int val) {
				try {
					_outer.ActionPointMax = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public bool isMovable() {
				try {
					return _outer.Movable;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public void setMovable(bool val) {
				try {
					_outer.Movable = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public int getMovePoint() {
				try {
					return _outer.MovePoint;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return -1;
				}
			}

			public void setMovePoint(int val) {
				try {
					_outer.MovePoint = val;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}
			/*
			public void useSkillWithStuntComplete(IJSAPI<SkillType> skillType, CharacterAction action, IJSAPI<SceneObject>[] targets, Action<bool, string> completeFunc) {
				useSkillWithStuntComplete(skillType, action, targets, completeFunc, false, true, 0, null);
			}

			public void useSkillWithStuntComplete(
				IJSAPI<SkillType> skillType, CharacterAction action, IJSAPI<SceneObject>[] targets,
				Action<bool, string> completeFunc, bool skipDMCheck, bool bigone, int extraPoint, int[] fixedDicePoints
				) {
				try {
					var origin_skillType = JSContextHelper.Instance.GetAPIOrigin(skillType);
					var origin_targets = new List<SceneObject>();
					foreach (var target in targets) {
						origin_targets.Add(JSContextHelper.Instance.GetAPIOrigin(target));
					}
					if (!skipDMCheck) {
						if (action == CharacterAction.CREATE_ASPECT) {
							bool result = Game.DM.DMClient.RequireDMCheck(_outer.CharacterRef.Controller,
								_outer.CharacterRef.Name + "想使用" + _outer.CharacterRef.GetSkill(origin_skillType).Name + ",可以吗？");
							if (result) {
								completeFunc(true, "");
								BattleScene.Instance.StartCheck(_outer, origin_targets, action, origin_skillType, () => { }, null, bigone, extraPoint, fixedDicePoints);
							} else {
								completeFunc(false, "DM拒绝了你的选择");
							}
							return;
						}
					}
					completeFunc(true, "");
					BattleScene.Instance.StartCheck(_outer, origin_targets, action, origin_skillType, () => { }, null, bigone, extraPoint, fixedDicePoints);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void displayUsingStunt(IJSAPI<Stunt> stunt) {
				try {
					var origin_stunt = JSContextHelper.Instance.GetAPIOrigin(stunt);
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.DisplayUsingStunt(_outer, origin_stunt);
					}
					Game.DM.Client.BattleScene.DisplayUsingStunt(_outer, origin_stunt);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}
			*/
			ActableGridObject IJSAPI<ActableGridObject>.Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private static void GetAroundReachablePlacesRecursively(List<ReachablePlace> list, ReachablePlace center) {
			int leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.POSITIVE_ROW, false, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row + 1,
						col = center.pos.col,
						highland = center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.POSITIVE_COL, false, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row,
						col = center.pos.col + 1,
						highland = center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.NEGATIVE_ROW, false, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row - 1,
						col = center.pos.col,
						highland = center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.NEGATIVE_COL, false, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row,
						col = center.pos.col - 1,
						highland = center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.POSITIVE_ROW, true, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row + 1,
						col = center.pos.col,
						highland = !center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.POSITIVE_COL, true, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row,
						col = center.pos.col + 1,
						highland = !center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.NEGATIVE_ROW, true, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row - 1,
						col = center.pos.col,
						highland = !center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
			leftMovePoint = center.leftMovePoint;
			if (CanMove(center.pos.row, center.pos.col, center.pos.highland, BattleMapDirection.NEGATIVE_COL, true, ref leftMovePoint)) {
				var next = new ReachablePlace() {
					pos = new GridPos() {
						row = center.pos.row,
						col = center.pos.col - 1,
						highland = !center.pos.highland
					},
					prevPlace = center,
					leftMovePoint = leftMovePoint
				};
				var markedPlace = MarkedReachablePlace(list, next.pos.row, next.pos.col, next.pos.highland);
				if (markedPlace != null) {
					if (markedPlace.leftMovePoint < next.leftMovePoint) {
						list.Remove(markedPlace);
						list.Add(next);
						GetAroundReachablePlacesRecursively(list, next);
					}
				} else {
					list.Add(next);
					GetAroundReachablePlacesRecursively(list, next);
				}
			}
		}

		private static bool CanMove(int srcRow, int srcCol, bool srcHighland, BattleMapDirection direction, bool stairway, ref int leftMovePoint) {
			int dstRow = srcRow;
			int dstCol = srcCol;
			switch (direction) {
				case BattleMapDirection.POSITIVE_ROW:
					++dstRow;
					break;
				case BattleMapDirection.POSITIVE_COL:
					++dstCol;
					break;
				case BattleMapDirection.NEGATIVE_ROW:
					--dstRow;
					break;
				case BattleMapDirection.NEGATIVE_COL:
					--dstCol;
					break;
				default:
					return false;
			}
			if (dstRow < 0 || dstRow >= BattleScene.Instance.BattleMap.Rows || dstCol < 0 || dstCol >= BattleScene.Instance.BattleMap.Cols) return false;
			Grid dstGrid = BattleScene.Instance.BattleMap[dstRow, dstCol];
			bool dstHighland = stairway ^ srcHighland;
			IReadOnlyList<GridObject> land = dstHighland ? dstGrid.HighlandObjects : dstGrid.LowlandObjects;
			if (land.Count <= 0 || land[land.Count - 1].Obstacle) return false;
			if (stairway) {
				LadderObject ladderObject = BattleScene.Instance.BattleMap[srcRow, srcCol].GetLadder(direction);
				leftMovePoint -= ladderObject.Stagnate;
			}
			leftMovePoint -= land[land.Count - 1].Stagnate;
			if (leftMovePoint < 0) return false;
			return true;
		}

		private static ReachablePlace MarkedReachablePlace(List<ReachablePlace> list, int row, int col, bool highland) {
			foreach (var place in list) {
				if (place.pos.row == row && place.pos.col == col && place.pos.highland == highland) return place;
			}
			return null;
		}

		private int _actionPriority = 0;
		private int _actionPoint = 0;
		private int _actionPointMax = 0;
		private bool _movable = false;
		private int _movePoint = 0;

		private readonly List<ActableGridObject> _hinderMovingObjectsRecord = new List<ActableGridObject>();
		private bool _hinderMovingSuccess = false;

		public int ActionPriority { get => _actionPriority; set => _actionPriority = value; }
		public int ActionPoint { get => _actionPoint; set => _actionPoint = value; }
		public int ActionPointMax { get => _actionPointMax; set => _actionPointMax = value; }
		public bool Movable { get => _movable; set => _movable = value; }
		public int MovePoint { get => _movable ? _movePoint : 0; set { if (_movable) _movePoint = value; } }

		public ActableGridObject(Character characterRef, bool movable = true) :
			base(characterRef) {
			_apiObj = new JSAPI(this);
			_movable = movable;
			_obstacle = true;
		}

		public void ResetHinderingRecords() {
			_hinderMovingObjectsRecord.Clear();
		}

		public void AddMovePoint() {
			var athletics = this.CharacterRef.GetSkill(SkillType.Athletics);
			int mp = 1 + (athletics.Level >= 1 ? (athletics.Level - 1) / 2 : 0);
			this.MovePoint += mp;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.UpdateMovePoint(this);
			}
			Game.DM.Client.BattleScene.UpdateMovePoint(this);
		}

		public bool CanTakeExtraMove() {
			if (this.ActionPoint - 1 < 0 || !this.Movable) return false;
			else return true;
		}

		public void TakeExtraMovePoint() {
			if (!CanTakeExtraMove()) throw new InvalidOperationException("Cannot take extra move.");
			--this.ActionPoint;
			this.AddMovePoint();
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.UpdateActionPoint(this);
				player.Client.BattleScene.DisplayTakeExtraMovePoint(this, SkillType.Athletics);
			}
			Game.DM.Client.BattleScene.UpdateActionPoint(this);
			Game.DM.Client.BattleScene.DisplayTakeExtraMovePoint(this, SkillType.Athletics);
		}

		public List<ReachablePlace> GetReachablePlaceList() {
			List<ReachablePlace> ret = new List<ReachablePlace>();
			ReachablePlace begin = new ReachablePlace {
				prevPlace = null,
				pos = new GridPos() {
					row = this.GridRef.PosRow,
					col = this.GridRef.PosCol,
					highland = this.Highland,
				},
				leftMovePoint = this.MovePoint
			};
			ret.Add(begin);
			GetAroundReachablePlacesRecursively(ret, begin);
			return ret;
		}
		
		public void MoveTo(int dstRow, int dstCol, bool dstHighland) {
			var path = GetReachablePlaceList();
			var dst = MarkedReachablePlace(path, dstRow, dstCol, dstHighland);
			if (dst == null) throw new InvalidOperationException("Cannot reach the place.");
			List<BattleMapDirection> moveDirections = new List<BattleMapDirection>();
			List<bool> moveStairways = new List<bool>();
			while (dst.prevPlace != null) {
				var prevPlace = dst.prevPlace;
				int direction = 0;
				if (prevPlace.pos.row - dst.pos.row == 1 && prevPlace.pos.col == dst.pos.col) direction = (int)BattleMapDirection.NEGATIVE_ROW;
				else if (prevPlace.pos.row - dst.pos.row == -1 && prevPlace.pos.col == dst.pos.col) direction = (int)BattleMapDirection.POSITIVE_ROW;
				else if (prevPlace.pos.row == dst.pos.row && prevPlace.pos.col - dst.pos.col == 1) direction = (int)BattleMapDirection.NEGATIVE_COL;
				else if (prevPlace.pos.row == dst.pos.row && prevPlace.pos.col - dst.pos.col == -1) direction = (int)BattleMapDirection.POSITIVE_COL;
				Debug.Assert(direction != 0, "GetReachablePlaceList() returns a invalid list.");
				bool stairway = prevPlace.pos.highland ^ dst.pos.highland;
				moveDirections.Add((BattleMapDirection)direction);
				moveStairways.Add(stairway);
				dst = prevPlace;
			}
			moveDirections.Reverse();
			moveStairways.Reverse();
			for (int i = 0; i < moveDirections.Count; ++i) {
				BattleMapDirection direction = moveDirections[i];
				bool stairway = moveStairways[i];
				int srcRow = this.GridRef.PosRow, srcCol = this.GridRef.PosCol;
				bool srcHighland = this.Highland;
				int leftMovePoint = this.MovePoint;
				if (!CanMove(srcRow, srcCol, srcHighland, direction, stairway, ref leftMovePoint))
					throw new InvalidOperationException("Cannot reach the place.");
				var hinderObjs = new Dictionary<User, List<ActableGridObject>>();
				for (int row = srcRow - 1; row <= srcRow + 1; ++row) {
					for (int col = srcCol - 1; col <= srcCol + 1; ++col) {
						if (row >= 0 && row < BattleScene.Instance.BattleMap.Rows && col >= 0 && col < BattleScene.Instance.BattleMap.Cols) {
							var grid = BattleScene.Instance.BattleMap[row, col];
							IReadOnlyList<SceneObject> objs;
							if (srcHighland) {
								objs = grid.HighlandObjects;
							} else {
								objs = grid.LowlandObjects;
							}
							foreach (var obj in objs) {
								var actableObj = obj as ActableGridObject;
								if (actableObj != null) {
									if (actableObj != this && actableObj.Movable && !actableObj.CharacterRef.IsPartyWith(this.CharacterRef) && !_hinderMovingObjectsRecord.Contains(actableObj)) {
										if (!hinderObjs.ContainsKey(actableObj.CharacterRef.Controller)) {
											hinderObjs.Add(actableObj.CharacterRef.Controller, new List<ActableGridObject>());
										}
										hinderObjs[actableObj.CharacterRef.Controller].Add(actableObj);
									}
								}
							}
						}
					}
				}
				int nextRow = this.GridRef.PosRow, nextCol = this.GridRef.PosCol;
				switch (direction) {
					case BattleMapDirection.POSITIVE_ROW:
						++nextRow;
						break;
					case BattleMapDirection.POSITIVE_COL:
						++nextCol;
						break;
					case BattleMapDirection.NEGATIVE_ROW:
						--nextRow;
						break;
					case BattleMapDirection.NEGATIVE_COL:
						--nextCol;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(direction));
				}
				bool nextHighland = stairway ^ this.Highland;
				foreach (var keyValuePair in hinderObjs) {
					var user = keyValuePair.Key;
					var list = keyValuePair.Value;
					if (list.Count > 0) {
						list.Sort((x, y) => {
							var x_physique = x.CharacterRef.GetSkill(SkillType.Physique);
							var y_physique = y.CharacterRef.GetSkill(SkillType.Physique);
							return y_physique.Level - x_physique.Level;
						});
						var hinderObj = list[0];
						int determin = user.Client.RequireDetermin("要让 " + hinderObj.Name + " 阻扰 " + this.Name + " 的移动吗？", new string[] { "是", "否" });
						if (determin == 1) {
							_hinderMovingObjectsRecord.Add(hinderObj);
							SkillChecker.Instance.InitiativeUseSkillWithoutDMCheck(hinderObj.CharacterRef, new Character[] { this.CharacterRef }, CharacterAction.HINDER, SkillType.Physique,
								() => { if (!_hinderMovingSuccess) MoveTo(dstRow, dstCol, dstHighland); },
								() => { },
								(initiativeResult, passiveResult, delta) => {
									if (passiveResult != CheckResult.FAIL) {
										this.MovePoint = leftMovePoint;
										foreach (Player player in Game.Players) {
											player.Client.BattleScene.DisplayActableObjectMove(this, direction, stairway);
											player.Client.BattleScene.UpdateMovePoint(this);
										}
										Game.DM.Client.BattleScene.DisplayActableObjectMove(this, direction, stairway);
										Game.DM.Client.BattleScene.UpdateMovePoint(this);
										BattleScene.Instance.BattleMap.MoveStack(srcRow, srcCol, srcHighland, nextRow, nextCol, nextHighland);
										_hinderMovingSuccess = false;
									} else {
										this.MovePoint = 0;
										foreach (Player player in Game.Players) {
											player.Client.BattleScene.UpdateMovePoint(this);
										}
										Game.DM.Client.BattleScene.UpdateMovePoint(this);
										_hinderMovingSuccess = true;
									}
								});
							return;
						}
					}
				}
				this.MovePoint = leftMovePoint;
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.DisplayActableObjectMove(this, direction, stairway);
					player.Client.BattleScene.UpdateMovePoint(this);
				}
				Game.DM.Client.BattleScene.DisplayActableObjectMove(this, direction, stairway);
				Game.DM.Client.BattleScene.UpdateMovePoint(this);
				BattleScene.Instance.BattleMap.MoveStack(srcRow, srcCol, srcHighland, nextRow, nextCol, nextHighland);
			}
		}

		public bool IsActionPointEnough(SkillBattleMapProperty battleMapProperty) {
			if (this.ActionPoint < this.ActionPointMax && this.ActionPoint - battleMapProperty.actionPointCost < 0) return false;
			return true;
		}
		
		public Situation GetStuntSituationForUsingCondition(Stunt stunt, CharacterAction action) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			Situation ret = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = true,
				InitiativeSS = null, InitiativeBS = this,
				PassivesSS = null, PassivesBS = null,
				Action = action,
				InitiativeSkillType = null
			};
			return ret;
		}

		public Situation GetStuntSituationForTargetCondition(SceneObject target, Stunt stunt, CharacterAction action) {
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			Situation ret = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = true,
				InitiativeSS = null, InitiativeBS = this,
				PassivesSS = null, PassivesBS = new SceneObject[] { target },
				Action = action,
				InitiativeSkillType = null
			};
			return ret;
		}
		
		private bool CanPutActionAt(SkillBattleMapProperty battleMapProperty, GridPos pos) {
			if (!this.Highland && pos.highland) return false;
			if (battleMapProperty.islinearUse) {
				do {
					if (pos.row == this.GridRef.PosRow && pos.col == this.GridRef.PosCol && battleMapProperty.useRange.InRange(0)) break;
					if ((battleMapProperty.linearUseDirection & BattleMapDirection.POSITIVE_ROW) != 0 && pos.col == this.GridRef.PosCol && battleMapProperty.useRange.InRange(pos.row - this.GridRef.PosRow)) break;
					if ((battleMapProperty.linearUseDirection & BattleMapDirection.NEGATIVE_ROW) != 0 && pos.col == this.GridRef.PosCol && battleMapProperty.useRange.InRange(this.GridRef.PosRow - pos.row)) break;
					if ((battleMapProperty.linearUseDirection & BattleMapDirection.POSITIVE_COL) != 0 && pos.row == this.GridRef.PosRow && battleMapProperty.useRange.InRange(pos.col - this.GridRef.PosCol)) break;
					if ((battleMapProperty.linearUseDirection & BattleMapDirection.NEGATIVE_COL) != 0 && pos.row == this.GridRef.PosRow && battleMapProperty.useRange.InRange(this.GridRef.PosCol - pos.col)) break;
					return false;
				} while (false);
			} else if (battleMapProperty.useRange.OutOfRange(Math.Abs(pos.row - this.GridRef.PosRow) + Math.Abs(pos.col - this.GridRef.PosCol))) return false;
			return true;
		}

		private List<GridPos> GetAffectArea(SkillBattleMapProperty battleMapProperty, GridPos center) {
			var area = new List<GridPos>();
			if (battleMapProperty.islinearAffect) {
				for (int r = 0; r < BattleScene.Instance.BattleMap.Rows; ++r) {
					for (int c = 0; c < BattleScene.Instance.BattleMap.Cols; ++c) {
						if (((battleMapProperty.linearAffectDirection & BattleMapDirection.POSITIVE_ROW) != 0 && c == center.col && battleMapProperty.affectRange.InRange(r - center.row))
							|| ((battleMapProperty.linearAffectDirection & BattleMapDirection.NEGATIVE_ROW) != 0 && c == center.col && battleMapProperty.affectRange.InRange(center.row - r))
							|| ((battleMapProperty.linearAffectDirection & BattleMapDirection.POSITIVE_COL) != 0 && r == center.row && battleMapProperty.affectRange.InRange(c - center.col))
							|| ((battleMapProperty.linearAffectDirection & BattleMapDirection.NEGATIVE_COL) != 0 && r == center.row && battleMapProperty.affectRange.InRange(center.col - c))) {
							area.Add(new GridPos() { row = r, col = c, highland = false });
							if (this.Highland) area.Add(new GridPos() { row = r, col = c, highland = true });
						}
					}
				}
			} else {
				for (int r = 0; r < BattleScene.Instance.BattleMap.Rows; ++r) {
					for (int c = 0; c < BattleScene.Instance.BattleMap.Cols; ++c) {
						if (battleMapProperty.affectRange.InRange(Math.Abs(r - center.row) + Math.Abs(c - center.col))) {
							area.Add(new GridPos() { row = r, col = c, highland = false });
							if (this.Highland) area.Add(new GridPos() { row = r, col = c, highland = true });
						}
					}
				}
			}
			return area;
		}

		public Dictionary<GridPos, List<GridPos>> GetSkillAffectableAreas(SkillType skillType) {
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			var battleMapProperty = this.CharacterRef.GetSkill(skillType).BattleMapProperty;
			var putableArea = new List<GridPos>();
			for (int r = 0; r < BattleScene.Instance.BattleMap.Rows; ++r) {
				for (int c = 0; c < BattleScene.Instance.BattleMap.Cols; ++c) {
					var pos = new GridPos() { row = r, col = c, highland = false };
					if (CanPutActionAt(battleMapProperty, pos)) {
						putableArea.Add(pos);
					}
					pos.highland = true;
					if (CanPutActionAt(battleMapProperty, pos)) {
						putableArea.Add(pos);
					}
				}
			}
			var ret = new Dictionary<GridPos, List<GridPos>>();
			foreach (var putableGrid in putableArea) {
				ret.Add(putableGrid, GetAffectArea(battleMapProperty, putableGrid));
			}
			return ret;
		}

		public Dictionary<GridPos, List<GridPos>> GetStuntAffectableAreas(Stunt stunt) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			var putableArea = new List<GridPos>();
			for (int r = 0; r < BattleScene.Instance.BattleMap.Rows; ++r) {
				for (int c = 0; c < BattleScene.Instance.BattleMap.Cols; ++c) {
					var pos = new GridPos() { row = r, col = c, highland = false };
					if (CanPutActionAt(stunt.BattleMapProperty, pos)) {
						putableArea.Add(pos);
					}
					pos.highland = true;
					if (CanPutActionAt(stunt.BattleMapProperty, pos)) {
						putableArea.Add(pos);
					}
				}
			}
			var ret = new Dictionary<GridPos, List<GridPos>>();
			foreach (var putableGrid in putableArea) {
				ret.Add(putableGrid, GetAffectArea(stunt.BattleMapProperty, putableGrid));
			}
			return ret;
		}

		public void UseSkill(SkillType skillType, CharacterAction action, GridPos center, IEnumerable<SceneObject> targets) {
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			if (center.row >= BattleScene.Instance.BattleMap.Rows || center.row < 0)
				throw new ArgumentOutOfRangeException(nameof(center));
			if (center.col >= BattleScene.Instance.BattleMap.Cols || center.col < 0)
				throw new ArgumentOutOfRangeException(nameof(center));

			var skill = this.CharacterRef.GetSkill(skillType);
			var battleMapProperty = skill.BattleMapProperty;

			if (!IsActionPointEnough(battleMapProperty)) throw new InvalidOperationException("Action point is not enough.");
			if (!SkillChecker.Instance.CanInitiativeUseSkill(this.CharacterRef, skillType, action)) throw new InvalidOperationException("Cannot use this skill.");

			var areas = GetSkillAffectableAreas(skillType);
			if (!areas.ContainsKey(center)) throw new InvalidOperationException("Cannot use this skill at the specified position.");

			var area = areas[center];
			if (skill.TargetMaxCount == -1) {
				var areaTargets = new List<SceneObject>();
				foreach (var place in area) {
					if (place.highland) {
						areaTargets.AddRange(BattleScene.Instance.BattleMap[place.row, place.col].HighlandObjects);
					} else {
						areaTargets.AddRange(BattleScene.Instance.BattleMap[place.row, place.col].LowlandObjects);
					}
				}
				targets = areaTargets;
			} else {
				if (targets == null) throw new ArgumentNullException(nameof(targets));
				int count = 0;
				foreach (GridObject target in targets) {
					if (target == null) throw new ArgumentNullException(nameof(target));
					if (!area.Contains(new GridPos() { row = target.GridRef.PosRow, col = target.GridRef.PosCol, highland = target.Highland }))
						throw new InvalidOperationException("Target is not in range!");
					++count;
				}
				if (count > skill.TargetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
			}

			var wrapperList = new List<SceneObject>(targets);
			var characterList = new List<Character>();
			foreach (var sceneObj in wrapperList) {
				characterList.Add(sceneObj.CharacterRef);
			}
			SkillChecker.Instance.InitiativeUseSkill(dmCheckResult => {
				if (dmCheckResult) {
					this.ActionPoint -= battleMapProperty.actionPointCost;
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.UpdateActionPoint(this);
					}
					Game.DM.Client.BattleScene.UpdateActionPoint(this);
				}
			} ,_characterRef, characterList, action, skillType, () => { }, () => { }, (iResult, pResult, delta) => { });
		}

		public void UseStunt(Stunt stunt, GridPos center, IEnumerable<SceneObject> targets, CharacterAction action) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (stunt.Belong != this.CharacterRef) throw new ArgumentException("This stunt is not belong to the character.", nameof(stunt));

			if (center.row >= BattleScene.Instance.BattleMap.Rows || center.row < 0)
				throw new ArgumentOutOfRangeException(nameof(center));
			if (center.col >= BattleScene.Instance.BattleMap.Cols || center.col < 0)
				throw new ArgumentOutOfRangeException(nameof(center));

			var battleMapProperty = stunt.BattleMapProperty;

			if (!IsActionPointEnough(battleMapProperty)) throw new InvalidOperationException("Action point is not enough.");

			var usingSituation = GetStuntSituationForUsingCondition(stunt, action);
			if (!SkillChecker.Instance.CanInitiativeUseStunt(this.CharacterRef, stunt, usingSituation)) throw new InvalidOperationException("Cannot use this stunt.");

			var areas = GetStuntAffectableAreas(stunt);
			if (!areas.ContainsKey(center)) throw new InvalidOperationException("Cannot use this stunt at the specified position.");

			var area = areas[center];
			if (stunt.TargetMaxCount == -1) {
				var areaTargets = new List<SceneObject>();
				foreach (var place in area) {
					if (place.highland) {
						foreach (var highlandObj in BattleScene.Instance.BattleMap[place.row, place.col].HighlandObjects) {
							var targetSituation = GetStuntSituationForTargetCondition(highlandObj, stunt, action);
							if (SkillChecker.Instance.CanInitiativeUseStuntOnCharacter(highlandObj.CharacterRef, stunt, targetSituation)) {
								areaTargets.Add(highlandObj);
							}
						}
					} else {
						foreach (var lowlandObj in BattleScene.Instance.BattleMap[place.row, place.col].LowlandObjects) {
							var targetSituation = GetStuntSituationForTargetCondition(lowlandObj, stunt, action);
							if (SkillChecker.Instance.CanInitiativeUseStuntOnCharacter(lowlandObj.CharacterRef, stunt, targetSituation)) {
								areaTargets.Add(lowlandObj);
							}
						}
					}
				}
				targets = areaTargets;
			} else {
				if (targets == null) throw new ArgumentNullException(nameof(targets));
				int count = 0;
				foreach (GridObject target in targets) {
					if (target == null) throw new ArgumentNullException(nameof(target));
					if (!area.Contains(new GridPos() { row = target.GridRef.PosRow, col = target.GridRef.PosCol, highland = target.Highland }))
						throw new InvalidOperationException("Target is not in range!");
					var targetSituation = GetStuntSituationForTargetCondition(target, stunt, action);
					if (!SkillChecker.Instance.CanInitiativeUseStuntOnCharacter(target.CharacterRef, stunt, targetSituation)) throw new InvalidOperationException("Cannot use this stunt on the target.");
					++count;
				}
				if (count > stunt.TargetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
			}

			var targetList = new List<SceneObject>(targets);
			Situation situation = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = true,
				InitiativeSS = null, InitiativeBS = this,
				PassivesSS = null, PassivesBS = targetList.ToArray(),
				Action = action,
				InitiativeSkillType = null
			};
			SkillChecker.Instance.InitiativeUseStunt(_characterRef, stunt, situation, success => {
				if (success) {
					this.ActionPoint -= battleMapProperty.actionPointCost;
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.UpdateActionPoint(this);
					}
					Game.DM.Client.BattleScene.UpdateActionPoint(this);
					BattleScene.Instance.Update();
				}
			});
		}

		public override IJSContext GetContext() {
			return _apiObj;
		}
	}

	public sealed class Grid : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<Grid> {
			private readonly Grid _outer;

			public JSAPI(Grid outer) {
				_outer = outer;
			}

			public Grid Origin(JSContextHelper proof) {
				try {
					if (proof == JSContextHelper.Instance) {
						return _outer;
					}
					return null;
				} catch (Exception) {
					return null;
				}
			}
		}
		#endregion
		private readonly JSAPI _apiObj;

		private readonly List<GridObject> _highlandObjects;
		private readonly List<GridObject> _lowlandObjects;
		private LadderObject _positiveRowLadder = null;
		private LadderObject _positiveColLadder = null;
		private LadderObject _negativeRowLadder = null;
		private LadderObject _negativeColLadder = null;
		private bool _isMiddleLand = false;
		private readonly int _posRow;
		private readonly int _posCol;

		public IReadOnlyList<GridObject> HighlandObjects => _highlandObjects;
		public IReadOnlyList<GridObject> LowlandObjects => _lowlandObjects;
		public LadderObject PositiveRowLadder => _positiveRowLadder;
		public LadderObject PositiveColLadder => _positiveColLadder;
		public LadderObject NegativeRowLadder => _negativeRowLadder;
		public LadderObject NegativeColLadder => _negativeColLadder;
		public bool IsMiddleLand { get => _isMiddleLand; set => _isMiddleLand = value; }
		public int PosRow => _posRow;
		public int PosCol => _posCol;

		public Grid(int row, int col) {
			_posRow = row;
			_posCol = col;
			_highlandObjects = new List<GridObject>();
			_lowlandObjects = new List<GridObject>();
			_apiObj = new JSAPI(this);
		}

		public LadderObject GetLadder(BattleMapDirection direction) {
			switch (direction) {
				case BattleMapDirection.POSITIVE_ROW:
					return _positiveRowLadder;
				case BattleMapDirection.POSITIVE_COL:
					return _positiveColLadder;
				case BattleMapDirection.NEGATIVE_ROW:
					return _negativeRowLadder;
				case BattleMapDirection.NEGATIVE_COL:
					return _negativeColLadder;
				default:
					return null;
			}
		}

		public void SetLadderRef(BattleMapDirection direction, LadderObject ladderRef) {
			switch (direction) {
				case BattleMapDirection.POSITIVE_ROW:
					_positiveRowLadder = ladderRef;
					break;
				case BattleMapDirection.POSITIVE_COL:
					_positiveColLadder = ladderRef;
					break;
				case BattleMapDirection.NEGATIVE_ROW:
					_negativeRowLadder = ladderRef;
					break;
				case BattleMapDirection.NEGATIVE_COL:
					_negativeColLadder = ladderRef;
					break;
				default:
					break;
			}
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}

	public sealed class BattleMap {
		private Grid[,] _grids;
		private int _rows;
		private int _cols;

		public int Rows => _rows;
		public int Cols => _cols;
		public Grid this[int row, int col] => _grids[row, col];

		public BattleMap() {
			this.Reset(0, 0);
		}

		public void Reset(int rows, int cols) {
			_rows = rows;
			_cols = cols;
			_grids = new Grid[_rows, _cols];
			for (int i = 0; i < _rows; ++i) {
				for (int j = 0; j < _cols; ++j) {
					_grids[i, j] = new Grid(i, j);
				}
			}
		}

		public void MoveStack(int srcRow, int srcCol, bool srcHighland, int dstRow, int dstCol, bool dstHighland, int count = 1) {
			Grid srcGrid = _grids[srcRow, srcCol];
			Grid dstGrid = _grids[dstRow, dstCol];
			List<GridObject> srcLand, dstLand;
			if (srcHighland) srcLand = (List<GridObject>)srcGrid.HighlandObjects;
			else srcLand = (List<GridObject>)srcGrid.LowlandObjects;
			if (dstHighland) dstLand = (List<GridObject>)dstGrid.HighlandObjects;
			else dstLand = (List<GridObject>)dstGrid.LowlandObjects;
			int dstInsertIndex = dstLand.Count;
			for (int i = 0; i < count; ++i) {
				int srcLastIndex = srcLand.Count - 1;
				if (srcLastIndex < 0) break;
				GridObject gridObject = srcLand[srcLastIndex];
				gridObject.SetGridRef(dstGrid);
				gridObject.SetHighland(dstHighland);
				dstLand.Insert(dstInsertIndex, gridObject);
				srcLand.RemoveAt(srcLastIndex);
			}
		}
	}

	public sealed class ReachablePlace {
		public ReachablePlace prevPlace;
		public GridPos pos;
		public int leftMovePoint;
	}
}