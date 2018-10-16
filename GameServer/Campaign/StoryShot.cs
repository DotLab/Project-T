using GameServer.Core;
using System;
using System.Collections.Generic;

namespace GameServer.Campaign {
	public sealed class StoryShot : Shot {
		private readonly List<StoryAction> _actions;
		private int _currentActionIndex;

		public List<StoryAction> Actions => _actions;
		public int CurrentActionIndex => _currentActionIndex;

		public override ShotType Type => ShotType.STORY;
		public override StoryShot Story => this;
		public override BattleShot Battle => null;
		public override InvestigationShot Investigation => null;

		public StoryShot(IEnumerable<StoryAction> actions) {
			_actions = new List<StoryAction>(actions);
			_currentActionIndex = -1;
		}

		public StoryAction NextAction() {
			if (_actions != null) {
				int nextIndex = _currentActionIndex + 1;
				if (nextIndex >= 0 && nextIndex < _actions.Count) {
					return _actions[nextIndex];
				}
			}
			return null;
		}

		public StoryAction CurrentAction() {
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

	public sealed class StoryAction : IDescribable {
		private readonly Command _command;
		private readonly string _name;
		private readonly string _description;

		public Command Command { get => _command; set { } }
		public string Name { get => _name; set { } }
		public string Description { get => _description; set { } }

		public StoryAction(Command command, string name, string description) {
			_command = command;
			_name = name;
			_description = description;
		}

		public void DoAction() {
			_command.DoAction();
		}
	}
}
