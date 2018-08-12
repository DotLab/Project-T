using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem
{
    public interface IJSContext { }

    public interface IJSContextProvider
    {
        IJSContext GetContext();
        void SetContext(IJSContext context);
    }
    
    public interface IJSAPI<out T> : IJSContext where T : IJSContextProvider
    {
        T Origin(JSContextHelper proof);
    }

    public sealed class JSContextHelper
    {
        private static readonly JSContextHelper _instance = new JSContextHelper();

        public static JSContextHelper Instance => _instance;

        private JSContextHelper()
        {

        }

        public T GetAPIOrigin<T>(IJSAPI<T> jsApi) where T : IJSContextProvider
        {
            return jsApi.Origin(this);
        }
    } 
}
