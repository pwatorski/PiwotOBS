using System.Diagnostics;

namespace PiwotOBS.Structure.Animations
{
    public class Animator
    {
        public List<Animation> Animations = new List<Animation>();
        public float FrameRate { get; protected set; }
        public int FrameLength { get; protected set; }
        protected Stopwatch stopwatch = new Stopwatch();
        public float Time { get => stopwatch.ElapsedMilliseconds / 1000f; }
        protected Thread AnimationThread;
        protected bool StopPending;
        public Animator(float framerate = 20)
        {
            FrameRate = framerate;
            FrameLength = (int)(1000 / framerate);
            StopPending = false;
        }

        public void Run()
        {
            StopPending = false;
            if (AnimationThread != null)
            {
                return;
            }
            AnimationThread = new Thread(AnimatingLoop);
            stopwatch.Restart();
            AnimationThread.Start();

        }
        public void Stop()
        {
            StopPending = true;
            AnimationThread.Join();
        }
        protected void AnimatingLoop()
        {
            Stopwatch stopwatch = new Stopwatch();

            while (!StopPending)
            {
                Thread.Sleep(Math.Max(0, FrameLength - (int)stopwatch.ElapsedMilliseconds));
                stopwatch.Restart();
                Step();
            }
        }

        protected void Step()
        {
            var stepTime = Time;
            foreach (var animation in Animations)
            {
                animation.Step(Time);
            }
        }

        public void RegisterAnimation(Animation animation)
        {
            if (!Animations.Contains(animation)) Animations.Add(animation);
        }

        public void UnRegisterAnimation(Animation animation)
        {
            if (Animations.Contains(animation)) Animations.Remove(animation);
        }

    }
}
