using System;

namespace ConsoleGame
{
    // Базовый класс для всех объектов на игровом поле
    internal abstract class GameObject
    {
        public abstract string Character { get; }
        public abstract ConsoleColor Color { get; }
        public abstract int Left { get; set; }
        public abstract int Top { get; set; }

        public virtual void Print()
        {
            Console.SetCursorPosition(Left, Top);
            Console.ForegroundColor = Color;
            Console.Write(Character);
        }
    }

    // Класс, описывающий поведение объектов, не двигующихся сами по себе
    // К таким относится: игрок, еда, предупреждение о появлении пули, счётчик
    internal class StaticObject : GameObject
    {
        public override string Character { get; }
        public override ConsoleColor Color { get; }
        public override int Left { get; set; }
        public override int Top { get; set; }

        public StaticObject(int startLeft, int startTop, string character, ConsoleColor color)
        {
            Character = character;
            Color = color;
            Left = startLeft;
            Top = startTop;
        }
    }

    // Класс, описывающий поведение счётчика
    internal class Counter : StaticObject
    {
        public int Points { get; private set; }

        public override string Character => $"{Points}";

        public Counter(int startLeft, int startTop, ConsoleColor color) : base(startLeft, startTop, "0", color)
        {
            Points = 0;
        }

        public void Reset()
        {
            Points = 0;
        }

        public void OneUp()
        {
            Points++;
        }
    }

    // Класс, описывающий поведение пули
    internal class Bullet : GameObject
    {
        public override string Character { get; }
        public override ConsoleColor Color { get; }


        private double _left;
        private double _top;

        public override int Left
        {
            get => (int)_left;
            set => _left = value;
        }

        public override int Top
        {
            get => (int)_top;
            set => _top = value;
        }

        private double _horSpeed;
        private double _verSpeed;

        private ConsoleCleaner Cleaner { get; }

        public bool IsAlive { get; private set; }


        public Bullet
        (
            int startLeft,
            int startTop,
            double horSpeed,
            double verSpeed,
            string character,
            ConsoleColor color,
            ConsoleCleaner cleaner
        )
        {
            Left = startLeft;
            Top = startTop;
            _horSpeed = horSpeed;
            _verSpeed = verSpeed;
            if (horSpeed < 0)
            {
                _left += 0.99;
            }

            if (verSpeed < 0)
            {
                _top += 0.99;
            }

            Character = character;
            Color = color;
            IsAlive = true;
            Cleaner = cleaner;
        }

        public void Update(double frameDuration)
        {
            int fps = (int)(1.0 / frameDuration);

            double newTop = _top + _verSpeed / fps;
            double newLeft = _left + _horSpeed / fps;

            if
            (
                (int)newTop != Top ||
                (int)newLeft != Left
            )
            {
                Cleaner.AddCoords(Left, Top);
            }

            _top = newTop;
            _left = newLeft;
        }

        public void Kill()
        {
            IsAlive = false;
        }

        public override void Print()
        {
            if (IsAlive)
            {
                base.Print();
            }
        }
    }
}