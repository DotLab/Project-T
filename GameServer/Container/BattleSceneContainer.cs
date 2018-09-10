using GameServer.CharacterSystem;
using GameServer.ClientComponents;
using GameServer.Container.BattleComponent;
using GameServer.Core;
using GameServer.Core.ScriptSystem;
using GameServer.EventSystem;
using GameServer.EventSystem.Events;
using GameUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameServer.Container {
	public sealed class BattleSceneContainer : IJSContextProvider {
		#region Javascript API class
		private sealed class JSAPI : IJSAPI<BattleSceneContainer> {
			private readonly BattleSceneContainer _outer;

			public JSAPI(BattleSceneContainer outer) {
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
			
			public void currentPassiveUseSkill(IJSAPI<SkillType> skillType) {
				currentPassiveUseSkill(skillType, true, 0, null);
			}

			public void currentPassiveUseSkill(IJSAPI<SkillType> skillType, bool skipDMCheck, int extraPoint, int[] fixedDicePoints) {
				try {
					var origin_skillType = JSContextHelper.Instance.GetAPIOrigin(skillType);
					_outer.CurrentPassiveUseSkill(origin_skillType, skipDMCheck, true, extraPoint, fixedDicePoints);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public BattleSceneContainer Origin(JSContextHelper proof) {
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

		private static readonly BattleSceneContainer _instance = new BattleSceneContainer();
		public static BattleSceneContainer Instance => _instance;

		private readonly IdentifiedObjList<SceneObject> _objList;
		private readonly List<ActableGridObject> _actableObjList;
		private readonly BattleMap _battleMap;
		private ActableGridObject _currentActable = null;
		private int _roundCount = 0;
		private bool _isChecking = false;

		private SceneObject _initiative;
		private SkillType _initiativeSkillType;
		private bool _initiativeSkillBigone;
		private int[] _initiativeRollPoints;
		private int _initiativeFixedExtraPoint;
		private CharacterAction _checkingAction;
		private List<SceneObject> _passives;
		private SceneObject _currentPassive;

		public IReadonlyIdentifiedObjList<SceneObject> ObjList => _objList;
		public IReadOnlyList<ActableGridObject> ActableObjList => _actableObjList;
		public BattleMap BattleMap => _battleMap;
		public ActableGridObject CurrentActable => _currentActable;
		public int RoundCount => _roundCount;
		public bool IsChecking => _isChecking;

		public SceneObject Initiative => _initiative;
		public SceneObject CurrentPassive => _currentPassive;
		public CharacterAction CheckingAction => _checkingAction;

		public BattleSceneContainer() {
			_objList = new IdentifiedObjList<SceneObject>();
			_actableObjList = new List<ActableGridObject>();
			_battleMap = new BattleMap();
			_passives = new List<SceneObject>();
			_apiObj = new JSAPI(this);
		}

		public void ClientSynchronizeData(BattleScene battleScene) {
			battleScene.Reset(_battleMap.Rows, _battleMap.Cols);
			for (int row = 0; row < _battleMap.Rows; ++row) {
				for (int col = 0; col < _battleMap.Cols; ++col) {
					var grid = _battleMap[row, col];
					battleScene.UpdateGridData(grid);
					foreach (var lowlandObj in grid.Lowland) {
						battleScene.PushGridObject(lowlandObj);
					}
					foreach (var highlandObj in grid.Highland) {
						battleScene.PushGridObject(highlandObj);
					}
					if (grid.PositiveRowLadder != null) battleScene.AddLadderObject(grid.PositiveRowLadder);
					if (grid.PositiveColLadder != null) battleScene.AddLadderObject(grid.PositiveColLadder);
					if (grid.NegativeRowLadder != null) battleScene.AddLadderObject(grid.NegativeRowLadder);
					if (grid.NegativeColLadder != null) battleScene.AddLadderObject(grid.NegativeColLadder);
				}
			}
		}

		public void ClientSynchronizeState(BattleScene battleScene) {
			battleScene.SetActingOrder(_actableObjList);
			if (_currentActable != null) {
				battleScene.ChangeTurn(_currentActable);
				battleScene.UpdateActionPoint(_currentActable);
				battleScene.DisplayTakeExtraMovePoint(_currentActable, SkillType.Athletics);
				if (_isChecking) {

				}
			}
		}

		public void Reset(int rows, int cols) {
			_battleMap.Reset(rows, cols);
			_objList.Clear();
			_actableObjList.Clear();
			_currentActable = null;
			_roundCount = 0;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.Reset(rows, cols);
			}
			Game.DM.Client.BattleScene.Reset(rows, cols);
			this.Update();
		}

		public SceneObject FindObject(string id) {
			if (_objList.TryGetValue(id, out SceneObject gridObject)) return gridObject;
			return null;
		}

		public void PushGridObject(int row, int col, bool highland, GridObject gridObject) {
			if (_objList.Contains(gridObject)) throw new ArgumentException("This object is already added to scene.", nameof(gridObject));
			List<GridObject> land;
			if (highland) land = (List<GridObject>)_battleMap[row, col].Highland;
			else land = (List<GridObject>)_battleMap[row, col].Lowland;
			land.Add(gridObject);
			gridObject.SetGridRef(_battleMap[row, col]);
			gridObject.SetHighland(highland);
			_objList.Add(gridObject);
			if (gridObject is ActableGridObject) {
				_actableObjList.Add((ActableGridObject)gridObject);
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.SetActingOrder(_actableObjList);
				}
				Game.DM.Client.BattleScene.SetActingOrder(_actableObjList);
			}
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.PushGridObject(gridObject);
			}
			Game.DM.Client.BattleScene.PushGridObject(gridObject);
		}

		public GridObject PopGridObject(int row, int col, bool highland) {
			GridObject ret = null;
			List<GridObject> land;
			if (highland) land = (List<GridObject>)_battleMap[row, col].Highland;
			else land = (List<GridObject>)_battleMap[row, col].Lowland;
			if (land.Count > 0) {
				ret = land[land.Count - 1];
				land.RemoveAt(land.Count - 1);
				ret.SetGridRef(null);
				_objList.Remove(ret);
				if (ret is ActableGridObject) {
					_actableObjList.Remove((ActableGridObject)ret);
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.SetActingOrder(_actableObjList);
					}
					Game.DM.Client.BattleScene.SetActingOrder(_actableObjList);
				}
			}
			if (ret != null) {
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.RemoveGridObject(ret);
				}
				Game.DM.Client.BattleScene.RemoveGridObject(ret);
			}
			return ret;
		}

		public bool RemoveGridObject(GridObject gridObject) {
			if (!_objList.Contains(gridObject)) return false;
			if (gridObject.Highland) ((List<GridObject>)gridObject.GridRef.Highland).Remove(gridObject);
			else ((List<GridObject>)gridObject.GridRef.Lowland).Remove(gridObject);
			gridObject.SetGridRef(null);
			_objList.Remove(gridObject);
			if (gridObject is ActableGridObject) {
				_actableObjList.Remove((ActableGridObject)gridObject);
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.SetActingOrder(_actableObjList);
				}
				Game.DM.Client.BattleScene.SetActingOrder(_actableObjList);
			}
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.RemoveGridObject(gridObject);
			}
			Game.DM.Client.BattleScene.RemoveGridObject(gridObject);
			return true;
		}

		public void AddLadderObject(int row, int col, BattleMapDirection direction, LadderObject ladderObject) {
			if (_objList.Contains(ladderObject)) throw new ArgumentException("This object is already added to scene.", nameof(ladderObject));
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
			_objList.Add(ladderObject);
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
			_objList.Remove(ladderObject);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.RemoveLadderObject(ladderObject);
			}
			Game.DM.Client.BattleScene.RemoveLadderObject(ladderObject);
			return ladderObject;
		}

		public bool RemoveLadderObject(LadderObject ladderObject) {
			if (ladderObject == null) throw new ArgumentNullException(nameof(ladderObject));
			if (!_objList.Contains(ladderObject)) return false;
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
			ladderObject.SetFirstGridRef(null);
			ladderObject.SetSecondGridRef(null);
			_objList.Remove(ladderObject);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.RemoveLadderObject(ladderObject);
			}
			Game.DM.Client.BattleScene.RemoveLadderObject(ladderObject);
			return true;
		}

		public void NewRound() {
			foreach (ActableGridObject actableObject in _actableObjList) {
				var notice = actableObject.CharacterRef.GetSkill(SkillType.Notice);
				var athletics = actableObject.CharacterRef.GetSkill(SkillType.Athletics);
				int[] dicePoints = FateDice.Roll();
				if (actableObject.CharacterRef.ControlPlayer != null) {
					actableObject.CharacterRef.ControlPlayer.Client.BattleScene.DisplayDicePoints(actableObject.CharacterRef.ControlPlayer, dicePoints);
				}
				int sumPoint = 0;
				foreach (int point in dicePoints) sumPoint += point;
				actableObject.ActionPriority = notice.Level + sumPoint;
				int ap = 1 + (athletics.Level >= 1 ? (athletics.Level - 1) / 2 : 0);
				actableObject.ActionPoint = actableObject.ActionPoint >= 0 ? ap : ap + actableObject.ActionPoint;
				actableObject.ActionPointMax = ap;
				actableObject.MovePoint = 0;
			}
			_actableObjList.Sort((ActableGridObject a, ActableGridObject b) => { return b.ActionPriority - a.ActionPriority; });
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.SetActingOrder(_actableObjList);
			}
			Game.DM.Client.BattleScene.SetActingOrder(_actableObjList);
			if (_actableObjList.Count > 0) {
				_currentActable = _actableObjList[0];
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.ChangeTurn(_currentActable);
					player.Client.BattleScene.UpdateActionPoint(_currentActable);
				}
				Game.DM.Client.BattleScene.ChangeTurn(_currentActable);
				Game.DM.Client.BattleScene.UpdateActionPoint(_currentActable);
				_currentActable.AddMovePoint();
			} else _currentActable = null;
			++_roundCount;
			this.Update();
		}

		public void CurrentTurnOver() {
			if (_currentActable == null) throw new InvalidOperationException("Current acting character is null.");
			int next = _actableObjList.IndexOf(_currentActable) + 1;
			if (next >= _actableObjList.Count) // new round
			{
				this.NewRound();
			} else {
				_currentActable = _actableObjList[next];
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.ChangeTurn(_currentActable);
					player.Client.BattleScene.UpdateActionPoint(_currentActable);
				}
				Game.DM.Client.BattleScene.ChangeTurn(_currentActable);
				Game.DM.Client.BattleScene.UpdateActionPoint(_currentActable);
				_currentActable.AddMovePoint();
			}
			this.Update();
		}

		public void Update() {
			List<SceneObject> removal = new List<SceneObject>();
			for (int row = 0; row < _battleMap.Rows; ++row) {
				for (int col = 0; col < _battleMap.Cols; ++col) {
					var grid = _battleMap[row, col];
					foreach (var lowlandObj in grid.Lowland) {
						var actable = lowlandObj as ActableGridObject;
						if (actable != null && actable.Dead) removal.Add(actable);
					}
					foreach (var highlandObj in grid.Highland) {

					}
					/*
					if (grid.PositiveRowLadder != null) battleScene.AddLadderObject(grid.PositiveRowLadder);
					if (grid.PositiveColLadder != null) battleScene.AddLadderObject(grid.PositiveColLadder);
					if (grid.NegativeRowLadder != null) battleScene.AddLadderObject(grid.NegativeRowLadder);
					if (grid.NegativeColLadder != null) battleScene.AddLadderObject(grid.NegativeColLadder);*/
				}
			}
		}
		
		public void StartCheck(
			SceneObject initiative, IEnumerable<SceneObject> passives,
			CharacterAction action, SkillType initiativeSkillType,
			bool bigone = false, int fixedExtraPoint = 0, int[] fixedDicePoints = null
			) {
			if (_isChecking) throw new InvalidOperationException("It's in checking state.");
			_initiative = initiative;
			_initiativeSkillType = initiativeSkillType;
			_checkingAction = action;
			_passives.Clear();
			_passives.AddRange(passives);
			_passives.Reverse();
			_currentPassive = null;
			_initiativeRollPoints = fixedDicePoints ?? FateDice.Roll();
			_initiativeSkillBigone = bigone;
			_initiativeFixedExtraPoint = fixedExtraPoint;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayDicePoints(initiative.CharacterRef.Controller, _initiativeRollPoints);
				player.Client.BattleScene.StartCheck(initiative, initiativeSkillType, action, passives);
			}
			Game.DM.Client.BattleScene.DisplayDicePoints(initiative.CharacterRef.Controller, _initiativeRollPoints);
			Game.DM.Client.BattleScene.StartCheck(initiative, initiativeSkillType, action, passives);
			_isChecking = true;
			this.NextPassiveCheck();
		}

		public void NextPassiveCheck() {
			if (!_isChecking) throw new InvalidOperationException("It's not in checking state.");
			if (_passives.Count <= 0) {
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.EndCheck();
				}
				Game.DM.Client.BattleScene.EndCheck();
				_isChecking = false;
				return;
			}
			_currentPassive = _passives[_passives.Count - 1];
			_passives.RemoveAt(_passives.Count - 1);
			SkillChecker.Instance.StartCheck(_initiative.CharacterRef, _currentPassive.CharacterRef, _checkingAction, this.OnceCheckOver);
			SkillChecker.Instance.InitiativeSelectSkill(_initiativeSkillType);
			SkillChecker.Instance.InitiativeRollDice(_initiativeRollPoints);
			SkillChecker.Instance.InitiativeExtraPoint = _initiativeFixedExtraPoint;
			SkillChecker.Instance.InitiativeSkillSelectionOver();
			int sumPoint = SkillChecker.Instance.GetInitiativePoint();
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplaySkillReady(_initiative, _initiativeSkillType, _initiativeSkillBigone);
				player.Client.BattleScene.UpdateSumPoint(_initiative, sumPoint);
				player.Client.BattleScene.CheckNextone(_currentPassive);
			}
			Game.DM.Client.BattleScene.DisplaySkillReady(_initiative, _initiativeSkillType, _initiativeSkillBigone);
			Game.DM.Client.BattleScene.UpdateSumPoint(_initiative, sumPoint);
			Game.DM.Client.BattleScene.CheckNextone(_currentPassive);
			_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectSkillOrStunt(_checkingAction, _currentPassive, _initiative, _initiativeSkillType);
		}

		private void OnceCheckOver(CheckResult initiativeResult, CheckResult passiveResult, int delta) {
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayCheckResult(initiativeResult, passiveResult, delta);
			}
			Game.DM.Client.BattleScene.DisplayCheckResult(initiativeResult, passiveResult, delta);

			var initiativeSkill = _initiative.CharacterRef.GetSkill(_initiativeSkillType);
			var aspectCreatedEvent = new BattleSceneOnceCheckOverAspectCreatedEvent();
			var aspectCreatedEventInfo = new BattleSceneOnceCheckOverAspectCreatedEvent.EventInfo() {
				from = (IJSAPI<SceneObject>)_initiative.GetContext(),
				to = (IJSAPI<SceneObject>)_currentPassive.GetContext(),
				action = _checkingAction,
				from_checkResult = initiativeResult,
				to_checkResult = passiveResult,
				pointDelta = delta
			};
			var causeDamageEvent = new BattleSceneOnceCheckOverCauseDamageEvent();
			var causeDamageEventInfo = new BattleSceneOnceCheckOverCauseDamageEvent.EventInfo() {
				from = (IJSAPI<SceneObject>)_initiative.GetContext(),
				to = (IJSAPI<SceneObject>)_currentPassive.GetContext(),
				action = _checkingAction,
				from_checkResult = initiativeResult,
				to_checkResult = passiveResult,
				pointDelta = delta
			};
			if (_checkingAction == CharacterAction.CREATE_ASPECT) {
				switch (initiativeResult) {
					case CheckResult.TIE: {
							var boost = new Aspect();
							boost.PersistenceType = PersistenceType.Temporary;
							boost.Name = "受到" + _initiative.CharacterRef.Name + "的" + initiativeSkill.Name + "影响";
							boost.Benefiter = _initiative.CharacterRef;
							boost.BenefitTimes = 1;
							_currentPassive.CharacterRef.Aspects.Add(boost);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)boost.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					case CheckResult.SUCCEED: {
							var aspect = new Aspect();
							aspect.PersistenceType = PersistenceType.Common;
							aspect.Name = "受到" + _initiative.CharacterRef.Name + "的" + initiativeSkill.Name + "影响";
							aspect.Benefiter = _initiative.CharacterRef;
							aspect.BenefitTimes = 1;
							_currentPassive.CharacterRef.Aspects.Add(aspect);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)aspect.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					case CheckResult.SUCCEED_WITH_STYLE: {
							var aspect = new Aspect();
							aspect.PersistenceType = PersistenceType.Common;
							aspect.Name = "受到" + _initiative.CharacterRef.Name + "的" + initiativeSkill.Name + "影响";
							aspect.Benefiter = _initiative.CharacterRef;
							aspect.BenefitTimes = 2;
							_currentPassive.CharacterRef.Aspects.Add(aspect);
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)aspect.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					default:
						break;
				}
			} else if (_checkingAction == CharacterAction.ATTACK) {
				switch (initiativeResult) {
					case CheckResult.TIE: {
							var boost = new Aspect();
							boost.PersistenceType = PersistenceType.Temporary;
							boost.Name = "受到" + _initiative.CharacterRef.Name + "的" + initiativeSkill.Name + "攻击";
							boost.Benefiter = _initiative.CharacterRef;
							boost.BenefitTimes = 1;
							aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)boost.GetContext();
							aspectCreatedEvent.Info = aspectCreatedEventInfo;
							GameEventBus.Instance.Publish(aspectCreatedEvent);
						}
						break;
					case CheckResult.SUCCEED: {
							_currentPassive.CharacterRef.Damage(delta, initiativeSkill.SituationLimit.damageMental, _initiative.CharacterRef, "使用" + initiativeSkill.Name + "发动攻击");
							causeDamageEventInfo.damage = delta;
							causeDamageEventInfo.mental = initiativeSkill.SituationLimit.damageMental;
							causeDamageEvent.Info = causeDamageEventInfo;
							GameEventBus.Instance.Publish(causeDamageEvent);
						}
						break;
					case CheckResult.SUCCEED_WITH_STYLE: {
							_initiative.CharacterRef.Controller.Client.RequestDetermin("降低一点伤害来换取一个增益吗？（1是，0否）", determin => {
								int damage = delta;
								if (determin == 1) {
									var boost = new Aspect();
									boost.PersistenceType = PersistenceType.Temporary;
									boost.Name = "受到" + _initiative.CharacterRef.Name + "的" + initiativeSkill.Name + "攻击";
									boost.Benefiter = _initiative.CharacterRef;
									boost.BenefitTimes = 1;
									_currentPassive.CharacterRef.Aspects.Add(boost);
									damage -= 1;
									aspectCreatedEventInfo.createdAspect = (IJSAPI<Aspect>)boost.GetContext();
									aspectCreatedEvent.Info = aspectCreatedEventInfo;
									GameEventBus.Instance.Publish(aspectCreatedEvent);
								}
								_currentPassive.CharacterRef.Damage(damage, initiativeSkill.SituationLimit.damageMental, _initiative.CharacterRef, "使用" + initiativeSkill.Name + "发动攻击");
								causeDamageEventInfo.damage = damage;
								causeDamageEventInfo.mental = initiativeSkill.SituationLimit.damageMental;
								causeDamageEvent.Info = causeDamageEventInfo;
								GameEventBus.Instance.Publish(causeDamageEvent);
							});
						}
						break;
					default:
						break;
				}
			} else if (_checkingAction == CharacterAction.HINDER) {
				switch (initiativeResult) {
					case CheckResult.FAIL:
						break;
					case CheckResult.TIE:
						break;
					case CheckResult.SUCCEED:
						break;
					case CheckResult.SUCCEED_WITH_STYLE:
						break;
					default:
						break;
				}
			}

			var checkOverEvent = new BattleSceneOnceCheckOverEvent();
			checkOverEvent.Info = new BattleSceneOnceCheckOverEvent.EventInfo() {
				initiative = (IJSAPI<SceneObject>)_initiative.GetContext(),
				initiativeSkillType = (IJSAPI<SkillType>)SkillChecker.Instance.InitiativeSkillType.GetContext(),
				initiativeRollPoints = SkillChecker.Instance.InitiativeRollPoints,
				initiativeExtraPoint = SkillChecker.Instance.InitiativeExtraPoint,
				passive = (IJSAPI<SceneObject>)_currentPassive.GetContext(),
				passiveSkillType = (IJSAPI<SkillType>)SkillChecker.Instance.PassiveSkillType.GetContext(),
				passiveRollPoints = SkillChecker.Instance.PassiveRollPoints,
				passiveExtraPoint = SkillChecker.Instance.PassiveExtraPoint,
				action = SkillChecker.Instance.CheckingAction,
				initiativeCheckResult = initiativeResult,
				passiveCheckResult = passiveResult,
				pointDelta = delta
			};
			GameEventBus.Instance.Publish(checkOverEvent);

			this.Update();
			this.NextPassiveCheck();
		}
		
		public bool CanCurrentPassiveUseSkill(SkillType skillType) {
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			return SkillChecker.CanPassiveUseSkillInAction(_currentPassive.CharacterRef, skillType, _checkingAction);
		}

		public bool CanCurrentPassiveUseStunt(Stunt stunt) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if ((stunt.SituationLimit.resistableSituation & _checkingAction) != 0) {
				bool canUse = true;
				if (stunt.UsingCondition != null) {
					stunt.UsingCondition.Situation = new Situation() {
						IsTriggerInvoking = false, EventID = "",
						IsInStoryScene = false, IsInitiative = false,
						InitiativeSS = null, InitiativeBS = _initiative,
						PassiveSS = null, PassiveBS = _currentPassive,
						Action = _checkingAction, IsOnInteract = false,
						InitiativeSkillType = _initiativeSkillType, TargetsBS = null
					};
					canUse &= stunt.UsingCondition.Judge();
				}
				if (stunt.TargetCondition != null) {
					stunt.TargetCondition.Situation = new Situation() {
						IsTriggerInvoking = false, EventID = "",
						IsInStoryScene = false, IsInitiative = false,
						InitiativeSS = null, InitiativeBS = _initiative,
						PassiveSS = null, PassiveBS = _currentPassive,
						Action = _checkingAction, IsOnInteract = false,
						InitiativeSkillType = _initiativeSkillType, TargetsBS = null
					};
					canUse &= stunt.TargetCondition.Judge();
				}
				return canUse;
			} else return false;
		}

		private void PassiveSkillTakesEffect(SkillType skillType, bool bigone, int extraPoint, int[] fixedDicePoints) {
			SkillChecker.Instance.PassiveSelectSkill(skillType);
			int[] dicePoints = SkillChecker.Instance.PassiveRollDice(fixedDicePoints);
			SkillChecker.Instance.PassiveExtraPoint = extraPoint;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayDicePoints(_currentPassive.CharacterRef.Controller, dicePoints);
				player.Client.BattleScene.DisplaySkillReady(_currentPassive, skillType, bigone);
				player.Client.BattleScene.UpdateSumPoint(_currentPassive, SkillChecker.Instance.GetPassivePoint());
			}
			Game.DM.Client.BattleScene.DisplayDicePoints(_currentPassive.CharacterRef.Controller, dicePoints);
			Game.DM.Client.BattleScene.DisplaySkillReady(_currentPassive, skillType, bigone);
			Game.DM.Client.BattleScene.UpdateSumPoint(_currentPassive, SkillChecker.Instance.GetPassivePoint());
			_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectSkillOrStuntComplete();
			SkillChecker.Instance.PassiveSkillSelectionOver();
			_initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspect();
		}

		public void CurrentPassiveUseSkill(SkillType skillType, bool skipDMCheck = false, bool bigone = false, int extraPoint = 0, int[] fixedDicePoints = null) {
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			if (!CanCurrentPassiveUseSkill(skillType)) throw new InvalidOperationException("Cannot use this skill.");
			if (skipDMCheck) {
				PassiveSkillTakesEffect(skillType, bigone, extraPoint, fixedDicePoints);
			} else {
				if (SkillChecker.CanResistSkillWithoutDMCheck(SkillChecker.Instance.InitiativeSkillType, skillType, SkillChecker.Instance.CheckingAction)) {
					PassiveSkillTakesEffect(skillType, bigone, extraPoint, fixedDicePoints);
				} else {
					Game.DM.DMClient.RequestDMCheck(_currentPassive.CharacterRef.Controller,
						SkillChecker.Instance.Passive.Name + "对" + SkillChecker.Instance.Initiative.Name + "使用" + SkillChecker.Instance.Passive.GetSkill(skillType).Name + ",可以吗？",
						result => {
							if (result) {
								PassiveSkillTakesEffect(skillType, bigone, extraPoint, fixedDicePoints);
							} else {
								_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectSkillOrStuntFailure("DM拒绝了你选择的技能");
							}
						});
				}
			}
		}
		
		public void CurrentPassiveUseStunt(Stunt stunt) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (stunt.Belong != SkillChecker.Instance.Passive) throw new ArgumentException("This stunt is not belong to passive character.", nameof(stunt));
			if (!CanCurrentPassiveUseStunt(stunt)) throw new InvalidOperationException("Cannot use this stunt.");
			stunt.Effect.Situation = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = false,
				InitiativeSS = null, InitiativeBS = _initiative,
				PassiveSS = null, PassiveBS = _currentPassive,
				Action = _checkingAction, IsOnInteract = false,
				InitiativeSkillType = _initiativeSkillType, TargetsBS = null
			};
			stunt.Effect.TakeEffect((success, message) => {
				if (success) {
					_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectSkillOrStuntComplete();
				} else {
					_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectSkillOrStuntFailure(message);
				}
			});
		}

		public void InitiativeSelectAspect(Aspect aspect, bool reroll) {
			if (aspect == null) throw new ArgumentNullException(nameof(aspect));
			if (!SkillChecker.Instance.CheckInitiativeAspectUsable(aspect, out string msg)) {
				_initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspectFailure(msg);
				return;
			}
			var ownerGridObject = FindObject(aspect.Belong.ID);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayUsingAspect(_initiative, ownerGridObject, aspect);
			}
			Game.DM.Client.BattleScene.DisplayUsingAspect(_initiative, ownerGridObject, aspect);
			Game.DM.DMClient.RequestDMCheck(_initiative.CharacterRef.Controller,
				SkillChecker.Instance.Initiative.Name + "想使用" + aspect.Belong.Name + "的特征“" + aspect.Name + "”可以吗？",
				result => {
					if (result) {
						int[] rerollPoints = SkillChecker.Instance.InitiativeUseAspect(aspect, reroll);
						foreach (Player player in Game.Players) {
							if (reroll) player.Client.BattleScene.DisplayDicePoints(_initiative.CharacterRef.Controller, rerollPoints);
							player.Client.BattleScene.UpdateSumPoint(_initiative, SkillChecker.Instance.GetInitiativePoint());
						}
						if (reroll) Game.DM.Client.BattleScene.DisplayDicePoints(_initiative.CharacterRef.Controller, rerollPoints);
						Game.DM.Client.BattleScene.UpdateSumPoint(_initiative, SkillChecker.Instance.GetInitiativePoint());
						_initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspectComplete();
					} else {
						_initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspectFailure("DM拒绝了你选择的特征");
					}
				});
		}

		public void InitiativeAspectSelectionOver() {
			if (SkillChecker.Instance.State != SkillChecker.CheckerState.INITIATIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			_initiative.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectAspectOver();
			SkillChecker.Instance.InitiativeAspectSelectionOver();
			_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectAspect();
		}

		public void CurrentPassiveSelectAspect(Aspect aspect, bool reroll) {
			if (aspect == null) throw new ArgumentNullException(nameof(aspect));
			if (!SkillChecker.Instance.CheckPassiveAspectUsable(aspect, out string msg)) {
				_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectAspectFailure(msg);
				return;
			}
			var ownerGridObject = FindObject(aspect.Belong.ID);
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayUsingAspect(_currentPassive, ownerGridObject, aspect);
			}
			Game.DM.Client.BattleScene.DisplayUsingAspect(_currentPassive, ownerGridObject, aspect);
			Game.DM.DMClient.RequestDMCheck(_currentPassive.CharacterRef.Controller,
				SkillChecker.Instance.Passive.Name + "想使用" + aspect.Belong.Name + "的特征“" + aspect.Name + "”可以吗？",
				result => {
					if (result) {
						int[] rerollPoints = SkillChecker.Instance.PassiveUseAspect(aspect, reroll);
						foreach (Player player in Game.Players) {
							if (reroll) player.Client.BattleScene.DisplayDicePoints(_currentPassive.CharacterRef.Controller, rerollPoints);
							player.Client.BattleScene.UpdateSumPoint(_currentPassive, SkillChecker.Instance.GetPassivePoint());
						}
						if (reroll) Game.DM.Client.BattleScene.DisplayDicePoints(_currentPassive.CharacterRef.Controller, rerollPoints);
						Game.DM.Client.BattleScene.UpdateSumPoint(_currentPassive, SkillChecker.Instance.GetPassivePoint());
						_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectAspectComplete();
					} else {
						_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectAspectFailure("DM拒绝了你选择的特征");
					}
				});
		}

		public void CurrentPassiveAspectSelectionOver() {
			if (SkillChecker.Instance.State != SkillChecker.CheckerState.PASSIVE_ASPECT) throw new InvalidOperationException("State incorrect.");
			_currentPassive.CharacterRef.Controller.Client.BattleScene.NotifyPassiveSelectAspectOver();
			SkillChecker.Instance.PassiveAspectSelectionOver();
			SkillChecker.Instance.EndCheck();
		}

		public IJSContext GetContext() {
			return _apiObj;
		}

		public void SetContext(IJSContext context) { }
	}
}

