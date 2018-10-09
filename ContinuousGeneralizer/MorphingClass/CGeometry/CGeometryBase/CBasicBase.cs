using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CBasicBase : IComparable<CBasicBase>, IEquatable<CBasicBase>
    //, EqualityComparer<CBasicBase>
    {
        protected int _intID = -1;
        public int GID { get; set; } //global id
        public int indexID { get; set; }
        public bool isTraversed { get; set; }


        public int CompareTo(CBasicBase other)
        {
            return this.GID.CompareTo(other.GID);
        }



        //Notes to Implementers:
        //If you implement Equals, you should also override the base class implementations of Object.Equals(Object) and GetHashCode 
        //so that their behavior is consistent with that of the IEquatable<T>.Equals method.
        //If you do override Object.Equals(Object), your overridden implementation is also called in calls to 
        //the static Equals(System.Object, System.Object) method on your class. 
        //In addition, you should overload the op_Equality and op_Inequality operators.
        //This ensures that all tests for equality return consistent results, which the example illustrates.
        public bool Equals(CBasicBase other)
        {
            return this.GID == other.GID;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;

            var baseother = other as CBasicBase;
            if (baseother == null)
                return false;
            else
                return Equals(baseother);
        }

        public override int GetHashCode()
        {
            return this.GID.GetHashCode();
        }

        public static bool operator ==(CBasicBase base1, CBasicBase base2)
        {
            if (((object)base1) == null || ((object)base2) == null)
                return Object.Equals(base1, base2);

            return base1.Equals(base2);
        }

        public static bool operator !=(CBasicBase base1, CBasicBase base2)
        {
            if (((object)base1) == null || ((object)base2) == null)
                return !Object.Equals(base1, base2);

            return !(base1.Equals(base2));
        }


        public int ID
        {
            get { return _intID; }
            set { _intID = value; }
        }
    }
}
