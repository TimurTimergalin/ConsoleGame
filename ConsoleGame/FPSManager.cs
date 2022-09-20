using System.Diagnostics;
using System.Threading;

namespace ConsoleGame
{
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

        public abstract LoopState Action();
        public virtual void Start() {}
        public virtual void End() {}

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