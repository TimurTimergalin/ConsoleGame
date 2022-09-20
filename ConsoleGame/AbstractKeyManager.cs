using System;
using System.Collections.Generic;

namespace ConsoleGame
{
    internal abstract class AbstractKeyManager
    {
        private readonly Dictionary<ConsoleKey, double> _durations;
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

        protected abstract FPSManager.LoopState OnPressKey(ConsoleKey key);

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

