using System;
using System.Collections.Generic;
using System.Text;
using Jint;
using Jint.Parser;
using Jint.Native;
using Jint.Runtime;

namespace GameLogic.Core.ScriptSystem.EngineWrapper
{
    public sealed class JintEngine : IJSEngineRaw
    {
        private Engine _jsEngine;

        public JintEngine()
        {
            this._jsEngine = new Engine(cfg => cfg.LimitRecursion(1024));
        }
        
        public void Execute(string code)
        {
            try
            {
                this._jsEngine.Execute(code);
            }
            catch (ParserException e)
            {
                throw new JSException(0, JSException.JSErrorType.Compile, e.Message);
            }
            catch (JavaScriptException e)
            {
                throw new JSException(0, JSException.JSErrorType.Runtime, e.Message);
            }
            catch (RecursionDepthOverflowException e)
            {
                throw new JSException(1, JSException.JSErrorType.Runtime, e.Message);
            }
            catch (StatementsCountOverflowException e)
            {
                throw new JSException(1, JSException.JSErrorType.Compile, e.Message);
            }
        }

        public void SetVar(string name, object[] array)
        {
            this._jsEngine.SetValue(name, array);
        }

        public void SetVar(string name, object poco)
        {
            this._jsEngine.SetValue(name, poco);
        }

        public void SetVar(string name, double number)
        {
            this._jsEngine.SetValue(name, number);
        }

        public void SetVar(string name, string str)
        {
            this._jsEngine.SetValue(name, str);
        }

        public void SetVar(string name, bool boolean)
        {
            this._jsEngine.SetValue(name, boolean);
        }

        public void SetVar(string name, Delegate func)
        {
            this._jsEngine.SetValue(name, func);
        }

        public object GetVar(string name)
        {
            return this._jsEngine.GetValue(name).ToObject();
        }

        public T GetVar<T>(string name)
        {
            return (T)this._jsEngine.GetValue(name).ToObject();
        }

        public void DelVar(string name)
        {
            this._jsEngine.SetValue(name, JsValue.Undefined);
        }
    }
}
