﻿using PiwotOBS.PMath;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{
    public class Animation
    {
        public SceneItem TargetItem;
        public float LastStepTime { get; protected set; }
        public Animation(SceneItem target) 
        {
            TargetItem = target;
        }

        public virtual void Step(float time)
        {
            LastStepTime = time;
        }
    }

    public class AnimationTransform
    {
        public SceneItem TargetItem { get; protected set; }
        public Float2? Position { get; protected set; }
        public Float2? Size { get; protected set; }
        public Float2? Scale { get; protected set; }
        public float? Rotation { get; protected set;}

        public AnimationTransform(SceneItem targetItem, Float2? position=null, Float2? size = null, Float2? scale = null, float? rotation = null)
        {
            TargetItem = targetItem;
            Position = position;
            Size = size;
            Scale = scale;
            Rotation = rotation;
        }

        public AnimationTransform(SceneItem targetItem)
        {
            TargetItem = targetItem;
            Position = TargetItem.CurPosition.Copy();
            Size = TargetItem.CurSize.Copy();
            Scale = TargetItem.CurScale.Copy();
            Rotation = TargetItem.CurRotation;
        }

        public static AnimationTransform GetMidFrame(AnimationTransform previous, AnimationTransform next, float time) 
        {
            return null;
        }

        public JsonObject ToJson()
        {
            JsonObject transform = new JsonObject()
            {

            };
            if (Size != null)
            {
                transform.Add("height", Size.X);
                transform.Add("width", Size.Y);
            }
            if (Position != null)
            {
                transform.Add("positionX", Position.X);
                transform.Add("positionY", Position.Y);
            }
            if (Scale != null)
            {
                transform.Add("scaleX", Scale.X);
                transform.Add("scaleY", Scale.Y);
            }
            if (Rotation != null)
            {
                transform.Add("rotation", Rotation);
            }

            return transform;
        }

    }

    public class AnimationKeyFrame
    {
        public float Time { get; protected set; }
        public AnimationTransform Transform { get; protected set; }
        public static AnimationTransform GetMidFrameYtansform(AnimationKeyFrame other, float time)
        {
            return null;
        }
    }

    public class FrameAnimation : Animation
    {
        protected List<AnimationKeyFrame> keyFrames;
        public FrameAnimation(SceneItem target) : base(target)
        {
        }
    }

    public class ProceduralAnimation : Animation
    {
        protected AnimationTransform baseTransform;
        protected Func<float, SceneItem, JsonObject> TransformFunction;
        public ProceduralAnimation(SceneItem target, Func<float, SceneItem, JsonObject> func) : base(target)
        {
            baseTransform = new AnimationTransform(target);
            TransformFunction = func;
        }

        public override void Step(float time)
        {
            var transform = TransformFunction.Invoke(time, TargetItem);
            OBSDeck.OBS.SetSceneItemTransform(TargetItem.SceneName, TargetItem.SceneItemId, transform);
            base.Step(time);
        }
    }

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
            if(AnimationThread != null)
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

            while(!StopPending)
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