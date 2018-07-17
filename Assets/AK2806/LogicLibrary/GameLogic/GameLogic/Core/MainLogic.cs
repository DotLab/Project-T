using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core
{
    public class MainLogic
    {
        private static bool _gameOver = true;

        public static void Init()
        {

            _gameOver = false;
        }

        public static bool IsGameover()
        {
            return _gameOver;
        }

        public static void Loop()
        {

        }
    }
}
