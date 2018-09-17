using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CEntity;


namespace MorphingClass.CCorrepondObjects
{
    public class CCorrCpgSS : IComparable<CCorrCpgSS>
    {
        public SortedSet<CPolygon> FrCpgSS { get; set; }
        public SortedSet<CPolygon> ToCpgSS { get; set; }

        public int CompareTo(CCorrCpgSS other)
        {
            return CCmpMethods.CmpColConsideringCount(FrCpgSS, ToCpgSS, cpg => cpg.GID);
        }
    }
}
