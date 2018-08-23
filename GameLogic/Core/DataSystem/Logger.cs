using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.DataSystem {
	public interface ILogger {
		void Write(string str);
		void WriteLine(string str);
	}

	public static class Logger {
		private static ILogger _logger = null;

		public static void ApplyLogger(ILogger logger) {
			_logger = logger;
		}

		public static void Write(string str) {
			if (_logger != null) _logger.Write(str);
			else Console.Write(str);
		}

		public static void WriteLine(string str) {
			if (_logger != null) _logger.WriteLine(str);
			else Console.WriteLine(str);
		}

	}
}
