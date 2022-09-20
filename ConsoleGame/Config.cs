using System;

namespace ConsoleGame
{
    internal class Config
    {
        public int FPS { get; }

        public ConsoleColor BackgroundColor { get; }
        public ConsoleColor ForegroundColor { get; }

        public string PlayerChar { get; }
        public ConsoleColor PlayerColor { get; }

        public string FoodChar { get; }
        public ConsoleColor FoodColor { get; }
        public double MinDelay { get; }
        public int MinDistance { get; }
        public string BulletCharacter { get; }
        public ConsoleColor BulletSummonerColor { get; }

        private Config
        (
            int fps,
            ConsoleColor backgroundColor,
            ConsoleColor foregroundColor,
            string playerChar,
            ConsoleColor playerColor,
            string foodChar,
            ConsoleColor foodColor,
            double minDelay,
            int minDistance,
            string bulletCharacter,
            ConsoleColor bulletSummonerColor
        )
        {
            FPS = fps;
            BackgroundColor = backgroundColor;
            ForegroundColor = foregroundColor;
            PlayerChar = playerChar;
            PlayerColor = playerColor;
            FoodChar = foodChar;
            FoodColor = foodColor;
            MinDelay = minDelay;
            MinDistance = minDistance;
            BulletCharacter = bulletCharacter;
            BulletSummonerColor = bulletSummonerColor;
        }

        private static readonly Config Inst = new Config
        (
            20,
            ConsoleColor.Green,
            ConsoleColor.Black,
            "O",
            ConsoleColor.Red,
            "+",
            ConsoleColor.DarkBlue,
            0.05,
            5,
            "#",
            ConsoleColor.DarkRed
        );

        public static Config Get()
        {
            return Inst;
        }
    }
}