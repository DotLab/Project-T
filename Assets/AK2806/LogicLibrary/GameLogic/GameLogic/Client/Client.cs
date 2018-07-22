using GameLogic.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Client
{
    public abstract class Client
    {

    }

    public abstract class Battleground
    {

        public void Reset()
        {

        }
    }

    public abstract class Storyboard
    {
        public event EventHandler<TextClickEventArgs> OnTextClick;

        public void Reset()
        {

        }

        public void AddObject(string id, View view)
        {

        }

        public void TransformObject(string id, Layout layout)
        {

        }

        public void ShowObject(string id)
        {

        }

        public void HideObject(string id)
        {

        }

        public void ChangeObjViewEffect(string id, StoryViewEffect effect)
        {

        }

        public void ChangeObjPortraitStyle(string id, PortraitStyle portrait)
        {

        }

        public void RemoveObject(string id)
        {

        }

        public void TransformCamera(Layout layout)
        {

        }

        public void ChangeCameraViewEffect(StoryViewEffect effect)
        {

        }

        public void AddParagraph(string text)
        {

        }

        public void AddClickableParagraph(string text, int code)
        {

        }

        public void ClearAllText()
        {

        }

        public void AttachPortraitToTextBox(View view)
        {

        }

        public void ChangeTextBoxPortraitStyle(PortraitStyle portrait)
        {

        }

        public void RemovePortraitFromTextBox()
        {

        }

        public void PlayBGM(string id)
        {

        }

        public void StopBGM()
        {

        }

        public void PlaySE(string id)
        {

        }

    }
}
