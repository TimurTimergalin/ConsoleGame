using System;

namespace ConsoleGame
{
    // Реализует глобальный объект, хранящий информацию о конфигурации игры
    internal class Config
    {
        // Максимальный FPS
        public int FPS { get; }

        // Цвет заднего фона
        public ConsoleColor BackgroundColor { get; }

        // Цвет Текста, границ и пуль
        public ConsoleColor ForegroundColor { get; }

        // Символ игрока
        public string PlayerChar { get; }

        // Цвет символа игрока
        public ConsoleColor PlayerColor { get; }

        // Символ еды
        public string FoodChar { get; }

        // Цвет символа еды
        public ConsoleColor FoodColor { get; }

        // Минимальный промежуток времени между нажатиями одной и той же клавиши
        public double MinDelay { get; }

        // Минимальное расстояние (декартовое) от игрока, на котором может появится еда
        public int MinDistance { get; }

        // Символ для пули
        public string BulletCharacter { get; }

        // Цвет предупреждения о появлении пулм
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