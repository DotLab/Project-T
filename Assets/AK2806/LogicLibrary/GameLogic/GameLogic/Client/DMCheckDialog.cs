using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;

namespace GameLogic.Client
{
    public class DMCheckDialog : ClientComponent
    {
        private bool _isChecking = false;
        private Action<bool> _resultCallback;

        public bool IsChecking => _isChecking;

        public override void MessageReceived(ulong timestamp, Message message)
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

        public void RequestCheck(string text, Action<bool> result)
        {
            if (_isChecking) throw new InvalidOperationException("Already in checking state.");
            if (text == null) throw new ArgumentNullException(nameof(text));
            _resultCallback = result ?? throw new ArgumentNullException(nameof(result));
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
