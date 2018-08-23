using GameLogic.Core.DataSystem;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Bit = Networkf.BitHelper;

namespace GameLogic.Core.Network
{
    public abstract class Message : IStreamable
    {
        #region Message Creator
        public static Message New(int messageType)
        {
            switch (messageType)
            {
                case IdentifiedMessage.MESSAGE_TYPE:
                    return new IdentifiedMessage();

                case ServerReadyMessage.MESSAGE_TYPE:
                    return new ServerReadyMessage();
                case StorySceneResetMessage.MESSAGE_TYPE:
                    return new StorySceneResetMessage();
                case StorySceneObjectAddMessage.MESSAGE_TYPE:
                    return new StorySceneObjectAddMessage();
                case StorySceneObjectRemoveMessage.MESSAGE_TYPE:
                    return new StorySceneObjectRemoveMessage();
                case StorySceneObjectTransformMessage.MESSAGE_TYPE:
                    return new StorySceneObjectTransformMessage();
                case StorySceneObjectViewEffectMessage.MESSAGE_TYPE:
                    return new StorySceneObjectViewEffectMessage();
                case StorySceneObjectPortraitStyleMessage.MESSAGE_TYPE:
                    return new StorySceneObjectPortraitStyleMessage();
                case StorySceneCameraTransformMessage.MESSAGE_TYPE:
                    return new StorySceneCameraTransformMessage();
                case StorySceneCameraEffectMessage.MESSAGE_TYPE:
                    return new StorySceneCameraEffectMessage();
                case PlayBGMMessage.MESSAGE_TYPE:
                    return new PlayBGMMessage();
                case StopBGMMessage.MESSAGE_TYPE:
                    return new StopBGMMessage();
                case PlaySEMessage.MESSAGE_TYPE:
                    return new PlaySEMessage();
                case ShowSceneMessage.MESSAGE_TYPE:
                    return new ShowSceneMessage();
                case TextBoxAddParagraphMessage.MESSAGE_TYPE:
                    return new TextBoxAddParagraphMessage();
                case TextBoxAddSelectionMessage.MESSAGE_TYPE:
                    return new TextBoxAddSelectionMessage();
                case TextBoxClearMessage.MESSAGE_TYPE:
                    return new TextBoxClearMessage();
                case TextBoxSetPortraitMessage.MESSAGE_TYPE:
                    return new TextBoxSetPortraitMessage();
                case TextBoxPortraitStyleMessage.MESSAGE_TYPE:
                    return new TextBoxPortraitStyleMessage();
                case TextBoxPortraitEffectMessage.MESSAGE_TYPE:
                    return new TextBoxPortraitEffectMessage();
                case CharacterInfoDataMessage.MESSAGE_TYPE:
                    return new CharacterInfoDataMessage();
                case CharacterSkillsDescriptionMessage.MESSAGE_TYPE:
                    return new CharacterSkillsDescriptionMessage();
                case CharacterAspectsDescriptionMessage.MESSAGE_TYPE:
                    return new CharacterAspectsDescriptionMessage();
                case CharacterStuntsDescriptionMessage.MESSAGE_TYPE:
                    return new CharacterStuntsDescriptionMessage();
                case CharacterExtrasDescriptionMessage.MESSAGE_TYPE:
                    return new CharacterExtrasDescriptionMessage();
                case CharacterConsequencesDescriptionMessage.MESSAGE_TYPE:
                    return new CharacterConsequencesDescriptionMessage();
                case CharacterStressDataMessage.MESSAGE_TYPE:
                    return new CharacterStressDataMessage();
                case CharacterFatePointDataMessage.MESSAGE_TYPE:
                    return new CharacterFatePointDataMessage();
                case AspectDataMessage.MESSAGE_TYPE:
                    return new AspectDataMessage();
                case ConsequenceDataMessage.MESSAGE_TYPE:
                    return new ConsequenceDataMessage();
                case SkillDataMessage.MESSAGE_TYPE:
                    return new SkillDataMessage();
                case StuntDataMessage.MESSAGE_TYPE:
                    return new StuntDataMessage();
                case ExtraDataMessage.MESSAGE_TYPE:
                    return new ExtraDataMessage();
                case DirectResistSkillsDataMessage.MESSAGE_TYPE:
                    return new DirectResistSkillsDataMessage();
                case SkillTypeListDataMessage.MESSAGE_TYPE:
                    return new SkillTypeListDataMessage();
                case SkillCheckPanelShowMessage.MESSAGE_TYPE:
                    return new SkillCheckPanelShowMessage();
                case SkillCheckPanelHideMessage.MESSAGE_TYPE:
                    return new SkillCheckPanelHideMessage();
                case DMCheckPanelShowMessage.MESSAGE_TYPE:
                    return new DMCheckPanelShowMessage();
                case DMCheckPanelHideMessage.MESSAGE_TYPE:
                    return new DMCheckPanelHideMessage();
                case DisplayDicePointsMessage.MESSAGE_TYPE:
                    return new DisplayDicePointsMessage();
                case StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage.MESSAGE_TYPE:
                    return new StorySceneCheckerNotifyInitiativeSelectSkillOrStuntMessage();
                case StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE:
                    return new StorySceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
                case CheckerSelectSkillOrStuntCompleteMessage.MESSAGE_TYPE:
                    return new CheckerSelectSkillOrStuntCompleteMessage();
                case StorySceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE:
                    return new StorySceneCheckerNotifySelectAspectMessage();
                case CheckerSelectAspectCompleteMessage.MESSAGE_TYPE:
                    return new CheckerSelectAspectCompleteMessage();
                case StorySceneCheckerUpdateSumPointMessage.MESSAGE_TYPE:
                    return new StorySceneCheckerUpdateSumPointMessage();
                case StorySceneCheckerDisplaySkillReadyMessage.MESSAGE_TYPE:
                    return new StorySceneCheckerDisplaySkillReadyMessage();
                case StorySceneCheckerDisplayUsingAspectMessage.MESSAGE_TYPE:
                    return new StorySceneCheckerDisplayUsingAspectMessage();
                case StorySceneAddPlayerCharacterMessage.MESSAGE_TYPE:
                    return new StorySceneAddPlayerCharacterMessage();
                case StorySceneRemovePlayerCharacterMessage.MESSAGE_TYPE:
                    return new StorySceneRemovePlayerCharacterMessage();
                case BattleScenePushGridObjectMessage.MESSAGE_TYPE:
                    return new BattleScenePushGridObjectMessage();
                case BattleSceneRemoveGridObjectMessage.MESSAGE_TYPE:
                    return new BattleSceneRemoveGridObjectMessage();
                case BattleSceneAddLadderObjectMessage.MESSAGE_TYPE:
                    return new BattleSceneAddLadderObjectMessage();
                case BattleSceneRemoveLadderObjectMessage.MESSAGE_TYPE:
                    return new BattleSceneRemoveLadderObjectMessage();
                case BattleSceneResetMessage.MESSAGE_TYPE:
                    return new BattleSceneResetMessage();
                case BattleSceneSetActingOrderMessage.MESSAGE_TYPE:
                    return new BattleSceneSetActingOrderMessage();
                case BattleSceneChangeTurnMessage.MESSAGE_TYPE:
                    return new BattleSceneChangeTurnMessage();
                case BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage.MESSAGE_TYPE:
                    return new BattleSceneCheckerNotifyPassiveSelectSkillOrStuntMessage();
                case BattleSceneCheckerNotifySelectAspectMessage.MESSAGE_TYPE:
                    return new BattleSceneCheckerNotifySelectAspectMessage();
                case BattleSceneCheckerUpdateSumPointMessage.MESSAGE_TYPE:
                    return new BattleSceneCheckerUpdateSumPointMessage();
                case BattleSceneCheckerDisplaySkillReadyMessage.MESSAGE_TYPE:
                    return new BattleSceneCheckerDisplaySkillReadyMessage();
                case BattleSceneCheckerDisplayUsingAspectMessage.MESSAGE_TYPE:
                    return new BattleSceneCheckerDisplayUsingAspectMessage();
                case BattleSceneMovePathInfoMessage.MESSAGE_TYPE:
                    return new BattleSceneMovePathInfoMessage();
                case BattleSceneDisplayActableObjectMovingMessage.MESSAGE_TYPE:
                    return new BattleSceneDisplayActableObjectMovingMessage();
                    
                case ClientInitMessage.MESSAGE_TYPE:
                    return new ClientInitMessage();
                case StorySceneObjectActionMessage.MESSAGE_TYPE:
                    return new StorySceneObjectActionMessage();
                case TextSelectedMessage.MESSAGE_TYPE:
                    return new TextSelectedMessage();
                case StorySceneNextActionMessage.MESSAGE_TYPE:
                    return new StorySceneNextActionMessage();
                case CheckerSkillSelectedMessage.MESSAGE_TYPE:
                    return new CheckerSkillSelectedMessage();
                case CheckerAspectSelectedMessage.MESSAGE_TYPE:
                    return new CheckerAspectSelectedMessage();
                case CheckerStuntSelectedMessage.MESSAGE_TYPE:
                    return new CheckerStuntSelectedMessage();
                case GetCharacterDataMessage.MESSAGE_TYPE:
                    return new GetCharacterDataMessage();
                case GetAspectDataMessage.MESSAGE_TYPE:
                    return new GetAspectDataMessage();
                case GetConsequenceDataMessage.MESSAGE_TYPE:
                    return new GetConsequenceDataMessage();
                case GetSkillDataMessage.MESSAGE_TYPE:
                    return new GetSkillDataMessage();
                case GetStuntDataMessage.MESSAGE_TYPE:
                    return new GetStuntDataMessage();
                case GetExtraDataMessage.MESSAGE_TYPE:
                    return new GetExtraDataMessage();
                case GetDirectResistSkillsMessage.MESSAGE_TYPE:
                    return new GetDirectResistSkillsMessage();
                case GetSkillTypeListMessage.MESSAGE_TYPE:
                    return new GetSkillTypeListMessage();
                case DMCheckResultMessage.MESSAGE_TYPE:
                    return new DMCheckResultMessage();
                case BattleSceneSetSkipSelectAspectMessage.MESSAGE_TYPE:
                    return new BattleSceneSetSkipSelectAspectMessage();
                case SelectAspectOverMessage.MESSAGE_TYPE:
                    return new SelectAspectOverMessage();
                case BattleSceneGetActableObjectMovePathInfoMessage.MESSAGE_TYPE:
                    return new BattleSceneGetActableObjectMovePathInfoMessage();
                case BattleSceneActableObjectMoveMessage.MESSAGE_TYPE:
                    return new BattleSceneActableObjectMoveMessage();
                case BattleSceneActableObjectDoActionMessage.MESSAGE_TYPE:
                    return new BattleSceneActableObjectDoActionMessage();
                case BattleSceneActableObjectDoSpecialActionMessage.MESSAGE_TYPE:
                    return new BattleSceneActableObjectDoSpecialActionMessage();
                    
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        public abstract void WriteTo(IDataOutputStream stream);
        public abstract void ReadFrom(IDataInputStream stream);

        public abstract int MessageType { get; }
    }

    public sealed class IdentifiedMessage : Message
    {
        public Message innerMessage;
        public Guid guid;
        public bool resp;

        public const int MESSAGE_TYPE = 0;
        public override int MessageType => MESSAGE_TYPE;
        public int InnerMsgType => innerMessage.MessageType;

        public IdentifiedMessage() { }

        public IdentifiedMessage(Message message)
        {
            this.innerMessage = message;
            this.guid = Guid.NewGuid();
            this.resp = false;
        }

        public override void ReadFrom(IDataInputStream stream)
        {
            int innerType = stream.ReadInt32();
            innerMessage = New(innerType);
            innerMessage.ReadFrom(stream);
            guid = InputStreamHelper.ReadGuid(stream);
            resp = stream.ReadBoolean();
        }

        public override void WriteTo(IDataOutputStream stream)
        {
            stream.WriteInt32(this.InnerMsgType);
            innerMessage.WriteTo(stream);
            OutputStreamHelper.WriteGuid(stream, guid);
            stream.WriteBoolean(resp);
        }
    }

    public interface IMessageReceiver
    {
        void MessageReceived(Message message);
    }

    public interface IRequestHandler
    {
        Message MakeResponse(Message request);
    }

    public abstract class Connection : IMessageReceiver
    {
        private readonly Dictionary<Guid, Action<Message>> _callbackDict = new Dictionary<Guid, Action<Message>>();
        private readonly Dictionary<int, IRequestHandler> _reqHandlerDict = new Dictionary<int, IRequestHandler>();
        
        public void MessageReceived(Message message)
        {
            var identifiedMessage = (IdentifiedMessage)message;
            if (identifiedMessage.resp)
            {
                if (_callbackDict.TryGetValue(identifiedMessage.guid, out Action<Message> callback))
                {
                    callback(identifiedMessage.innerMessage);
                    _callbackDict.Remove(identifiedMessage.guid);
                }
            }
            else
            {
                Message resp = null;
                if (_reqHandlerDict.TryGetValue(identifiedMessage.InnerMsgType, out IRequestHandler handler))
                {
                    resp = handler.MakeResponse(identifiedMessage.innerMessage);
                }
                var respWrapper = new IdentifiedMessage() { innerMessage = resp, guid = identifiedMessage.guid, resp = true };
                SendMessage(respWrapper);
            }
        }

        public void Request(Message message, Action<Message> callback)
        {
            var identifiedMsg = new IdentifiedMessage(message);
            while (_callbackDict.ContainsKey(identifiedMsg.guid)) identifiedMsg.guid = Guid.NewGuid();
            _callbackDict.Add(identifiedMsg.guid, callback);
            SendMessage(identifiedMsg);
        }

        public void SetRequestHandler(int messageType, IRequestHandler handler)
        {
            _reqHandlerDict[messageType] = handler;
        }

        public Connection()
        {
            AddMessageReceiver(IdentifiedMessage.MESSAGE_TYPE, this);
        }

        protected void OnEventCaught(NetworkEventCaughtEventArgs args)
        {
            EventCaught?.Invoke(this, args);
        }

        public event EventHandler<NetworkEventCaughtEventArgs> EventCaught; // network thread invoke
        
        public abstract void SendMessage(Message message);
        public abstract void AddMessageReceiver(int messageType, IMessageReceiver receiver);
        public abstract bool RemoveMessageReceiver(int messageType, IMessageReceiver receiver);
        public abstract void UpdateReceiver();
    }

    public sealed class NetworkEventCaughtEventArgs : EventArgs
    {
        public string message;
        // ...
    }
    
    public sealed class NetworkfMessage : Networkf.Message
    {
        private readonly Message _innerMessage;

        public Message InnerMessage => _innerMessage;

        public static Networkf.Message ParseMessage(byte[] buf, ref int i)
        {
            int type = ReadMessageType(buf, ref i);
            var stream = new BitDataInputStream(buf, i);
            var message = Message.New(type);
            message.ReadFrom(stream);
            i = stream.i;
            return new NetworkfMessage(message);
        }

        public NetworkfMessage(Message message) :
            base(message.MessageType)
        {
            _innerMessage = message;
        }

        public override void WriteTo(byte[] buf, ref int i)
        {
            var stream = new BitDataOutputStream(buf, i);
            _innerMessage.WriteTo(stream);
            i = stream.i;
        }
    }

    public sealed class NetworkfConnection : Connection
    {
        private volatile Networkf.NetworkService _service = null;
        private readonly List<Message> _sendingMsgCache = new List<Message>();
        private readonly List<Message> _receivedMsgCache = new List<Message>();
        private readonly Dictionary<int, List<IMessageReceiver>> _messageReceiverDict = new Dictionary<int, List<IMessageReceiver>>();

        public NetworkfConnection()
        {
            Task.Run((Action)SendCachedMessage);
        }

        public void ApplyService(Networkf.NetworkService service)
        {
            if (_service != null) return;
            service.ParseMessage = NetworkfMessage.ParseMessage;
            service.OnMessageReceived += OnMessageReceived;
            service.OnServiceTeardown += OnServiceTeardown;
            _service = service;
        }

        public override void AddMessageReceiver(int messageType, IMessageReceiver receiver)
        {
            if (_messageReceiverDict.ContainsKey(messageType))
            {
                var receivers = _messageReceiverDict[messageType];
                if (!receivers.Contains(receiver)) receivers.Add(receiver);
            }
            else
            {
                _messageReceiverDict.Add(messageType, new List<IMessageReceiver>() { receiver });
            }
        }

        public override bool RemoveMessageReceiver(int messageType, IMessageReceiver receiver)
        {
            if (_messageReceiverDict.ContainsKey(messageType))
            {
                return _messageReceiverDict[messageType].Remove(receiver);
            }
            return false;
        }

        public override void SendMessage(Message message)
        {
            if (_service != null)
            {
                lock (_sendingMsgCache)
                {
                    _sendingMsgCache.Add(message);
                }
            }
            else
            {
                var identifiedMsg = message as IdentifiedMessage;
                if (identifiedMsg != null)
                {
                    if (_messageReceiverDict.TryGetValue(IdentifiedMessage.MESSAGE_TYPE, out List<IMessageReceiver> receivers))
                    {
                        foreach (var receiver in receivers)
                        {
                            receiver.MessageReceived(new IdentifiedMessage() { resp = true, guid = identifiedMsg.guid, innerMessage = null });
                        }
                    }
                }
            }
        }

        public override void UpdateReceiver()
        {
            lock (_receivedMsgCache)
            {
                foreach (var message in _receivedMsgCache)
                {
                    if (_messageReceiverDict.ContainsKey(message.MessageType))
                    {
                        foreach (var receiver in _messageReceiverDict[message.MessageType])
                        {
                            receiver.MessageReceived(message);
                        }
                    }
                }
                _receivedMsgCache.Clear();
            }
        }
        
        private void SendCachedMessage()
        {
            while (true)
            {
                var service = _service;
                if (service != null)
                {
                    Message message = null;
                    lock (_sendingMsgCache)
                    {
                        if (_sendingMsgCache.Count > 0)
                        {
                            message = _sendingMsgCache[0];
                            _sendingMsgCache.RemoveAt(0);
                        }
                    }
                    if (message != null)
                    {
                        int sendingResult = service.SendMessage(new NetworkfMessage(message));
                        if (sendingResult == -1)
                        {
                            var eventArgs = new NetworkEventCaughtEventArgs()
                            {
                                message = "A network error occured during sending data."
                            };
                            OnEventCaught(eventArgs);
                        }
                    }
                }
            }
        }

        private void OnMessageReceived(int id, Networkf.Message message)
        {
            lock (_receivedMsgCache)
            {
                _receivedMsgCache.Add(((NetworkfMessage)message).InnerMessage);
            }
        }
        
        private void OnServiceTeardown()
        {
            var service = _service;
            _service = null;
            service.ParseMessage = null;
            service.OnMessageReceived -= OnMessageReceived;
            service.OnServiceTeardown -= OnServiceTeardown;
            var eventArgs = new NetworkEventCaughtEventArgs()
            {
                message = "Connection is closed."
            };
            OnEventCaught(eventArgs);
        }
    }
    
    public sealed class BitDataOutputStream : IDataOutputStream
    {
        public readonly byte[] bytes;
        public int start, i;

        public BitDataOutputStream(byte[] bytes, int i = 0)
        {
            this.bytes = bytes;
            this.start = this.i = i;
        }

        public void WriteBoolean(bool val)
        {
            Bit.WriteUInt8(bytes, ref i, (byte)(val ? 1 : 0));
        }

        public void WriteByte(byte val)
        {
            Bit.WriteUInt8(bytes, ref i, val);
        }

        public void WriteInt32(int val)
        {
            Bit.WriteInt32(bytes, ref i, val);
        }

        public void WriteSingle(float val)
        {
            Bit.WriteSingle(bytes, ref i, val);
        }

        public void WriteString(string val)
        {
            Bit.WriteUInt16(bytes, ref i, (ushort)Bit.GetStringByteCount(val));
            Bit.WriteString(bytes, ref i, val);
        }
    }

    public sealed class BitDataInputStream : IDataInputStream
    {
        public readonly byte[] bytes;
        public int start, i;

        public BitDataInputStream(byte[] bytes, int i = 0)
        {
            this.bytes = bytes;
            this.start = this.i = i;
        }

        public bool ReadBoolean()
        {
            byte val = Bit.ReadUInt8(bytes, ref i);
            return val != 0;
        }

        public byte ReadByte()
        {
            return Bit.ReadUInt8(bytes, ref i);
        }

        public int ReadInt32()
        {
            return Bit.ReadInt32(bytes, ref i);
        }

        public float ReadSingle()
        {
            return Bit.ReadSingle(bytes, ref i);
        }

        public string ReadString()
        {
            int byteCount = Bit.ReadUInt16(bytes, ref i);
            return Bit.ReadString(bytes, ref i, byteCount);
        }
    }
}

namespace GameLogic.Core.Network.Initializer
{
    public sealed class NSInitializer : IMessageReceiver
    {
        private sealed class SerRequireVerifyMessage : Networkf.Message
        {
            public const int KType = 0;
            public SerRequireVerifyMessage() : base(KType) { }
        }

