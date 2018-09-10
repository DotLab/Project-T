using GameServer.Core.DataSystem;
using GameServer.Core.ScriptSystem;
using System;

namespace GameServer.Core {
	public class Command {
		protected readonly string _logName;
		protected readonly bool _javascript;
		protected readonly string _actionJS;
		protected readonly Action _actionCLR;

		public string LogName => _logName;
		public string ActionJS => _actionJS;
		public Action ActionCLR => _actionCLR;

		public Command(string jscode) : this("", true, null, jscode) { }

		public Command(Action action) : this("", false, action, null) { }

		public Command(string logName, string jscode) : this(logName, true, null, jscode) { }

		public Command(string logName, Action action) : this(logName, false, action, null) { }

		public Command(string logName, bool javascript, Action action, string jscode) {
			_logName = logName ?? throw new ArgumentNullException(nameof(logName));
			_javascript = javascript;
			if (_javascript) {
				_actionCLR = null;
				_actionJS = jscode ?? throw new ArgumentNullException(nameof(jscode));
			} else {
				_actionCLR = action ?? throw new ArgumentNullException(nameof(action));
				_actionJS = null;
			}
		}

		public virtual void DoAction() {
			try {
				if (_javascript) JSEngineManager.Engine.Execute(_actionJS, _logName);
				else _actionCLR();
			} catch (Exception e) {
				Logger.WriteLine("Command '" + _logName + "' performing error: " + e.Message);
			}
		}

	}

}
