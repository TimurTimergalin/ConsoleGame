using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    // Описывает базовое поведение программы при нажатии клавиш
    internal abstract class AbstractKeyManager
    {
        // Хранит, сколько времени прошло с момента последнего нажатия клавиши
        private readonly Dictionary<ConsoleKey, double> _durations;

        // Минимальный период времени, который должен пройти, чтобы нажатие клавиши было обработано
        private readonly double _minDelay;

        protected AbstractKeyManager(double minDelay)
        {
            _minDelay = minDelay;
            _durations = new Dictionary<ConsoleKey, double>();
        }

        public void Reset()
        {
            _durations.Clear();
        }

        public void Update(double fDuration)
        {
            foreach (KeyValuePair<ConsoleKey, double> pressedKey in _durations)
            {
                _durations[pressedKey.Key] = pressedKey.Value + fDuration;
            }
        }

        // Проверяет, нужно ли обработать нажатие клавиши
        private bool IsPressed(ConsoleKey key)
        {
            if (!_durations.ContainsKey(key))
            {
                _durations[key] = 0.0;
                return true;
            }

            bool res = _durations[key] > _minDelay;
            _durations[key] = 0.0;
            return res;
        }

        // В подклассе будет реализованы конкретные правила обработки клавиш
        protected abstract FPSManager.LoopState OnPressKey(ConsoleKey key);

        // Метод, обрабатывающий нажатие клавиши с учётом того, когда последний раз была нажата эта клавиша
        public FPSManager.LoopState HandleKey(ConsoleKey key)
        {
            if (IsPressed(key))
            {
                FPSManager.LoopState res = OnPressKey(key);
                if (res == FPSManager.LoopState.Stop)
                {
                    return res;
                }
            }

            return FPSManager.LoopState.Continue;
        }
    }
}