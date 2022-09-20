using System;

namespace ConsoleGame
{
    // Вынес этот функционал в отдельный класс, потому что думал, что понадобятся ещё реализации
    internal abstract class Border
    {
        public abstract int Top { get; }
        public abstract int Bottom { get; }
        public abstract int Left { get; }
        public abstract int Right { get; }

        public bool IsInBorders(int left, int top)
        {
            return (
                left >= Left &&
                left <= Right &&
                top >= Top &&
                top <= Bottom
            );
        }
    }

    // Класс, хранящий параметры консоли
    internal class Map : Border
    {
        public int ConsoleWidth { get; }
        public int ConsoleHeight { get; }

        private int HorShift { get; }
        private int VerShift { get; }
        private int MapWidth { get; }
        private int MapHeight { get; }

        public override int Top => VerShift + 1;
        public override int Bottom => VerShift + MapHeight - 2;
        public override int Left => HorShift + 1;
        public override int Right => HorShift + MapWidth - 2;


        private ConsoleColor _backgroundColor = Config.Get().BackgroundColor;
        private ConsoleColor _foregroundColor = Config.Get().ForegroundColor;

        public Map(int consoleWidth, int consoleHeight)
        {
            HorShift = consoleWidth / 4;
            MapWidth = HorShift * 2;
            ConsoleWidth = 2 * HorShift + MapWidth;

            VerShift = consoleHeight / 4;
            MapHeight = VerShift * 2;
            ConsoleHeight = 2 * VerShift + MapHeight;
        }

        public void Print()
        {
            Console.Clear();
            Console.BackgroundColor = _backgroundColor;
            Console.ForegroundColor = _foregroundColor;
            for (int i = 0; i < VerShift; i++)
            {
                Console.WriteLine();
            }

            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < HorShift + MapWidth; j++)
                {
                    char sym;
                    if (j < HorShift)
                    {
                        sym = ' ';
                    }
                    else if (i == 0 || i == MapHeight - 1)
                    {
                        sym = '-';
                    }
                    else if (j == HorShift ||
                             j == HorShift + MapWidth - 1)
                    {
                        sym = '|';
                    }
                    else
                    {
                        sym = ' ';
                    }

                    Console.Write(sym);
                }

                Console.WriteLine();
            }
        }


        public (int, int) Center => (MapWidth, MapHeight);

        public (int, int) RandomCoordinates()
        {
            Random rand = new Random();
            return (rand.Next(Left, Right + 1), rand.Next(Top, Bottom + 1));
        }
    }
}