﻿using GameLogic.Core.Network;
using GameLogic.Core.Network.ClientMessages;
using GameLogic.Core.Network.ServerMessages;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetworkHelper = Networkf.NetworkHelper;
using System.Threading;
using GameLogic.Core.Network.Initializer;

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
        public static NetworkfConnection connection = new NetworkfConnection();
        public static List<CharacterPropertyInfo> skillTypes = new List<CharacterPropertyInfo>();
        
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
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
            
            var service = NetworkHelper.StartClient("127.0.0.1");
            var initializer = new NSInitializer(service);
            if (!initializer.ClientInit(verificationCode, connection))
            {
                MessageBox.Show("登陆失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                service.socket.Close();
                return;
            }
            
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
