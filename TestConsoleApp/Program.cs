using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Dynamic;
using System.Text;
using GameLogic.Core;
using GameLogic.Core.Network;
using GameLogic.CharacterSystem;

namespace TestConsoleApp
{
    class Program
    {
        static Connection dmConnection = null;
        static Connection player1Connection = null;
        static Connection player2Connection = null;

        static void Main(string[] args)
        {
            KeyCharacter[] player1characters = new KeyCharacter[1];
            KeyCharacter[] player2characters = new KeyCharacter[2];

            // ...

            Player[] players = new Player[2];
            players[0] = new Player("Player1", "Player1", player1Connection, 1, player1characters);
            players[1] = new Player("Player2", "Player2", player2Connection, 2, player2characters);
            
            DM dm = new DM("DM", "DM", dmConnection);
            
            MainLogic.Init(dm, players);

            while (!MainLogic.GameOver)
            {
                MainLogic.Update();
            }

            MainLogic.Cleanup();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
