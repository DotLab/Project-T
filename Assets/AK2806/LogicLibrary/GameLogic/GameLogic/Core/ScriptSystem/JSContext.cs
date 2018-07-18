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
    
    public interface IJSAPI : IJSContext
    {
        IJSContextProvider Origin(JSContextHelper proof);
    }

    public sealed class JSContextHelper
    {
        private static readonly JSContextHelper _instance = new JSContextHelper();

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
