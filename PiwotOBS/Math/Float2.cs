using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBS.PMath
{
    public class Float2 : IComparable, ICloneable
    {
        #region Fields
        /// <summary>
        /// The x value;
        /// </summary>
        protected float x;
        /// <summary>
        /// The y value;
        /// </summary>
        protected float y;

        /// <summary>
        /// The x value.
        /// </summary>
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }
        /// <summary>
        /// The y value.
        /// </summary>
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
            }
        }
        #endregion

        #region Static values
        /// <summary>
        /// (0,0)
        /// </summary>
        public static Float2 Zero { get { return new Float2(0); } }
        /// <summary>
        /// (1,1)
        /// </summary>
        public static Float2 One { get { return new Float2(1); } }
        /// <summary>
        /// (2147483647,2147483647)
        /// </summary>
        public static Float2 MaxValue { get { return new Float2(int.MaxValue); } }
        /// <summary>
        /// (-2147483648,-2147483648)
        /// </summary>
        public static Float2 MinValue { get { return new Float2(int.MinValue); } }
        /// <summary>
        /// (0,1)
        /// </summary>
        public static Float2 Up { get { return new Float2(0, 1); } }
        /// <summary>
        /// (1,0)
        /// </summary>
        public static Float2 Right { get { return new Float2(1, 0); } }
        /// <summary>
        /// (0,-1)
        /// </summary>
        public static Float2 Down { get { return new Float2(0, -1); } }
        /// <summary>
        /// (-1,0)
        /// </summary>
        public static Float2 Left { get { return new Float2(-1, 0); } }
        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        public Float2()
        {
            x = 0;
            y = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xy"></param>
        public Float2(float xy)
        {
            x = xy;
            y = xy;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i2"></param>
        public Float2(Float2 i2)
        {
            x = i2.x;
            y = i2.y;
        }
        #endregion

        #region Manipulators
        ///<summary>Swaps values of x and y in this instance of the object.
        ///</summary>
        public void Flip()
        {
            float t = x;
            x = y;
            y = t;
        }

        ///<summary>Returns a new instance of <see cref="Float2"/> with swapped x and y values.
        ///</summary>
        ///<param name="i">The instance of <see cref="Float2"/> class to flip.</param>
        public static Float2 Flip(Float2 i)
        {
            return i.Flipped();
        }

        ///<summary>Returns a new instance of <see cref="Float2"/> with swapped x and y values.
        ///</summary>
        public Float2 Flipped()
        {
            return new Float2(y, x);
        }

        ///<summary>Returns a new instance of <see cref="Float2"/> with values of x and y clamped between 0 and inclusiveMax.
        ///</summary>
        ///<param name="i">The instance of <see cref="Float2"/> class to be clamped.</param>
        ///<param name="inclusiveMax">The inclusive upper bound of the clamping range.</param>
        public static Float2 Clamp(Float2 i, float inclusiveMax)
        {
            return new Float2(Arit.Clamp(i.x, inclusiveMax), Arit.Clamp(i.y, inclusiveMax));
        }

        ///<summary>Returns a new instance of <see cref="Float2"/> with values of x and y clamped between inclusiveMin and inclusiveMax.
        ///</summary>
        ///<param name="i">The instance of <see cref="Float2"/> class to be clamped.</param>
        ///<param name="inclusiveMin">The inclusive lower bound of the clamping range.</param>
        ///<param name="inclusiveMax">The inclusive upper bound of the clamping range.</param>
        public static Float2 Clamp(Float2 i, float inclusiveMin, float inclusiveMax)
        {
            return new Float2(Arit.Clamp(i.x, inclusiveMin, inclusiveMax), Arit.Clamp(i.y, inclusiveMin, inclusiveMax));
        }

        ///<summary>Returns a new instance of <see cref="Float2"/> with values of x and y clamped between 0 and respective inclusiveMax values.
        ///</summary>
        ///<param name="i">The instance of <see cref="Float2"/> class to be clamped.</param>
        ///<param name="inclusiveMax">The respective inclusive upper bounds of the clamping range.</param>
        public static Float2 Clamp(Float2 i, Float2 inclusiveMax)
        {
            return new Float2(Arit.Clamp(i.x, inclusiveMax.x), Arit.Clamp(i.y, inclusiveMax.y));
        }

        ///<summary>Returns a new instance of <see cref="Float2"/> with values of x and y clamped between respective inclusiveMin and inclusiveMax values.
        ///</summary>
        ///<param name="i">The instance of <see cref="Float2"/> class to be clamped.</param>
        ///<param name="inclusiveMin">The respective inclusive lower bounds of the clamping range.</param>
        ///<param name="inclusiveMax">The respective inclusive upper bounds of the clamping range.</param>
        public static Float2 Clamp(Float2 i, Float2 inclusiveMin, Float2 inclusiveMax)
        {
            return new Float2(Arit.Clamp(i.x, inclusiveMin.x, inclusiveMax.x), Arit.Clamp(i.y, inclusiveMin.y, inclusiveMax.y));
        }

        ///<summary>Clamps this instance x and y values between 0 and inclusiveMax.
        ///</summary>
        ///<param name="inclusiveMax">The inclusive upper bound of the clamping range.</param>
        public void ClampThis(float inclusiveMax)
        {
            Arit.Clamp(x, inclusiveMax);
            Arit.Clamp(y, inclusiveMax);
        }

        ///<summary>Clamps this instance x and y values between inclusiveMin and inclusiveMax.
        ///</summary>
        ///<param name="inclusiveMin">The inclusive lower bound of the clamping range.</param>
        ///<param name="inclusiveMax">The inclusive upper bound of the clamping range.</param>
        public void ClampThis(float inclusiveMin, float inclusiveMax)
        {
            Arit.Clamp(x, inclusiveMin, inclusiveMax);
            Arit.Clamp(y, inclusiveMin, inclusiveMax);
        }

        ///<summary>Clamps this instance x and y values between 0 and respective inclusiveMax values.
        ///</summary>
        ///<param name="inclusiveMax">The respective inclusive upper bounds of the clamping range.</param>
        public void ClampThis(Float2 inclusiveMax)
        {
            Arit.Clamp(x, inclusiveMax.x);
            Arit.Clamp(y, inclusiveMax.y);
        }

        ///<summary>Clamps this instance x and y values between respective inclusiveMin and inclusiveMax values.
        ///</summary>
        ///<param name="inclusiveMin">The respective inclusive lower bounds of the clamping range.</param>
        ///<param name="inclusiveMax">The respective inclusive upper bounds of the clamping range.</param>
        public void ClampThis(Float2 inclusiveMin, Float2 inclusiveMax)
        {
            Arit.Clamp(x, inclusiveMin.x, inclusiveMax.x);
            Arit.Clamp(y, inclusiveMin.y, inclusiveMax.y);
        }


        #endregion

        #region Random
        ///<summary>Returns new instance of <see cref="Float2"/> with random values of x and y.
        ///</summary>
        public static Float2 Random()
        {
            return new Float2(Rand.Float(), Rand.Float());
        }

        ///<summary> Returns new instance of <see cref="Float2"/> with random values of x and y.
        ///<para>Both x and y will be winthin range from 0 to exclusiveMax.</para>
        ///</summary>
        ///<param name="exclusiveMax">The upper bound of the range(exclusive)</param>
        public static Float2 Random(float exclusiveMax)
        {
            return new Float2(Rand.Float(exclusiveMax), Rand.Float(exclusiveMax));
        }

        ///<summary> Returns new instance of <see cref="Float2"/> with random values of x and y.
        ///<para>Both x and y will be winthin range from 0 to respective exclusiveMax.</para>
        ///</summary>
        ///<param name="exclusiveMax">The upper respective bounds of the range(exclusive)</param>
        public static Float2 Random(Float2 exclusiveMax)
        {
            return new Float2(Rand.Float(exclusiveMax.x), Rand.Float(exclusiveMax.y));
        }

        ///<summary> Returns new instance of <see cref="Float2"/> with random values of x and y.
        ///<para>Both x and y will be winthin range from inclusiveMin to exclusiveMax.</para>
        ///</summary>
        ///<param name="inclusiveMin">The lower bound of the range(inclusive)</param>
        ///<param name="exclusiveMax">The upper bound of the range(exclusive)</param>
        public static Float2 Random(float inclusiveMin, float exclusiveMax)
        {
            return new Float2(Rand.Float(inclusiveMin, exclusiveMax), Rand.Float(inclusiveMin, exclusiveMax));
        }

        ///<summary> Returns new instance of <see cref="Float2"/> with random values of x and y.
        ///<para>Both x and y will be winthin range from respective inclusiveMin to respective exclusiveMax.</para>
        ///</summary>
        ///<param name="inclusiveMin">The lower respective bounds of the range(inclusive)</param>
        ///<param name="exclusiveMax">The upper respective bounds of the range(exclusive)</param>
        public static Float2 Random(Float2 inclusiveMin, Float2 exclusiveMax)
        {
            return new Float2(Rand.Float(inclusiveMin.x, exclusiveMax.y), Rand.Float(inclusiveMin.y, exclusiveMax.y));
        }
        #endregion

        #region Box checks

        ///<summary>
        ///Checks if a given pofloat is inside a given box (inclusve version).
        ///</summary>
        ///<param name="boxSize">The size of a box.</param>
        ///<param name="point">The pofloat to be checked.</param>
        public static bool InBox(Float2 boxSize, Float2 point)
        {
            return point <= boxSize && point >= Zero;
        }

        ///<summary>
        ///Checks if a given pofloat is inside a given box (exclusive version).
        ///</summary>
        ///<param name="boxSize">The size of a box.</param>
        ///<param name="point">The pofloat to be checked.</param>
        public static bool InBoxEx(Float2 boxSize, Float2 point)
        {
            return point < boxSize && point > Zero;
        }

        ///<summary>
        ///Checks if a given pofloat is inside a given box lying at specified position (inclusve version).
        ///</summary>
        ///<param name="boxSize">The size of a box.</param>
        ///<param name="boxPosition">The position of a box.</param>
        ///<param name="point">The pofloat to be checked.</param>
        public static bool InBox(Float2 boxSize, Float2 boxPosition, Float2 point)
        {
            return InBox(boxSize, point - boxPosition);
        }

        ///<summary>
        ///Checks if a given pofloat is inside a given box lying at specified position (exclusive version).
        ///</summary>
        ///<param name="boxSize">The size of a box.</param>
        ///<param name="boxPosition">The position of a box.</param>
        ///<param name="point">The pofloat to be checked.</param>
        public static bool InBoxEx(Float2 boxSize, Float2 boxPosition, Float2 point)
        {
            return InBoxEx(boxSize, point - boxPosition);
        }

        ///<summary>
        ///Checks if a given box is inside another given box(inclusve version).
        ///</summary>
        ///<param name="outsideBoxSize">The size of the outside box.</param>
        ///<param name="outsideBoxPosition">The position of the outside box.</param>
        ///<param name="insideBoxSize">The size of the inside box.</param>
        ///<param name="insideBoxPosition">The position of the inside box.</param>
        public static bool InBox(Float2 outsideBoxSize, Float2 outsideBoxPosition, Float2 insideBoxSize, Float2 insideBoxPosition)
        {
            return insideBoxPosition + insideBoxSize <= outsideBoxPosition + outsideBoxSize && insideBoxPosition >= outsideBoxPosition;
        }

        ///<summary>
        ///Checks if a given box is inside another given box(exclusive version).
        ///</summary>
        ///<param name="outsideBoxSize">The size of the outside box.</param>
        ///<param name="outsideBoxPosition">The position of the outside box.</param>
        ///<param name="insideBoxSize">The size of the inside box.</param>
        ///<param name="insideBoxPosition">The position of the inside box.</param>
        public static bool InBoxEx(Float2 outsideBoxSize, Float2 outsideBoxPosition, Float2 insideBoxSize, Float2 insideBoxPosition)
        {
            return insideBoxPosition + insideBoxSize < outsideBoxPosition + outsideBoxSize && insideBoxPosition > outsideBoxPosition;
        }
        #endregion

        #region Operators
        /// <summary>
        /// The pointwise addition operation between two <see cref="Float2"/> objects.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static Float2 operator +(Float2 i1, Float2 i2) { return new Float2(i1.x + i2.x, i1.y + i2.y); }

        /// <summary>
        /// The addition operation between a <see cref="Float2"/> object and a integer. Both X and Y are incremented by a given value.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Float2 operator +(Float2 i1, float x) { return new Float2(i1.x + x, i1.y + x); }

        /// <summary>
        /// The pointwise subtraction operation between two <see cref="Float2"/> objects.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static Float2 operator -(Float2 i1, Float2 i2) { return new Float2(i1.x - i2.x, i1.y - i2.y); }

        /// <summary>
        /// The subtraction operation between a <see cref="Float2"/> object and a integer. Both X and Y are decremented by a given value.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Float2 operator -(Float2 i1, float x) { return new Float2(i1.x - x, i1.y - x); }

        /// <summary>
        /// The pointwise multiplication operation between two <see cref="Float2"/> objects.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static Float2 operator *(Float2 i1, Float2 i2) { return new Float2(i1.x * i2.x, i1.y * i2.y); }

        /// <summary>
        /// The multiplication operation between a <see cref="Float2"/> object and a integer. Both X and Y are multiplied by a given value.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Float2 operator *(Float2 i1, float x) { return new Float2(i1.x * x, i1.y * x); }

        /// <summary>
        /// The pointwise division operation between two <see cref="Float2"/> objects.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static Float2 operator /(Float2 i1, Float2 i2) { return new Float2(i1.x / i2.x, i1.y / i2.y); }
        /// <summary>
        /// The division operation between a <see cref="Float2"/> object and a integer. Both X and Y are divided by a given value.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Float2 operator /(Float2 i1, float x) { return new Float2(i1.x / x, i1.y / x); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(Float2? i1, Float2? i2)
        {
            if (i2 is null) return i1 is null; if (i1 is null) return i2 is null;
            if (i1.x == i2.x && i1.y == i2.y) return true; return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(Float2? i1, Float2? i2)
        {
            if (i2 is null) return i1 is not null; if (i1 is null) return i2 is not null;
            if (i1.x == i2.x && i1.y == i2.y) return false; return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator >=(Float2 i1, Float2 i2)
        {
            if (i1 == null) throw new ArgumentNullException("i1");
            if (i2 == null) throw new ArgumentNullException("i2");
            if (i1.x >= i2.x && i1.y >= i2.y) return true; return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator <=(Float2 i1, Float2 i2)
        {
            if (i1 == null) throw new ArgumentNullException("i1");
            if (i2 == null) throw new ArgumentNullException("i2");
            if (i1.x <= i2.x && i1.y <= i2.y) return true; return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator >(Float2 i1, Float2 i2)
        {
            if (i1 == null) throw new ArgumentNullException("i1");
            if (i2 == null) throw new ArgumentNullException("i2");
            if (i1.x > i2.x && i1.y > i2.y) return true; return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator <(Float2 i1, Float2 i2)
        {
            if (i1 == null) throw new ArgumentNullException("i1");
            if (i2 == null) throw new ArgumentNullException("i2");
            if (i1.x < i2.x && i1.y < i2.y) return true; return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (this.GetType() != obj.GetType()) return false;
            Float2 p = (Float2)obj;
            return this == p;
        }
        #endregion

        #region Misc
        /// <summary>
        /// Creates a hash code of this object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var result = 23;
                result += (result * 31) + x.GetHashCode();
                result += (result * 31) + x.GetHashCode();
                return result;
            }
        }

        /// <summary>
        /// Returns a <see cref="string"/> representation of this <see cref="Float2"/> object.
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            return x + ", " + y;
        }

        /// <summary>
        /// Copares two <see cref="Float2"/> objects.
        /// </summary>
        /// <param name="obj">The other object</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            Float2 other = obj as Float2;
            if (other != null)
            {
                if (this == other)
                    return 0;
                return this > other ? 1 : -1;
            }
            else
                throw new ArgumentException("Object is not an Float2");
        }

        public Float2 Copy()
        {
            return new Float2(this);
        }

        public object Clone()
        {
            return Copy();
        }



        #endregion
    }
}
