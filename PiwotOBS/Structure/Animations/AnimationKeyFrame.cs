using PiwotOBS.PMath;
using System.Text.Json.Nodes;

namespace PiwotOBS.Structure.Animations
{
    public class AnimationKeyFrame
    {
        public float TimePoint { get; protected set; }
        public AnimatioItemEnableAction EnableAction { get; protected set; }
        public AnimationTransform Transform { get; protected set; }
        public AnimationKeyFrame(AnimationTransform transform, float timePoint, AnimatioItemEnableAction? enableAction = null)
        {
            Transform = transform;
            EnableAction = enableAction ?? new AnimatioItemEnableAction(false, false);
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

            if (newRotation != null)
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
                {nameof(Transform), Transform.ToJson() },
                {nameof(EnableAction), EnableAction.ToJson() }
            };
        }

        public static AnimationKeyFrame FromJson(JsonObject source, Scene rootScene)
        {
            var timePoint = (float?)source["TimePoint"] ?? throw new Exception("No timepoint!");
            var transformJson = source["Transform"]?.AsObject() ?? throw new Exception("No transform!");
            var enableAction = source["EnableAction"]?.AsObject() ?? throw new Exception("No enable action!");

            return new AnimationKeyFrame(AnimationTransform.FromJson(transformJson, rootScene), timePoint, AnimatioItemEnableAction.FromJson(enableAction));
        }
    }
}
