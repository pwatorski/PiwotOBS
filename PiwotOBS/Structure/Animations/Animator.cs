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
        protected List<Tuple<FrameAnimation, float>> OneOffAnimations = new List<Tuple<FrameAnimation, float>>();
        public Animator(float framerate = 20)
        {
            FrameRate = framerate;
            FrameLength = (int)(1000 / framerate);
            StopPending = false;
        }

        public void Reset()
        {
            stopwatch.Reset();
            Step();
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
            if (AnimationThread != null)
            {
                StopPending = true;
                AnimationThread.Join();
                AnimationThread = null;
            }
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
            //if (OneOffAnimations.Count > 0)
            //{
            //    foreach (var animation in OneOffAnimations)
            //    {
            //        animation.Item1.Step(stepTime - animation.Item2);
            //    }
            //    OneOffAnimations = OneOffAnimations.Where((x)=>x.Item1.Duration < stepTime - x.Item2).ToList();
            //}
        }

        public void RegisterAnimation(Animation animation)
        {
            if (!Animations.Contains(animation)) Animations.Add(animation);
            animation.AddAnimator(this);
        }

        public void UnRegisterAnimation(Animation animation)
        {
            if (Animations.Contains(animation)) Animations.Remove(animation);
            animation.RemoveAnimator();
        }

        public void Play(FrameAnimation animation)
        {
            OneOffAnimations.Add( new Tuple<FrameAnimation, float>(animation, Time));
        }

        public void DumpAnimations()
        {   
            List<Animation> curAnimations = new List<Animation>(Animations);
            foreach (var animation in curAnimations)
            {
                UnRegisterAnimation(animation);
            }
        }
    }
}
