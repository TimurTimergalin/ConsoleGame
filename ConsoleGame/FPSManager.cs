using System.Diagnostics;
using System.Threading;

namespace ConsoleGame
{
    // Описывает, каким образом запускается основной игровой цикл
    // Ограничивает FPS игры
    public abstract class FPSManager
    {
        public enum LoopState
        {
            Continue = 1,
            Stop = 2
        }

        public int FPS => (int)(1000.0 / _frameDuration);
        private readonly double _nominalFrameDuration;

        private double _frameDuration;

        public double FrameDuration
        {
            get => _frameDuration / 1000.0;
            private set => _frameDuration = value;
        }

        public FPSManager(int fps)
        {
            _nominalFrameDuration = 1000.0 / fps;
        }

        // Тело основного цикла игры
        public abstract LoopState Action();

        // Код, запускающийся перед началом цикла
        public virtual void Start()
        {
        }

        // Код, запускающийся сразу после окончания цикла
        public virtual void End()
        {
        }

        // Запуск цикла
        public void Run()
        {
            Start();

            try
            {
                LoopState state;

                do
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    state = Action();
                    sw.Stop();

                    double mls = sw.Elapsed.TotalMilliseconds;

                    if (mls < _nominalFrameDuration)
                    {
                        Thread.Sleep((int)(_nominalFrameDuration - mls));
                        FrameDuration = _nominalFrameDuration;
                    }
                    else
                    {
                        FrameDuration = mls;
                    }
                } while (state != LoopState.Stop);
            }
            finally
            {
                End();
            }
        }
    }
}