namespace GameServer.Container.BattleComponent {
	public abstract class SceneObject : IIdentifiable {
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
			
			public bool isTerrian() {
				try {
					return _outer.Terrain;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
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

			public void markDead() {
				try {
					_outer.MarkDead();
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

		public SceneObject(Character characterRef) {
			_characterRef = characterRef ?? throw new ArgumentNullException(nameof(characterRef));
			_apiObj = new JSAPI(this);
		}

		public string ID => _characterRef.ID;
		public string Name { get => _characterRef.Name; set => _characterRef.Name = value; }
		public string Description { get => _characterRef.Description; set => _characterRef.Description = value; }
		public Character CharacterRef => _characterRef;
		public Grid GridRef => _gridRef;
		public abstract bool Terrain { get; }
		public int Stagnate { get => _stagnate; set => _stagnate = value; }

		public void MarkDead() {
			_characterRef.MarkDead();
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
		protected readonly bool _terrain;
		protected BattleMapDirection _direction;

		public bool Obstacle { get => _obstacle; set => _obstacle = value; }
		public bool Highland => _highland;
		public override bool Terrain => _terrain;
		public BattleMapDirection Direction { get => _direction; set => _direction = value; }

		public GridObject(Character characterRef, bool isTerrian) :
			base(characterRef) {
			_terrain = isTerrian;
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
		public override bool Terrain => true;
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

			public bool isDead() {
				try {
					return _outer.Dead;
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
					return false;
				}
			}

			public void useSkill(IJSAPI<SkillType> skillType, CharacterAction action, IJSAPI<SceneObject>[] targets) {
				useSkill(skillType, action, targets, true, true, 0, null);
			}

			public void useSkill(
				IJSAPI<SkillType> skillType, CharacterAction action, IJSAPI<SceneObject>[] targets,
				bool skipDMCheck, bool bigone, int extraPoint, int[] fixedDicePoints
				) {
				try {
					var origin_skillType = JSContextHelper.Instance.GetAPIOrigin(skillType);
					var origin_targets = new List<SceneObject>();
					foreach (var target in targets) {
						origin_targets.Add(JSContextHelper.Instance.GetAPIOrigin(target));
					}
					_outer.SkillTakesEffect(origin_skillType, action, origin_targets, true, skipDMCheck, bigone, extraPoint, fixedDicePoints);
				} catch (Exception e) {
					JSEngineManager.Engine.Log(e.Message);
				}
			}

			public void useSkillNormally(
				IJSAPI<SkillType> skillType, CharacterAction action, IJSAPI<SceneObject>[] targets,
				bool skipDMCheck, bool bigone, int extraPoint, int[] fixedDicePoints
				) {
				try {
					var origin_skillType = JSContextHelper.Instance.GetAPIOrigin(skillType);
					var origin_targets = new List<SceneObject>();
					foreach (var target in targets) {
						origin_targets.Add(JSContextHelper.Instance.GetAPIOrigin(target));
					}
					_outer.SkillTakesEffect(origin_skillType, action, origin_targets, false, skipDMCheck, bigone, extraPoint, fixedDicePoints);
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

			/*
			public void addMovePoint() {

			}

			public bool canTakeExtraMove() {

			}

			public void takeExtraMove() {
				
			}

			public List<ReachablePlace> getReachablePlaceList() {
				
			}

			public void move(BattleMapDirection direction, bool stairway) {
				
			}

			public void moveTo(int dstRow, int dstCol, bool dstHighland) {
				
			}

			public bool isActionPointEnough(BattleMapSkillProperty battleMapProperty) {
				
			}

			public bool canUseSkillInAction(SkillType skillType, CharacterAction action) {
				
			}

			public bool canUseStuntInAction(Stunt stunt, CharacterAction action) {

			}
			
			public bool canUseStuntOnTarget(SceneObject target, Stunt stunt, CharacterAction action) {
				
			}
			
			public void useStunt(Stunt stunt, CharacterAction action, GridPos center, IEnumerable<SceneObject> targets) {
				
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
			if (dstRow < 0 || dstRow >= BattleSceneContainer.Instance.BattleMap.Rows || dstCol < 0 || dstCol >= BattleSceneContainer.Instance.BattleMap.Cols) return false;
			Grid dstGrid = BattleSceneContainer.Instance.BattleMap[dstRow, dstCol];
			bool dstHighland = stairway ^ srcHighland;
			IReadOnlyList<GridObject> land = dstHighland ? dstGrid.Highland : dstGrid.Lowland;
			if (land.Count <= 0 || land[land.Count - 1].Obstacle) return false;
			if (stairway) {
				LadderObject ladderObject = BattleSceneContainer.Instance.BattleMap[srcRow, srcCol].GetLadder(direction);
				leftMovePoint -= ladderObject.Stagnate;
			}
			GridObject terrian = null;
			for (int i = land.Count - 1; i >= 0; --i) {
				if (land[i].Terrain) {
					terrian = land[i];
					break;
				}
			}
			if (terrian == null) return false;
			leftMovePoint -= terrian.Stagnate;
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

		public int ActionPriority { get => _actionPriority; set => _actionPriority = value; }
		public int ActionPoint { get => _actionPoint; set => _actionPoint = value; }
		public int ActionPointMax { get => _actionPointMax; set => _actionPointMax = value; }
		public bool Movable { get => _movable; set => _movable = value; }
		public int MovePoint { get => _movePoint; set => _movePoint = value; }
		public bool Dead => _characterRef.Dead;

		public ActableGridObject(Character characterRef, bool movable = true) :
			base(characterRef, false) {
			_apiObj = new JSAPI(this);
			_movable = movable;
			_obstacle = true;
		}

		/*
		public void AddMovePoint(int[] fixedDicePoints = null) {
			int[] dicePoints = fixedDicePoints ?? FateDice.Roll();
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayDicePoints(this.CharacterRef.Controller, dicePoints);
			}
			Game.DM.Client.BattleScene.DisplayDicePoints(this.CharacterRef.Controller, dicePoints);
			int sumPoint = 0;
			foreach (int point in dicePoints) sumPoint += point;
			var athletics = this.CharacterRef.GetSkill(SkillType.Athletics);
			sumPoint += athletics.Level;
			this.MovePoint += sumPoint > 0 ? sumPoint : 1;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.UpdateMovePoint(this);
			}
			Game.DM.Client.BattleScene.UpdateMovePoint(this);
		}
		*/
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
			if (this.ActionPoint - 1 < 0) return false;
			return true;
		}

		public void TakeExtraMove() {
			int leftActionPoint = this.ActionPoint;
			if (--leftActionPoint < 0) throw new InvalidOperationException("Action point is not enough.");
			this.ActionPoint = leftActionPoint;
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

		public void Move(BattleMapDirection direction, bool stairway) {
			int srcRow = this.GridRef.PosRow;
			int srcCol = this.GridRef.PosCol;
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
					throw new ArgumentOutOfRangeException(nameof(direction));
			}
			if (dstRow < 0 || dstRow >= BattleSceneContainer.Instance.BattleMap.Rows || dstCol < 0 || dstCol >= BattleSceneContainer.Instance.BattleMap.Cols)
				throw new InvalidOperationException("Move out of map.");
			Grid dstGrid = BattleSceneContainer.Instance.BattleMap[dstRow, dstCol];
			bool dstHighland = stairway ^ this.Highland;
			IReadOnlyList<GridObject> land = dstHighland ? dstGrid.Highland : dstGrid.Lowland;
			if (land.Count <= 0 || land[land.Count - 1].Obstacle) throw new InvalidOperationException("Cannot reach the place.");
			int leftMovePoint = this.MovePoint;
			if (stairway) {
				LadderObject ladderObject = this.GridRef.GetLadder(direction);
				leftMovePoint -= ladderObject.Stagnate;
			}
			GridObject terrian = null;
			for (int i = land.Count - 1; i >= 0; --i) {
				if (land[i].Terrain) {
					terrian = land[i];
					break;
				}
			}
			if (terrian == null) throw new InvalidOperationException("Cannot reach the place.");
			leftMovePoint -= terrian.Stagnate;
			if (leftMovePoint < 0) throw new InvalidOperationException("Move points are not enough.");
			this.MovePoint = leftMovePoint;
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.DisplayActableObjectMove(this, direction, stairway);
				player.Client.BattleScene.UpdateMovePoint(this);
			}
			Game.DM.Client.BattleScene.DisplayActableObjectMove(this, direction, stairway);
			Game.DM.Client.BattleScene.UpdateMovePoint(this);
			BattleSceneContainer.Instance.BattleMap.MoveStack(srcRow, srcCol, this.Highland, dstRow, dstCol, dstHighland);
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
				Move(moveDirections[i], moveStairways[i]);
			}
		}

		public bool IsActionPointEnough(SkillBattleMapProperty battleMapProperty) {
			if (this.ActionPoint < this.ActionPointMax && this.ActionPoint - battleMapProperty.actionPointCost < 0) return false;
			return true;
		}

		public bool CanUseSkillInAction(SkillType skillType, CharacterAction action) {
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			return SkillChecker.CanInitiativeUseSkillInAction(this.CharacterRef, skillType, action);
		}

		public bool CanUseStuntInAction(Stunt stunt, CharacterAction action) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if ((stunt.SituationLimit.usableSituation & action) != 0) {
				if (stunt.UsingCondition == null) return true;
				stunt.UsingCondition.Situation = new Situation() {
					IsTriggerInvoking = false, EventID = "",
					IsInStoryScene = false, IsInitiative = true,
					InitiativeSS = null, InitiativeBS = this,
					PassiveSS = null, PassiveBS = null,
					Action = action, IsOnInteract = false,
					InitiativeSkillType = null, TargetsBS = null
				};
				return stunt.UsingCondition.Judge();
			} else return false;
		}

		public bool CanUseStuntOnInteract(Stunt stunt) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (stunt.SituationLimit.canUseOnInteract) {
				if (stunt.UsingCondition == null) return true;
				stunt.UsingCondition.Situation = new Situation() {
					IsTriggerInvoking = false, EventID = "",
					IsInStoryScene = false, IsInitiative = true,
					InitiativeSS = null, InitiativeBS = this,
					PassiveSS = null, PassiveBS = null,
					Action = 0, IsOnInteract = true,
					InitiativeSkillType = null, TargetsBS = null
				};
				return stunt.UsingCondition.Judge();
			} else return false;
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
				for (int r = 0; r < BattleSceneContainer.Instance.BattleMap.Rows; ++r) {
					for (int c = 0; c < BattleSceneContainer.Instance.BattleMap.Cols; ++c) {
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
				for (int r = 0; r < BattleSceneContainer.Instance.BattleMap.Rows; ++r) {
					for (int c = 0; c < BattleSceneContainer.Instance.BattleMap.Cols; ++c) {
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
			for (int r = 0; r < BattleSceneContainer.Instance.BattleMap.Rows; ++r) {
				for (int c = 0; c < BattleSceneContainer.Instance.BattleMap.Cols; ++c) {
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
			for (int r = 0; r < BattleSceneContainer.Instance.BattleMap.Rows; ++r) {
				for (int c = 0; c < BattleSceneContainer.Instance.BattleMap.Cols; ++c) {
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

		public bool CanUseStuntOnTarget(SceneObject target, Stunt stunt, bool isInteract, CharacterAction action) {
			if (target == null) throw new ArgumentNullException(nameof(target));
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (stunt.TargetCondition == null) return true;
			stunt.TargetCondition.Situation = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = true,
				InitiativeSS = null, InitiativeBS = this,
				PassiveSS = null, PassiveBS = target,
				Action = action, IsOnInteract = isInteract,
				InitiativeSkillType = null, TargetsBS = null
			};
			return stunt.TargetCondition.Judge();
		}

		private void SkillCheckFilter(bool interact, ref SkillType skillType, ref CharacterAction action, ref GridPos center, ref IEnumerable<SceneObject> targets) {
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			if (center.row >= BattleSceneContainer.Instance.BattleMap.Rows || center.row < 0)
				throw new ArgumentOutOfRangeException(nameof(center));
			if (center.col >= BattleSceneContainer.Instance.BattleMap.Cols || center.col < 0)
				throw new ArgumentOutOfRangeException(nameof(center));

			var battleMapProperty = this.CharacterRef.GetSkill(skillType).BattleMapProperty;

			if (!interact && !CanUseSkillInAction(skillType, action)) throw new InvalidOperationException("Cannot use this skill.");
			if (!IsActionPointEnough(battleMapProperty)) throw new InvalidOperationException("Action point is not enough.");

			var areas = GetSkillAffectableAreas(skillType);
			if (!areas.ContainsKey(center)) throw new InvalidOperationException("Cannot use this skill at the specified position.");

			var area = areas[center];
			if (battleMapProperty.targetMaxCount == -1) {
				var areaTargets = new List<SceneObject>();
				foreach (var place in area) {
					if (place.highland) {
						areaTargets.AddRange(BattleSceneContainer.Instance.BattleMap[place.row, place.col].Highland);
					} else {
						areaTargets.AddRange(BattleSceneContainer.Instance.BattleMap[place.row, place.col].Lowland);
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
				if (count > battleMapProperty.targetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
			}
		}

		private void StuntCheckFilter(bool interact, ref Stunt stunt, ref CharacterAction action, ref GridPos center, ref IEnumerable<SceneObject> targets) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			if (stunt.Belong != this.CharacterRef) throw new ArgumentException("This stunt is not belong to the character.", nameof(stunt));

			if (center.row >= BattleSceneContainer.Instance.BattleMap.Rows || center.row < 0)
				throw new ArgumentOutOfRangeException(nameof(center));
			if (center.col >= BattleSceneContainer.Instance.BattleMap.Cols || center.col < 0)
				throw new ArgumentOutOfRangeException(nameof(center));
			
			var battleMapProperty = stunt.BattleMapProperty;

			if (interact) {
				if (!CanUseStuntOnInteract(stunt)) throw new InvalidOperationException("Cannot use this stunt.");
			} else {
				if (!CanUseStuntInAction(stunt, action)) throw new InvalidOperationException("Cannot use this stunt.");
			}
			if (!IsActionPointEnough(battleMapProperty)) throw new InvalidOperationException("Action point is not enough.");

			var areas = GetStuntAffectableAreas(stunt);
			if (!areas.ContainsKey(center)) throw new InvalidOperationException("Cannot use this skill at the specified position.");

			var area = areas[center];
			if (battleMapProperty.targetMaxCount == -1) {
				var areaTargets = new List<SceneObject>();
				foreach (var place in area) {
					if (place.highland) {
						areaTargets.AddRange(BattleSceneContainer.Instance.BattleMap[place.row, place.col].Highland);
					} else {
						areaTargets.AddRange(BattleSceneContainer.Instance.BattleMap[place.row, place.col].Lowland);
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
				if (count > battleMapProperty.targetMaxCount) throw new InvalidOperationException("Targets count is more than limit!");
			}
		}

		private void SkillTakesEffect(
			SkillType skillType, CharacterAction action, IEnumerable<SceneObject> passives,
			bool stuntDo = false, bool skipDMCheck = false,
			bool bigone = false, int fixedExtraPoint = 0, int[] fixedDicePoints = null
			) {
			if (passives == null) throw new ArgumentNullException(nameof(passives));
			if (skillType == null) throw new ArgumentNullException(nameof(skillType));
			bool hasElement = false;
			foreach (var obj in passives) {
				hasElement = true;
				break;
			}
			if (!hasElement) throw new ArgumentException("There is no passive character for checking.", nameof(passives));
			var battleMapProperty = this.CharacterRef.GetSkill(skillType).BattleMapProperty;
			if (!skipDMCheck) {
				if (action == CharacterAction.CREATE_ASPECT) {
					Game.DM.DMClient.RequestDMCheck(this.CharacterRef.Controller,
						this.CharacterRef.Name + "想使用" + this.CharacterRef.GetSkill(skillType).Name + ",可以吗？",
						result => {
							if (result) {
								if (!stuntDo) {
									this.ActionPoint -= battleMapProperty.actionPointCost;
									this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntComplete();
									foreach (Player player in Game.Players) {
										player.Client.BattleScene.UpdateActionPoint(this);
									}
									Game.DM.Client.BattleScene.UpdateActionPoint(this);
								}
								BattleSceneContainer.Instance.StartCheck(this, passives, action, skillType, bigone, fixedExtraPoint, fixedDicePoints);
							} else {
								this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntFailure("DM拒绝了你选择的技能");
							}
						});
					return;
				}
			}
			if (!stuntDo) {
				this.ActionPoint -= battleMapProperty.actionPointCost;
				this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntComplete();
				foreach (Player player in Game.Players) {
					player.Client.BattleScene.UpdateActionPoint(this);
				}
				Game.DM.Client.BattleScene.UpdateActionPoint(this);
			}
			BattleSceneContainer.Instance.StartCheck(this, passives, action, skillType, bigone, fixedExtraPoint, fixedDicePoints);
		}

		public void UseSkill(SkillType skillType, CharacterAction action, GridPos center, IEnumerable<SceneObject> targets) {
			SkillCheckFilter(false, ref skillType, ref action, ref center, ref targets);
			SkillTakesEffect(skillType, action, targets);
		}
		
		public void UseSkillOnInteract(SkillType skillType, GridPos center, IEnumerable<SceneObject> targets) {
			CharacterAction empty = 0;
			SkillCheckFilter(true, ref skillType, ref empty, ref center, ref targets);
			var battleMapProperty = this.CharacterRef.GetSkill(skillType).BattleMapProperty;
			this.ActionPoint -= battleMapProperty.actionPointCost;
			this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntComplete();
			foreach (Player player in Game.Players) {
				player.Client.BattleScene.UpdateActionPoint(this);
			}
			Game.DM.Client.BattleScene.UpdateActionPoint(this);
			var gameEvent = new BattleSceneUseSkillOnInteractEvent();
			var targetList = new List<SceneObject>(targets);
			var targetApiArray = new IJSAPI<SceneObject>[targetList.Count];
			for (int i = 0; i < targetList.Count; ++i) {
				targetApiArray[i] = (IJSAPI<SceneObject>)targetList[i].GetContext();
			}
			gameEvent.Info = new BattleSceneUseSkillOnInteractEvent.EventInfo() {
				user = (IJSAPI<SceneObject>)this.GetContext(),
				skillType = (IJSAPI<SkillType>)skillType.GetContext(),
				usingCenter = center,
				targets = targetApiArray
			};
			GameEventBus.Instance.Publish(gameEvent);
		}
		
		public void UseStunt(Stunt stunt, CharacterAction action, GridPos center, IEnumerable<SceneObject> targets) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			StuntCheckFilter(false, ref stunt, ref action, ref center, ref targets);
			var targetList = new List<SceneObject>();
			foreach (var target in targets) {
				targetList.Add(target);
			}
			stunt.Effect.Situation = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = true,
				InitiativeSS = null, InitiativeBS = this,
				PassiveSS = null, PassiveBS = null,
				Action = action, IsOnInteract = false,
				InitiativeSkillType = null, TargetsBS = targetList.ToArray()
			};
			stunt.Effect.TakeEffect((success, message) => {
				if (success) {
					var battleMapProperty = stunt.BattleMapProperty;
					this.ActionPoint -= battleMapProperty.actionPointCost;
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.DisplayUsingStunt(this, stunt);
						player.Client.BattleScene.UpdateActionPoint(this);
					}
					Game.DM.Client.BattleScene.DisplayUsingStunt(this, stunt);
					Game.DM.Client.BattleScene.UpdateActionPoint(this);
					this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntComplete();
				} else {
					this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntFailure(message);
				}
			});
		}
		
		public void UseStuntOnInteract(Stunt stunt, GridPos center, IEnumerable<SceneObject> targets) {
			if (stunt == null) throw new ArgumentNullException(nameof(stunt));
			CharacterAction empty = 0;
			StuntCheckFilter(true, ref stunt, ref empty, ref center, ref targets);
			var targetList = new List<SceneObject>();
			foreach (var target in targets) {
				targetList.Add(target);
			}
			stunt.Effect.Situation = new Situation() {
				IsTriggerInvoking = false, EventID = "",
				IsInStoryScene = false, IsInitiative = true,
				InitiativeSS = null, InitiativeBS = this,
				PassiveSS = null, PassiveBS = null,
				Action = 0, IsOnInteract = true,
				InitiativeSkillType = null, TargetsBS = targetList.ToArray()
			};
			stunt.Effect.TakeEffect((success, message) => {
				if (success) {
					var battleMapProperty = stunt.BattleMapProperty;
					this.ActionPoint -= battleMapProperty.actionPointCost;
					foreach (Player player in Game.Players) {
						player.Client.BattleScene.DisplayUsingStunt(this, stunt);
						player.Client.BattleScene.UpdateActionPoint(this);
					}
					Game.DM.Client.BattleScene.DisplayUsingStunt(this, stunt);
					Game.DM.Client.BattleScene.UpdateActionPoint(this);
					this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntComplete();
				} else {
					this.CharacterRef.Controller.Client.BattleScene.NotifyInitiativeSelectSkillOrStuntFailure(message);
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
			_highland = new List<GridObject>();
			_lowland = new List<GridObject>();
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
			if (srcHighland) srcLand = (List<GridObject>)srcGrid.Highland;
			else srcLand = (List<GridObject>)srcGrid.Lowland;
			if (dstHighland) dstLand = (List<GridObject>)dstGrid.Highland;
			else dstLand = (List<GridObject>)dstGrid.Lowland;
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