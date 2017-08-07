using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;

using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace MorphingClass.CGeometry
{
   public class CptEdgeDis: CBasicBase
    {
        private static int _intStaticGID;
        public double dblDis { get; set; }

        /// <summary> FrCpt is one of the ends of FrCEdge </summary>
        public CPoint FrCpt { get; set; }
        /// <summary> ToCpt can be anywhere on ToCEdge </summary>
        public CPoint ToCpt { get; set; }

        public CEdge FrCEdge { get; set; }
        public CEdge ToCEdge { get; set; }

        public CEdge ConnectCEdge { get; set; }
        public bool blnHeight { get; set; }
        public double t { get; set; }

        ///// <summary> dblBridgeLength may vary depend on scale </summary>
        //public double dblBridgeLength { get; set; }

        public CValPairIncr<CPolygon> CpgPairIncr { get; set; }
        public Paths BridgeOnlyCutPaths { get; set; }



        //public CptEdgeDis()
        //{

        //}

        public CptEdgeDis(double dbldis=0, double dblt = -1, CPoint frcpt = null, CEdge frcedge = null,
            CPoint tocpt = null, CEdge tocedge = null, bool blnheight = false)
        {
            this.GID = _intStaticGID++;
            this.dblDis = dbldis;
            this.t = dblt;
            this.FrCpt = frcpt;
            this.FrCEdge = frcedge;
            this.ToCpt = tocpt;
            this.ToCEdge = tocedge;
            this.blnHeight = blnheight;
        }

        public void SetConnectEdge()
        {
            this.ConnectCEdge = new CGeometry.CEdge(this.FrCpt, this.ToCpt);
        }


        //public int CompareTo(CptEdgeDis other)
        //{
        //    return CCmpMethods.CmpTernary(this, other, 
        //        cptEdgeDis => cptEdgeDis.dblDis, cptEdgeDis => cptEdgeDis.FrCEdge, cptEdgeDis => cptEdgeDis.ToCEdge);
        //}

        //public int Compare(CptEdgeDis x, CptEdgeDis y)
        //{
        //    return CCmpMethods.CmpDual(x, y, cptEdgeDis => cptEdgeDis.dblDis, cptEdgeDis => cptEdgeDis.Cpt.GID);
        //}
    }
}