        private sealed class CltVerifyMessage : Networkf.Message
        {
            public const int KType = 1;

            public byte[] verificationCode;

            public CltVerifyMessage() : base(KType) { }

            public CltVerifyMessage(byte[] buf, ref int i) : base(KType)
            {
                int byteCount = Bit.ReadInt32(buf, ref i);
                verificationCode = new byte[byteCount];
                for (int j = 0; j < byteCount; ++j)
                {
                    verificationCode[j] = Bit.ReadUInt8(buf, ref i);
                }
            }

            public override void WriteTo(byte[] buf, ref int i)
            {
                Bit.WriteInt32(buf, ref i, verificationCode.Length);
                foreach (byte b in verificationCode)
                {
                    Bit.WriteUInt8(buf, ref i, b);
                }
            }
        }

        private sealed class SerVerifyResultMessage : Networkf.Message
        {
            public const int KType = 2;

            public bool result;

            public SerVerifyResultMessage() : base(KType) { }

            public SerVerifyResultMessage(byte[] buf, ref int i) : base(KType)
            {
                byte val = Bit.ReadUInt8(buf, ref i);
                result = val != 0;
            }

            public override void WriteTo(byte[] buf, ref int i)
            {
                Bit.WriteUInt8(buf, ref i, (byte)(result ? 1 : 0));
            }
        }
        
