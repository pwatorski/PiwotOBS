using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiwotOBS.Structure.Animations
{
    public class Animation
    {
        public SceneItem TargetItem;
        public float LastStepTime { get; protected set; }
        public Animator? Animator { get; protected set; }
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

        internal void AddAnimator(Animator animator)
        {
            Animator = animator;
        }

        internal void RemoveAnimator()
        {
            Animator = null;
        }
    }
}
