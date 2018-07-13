using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.ScriptSystem
{
    public interface IJSContextProvider
    {
        object GetContext();
        void SetContext(object context);
    }
    
    public interface IJSAPI
    {
        IJSContextProvider Origin(JSContextHelper proof);
    }

    public sealed class JSContextHelper
    {
        private static JSContextHelper _instance = new JSContextHelper();

        public static JSContextHelper Instance => _instance;

        private JSContextHelper()
        {

        }

        public IJSContextProvider GetAPIOrigin(IJSAPI jsApi)
        {
            return jsApi.Origin(this);
        }
    } 
}
