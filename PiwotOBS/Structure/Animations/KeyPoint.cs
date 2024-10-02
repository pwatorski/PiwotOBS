using PiwotOBS.PMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBS.Structure.Animations
{
    public abstract class KeyPoint
    {
        public float TimePoint { get; protected set; }
        public KeyPoint(float timePoint) { TimePoint = timePoint; }

        protected float CalculateTransitionPoint(KeyPoint other, float time)
        {
            float transitionDuration = other.TimePoint - TimePoint;
            float transitionTimePoint = time - TimePoint;
            return transitionTimePoint / transitionDuration;
        }
    }

    public class KeyPointFlag : KeyPoint
    {
        public bool Flag = true;
        public KeyPointFlag(float timePoint, bool flag) : base(timePoint)
        {
            Flag = flag;
        }
    }

    public class KeyPointFloat : KeyPoint
    {
        public float Value;
        public KeyPointFloat(float timePoint, float value) : base(timePoint)
        {
            Value = value;
        }

        public float LerpTo(KeyPointFloat other, float time)
        {
            float transitionRatio = CalculateTransitionPoint(other, time);
            return Arit.Larp(Value, other.Value, transitionRatio);
        }
    }
    public class KeyPointFloat2:KeyPoint
    {
        public Float2 Value = Float2.Zero;

        public KeyPointFloat2(float timePoint, Float2 value) : base(timePoint)
        {
            Value = value;
        }
        public Float2 LerpTo(KeyPointFloat2 other, float time)
        {
            float transitionRatio = CalculateTransitionPoint(other, time);
            return Float2.Larp(Value, other.Value, transitionRatio);
        }
    }

    public abstract class KeyPointSequence
    {
        protected List<KeyPoint> KeyPoints = new List<KeyPoint>();
        public float Duration { get { return duration; } set { duration = value; setDuration = true; } }
        protected float duration;
        protected bool setDuration = false;

        public KeyPointSequence()
        {
        }

        public void AddKeyPoint(KeyPoint keyPoint) 
        { 
            KeyPoints.Add(keyPoint);
            KeyPoints.Sort((x, y)=> x.TimePoint.CompareTo(x.TimePoint));
            if(!setDuration)
            {
                duration = KeyPoints.Last().TimePoint;
            }
        }
    }

    public class KeyPointSequenceFlag:KeyPointSequence
    {
        public KeyPointSequenceFlag()
        {

        }

        public bool GetValue(float animationTime)
        {
            return false;
        }
    }
}
