using PiwotOBS.PMath;
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

        protected void ApplyStep(AnimationTransform transform)
        {
            TargetItem.TransformObject(newPos: transform.Position, newScale: transform.Scale, newRotation: transform.Rotation);
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
            Position = new Float2(TargetItem.CurPosition);
            Size = new Float2(TargetItem.CurSize);
            Scale = new Float2(TargetItem.CurScale);
            Rotation = TargetItem.CurRotation;
        }

        public AnimationTransform(SceneItem targetItem, SceneItemTransform transform)
        {
            TargetItem = targetItem;
            Position = new Float2(transform.Position);
            Size = new Float2(transform.Size);
            Scale = new Float2(transform.Scale);
            Rotation = transform.rotation;
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
                transform.Add(nameof(Size), Size.ToJson());
                //transform.Add("height", Size.X);
                //transform.Add("width", Size.Y);
            }
            if (Position != null)
            {
                transform.Add(nameof(Position), Position.ToJson());
                //transform.Add("positionX", Position.X);
                //transform.Add("positionY", Position.Y);
            }
            if (Scale != null)
            {
                transform.Add(nameof(Scale), Scale.ToJson());
                //transform.Add("scaleX", Scale.X);
                //transform.Add("scaleY", Scale.Y);
            }
            if (Rotation != null)
            {
                transform.Add("Rotation", Rotation);
            }
            transform.Add("targetName", TargetItem.Name);

            return transform;
        }

        public static AnimationTransform FromJson(JsonObject json, Scene rootScene)
        {
            string? targetName = (string?)json["targetName"] ?? throw new Exception("No target scene item name in animation frame!");
            var targetItem = rootScene.FindItem(targetName) ?? throw new Exception($"Could not find target object: \"{targetName}\"!");
            Float2? position = null;
            Float2? scale = null;
            Float2? size = null;
            float? rotation = null;
            JsonNode? n;
            if ((n = json["Position"]) != null) position = Float2.FromJson(n.AsObject());
            if ((n = json["Scale"]) != null) scale = Float2.FromJson(n.AsObject());
            if ((n = json["Size"]) != null) size = Float2.FromJson(n.AsObject());
            if ((n = json["Rotation"]) != null) rotation = (float)n;

            return new AnimationTransform(targetItem, position, size, scale, rotation);
        }

        public void UpdateTargetCurVals()
        {
            
        }

        public static AnimationTransform operator +(AnimationTransform a, AnimationTransform b)
        {
            Float2? newPosition = a.Position == null || b.Position == null ? null : a.Position + b.Position;
            Float2? newSize = a.Size == null || b.Size == null ? null : a.Size + b.Size;
            Float2? newScale = a.Scale == null || b.Scale == null ? null : a.Scale + b.Scale;
            float? newRotation = a.Rotation == null || b.Rotation == null ? null : a.Rotation + b.Rotation;
            return new AnimationTransform(a.TargetItem, newPosition, newSize, newScale, newRotation);
        }

        public static AnimationTransform operator -(AnimationTransform a, AnimationTransform b)
        {
            Float2? newPosition = a.Position == null || b.Position == null ? null : a.Position - b.Position;
            Float2? newSize = a.Size == null || b.Size == null ? null : a.Size - b.Size;
            Float2? newScale = a.Scale == null || b.Scale == null ? null : a.Scale - b.Scale;
            float? newRotation = a.Rotation == null || b.Rotation == null ? null : a.Rotation - b.Rotation;
            return new AnimationTransform(a.TargetItem, newPosition, newSize, newScale, newRotation);
        }

        public static AnimationTransform operator *(AnimationTransform a, float b)
        {
            Float2? newPosition = a.Position == null ? null : a.Position * b;
            Float2? newSize = a.Size == null ? null : a.Size * b;
            Float2? newScale = a.Scale == null ? null : a.Scale * b;
            float? newRotation = a.Rotation == null ? null : a.Rotation * b;
            return new AnimationTransform(a.TargetItem, newPosition, newSize, newScale, newRotation);
        }

        public static AnimationTransform operator /(AnimationTransform a, float b)
        {
            Float2? newPosition = a.Position == null ? null : a.Position / b;
            Float2? newSize = a.Size == null ? null : a.Size / b;
            Float2? newScale = a.Scale == null ? null : a.Scale / b;
            float? newRotation = a.Rotation == null ? null : a.Rotation / b;
            return new AnimationTransform(a.TargetItem, newPosition, newSize, newScale, newRotation);
        }

    }

    public class AnimationKeyFrame
    {
        public float TimePoint { get; protected set; }
        public AnimationTransform Transform { get; protected set; }
        public AnimationKeyFrame(AnimationTransform transform, float timePoint)
        {
            Transform = transform;
            TimePoint = timePoint;
        }

        protected static Float2? GetMidValue(Float2? a, Float2? b, float ratio, Float2 defaultA)
        {
            if (b != null)
            {
                if (a == null)
                {
                    return Float2.Larp(defaultA, b, ratio);
                }
                else
                {
                    return Float2.Larp(a, b, ratio);
                }
            }
            return null;
        }

        protected static float? GetMidValue(float? a, float? b, float ratio, float defaultA)
        {
            if (b != null)
            {
                if (a == null)
                {
                    return Arit.Larp(defaultA, (float)b, ratio);
                }
                else
                {
                    return Arit.Larp((float)a, (float)b, ratio);
                }
            }
            return null;
        }


        public static AnimationTransform GetMidFrameTansform(AnimationKeyFrame a, AnimationKeyFrame b, float time)
        {
            float transitionDuration = b.TimePoint - a.TimePoint;
            float transitionTimePoint = time - a.TimePoint;
            float transitionRatio = transitionTimePoint / transitionDuration;

            Float2? newPosition = GetMidValue(a.Transform.Position, b.Transform.Position, transitionRatio, a.Transform.TargetItem.CurPosition);
            Float2? newSize = GetMidValue(a.Transform.Size, b.Transform.Size, transitionRatio, a.Transform.TargetItem.CurSize);
            Float2? newScale = GetMidValue(a.Transform.Scale, b.Transform.Scale, transitionRatio, a.Transform.TargetItem.CurScale);
            float? newRotation = GetMidValue(a.Transform.Rotation, b.Transform.Rotation, transitionRatio, a.Transform.TargetItem.CurRotation);

            if(newRotation != null)
            {
                while (newRotation < 0) newRotation += 360;
                newRotation %= 360;
            }    

            return new AnimationTransform(a.Transform.TargetItem, newPosition, newSize, newScale, newRotation);
        }

        public JsonObject ToJson()
        {
            return new JsonObject()
            {
                {nameof(TimePoint), TimePoint },
                {nameof(Transform), Transform.ToJson() }
            };
        }

        public static AnimationKeyFrame FromJson(JsonObject source, Scene rootScene)
        {
            var timePoint = (float?)source["TimePoint"] ?? throw new Exception("No timepoint!");
            var transformJson = source["Transform"] ?? throw new Exception("No transform!");

            return new AnimationKeyFrame(AnimationTransform.FromJson(transformJson.AsObject(), rootScene), timePoint);
        }
    }

    public class FrameAnimation : Animation
    {
        protected List<AnimationKeyFrame> keyFrames = new List<AnimationKeyFrame>();
        public float StartTime { get; protected set; } = 0;
        public float Duration { get; protected set; } = 0;
        public bool Loop { get; set; } = false;
        protected bool oneOffFinished = false;
        public FrameAnimation(SceneItem target) : base(target)
        {
        }

        public override void Step(float time)
        {
            var animationTime = (time - StartTime);
            if (!Loop)
            {
                if (animationTime > Duration)
                {
                    if (!oneOffFinished)
                    {
                        ApplyStep(keyFrames.Last().Transform);
                        oneOffFinished = true;
                    }
                    return;
                }
                oneOffFinished = false;
            }

            ApplyStep(GetMidFrameTransform(time));

            base.Step(time);
        }

        protected float CalculateDuration()
        {
            return keyFrames.Max((x) => x.TimePoint);
        }

        protected AnimationTransform GetMidFrameTransform(float animationTime)
        {
            animationTime %= Duration;
            int frameID = 0;
            int maxFrame = keyFrames.Count - 1;
            while (keyFrames[frameID].TimePoint <= animationTime && frameID < maxFrame)
            {
                frameID++;
            }
            frameID--;
            return AnimationKeyFrame.GetMidFrameTansform(
                keyFrames[frameID], 
                keyFrames[(frameID + 1) % keyFrames.Count], 
                animationTime
                );
        }

        public void AddKeyFrame(AnimationTransform transform, float timePoint)
        {
            if(transform.Scale == null)
            {
                throw new Exception(nameof(transform.Scale));
            }
            if(transform.Rotation == null)
            {
                throw new Exception(nameof(transform.Rotation));
            }
            if(transform.Position==null)
            {
                throw new Exception(nameof(transform.Position));
            }
            if (transform.Size == null)
            {
                throw new Exception(nameof(transform.Size));
            }
            AddKeyFrame(new AnimationKeyFrame(transform, timePoint));
        }
        public void AddKeyFrame(AnimationKeyFrame keyFrame)
        {
            keyFrames.Add(keyFrame);
            keyFrames.Sort((x, y) => x.TimePoint.CompareTo(y.TimePoint));
            Duration = CalculateDuration();
        }

        public void AddKeyFrameFromOBS(float? timePoint=null, float timePointDelta=1)
        {
            if(timePoint == null)
            {
                timePoint = keyFrames.Count > 0 ? keyFrames.Last().TimePoint + timePointDelta : 0;
            }
            AddKeyFrame(new AnimationKeyFrame(new AnimationTransform(TargetItem, TargetItem.GetCurrentOBSTransform()), (float)timePoint));
        }

        public JsonObject ToJson()
        {
            return new JsonObject()
            {
                {"TargetName", TargetItem.Name },
                {nameof(keyFrames), new JsonArray(keyFrames.Select((x)=>x.ToJson()).ToArray())},
                {nameof(Loop), Loop }
            };
        }
        public static FrameAnimation FromJson(JsonObject source, Scene rootScene)
        {
            string? targetName = (string?)source["TargetName"] ?? throw new Exception("No target scene item name in animation definition!");
            var targetItem = rootScene.FindItem(targetName) ?? throw new Exception($"Could not find target object: \"{targetName}\"!");
            bool? loop = (bool?)source["Loop"] ?? throw new Exception("No loop flag!");
            JsonArray? framesJson = (JsonArray ?)source["keyFrames"] ?? throw new Exception("No key frames array!");
            FrameAnimation animation = new FrameAnimation(targetItem);
            foreach(var frameJson in framesJson)
            {
                if(frameJson != null)
                    animation.AddKeyFrame(AnimationKeyFrame.FromJson(frameJson.AsObject(), rootScene));
            }
            return animation;
        }
    }

    public class ProceduralAnimation : Animation
    {
        protected AnimationTransform baseTransform;
        protected Func<float, SceneItem, AnimationTransform> TransformFunction;
        public ProceduralAnimation(SceneItem target, Func<float, SceneItem, AnimationTransform> func) : base(target)
        {
            baseTransform = new AnimationTransform(target);
            TransformFunction = func;
        }

        public override void Step(float time)
        {
            var transform = TransformFunction.Invoke(time, TargetItem);
            ApplyStep(transform);
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
