using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandyBubble.GameObjects;

namespace CandyBubble
{
    class Singleton
    {

        public const int WIDTH = 1080;
        public const int HEIGHT = 720;

        public const int INVADERHORDEWIDTH = 576;
        public const int INVADERHORDEHEIGHT = 576;

        public const int CANNONAREAWIDTH = 576;
        public const int CANNONAREAHEIGHT = 216;

        public const int SCOREAREAWIDTH = 360;
        public const int SCOREAREAHEIGHT = 720;

        public const int HITBOX = 72;

        public const int INITPOSITION_Y = -936; //-936

        public const int INITROW = 16;
        public Dictionary<int, Candy> candyMap;
        public Queue<int> randomColor = new Queue<int>();

        

        public int Score;
        public int CandyLeft;
        public int Level;
       


        public enum GameState
        {
            GameMain,
            GameStart,
            GamePlaying,
            GameEnded,
        }
        public GameState CurrentGameState;

        public enum GameResult
        {
            Win,
            Lose
        }
        public GameResult CurrentGameResult;

        public KeyboardState PreviousKey, CurrentKey;
        public MouseState CurrentMouse;

        private Singleton() { }
        private static Singleton instance;

        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Singleton();
                }
                return instance;
            }
        }
    }
}
