using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

namespace MorphingClass.CCorrepondObjects
{
    public class CPairInt : IComparable<CPairInt>
    {
        public int Int1 { get; set; }
        public int Int2 { get; set; }


        public CPairInt()
        {
        }

        public CPairInt(int pInt1, int pInt2)
        {
            this.Int1 = pInt1;
            this.Int2 = pInt2;
        }

        public int CompareTo(CPairInt other)
        {
            return CCmpMethods.CmpDual(this, other, cpi => cpi.Int1, cpi => cpi.Int2);
        }
    }


    public class CPairIntIncrease : CPairInt
    {
        public CPairIntIncrease(int pInt1, int pInt2)
        {
            this.Int1 = Math.Min(pInt1, pInt2);
            this.Int2 = Math.Max(pInt1, pInt2);
        }
    }
}