        private sealed class CltReadyMessage : Networkf.Message
        {
            public const int KType = 3;
            public CltReadyMessage() : base(KType) { }
        }
        
        private static Networkf.Message ParseInitMessage(byte[] buf, ref int i)
        {
            var type = Networkf.Message.ReadMessageType(buf, ref i);
            switch (type)
            {
                case SerRequireVerifyMessage.KType:
                    return new SerRequireVerifyMessage();
                case CltVerifyMessage.KType:
                    return new CltVerifyMessage(buf, ref i);
                case SerVerifyResultMessage.KType:
                    return new SerVerifyResultMessage(buf, ref i);
                case CltReadyMessage.KType:
                    return new CltReadyMessage();
                default:
                    throw new NotImplementedException();
            }
        }

        private readonly Networkf.NetworkService _service;
        private readonly object mtx = new object();
        private readonly int _retryCount;
        private readonly int _retryDelay;

        private bool _hasReceivedServerHello = false;
        private bool _hasReceivedServerResult = false;
        private bool _verificationResult = false;
        private bool _hasReceivedServerReady = false;

        private byte[] _verificationCode = null;
        private bool _hasReceivedClientReady = false;

        public NSInitializer(Networkf.NetworkService service, int retryCount = 10, int retryDelay = 1000)
        {
            _service = service;
            service.ParseMessage = ParseInitMessage;
            service.OnMessageReceived += OnRawMessageReceived;
            _retryCount = retryCount;
            _retryDelay = retryDelay;
        }
        
