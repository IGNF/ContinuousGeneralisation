using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MorphingClass.CGeometry;
using MorphingClass.CUtility;

namespace MorphingClass.CGeometry
{
   public class CptEdgeDis: IComparable<CptEdgeDis>
    {
        public double dblDis { get; set; }
        public CPoint Cpt { get; set; }
        public CPoint CptOnTargetCEdge { get; set; }
        public CEdge TargetCEdge { get; set; }
        public CEdge ConnectCEdge { get; set; }
        public bool blnHeight { get; set; }
        public double t { get; set; }



        public CptEdgeDis()
        {

        }

        public CptEdgeDis(double dbldis, double dblt = -1, CPoint cpt = null, 
            CPoint cptOnTargetCEdge = null, CEdge targetCEdge = null, bool blnheight = false)
        {
            this.dblDis = dbldis;
            this.t = dblt;
            this.Cpt = cpt;
            this.CptOnTargetCEdge = cptOnTargetCEdge;
            this.TargetCEdge = targetCEdge;
            this.blnHeight = blnheight;
        }

        public CEdge SetConnectEdge()
        {
            this.ConnectCEdge = new CGeometry.CEdge(this.Cpt, this.CptOnTargetCEdge);
            return this.ConnectCEdge;
        }


        public int CompareTo(CptEdgeDis other)
        {
            return CCmpMethods.CmpTernary(this, other, 
                cptEdgeDis => cptEdgeDis.dblDis, cptEdgeDis => cptEdgeDis.Cpt, cptEdgeDis => cptEdgeDis.TargetCEdge);
        }

        //public int Compare(CptEdgeDis x, CptEdgeDis y)
        //{
        //    return CCmpMethods.CmpDual(x, y, cptEdgeDis => cptEdgeDis.dblDis, cptEdgeDis => cptEdgeDis.Cpt.GID);
        //}
    }
}
