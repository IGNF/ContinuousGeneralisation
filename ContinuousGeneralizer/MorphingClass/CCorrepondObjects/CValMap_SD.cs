using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

namespace MorphingClass.CCorrepondObjects
{
    public class CValMap_Dt<T1, T2>
        //where T1 : IEquatable<T1>
        //where T2 : IEquatable<T2>
    {
        public Dictionary<T1, T2> Dt { get; set; }
        public Dictionary<T2, T1> Dt_R { get; set; }    //reversed kvp

        public CValMap_Dt()
            : this(null)
        {
        }

        public CValMap_Dt(IEqualityComparer<T1> eqcmp1 = null)//, IComparer<T2> cmp2 = null)
        {
            if (eqcmp1 == null) { eqcmp1 = EqualityComparer<T1>.Default; }
            //if (cmp2 == null) { cmp2 = Comparer<T2>.Default; }

            this.Dt = new Dictionary<T1, T2>(eqcmp1);
            //this.SD_R = new SortedDictionary<T2, T1>(cmp2);
        }

        public void CreateSD_R(IEqualityComparer<T2> eqcmp2 = null)
        {
            if (eqcmp2 == null) { eqcmp2 = EqualityComparer<T2>.Default; }
            this.Dt_R = new Dictionary<T2, T1>(eqcmp2);
            foreach (var kvp in this.Dt)
            {
                this.Dt_R.Add(kvp.Value, kvp.Key);
            }
        }
    }
}
