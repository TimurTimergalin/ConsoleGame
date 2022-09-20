using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    // Класс, отвечающий за очищение символов, оставшиеся на экране с предыдущего кадра
    internal class ConsoleCleaner
    {
        private readonly HashSet<Tuple<int, int>> _toClear = new HashSet<Tuple<int, int>>();

        public void AddCoords(int left, int top)
        {
            _toClear.Add(Tuple.Create(left, top));
        }

        public void Reset()
        {
            _toClear.Clear();
        }

        public void Clear()
        {
            foreach (Tuple<int, int> coord in _toClear)
            {
                int left, top;
                (left, top) = coord;

                Console.SetCursorPosition(left, top);
                Console.Write(' ');
            }

            _toClear.Clear();
        }
    }
}