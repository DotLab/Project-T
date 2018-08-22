using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Dynamic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.CharacterSystem;
using Message = Networkf.Message;
using NetworkHelper = Networkf.NetworkHelper;
using NetworkService = Networkf.NetworkService;
using BitHelper = Networkf.BitHelper;
using System.Threading;
using System.Linq;
using GameLogic.Core.Network.ServerMessages;

namespace TestConsoleApp
{
    class Program
    {
        sealed class SerRequireVerifyMessage : Message
        {
            public const int KType = 0;
            public SerRequireVerifyMessage() : base(KType) { }
        }

        sealed class CltHelloMessage : Message
        {
            public const int KType = 1;
            public CltHelloMessage() : base(KType) { }
        }

        sealed class CltVerifyMessage : Message
        {
            public const int KType = 2;

            public CltVerifyMessage() : base(KType) { }

            public CltVerifyMessage(byte[] buf, ref int i) : base(KType)
            {
                int byteCount = BitHelper.ReadInt32(buf, ref i);
                verificationCode = new byte[byteCount];
                for (int j = 0; j < byteCount; ++j)
                {
                    verificationCode[j] = BitHelper.ReadUInt8(buf, ref i);
                }
            }

            public byte[] verificationCode;

            public override void WriteTo(byte[] buf, ref int i)
            {
                BitHelper.WriteInt32(buf, ref i, verificationCode.Length);
                foreach (byte b in verificationCode)
                {
                    BitHelper.WriteUInt8(buf, ref i, b);
                }
            }
        }

        sealed class ClientHandler
        {
            private readonly NetworkService _service;
            private readonly object mtx = new object();
            private bool _hasReceivedClientHello = false;
            private byte[] _code = null;
            
            public ClientHandler(NetworkService service)
            {
                _service = service;
            }

            private void Log(string str)
            {
                Console.WriteLine("server {0,3}: " + str, _service.id);
            }

            public void OnMessageReceived(int id, Message message)
            {
                switch (message.type)
                {
                    case CltHelloMessage.KType:
                        _hasReceivedClientHello = true;
                        break;
                    case CltVerifyMessage.KType:
                        {
                            var cltMsg = (CltVerifyMessage)message;
                            _code = cltMsg.verificationCode;
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                lock (mtx)
                {
                    Monitor.Pulse(mtx);
                }
            }
            
            public byte[] RequireVerify()
            {
                _service.OnMessageReceived += OnMessageReceived;
                Log("waiting client hello...");
                lock (mtx)
                {
                    while (!_hasReceivedClientHello) Monitor.Wait(mtx);
                }
                Log("client hello received");
                _service.SendMessage(new SerRequireVerifyMessage());
                Log("waiting client verify...");
                lock (mtx)
                {
                    while (_code == null) Monitor.Wait(mtx);
                }
                Log("client verification received");
                _service.OnMessageReceived -= OnMessageReceived;
                return _code;
            }
        }

        static Message ParseInitMessage(byte[] buf, ref int i)
        {
            var type = Message.ReadMessageType(buf, ref i);
            switch (type)
            {
                case SerRequireVerifyMessage.KType:
                    return new SerRequireVerifyMessage();
                case CltHelloMessage.KType:
                    return new CltHelloMessage();
                case CltVerifyMessage.KType:
                    return new CltVerifyMessage(buf, ref i);
                default:
                    throw new NotImplementedException();
            }
        }

        static readonly byte[] dmVerificationCode = { 0x00, 0x10, 0x20, 0xAB };
        static readonly byte[] player1VerificationCode = { 0x00, 0x10, 0x20, 0x3B };
        static readonly byte[] player2VerificationCode = { 0x00, 0x10, 0x20, 0xC5 };

        static readonly NetworkfConnection dmConnection = new NetworkfConnection();
        static readonly NetworkfConnection player1Connection = new NetworkfConnection();
        static readonly NetworkfConnection player2Connection = new NetworkfConnection();
        
        static void Main(string[] args)
        {
            KeyCharacter[] player1characters = new KeyCharacter[1];
            player1characters[0] = new KeyCharacter("ddd", new CharacterView());
            KeyCharacter[] player2characters = new KeyCharacter[2];
            player2characters[0] = new KeyCharacter("aaa", new CharacterView());
            player2characters[1] = new KeyCharacter("eee", new CharacterView());

            Player[] players = new Player[2];
            players[0] = new Player("Player1", "Player1", player1Connection, 1, player1characters);
            players[1] = new Player("Player2", "Player2", player2Connection, 2, player2characters);
            
            DM dm = new DM("DM", "DM", dmConnection);

            NetworkHelper.StartServer(OnConnectionEstablished);

            MainLogic.Init(dm, players);

            while (!MainLogic.GameOver)
            {
                MainLogic.Update();
            }

            MainLogic.Cleanup();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void OnConnectionEstablished(NetworkService service)
        {
            service.ParseMessage = ParseInitMessage;
            var clientHandler = new ClientHandler(service);
            byte[] code = clientHandler.RequireVerify();
            if (code.SequenceEqual(dmVerificationCode))
            {
                service.ParseMessage = NetworkfMessage.ParseMessage;
                dmConnection.ApplyService(service);
                dmConnection.SendMessage(new ServerReadyMessage());
            }
            else if (code.SequenceEqual(player1VerificationCode))
            {
                service.ParseMessage = NetworkfMessage.ParseMessage;
                player1Connection.ApplyService(service);
                player1Connection.SendMessage(new ServerReadyMessage());
            }
            else if (code.SequenceEqual(player2VerificationCode))
            {
                service.ParseMessage = NetworkfMessage.ParseMessage;
                player2Connection.ApplyService(service);
                player2Connection.SendMessage(new ServerReadyMessage());
            }
        }

        
    }
}
