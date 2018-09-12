using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.CharacterSystem;
using GameServer.Container.BattleComponent;
using GameServer.Core.ScriptSystem;
using GameUtil;

namespace GameServer.EventSystem.Events {
	public sealed class BattleSceneUseSkillOnInteractEvent : Event {
		public struct EventInfo : IEventInfo {
			public bool swallowed;

			public IJSAPI<SceneObject> user;
			public IJSAPI<SkillType> skillType;
			public GridPos usingCenter;
			public IJSAPI<SceneObject>[] targets;
			
			public bool Swallowed { get => swallowed; set => swallowed = value; }
		}

		private static readonly string[] _idList = {
			"event.battleScene.useSkillOnInteract"
		};

		public override string[] NotifyList => _idList;

		private EventInfo _info;

		public EventInfo Info { get => _info; set => _info = value; }

		public override IJSContext GetContext() {
			return _info;
		}

		public override void SetContext(IJSContext context) {
			_info = (EventInfo)context;
		}
	}

	public sealed class BattleSceneOnceCheckOverEvent : Event {
		public struct EventInfo : IEventInfo {
			public bool swallowed;

			public IJSAPI<SceneObject> initiative;
			public IJSAPI<SkillType> initiativeSkillType;
			public int[] initiativeRollPoints;
			public int initiativeExtraPoint;
			public IJSAPI<SceneObject> passive;
			public IJSAPI<SkillType> passiveSkillType;
			public int[] passiveRollPoints;
			public int passiveExtraPoint;
			public CharacterAction action;

			public CheckResult initiativeCheckResult;
			public CheckResult passiveCheckResult;
			public int pointDelta;
			
			public bool Swallowed { get => swallowed; set => swallowed = value; }
		}

		private static readonly string[] _idList = {
			"event.battleScene.onceCheckOver"
		};

		public override string[] NotifyList => _idList;

		private EventInfo _info;

		public EventInfo Info { get => _info; set => _info = value; }

		public override IJSContext GetContext() {
			return _info;
		}

		public override void SetContext(IJSContext context) {
			_info = (EventInfo)context;
		}
	}
	
	public sealed class BattleSceneOnceCheckOverAspectCreatedEvent : Event {
		public struct EventInfo : IEventInfo {
			public bool swallowed;

			public IJSAPI<Aspect> createdAspect;
			public IJSAPI<SceneObject> from;
			public IJSAPI<SceneObject> to;
			public CharacterAction action;
			public CheckResult from_checkResult;
			public CheckResult to_checkResult;
			public int pointDelta;

			public bool Swallowed { get => swallowed; set => swallowed = value; }
		}

		private static readonly string[] _idList = {
			"event.battleScene.onceCheckOverAspectCreated"
		};

		public override string[] NotifyList => _idList;

		private EventInfo _info;

		public EventInfo Info { get => _info; set => _info = value; }

		public override IJSContext GetContext() {
			return _info;
		}

		public override void SetContext(IJSContext context) {
			_info = (EventInfo)context;
		}
	}

	public sealed class BattleSceneOnceCheckOverCauseDamageEvent : Event {
		public struct EventInfo : IEventInfo {
			public bool swallowed;

			public int damage;
			public bool mental;
			public IJSAPI<SceneObject> from;
			public IJSAPI<SceneObject> to;
			public CharacterAction action;
			public CheckResult from_checkResult;
			public CheckResult to_checkResult;
			public int pointDelta;

			public bool Swallowed { get => swallowed; set => swallowed = value; }
		}

		private static readonly string[] _idList = {
			"event.battleScene.onceCheckOverCauseDamage"
		};

		public override string[] NotifyList => _idList;

		private EventInfo _info;

		public EventInfo Info { get => _info; set => _info = value; }

		public override IJSContext GetContext() {
			return _info;
		}

		public override void SetContext(IJSContext context) {
			_info = (EventInfo)context;
		}
	}
}
