using Jint;
using Jint.Native;
using Jint.Parser;
using Jint.Runtime;
using Jint.Runtime.Interop;
using System;

namespace GameLib.Core.ScriptSystem.EngineWrapper {
	public sealed class JintEngine : IJSEngineRaw {
		private readonly Engine _jsEngine;

		public JintEngine() {
			_jsEngine = new Engine(cfg => cfg.LimitRecursion(1024));
		}

		public void Execute(string code) {
			try {
				_jsEngine.Execute(code);
			} catch (ParserException e) {
				throw new JSException(0, JSException.JSErrorType.Compile, e.Message);
			} catch (JavaScriptException e) {
				throw new JSException(0, JSException.JSErrorType.Runtime, e.Message);
			} catch (RecursionDepthOverflowException e) {
				throw new JSException(1, JSException.JSErrorType.Runtime, e.Message);
			} catch (StatementsCountOverflowException e) {
				throw new JSException(2, JSException.JSErrorType.Runtime, e.Message);
			} catch (Exception e) {
				throw new JSException(3, JSException.JSErrorType.Runtime, e.Message);
			}
		}

		public void SetVar(string name, object[] array) {
			_jsEngine.SetValue(name, array);
		}

		public void SetVar(string name, object obj) {
			_jsEngine.SetValue(name, obj);
		}

		public void SetVar(string name, double number) {
			_jsEngine.SetValue(name, number);
		}

		public void SetVar(string name, string str) {
			_jsEngine.SetValue(name, str);
		}

		public void SetVar(string name, bool boolean) {
			_jsEngine.SetValue(name, boolean);
		}

		public void SetVar(string name, Delegate func) {
			_jsEngine.SetValue(name, func);
		}

		public object GetVar(string name) {
			return _jsEngine.GetValue(name).ToObject();
		}

		public void DelVar(string name) {
			_jsEngine.SetValue(name, JsValue.Undefined);
		}

		public void BindType(string typeName, Type type) {
			_jsEngine.SetValue(typeName, TypeReference.CreateTypeReference(_jsEngine, type));
		}
	}
}
