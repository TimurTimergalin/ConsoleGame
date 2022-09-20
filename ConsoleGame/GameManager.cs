using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    public class GameManager : FPSManager
    {
        static void Main(string[] args)
        {
            ConsoleHelper.SetCurrentFont("Consolas", 25);
            Console.SetWindowSize(60, 10);
            Console.BackgroundColor = Config.Get().BackgroundColor;
            Console.ForegroundColor = Config.Get().ForegroundColor;
            Console.Clear();

            GameManager inst;

            while (true)
            {
                Console.WriteLine("Введите размеры окна");
                if (!int.TryParse(Console.ReadLine(), out int left) || left <= 0 ||
                    !int.TryParse(Console.ReadLine(), out int top) || top <= 0)
                {
                    Console.WriteLine("некорректные размеры окна");
                    continue;
                }

                try
                {
                    inst = new GameManager(Config.Get().FPS, left, top);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.WriteLine("Слишком большой размер");
                    continue;
                }

                break;
            }

            inst.Run();
        }

        private class KeyManager : AbstractKeyManager
        {
            private GameManager _parent;

            public KeyManager(GameManager parent, double minDelay) : base(minDelay)
            {
                _parent = parent;
            }

            // Метод, отвечающий за обработку "аналогичных клавишь
            public ConsoleKey ActualKey(ConsoleKey key)
            {
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        return ConsoleKey.W;
                    case ConsoleKey.LeftArrow:
                        return ConsoleKey.A;
                    case ConsoleKey.DownArrow:
                        return ConsoleKey.S;
                    case ConsoleKey.RightArrow:
                        return ConsoleKey.D;
                    default:
                        return key;
                }
            }

            protected override LoopState OnPressKey(ConsoleKey key)
            {
                int dx = 0, dy = 0;

                switch (key)
                {
                    case ConsoleKey.Escape:
                        return LoopState.Stop;
                    case ConsoleKey.W:
                        dy = -1;
                        break;
                    case ConsoleKey.A:
                        dx = -1;
                        break;
                    case ConsoleKey.S:
                        dy = 1;
                        break;
                    case ConsoleKey.D:
                        dx = 1;
                        break;
                }

                int finalLeft = _parent.Player.Left + dx;
                int finalTop = _parent.Player.Top + dy;

                if (finalLeft > _parent.GameMap.Right)
                {
                    finalLeft = _parent.GameMap.Left;
                }
                else if (finalLeft < _parent.GameMap.Left)
                {
                    finalLeft = _parent.GameMap.Right;
                }

                if (finalTop > _parent.GameMap.Bottom)
                {
                    finalTop = _parent.GameMap.Top;
                }
                else if (finalTop < _parent.GameMap.Top)
                {
                    finalTop = _parent.GameMap.Bottom;
                }

                if (_parent.GameMap.IsInBorders(finalLeft, finalTop))
                {
                    _parent.Cleaner.AddCoords(_parent.Player.Left, _parent.Player.Top);
                    _parent.Player.Left = finalLeft;
                    _parent.Player.Top = finalTop;
                }

                return LoopState.Continue;
            }
        }

        private KeyManager keyManager { get; }
        private StaticObject Player { get; }
        private Map GameMap { get; }
        private ConsoleCleaner Cleaner { get; }
        private StaticObject Food { get; }
        private Counter counter { get; }
        private BulletManager bulletManager { get; }


        public GameManager(int fps, int consoleWidth, int consoleHeight) : base(fps)
        {
            keyManager = new KeyManager(this, Config.Get().MinDelay);

            GameMap = new Map(consoleWidth, consoleHeight);

            Console.SetWindowSize(GameMap.ConsoleWidth, GameMap.ConsoleHeight);
            Console.CursorVisible = false;

            int playerLeft, playerTop;
            (playerLeft, playerTop) = GameMap.Center;

            Player = new StaticObject(playerLeft, playerTop, Config.Get().PlayerChar, Config.Get().PlayerColor);

            Cleaner = new ConsoleCleaner();

            Food = new StaticObject(0, 0, Config.Get().FoodChar, Config.Get().FoodColor);
            ChangeFoodLocation();

            counter = new Counter(2, 1, Config.Get().ForegroundColor);

            bulletManager = new BulletManager
            (
                GameMap,
                Player,
                Cleaner,
                counter,
                Config.Get().BulletCharacter,
                Config.Get().ForegroundColor,
                Config.Get().BulletSummonerColor
            );
        }

        private void ChangeFoodLocation()
        {
            int left, top;

            do
            {
                (left, top) = GameMap.RandomCoordinates();
            } while (Math.Abs(left - Player.Left) + Math.Abs(top - Player.Top) < Config.Get().MinDistance);

            Food.Left = left;
            Food.Top = top;
        }

        public override void Start()
        {
            // Console.Clear();
            // GameMap.Print();
            Console.Clear();
            GameMap.Print();
        }

        // Проверка состояния игрока, еды и пуль
        // Отрисовка текущего кадра
        private LoopState UpdateFrame()
        {
            LoopState res = bulletManager.Update(FrameDuration);
            if (res == LoopState.Stop)
            {
                return res;
            }

            Cleaner.Clear();

            if
            (
                Player.Left == Food.Left &&
                Player.Top == Food.Top
            )
            {
                counter.OneUp();
                ChangeFoodLocation();
            }

            Player.Print();
            Food.Print();
            counter.Print();
            bulletManager.Print();
            return LoopState.Continue;
        }


        public override LoopState Action()
        {
            keyManager.Update(FrameDuration);

            HashSet<ConsoleKey> pressed = new HashSet<ConsoleKey>();

            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo cki = Console.ReadKey(true);


                pressed.Add(keyManager.ActualKey(cki.Key));
            }

            foreach (ConsoleKey key in pressed)
            {
                LoopState res = keyManager.HandleKey(key);
                if (res == LoopState.Stop)
                {
                    return LoopState.Stop;
                }
            }

            return UpdateFrame();
        }

        public override void End()
        {
            Console.Clear();
            Console.ForegroundColor = Config.Get().ForegroundColor;
            Console.WriteLine($"Ваш счёт: {counter.Points}");
            Console.WriteLine("Нажмите 'Escape', чтобы выйти,\nили 'R', чтобы начать заново");

            ConsoleKeyInfo cki;

            do
            {
                cki = Console.ReadKey(true);
            } while (cki.Key != ConsoleKey.R && cki.Key != ConsoleKey.Escape);

            if (cki.Key != ConsoleKey.Escape)
            {
                keyManager.Reset();

                Player.Left = GameMap.Center.Item1;
                Player.Top = GameMap.Center.Item2;

                Cleaner.Reset();

                ChangeFoodLocation();

                counter.Reset();

                bulletManager.Reset();
                Run();
            }
        }
    }
}