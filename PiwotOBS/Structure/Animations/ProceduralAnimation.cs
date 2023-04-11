namespace PiwotOBS.Structure.Animations
{
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
}
