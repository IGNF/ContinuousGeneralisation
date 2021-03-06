﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CEntity;


namespace MorphingClass.CCorrepondObjects
{
    public class CCorrCphs : CBasicBase
    {
        public static CCmpCCorrCphs_CphsGID pCmpCCorrCphs_CphsGID = new CCmpCCorrCphs_CphsGID();
        private static int _intStaticGID;
        public CPatch FrCph { get; set; }
        public CPatch ToCph { get; set; }
        public double dblSharedSegLength { get; set; }
        public int intSharedCEdgeCount { get; set; }
        public List<CEdge> SharedCEdgeLt { get; set; }
        //public bool isSharedCEdgeLtComplete { get; set; }


        //public CCorrCphs(CPatch pCph1, CPatch pCph2)
        //{
        //    this.GID = _intStaticGID++;

        //    SharedCEdgeLt = new List<CEdge>();
        //    if (pCph1.GID <= pCph2.GID)
        //    {
        //        FrCph = pCph1;
        //        ToCph = pCph2;
        //    }
        //    else
        //    {
        //        FrCph = pCph2;
        //        ToCph = pCph1;
        //    }
        //}

        public CCorrCphs(CPatch pCph1, CPatch pCph2, CCorrCphs pCorrCphs)
            : this(pCph1, pCph2, pCorrCphs.SharedCEdgeLt, pCorrCphs.dblSharedSegLength, pCorrCphs.intSharedCEdgeCount)
        {
        }

        public CCorrCphs(CPatch pCph1, CPatch pCph2, List<CEdge> pSharedCEdgeLt = null, 
            double fdblSharedSegLength = 0, int fintSharedCEdgeCount = 0)
        {
            this.GID = _intStaticGID++;

            if (pSharedCEdgeLt==null)
            {
               this.SharedCEdgeLt = new List<CEdge>(); 
            }
            else
            {
                this.SharedCEdgeLt = new List<CEdge>(pSharedCEdgeLt); 
            }
            //this.SharedCEdgeLt = new List<CEdge>();
            //this.SharedCEdgeLt.AddRange(pSharedCEdgeLt);
            this.dblSharedSegLength = fdblSharedSegLength;
            this.intSharedCEdgeCount = fintSharedCEdgeCount;

            //if (intSharedCEdgeCount == 0 || dblSharedSegLength == 0)
            //{
            //    throw new ArgumentException("this should not happen!");
            //}

            if (pCph1.GID <= pCph2.GID)
            {
                FrCph = pCph1;
                ToCph = pCph2;
            }
            else
            {
                FrCph = pCph2;
                ToCph = pCph1;
            }
        }



        //public int CompareTo(CCorrCphs other)
        //{
        //    return CCmpMethods.CmpDual(this, other, corrcphs => 
        //      corrcphs.FrCph.GID, corrcphs => corrcphs.ToCph.GID, CConstants.ComparerInt);

        //    //return CCmpMethods.Cmp(this.FrCph.CpgSS, ToCpgSS, cpg => cpg.GID);
        //}
    }
}
