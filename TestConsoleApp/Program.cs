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
using GameLogic.Core.Network.Initializer;

namespace TestConsoleApp
{
    class Program
    {
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
            var initializer = new NSInitializer(service);
            byte[] code = initializer.ServerRequireClientVerify();
            if (code != null)
            {
                if (code.SequenceEqual(dmVerificationCode))
                {
                    initializer.ServerApplyConnection(dmConnection);
                }
                else if (code.SequenceEqual(player1VerificationCode))
                {
                    initializer.ServerApplyConnection(player1Connection);
                }
                else if (code.SequenceEqual(player2VerificationCode))
                {
                    initializer.ServerApplyConnection(player2Connection);
                }
                else
                {
                    initializer.ServerApplyConnection(null);
                }
            }
        }

        
    }
}
