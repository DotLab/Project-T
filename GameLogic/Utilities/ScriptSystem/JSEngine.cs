using GameLogic.Utilities.DataSystem;
using GameLogic.Utilities.ScriptSystem.EngineWrapper;
using System;

namespace GameLogic.Utilities.ScriptSystem {
	public interface IJSEngineRaw {
		void BindType(string typeName, Type type);
		object GetVar(string name);
		void SetVar(string name, object[] array);
		void SetVar(string name, object obj);
		void SetVar(string name, double number);
		void SetVar(string name, string str);
		void SetVar(string name, bool boolean);
		void SetVar(string name, Delegate func);
		void DelVar(string name);
		void Execute(string code);
	}

	public class JSException : Exception {
		public enum JSErrorType {
			Runtime, Compile
		}

		protected int _jsErrID = 0;
		protected JSErrorType _jsErrType = JSErrorType.Runtime;
		protected string _jsErrMessage = "";

		public int JSErrID => _jsErrID;
		public JSErrorType JSErrType => _jsErrType;
		public string JSErrMessage => _jsErrMessage;

		protected static string FullMessage(int errID, JSErrorType errType, string errMessage) {
			string ret = "Javascript ";
			switch (errType) {
				case JSErrorType.Runtime:
					ret += "Runtime ";
					break;
				case JSErrorType.Compile:
					ret += "Compile ";
					break;
				default:
					break;
			}
			ret += "Error (" + errID + "): " + errMessage;
			return ret;
		}

		public JSException() { }
		public JSException(string message) : base(message) { }
		public JSException(string message, Exception inner) : base(message, inner) { }

		public JSException(int errID, JSErrorType errType, string errMessage) :
			base(FullMessage(errID, errType, errMessage)) {
			_jsErrID = errID;
			_jsErrType = errType;
			_jsErrMessage = errMessage;
		}

	}

	public sealed class JSEngine {
		private readonly IJSEngineRaw _engine;

		public JSEngine(IJSEngineRaw engine) {
			_engine = engine ?? throw new ArgumentNullException(nameof(engine));
		}

		public void Log(string log) {
			Logger.WriteLine("Javascript Engine Log:" + log);
		}

		public void SynchronizeContext(string varname, IJSContextProvider provider) {
			IJSContext context = (IJSContext)_engine.GetVar(varname);
			if (context == null) _engine.SetVar(varname, provider.GetContext());
			else {
				if (!(context is IJSAPI<IJSContextProvider>)) {
					provider.SetContext(context);
				}
			}
		}

		public void RemoveContext(string varname) {
			_engine.DelVar(varname);
		}

		public void Execute(string code) {
			_engine.Execute(code);
		}
	}

	public static class JSEngineManager {
		private readonly static IJSEngineRaw _engineRaw;
		private readonly static JSEngine _engine;

		static JSEngineManager() {
			_engineRaw = new JintEngine();
			_engine = new JSEngine(_engineRaw);
		}

		public static IJSEngineRaw EngineRaw => _engineRaw;
		public static JSEngine Engine => _engine;

	}
}
