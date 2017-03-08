using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

namespace MorphingClass.CCorrepondObjects
{
    public class CValMap_SD<T1, T2>
        where T1 : IComparable
        where T2 : IComparable
    {
        //public T1 Int1 { get; set; }
        //public T2 Int2 { get; set; }

        public SortedDictionary<T1, T2> SD { get; set; }
        public SortedDictionary<T2, T1> SD_R { get; set; }    //reversed kvp

        public CValMap_SD()
            : this(null)
        {
        }

        public CValMap_SD(IComparer<T1> cmp1 = null)//, IComparer<T2> cmp2 = null)
        {
            if (cmp1 == null) { cmp1 = Comparer<T1>.Default; }
            //if (cmp2 == null) { cmp2 = Comparer<T2>.Default; }

            this.SD = new SortedDictionary<T1, T2>(cmp1);
            //this.SD_R = new SortedDictionary<T2, T1>(cmp2);
        }

        public void CreateSD_R(IComparer<T2> cmp2 = null)
        {
            if (cmp2 == null) { cmp2 = Comparer<T2>.Default; }
            this.SD_R = new SortedDictionary<T2, T1>(cmp2);
            foreach (var kvp in this.SD)
            {
                this.SD_R.Add(kvp.Value, kvp.Key);
            }
        }
    }
}
