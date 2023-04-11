using PiwotOBS.PMath;
using System.Text.Json.Nodes;

namespace PiwotOBS.Structure.Animations
{
    public class AnimationTransform
    {
        public SceneItem TargetItem { get; protected set; }
        public Float2? Position { get; protected set; }
        public Float2? Size { get; protected set; }
        public Float2? Scale { get; protected set; }
        public float? Rotation { get; protected set; }

        public AnimationTransform(SceneItem targetItem, Float2? position = null, Float2? size = null, Float2? scale = null, float? rotation = null)
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
}
