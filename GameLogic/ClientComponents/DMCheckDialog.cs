using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;

namespace GameLogic.ClientComponents
{
    public class DMCheckDialog : ClientComponent
    {
        private bool _isChecking = false;
        private Action<bool> _resultCallback;

        public bool IsChecking => _isChecking;

        public override void MessageReceived(Message message)
        {
            DMCheckResultMessage resultMessage = (DMCheckResultMessage)message;
            if (_isChecking)
            {
                _resultCallback(resultMessage.result);
                _isChecking = false;
                this.Hide();
            }
        }

        public DMCheckDialog(Connection connection, User owner) :
            base(connection, owner)
        {
            _connection.AddMessageReceiver(DMCheckResultMessage.MESSAGE_TYPE, this);
        }

        public void RequestCheck(User user, string text, Action<bool> result)
        {
            if (_isChecking) return;
            if (user.IsDM)
            {
                result(true);
                return;
            }
            _resultCallback = result;
            _isChecking = true;
            this.Show(text);
        }
        
        private void Show(string text)
        {
            DMCheckPanelShowMessage message = new DMCheckPanelShowMessage();
            message.text = text;
            _connection.SendMessage(message);
        }
        
        private void Hide()
        {
            DMCheckPanelHideMessage message = new DMCheckPanelHideMessage();
            _connection.SendMessage(message);
        }
    }
}
