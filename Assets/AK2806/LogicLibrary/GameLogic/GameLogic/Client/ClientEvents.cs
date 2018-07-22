using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public class SceneObjectEventArgs : EventArgs
    {
        protected readonly string _objID;
        public string ObjectID => _objID;

        public SceneObjectEventArgs(string objID)
        {
            _objID = objID;
        }

    }

    public class TextClickEventArgs : EventArgs
    {
        protected readonly int _selectionCode;
        public int SelectionCode => _selectionCode;

        public TextClickEventArgs(int selectionCode)
        {
            _selectionCode = selectionCode;
        }
    }

}
