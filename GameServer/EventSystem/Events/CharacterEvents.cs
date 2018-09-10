using GameServer.CharacterSystem;
using GameServer.Core.ScriptSystem;
using System;

namespace GameServer.EventSystem.Events {
	public class CharacterEvent : Event {
		private static readonly string[] _idList = {
			"event.character"
		};

		public override string[] NotifyList => _idList;

		public override IJSContext GetContext() {
			throw new NotImplementedException();
		}

		public override void SetContext(IJSContext context) {
			throw new NotImplementedException();
		}
	}
	
}
