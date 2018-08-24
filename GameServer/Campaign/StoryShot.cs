using GameLib.Core;
using System;
using System.Collections.Generic;

namespace GameLib.Campaign {
	public sealed class StoryShot : Shot {
		private readonly List<SceneAction> _actions;
		private int _currentActionIndex;

		public List<SceneAction> Actions => _actions;
		public int CurrentActionIndex => _currentActionIndex;

		public override ShotType Type => ShotType.STORY;
		public override StoryShot Story => this;
		public override BattleShot Battle => null;
		public override FreedomShot Freedom => null;

		public StoryShot(List<SceneAction> actions) {
			_actions = actions ?? throw new ArgumentNullException(nameof(actions));
			_currentActionIndex = -1;
		}

		public SceneAction NextAction() {
			if (_actions != null) {
				int nextIndex = _currentActionIndex + 1;
				if (nextIndex >= 0 && nextIndex < _actions.Count) {
					return _actions[nextIndex];
				}
			}
			return null;
		}

		public SceneAction CurrentAction() {
			if (_actions != null) {
				if (_currentActionIndex >= 0 && _currentActionIndex < _actions.Count) {
					return _actions[_currentActionIndex];
				}
			}
			return null;
		}

		public bool Next() {
			if (_actions != null) {
				int nextIndex = _currentActionIndex + 1;
				if (nextIndex >= 0 && nextIndex < _actions.Count) {
					_currentActionIndex = nextIndex;
					return true;
				}
			}
			return false;
		}
	}

	public sealed class SceneAction {
		private Command _command;
		private string _comment;

		public Command Command { get => _command; set => _command = value; }
		public string Comment { get => _comment; set => _comment = value ?? throw new ArgumentNullException(nameof(Comment)); }

		public SceneAction(Command command = null, string comment = "") {
			_command = command;
			_comment = comment ?? throw new ArgumentNullException(nameof(comment));
		}

		public void DoAction() {
			if (_command != null) {
				_command.DoAction();
			}
		}
	}
}
