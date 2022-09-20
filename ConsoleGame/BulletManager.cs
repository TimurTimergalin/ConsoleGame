using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    internal enum Directions
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    internal static class DirectionExtension
    {
        public static (int, int) GetDirection(this Directions d)
        {
            switch (d)
            {
                case Directions.Up:
                    return (0, -1);
                case Directions.Down:
                    return (0, 1);
                case Directions.Left:
                    return (-1, 0);
                default: // Directions.Right
                    return (1, 0);
            }
        }
    }

    // Класс, отвечающий за поведение пуль
    internal class BulletManager
    {
        // Класс, отвечающий за расчёт скорости пуль и частоты их появления
        private class DifficultyManager
        {
            public double SpawnPeriod => 1.0 / (0.01 * Math.Pow(_counter.Points, 2) + 1);
            public double BulletSpeed => 0.4 * _counter.Points + 6;
            public double ShootDelay => 2 / 6.0;
            public double RowsToColumnsRatio => 2.0;

            private readonly Counter _counter;

            public DifficultyManager(Counter counter)
            {
                _counter = counter;
            }
        }

        // Предупреждение (красная решетка), появляющееся перед появление пули
        // Время появление пули описано здесь, не в BulletManager
        private class BulletSummoner : StaticObject
        {
            private Directions Direction { get; }
            private double _counter = 0.0;
            private double Delay { get; }
            private BulletManager _parent;

            public bool IsAlive { get; private set; }

            public BulletSummoner
            (
                int left,
                int top,
                Directions direction,
                double delay,
                BulletManager parent,
                string character,
                ConsoleColor color
            ) : base(left, top, character, color)
            {
                Direction = direction;
                Delay = delay;
                _parent = parent;
                IsAlive = true;
            }

            public void Kill()
            {
                IsAlive = false;
            }

            // Обновление состояния:
            // После прохождения достаточного количества времени BulletSummoner "умирает"
            // и появляется пуля
            public void Update(double frameDuration)
            {
                _counter += frameDuration;
                if (_counter >= Delay)
                {
                    int dx, dy;
                    (dx, dy) = Direction.GetDirection();

                    _parent.SpawnBullet
                    (
                        Left + 2 * dx,
                        Top + 2 * dy,
                        dx * _parent.difficultyManager.BulletSpeed *
                        _parent.difficultyManager.RowsToColumnsRatio,
                        dy * _parent.difficultyManager.BulletSpeed
                    );
                    Kill();
                }
            }
        }

        private Map GameMap { get; }
        private StaticObject Player { get; }
        private ConsoleCleaner Cleaner { get; }
        private Counter counter { get; }
        private HashSet<Bullet> Bullets { get; }
        private HashSet<BulletSummoner> BulletSummoners { get; }
        private DifficultyManager difficultyManager { get; }

        private string BulletCharacter { get; }
        private ConsoleColor BulletColor { get; }
        private ConsoleColor BulletSummonerColor { get; }
        private double _counter = 0.0;

        public BulletManager
        (
            Map gameMap,
            StaticObject player,
            ConsoleCleaner cleaner,
            Counter _counter,
            string bulletCharacter,
            ConsoleColor bulletColor,
            ConsoleColor bulletSummonerColor
        )
        {
            GameMap = gameMap;
            Player = player;
            Cleaner = cleaner;
            counter = _counter;
            Bullets = new HashSet<Bullet>();
            BulletSummoners = new HashSet<BulletSummoner>();
            difficultyManager = new DifficultyManager(counter);
            BulletCharacter = bulletCharacter;
            BulletColor = bulletColor;
            BulletSummonerColor = bulletSummonerColor;
        }

        public void Reset()
        {
            Bullets.Clear();
            BulletSummoners.Clear();
        }

        private void SpawnBullet(int left, int top, double horSpeed, double verSpeed)
        {
            Bullets.Add
            (
                new Bullet(left,
                    top,
                    horSpeed,
                    verSpeed,
                    BulletCharacter,
                    BulletColor,
                    Cleaner)
            );
        }

        // Обновление состояния менеджера:
        // Обновляются состояния всех пуль и BulletMeneger-ов;
        // Удаляются все "мёртвые" пули и BulletMeneger-ы;
        // Проверяется столкновение с игроком;
        public FPSManager.LoopState Update(double frameDuration)
        {
            HashSet<BulletSummoner> BStoRemove = new HashSet<BulletSummoner>();

            foreach (BulletSummoner bs in BulletSummoners)
            {
                bs.Update(frameDuration);
                if (!bs.IsAlive)
                {
                    Cleaner.AddCoords(bs.Left, bs.Top);
                    BStoRemove.Add(bs);
                }
            }

            BulletSummoners.RemoveWhere(bs => BStoRemove.Contains(bs));

            bool hitPlayer = false;

            HashSet<Bullet> BtoRemove = new HashSet<Bullet>();

            foreach (Bullet b in Bullets)
            {
                b.Update(frameDuration);

                if (!GameMap.IsInBorders(b.Left, b.Top))
                {
                    b.Kill();
                }

                if
                (
                    b.Left == Player.Left &&
                    b.Top == Player.Top
                )
                {
                    hitPlayer = true;
                }

                if (!b.IsAlive)
                {
                    BtoRemove.Add(b);
                }
            }

            Bullets.RemoveWhere(b => BtoRemove.Contains(b));

            if (hitPlayer)
            {
                return FPSManager.LoopState.Stop;
            }

            _counter += frameDuration;

            if (_counter >= difficultyManager.SpawnPeriod)
            {
                _counter = 0.0;
                CreateBulletSummoner();
            }

            return FPSManager.LoopState.Continue;
        }

        private void CreateBulletSummoner()
        {
            HashSet<(int, int)> reserved = new HashSet<(int, int)>();

            foreach (BulletSummoner bs in BulletSummoners)
            {
                reserved.Add((bs.Left, bs.Top));
            }

            int left, top;
            Directions direction;
            Random rand = new Random();

            int iterCounter = 0;
            do
            {
                direction = (Directions)rand.Next(1, 5);

                switch (direction)
                {
                    case Directions.Up:
                        top = GameMap.Bottom + 2;
                        left = rand.Next(GameMap.Left, GameMap.Right + 1);
                        break;
                    case Directions.Down:
                        top = GameMap.Top - 2;
                        left = rand.Next(GameMap.Left, GameMap.Right + 1);
                        break;
                    case Directions.Left:
                        left = GameMap.Right + 2;
                        top = rand.Next(GameMap.Top, GameMap.Bottom + 1);
                        break;
                    default: // Directions.Right
                        left = GameMap.Left - 2;
                        top = rand.Next(GameMap.Top, GameMap.Bottom + 1);
                        break;
                }

                iterCounter++;
                if (iterCounter > 20)
                {
                    break;
                }
            } while (reserved.Contains((left, top)));

            BulletSummoners.Add
            (
                new BulletSummoner
                (
                    left,
                    top,
                    direction,
                    difficultyManager.ShootDelay,
                    this,
                    BulletCharacter,
                    BulletSummonerColor
                )
            );
        }

        public void Print()
        {
            foreach (Bullet b in Bullets)
            {
                b.Print();
            }

            foreach (BulletSummoner bs in BulletSummoners)
            {
                bs.Print();
            }
        }
    }
}