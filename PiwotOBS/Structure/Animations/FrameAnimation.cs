using System.Text;
using System.Text.Json.Nodes;

namespace PiwotOBS.Structure.Animations
{
    public class FrameAnimation : Animation
    {
        protected List<AnimationKeyFrame> keyFrames = new List<AnimationKeyFrame>();
        public float StartTime { get; protected set; } = 0;
        public float Duration { get; protected set; } = 0;
        public bool Loop { get; set; } = false;
        public bool Pause { get; set; } = false;
        protected bool oneOffFinished = false;
        public FrameAnimation(SceneItem target) : base(target)
        {
        }

        public override void Step(float time)
        {
            var animationTime = time - StartTime;
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
            if (frameID < 0)
                frameID = 0;
            return AnimationKeyFrame.GetMidFrameTansform(
                keyFrames[frameID],
                keyFrames[(frameID + 1) % keyFrames.Count],
                animationTime
                );
        }

        public void AddKeyFrame(AnimationTransform? transform=null, float? timePoint=null, bool enable=false, bool disable=false)
        {
            if (transform != null)
            {
                if (transform.Scale == null)
                {
                    throw new Exception(nameof(transform.Scale));
                }
                if (transform.Rotation == null)
                {
                    throw new Exception(nameof(transform.Rotation));
                }
                if (transform.Position == null)
                {
                    throw new Exception(nameof(transform.Position));
                }
                if (transform.Size == null)
                {
                    throw new Exception(nameof(transform.Size));
                }
            }
            
            timePoint ??= keyFrames.Count > 0 ? keyFrames.Last().TimePoint + 1 : 0;



            AddKeyFrame(new AnimationKeyFrame(transform, (float)timePoint, new AnimatioItemEnableAction(enable, disable)));
        }
        public void AddKeyFrame(AnimationKeyFrame keyFrame)
        {

            //TODO Make generated frames for those without transform - case when switching enabled status!
            keyFrames.Add(keyFrame);
            keyFrames.Sort((x, y) => x.TimePoint.CompareTo(y.TimePoint));
            Duration = CalculateDuration();
        }

        public void AddKeyFrameFromOBS(float? timePoint = null, float timePointDelta = 1)
        {
            if (timePoint == null)
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
        public static FrameAnimation FromJson(string savePath, Scene rootScene)
        {
            using StreamReader sr = new(savePath, Encoding.UTF8);
            var node = JsonNode.Parse(sr.ReadToEnd());
            sr.Close();
            sr.Dispose();
            return FromJson(node.AsObject(), rootScene);
        }
        public static FrameAnimation FromJson(JsonObject source, Scene rootScene)
        {
            string? targetName = (string?)source["TargetName"] ?? throw new Exception("No target scene item name in animation definition!");
            var targetItem = rootScene.FindItem(targetName) ?? throw new Exception($"Could not find target object: \"{targetName}\"!");
            bool? loop = (bool?)source["Loop"] ?? throw new Exception("No loop flag!");
            JsonArray? framesJson = (JsonArray?)source["keyFrames"] ?? throw new Exception("No key frames array!");
            FrameAnimation animation = new FrameAnimation(targetItem);
            foreach (var frameJson in framesJson)
            {
                if (frameJson != null)
                    animation.AddKeyFrame(AnimationKeyFrame.FromJson(frameJson.AsObject(), rootScene));
            }
            return animation;
        }
    }
}
