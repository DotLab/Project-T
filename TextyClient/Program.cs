using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetworkHelper = Networkf.NetworkHelper;
using NetworkService = Networkf.NetworkService;
using Message = Networkf.Message;
using BitHelper = Networkf.BitHelper;
using System.Threading;

namespace TextyClient
{
    public struct CharacterPropertyInfo
    {
        public string ownerID;
        public string propertyID;
        public string name;
        public string description;
        public string extraMessage;

        public override string ToString()
        {
            return name + " " + extraMessage;
        }
    }

    static class Program
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

        public static NetworkfConnection connection = new NetworkfConnection();
        public static List<CharacterPropertyInfo> skillTypes = new List<CharacterPropertyInfo>();

        static readonly object mtx = new object();
        static bool hasReceivedServerHello = false;

        static void OnMessageReceived(int id, Message message)
        {
            switch (message.type)
            {
                case SerRequireVerifyMessage.KType:
                    hasReceivedServerHello = true;
                    break;
                default:
                    throw new NotImplementedException();
            }

            lock (mtx)
            {
                Monitor.Pulse(mtx);
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

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // connection initialization

            var service = NetworkHelper.StartClient("127.0.0.1");
            service.ParseMessage = ParseInitMessage;
            service.OnMessageReceived += OnMessageReceived;
            byte[] verificationCode = { };
            var dialogResult = MessageBox.Show("DM(Yes) or Player(No)?", "Choose", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                verificationCode = new byte[] { 0x00, 0x10, 0x20, 0xAB };
            }
            else
            {
                var dialogResult2 = MessageBox.Show("Player1(Yes) or Player2(No)?", "Choose", MessageBoxButtons.YesNo);
                if (dialogResult2 == DialogResult.Yes)
                {
                    verificationCode = new byte[] { 0x00, 0x10, 0x20, 0x3B };
                }
                else
                {
                    verificationCode = new byte[] { 0x00, 0x10, 0x20, 0xC5 };
                }
            }
            service.SendMessage(new CltHelloMessage());
            lock (mtx)
            {
                while (!hasReceivedServerHello) Monitor.Wait(mtx);
            }
            var verifyMessage = new CltVerifyMessage();
            verifyMessage.verificationCode = verificationCode;
            service.SendMessage(verifyMessage);
            service.ParseMessage = NetworkfMessage.ParseMessage;
            service.OnMessageReceived -= OnMessageReceived;
            connection.ApplyService(service);
            
            var getAllSkillTypesRequest = new GetSkillTypeListMessage();
            connection.Request(getAllSkillTypesRequest, result => {
                var resp = result as SkillTypeListDataMessage;
                if (resp != null)
                {
                    foreach (var skillType in resp.skillTypes)
                    {
                        skillTypes.Add(new CharacterPropertyInfo() {
                            name = skillType.name,
                            propertyID = skillType.id
                        });
                    }
                }
            });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BattleSceneForm());
        }
        
    }
}