        private void ReleaseService()
        {
            _service.ParseMessage = null;
            _service.OnMessageReceived -= OnRawMessageReceived;
        }

        private void Log(string str)
        {
            Console.WriteLine(str);
        }

        public void OnRawMessageReceived(int id, Networkf.Message message)
        {
            lock (mtx)
            {
                switch (message.type)
                {
                    case SerRequireVerifyMessage.KType:
                        _hasReceivedServerHello = true;
                        break;
                    case CltVerifyMessage.KType:
                        {
                            var cltMsg = (CltVerifyMessage)message;
                            _verificationCode = cltMsg.verificationCode;
                        }
                        break;
                    case SerVerifyResultMessage.KType:
                        {
                            var serMsg = (SerVerifyResultMessage)message;
                            _verificationResult = serMsg.result;
                            _hasReceivedServerResult = true;
                        }
                        break;
                    case CltReadyMessage.KType:
                        _hasReceivedClientReady = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }
                Monitor.PulseAll(mtx);
            }
        }

        public void MessageReceived(Message message)
        {
            var serReadyMsg = message as ServerReadyMessage;
            if (serReadyMsg != null) _hasReceivedServerReady = true;
        }

        public bool ClientInit(byte[] verificationCode, NetworkfConnection applyTo)
        {
            Log("waiting server hello...");
            lock (mtx)
            {
                while (!_hasReceivedServerHello) Monitor.Wait(mtx);
            }
            Log("server hello received");
            var verifyMsg = new CltVerifyMessage();
            verifyMsg.verificationCode = verificationCode;
            _service.SendMessage(verifyMsg);
            Log("waiting verification result...");
            lock (mtx)
            {
                while (!_hasReceivedServerResult) Monitor.Wait(mtx);
            }
            Log("verification result received");
            if (_verificationResult)
            {
                ReleaseService();
                applyTo.ApplyService(_service);
                applyTo.AddMessageReceiver(ServerReadyMessage.MESSAGE_TYPE, this);
                _service.SendMessage(new CltReadyMessage());
                while (!_hasReceivedServerReady)
                {
                    Thread.Sleep(100);
                    applyTo.UpdateReceiver();
                }
                applyTo.RemoveMessageReceiver(ServerReadyMessage.MESSAGE_TYPE, this);
                return true;
            } else return false;
        }

        public byte[] ServerRequireClientVerify()
        {
            var serMsg = new SerRequireVerifyMessage();
            int tryCount = 0;
            Log("waiting client verify...");
            lock (mtx)
            {
                while (_verificationCode == null && tryCount < _retryCount)
                {
                    _service.SendMessage(serMsg);
                    ++tryCount;
                    Monitor.Wait(mtx, _retryDelay);
                }
            }
            if (_verificationCode == null)
            {
                Log("broken client");
                ReleaseService();
                _service.socket.Close();
                return null;
            }
            Log("client verification received");
            return _verificationCode;
        }

        public void ServerApplyConnection(NetworkfConnection applyTo)
        {
            var serMsg = new SerVerifyResultMessage();
            serMsg.result = applyTo != null;
            _service.SendMessage(serMsg);
            Log("verification result has sent");
            if (applyTo != null)
            {
                Log("waiting client ready...");
                lock (mtx)
                {
                    while (!_hasReceivedClientReady) Monitor.Wait(mtx);
                }
                Log("client ready received");
                ReleaseService();
                applyTo.ApplyService(_service);
                applyTo.SendMessage(new ServerReadyMessage());
            }
            else
            {
                ReleaseService();
                _service.socket.Close();
            }
        }
    }
}