using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MorphingClass.CUtility;

namespace MorphingClass.CGeometry
{
    public class CEdgeRelation
    {
        private CEdge _CEdge1;   //usually, CEdge1 is the edge itself, CEdge2 is the other edge
        private CEdge _CEdge2;   //usually, CEdge1 is the edge itself, CEdge2 is the other edge
        public CIntersection pIntersection { get; set; }
        public CptEdgeDis cptEdgeDis { get; set; }
        public  CEnumDisMode pEnumDisMode { get; set; }

        public CEdgeRelation()
        {

        }

        public CEdgeRelation(CEdge cedge1, CEdge cedge2)
        {
            _CEdge1 = cedge1;
            _CEdge2 = cedge2;
        }

        public void DetectRelation()
        {
            CIntersection pintersec = new CGeometry.CIntersection(_CEdge1, _CEdge2);
            pintersec.DetectIntersection();

            switch (pintersec.enumIntersectionType)
            {
                case CEnumIntersectionType.NoNo:
                    DetectPtEdge();
                    break;
                case CEnumIntersectionType.FrFr:
                    this.pEnumDisMode = CEnumDisMode.FrFr;
                    this.cptEdgeDis = new CGeometry.CptEdgeDis
                        (0, 0, _CEdge1.FrCpt, _CEdge1, _CEdge2.FrCpt, _CEdge2, false);
                    break;
                case CEnumIntersectionType.FrIn:
                    this.pEnumDisMode = CEnumDisMode.FrIn;
                    this.cptEdgeDis = _CEdge1.FrCpt.DistanceTo(_CEdge2,false, _CEdge1);
                    break;
                case CEnumIntersectionType.FrTo:
                    this.pEnumDisMode = CEnumDisMode.FrTo;
                    this.cptEdgeDis = new CGeometry.CptEdgeDis
                        (0, 1, _CEdge1.FrCpt, _CEdge1, _CEdge2.ToCpt, _CEdge2, false);
                    break;
                case CEnumIntersectionType.InFr:
                    this.pEnumDisMode = CEnumDisMode.InFr;
                    this.cptEdgeDis = _CEdge2.FrCpt.DistanceTo(_CEdge1, false, _CEdge2);
                    break;
                case CEnumIntersectionType.InIn:
                    this.pEnumDisMode = CEnumDisMode.InIn;
                    this.cptEdgeDis = pintersec.IntersectCpt.DistanceTo(_CEdge2);
                    break;
                case CEnumIntersectionType.InTo:
                    this.pEnumDisMode = CEnumDisMode.InTo;
                    this.cptEdgeDis = _CEdge2.ToCpt.DistanceTo(_CEdge1, false, _CEdge2);
                    break;
                case CEnumIntersectionType.ToFr:
                    this.pEnumDisMode = CEnumDisMode.ToFr;
                    this.cptEdgeDis = new CGeometry.CptEdgeDis
                        (0, 0, _CEdge1.ToCpt, _CEdge1, _CEdge2.FrCpt, _CEdge2, false);
                    break;
                case CEnumIntersectionType.ToIn:
                    this.pEnumDisMode = CEnumDisMode.ToIn;
                    this.cptEdgeDis = _CEdge1.ToCpt.DistanceTo(_CEdge2, false, _CEdge1);
                    break;
                case CEnumIntersectionType.ToTo:
                    this.pEnumDisMode = CEnumDisMode.ToTo;
                    this.cptEdgeDis = new CGeometry.CptEdgeDis
                        (0, 1, _CEdge1.ToCpt, _CEdge1, _CEdge2.ToCpt, _CEdge2, false);
                    break;
                case CEnumIntersectionType.Overlap:
                    DetectPtEdge();
                    this.pEnumDisMode = CEnumDisMode.Overlap;
                    break;
                default:
                    break;
            }
        }

        private void DetectPtEdge()
        {
            CEdge cedge1 = _CEdge1;
            CEdge cedge2 = _CEdge2;

            var FrcptEdgeDis = cedge1.FrCpt.DistanceTo(cedge2, false, cedge1);
            var TocptEdgeDis = cedge1.ToCpt.DistanceTo(cedge2, false, cedge1);
            var EdgeFrcptDis = cedge2.FrCpt.DistanceTo(cedge1, false, cedge2);
            var EdgeTocptDis = cedge2.ToCpt.DistanceTo(cedge1, false, cedge2);

            if (FrcptEdgeDis.dblDis <= TocptEdgeDis.dblDis &&
                FrcptEdgeDis.dblDis <= EdgeFrcptDis.dblDis &&
                FrcptEdgeDis.dblDis <= EdgeTocptDis.dblDis)
            {
                this.cptEdgeDis= FrcptEdgeDis;

                if (FrcptEdgeDis.t==0)
                {
                    this.pEnumDisMode = CEnumDisMode.FrFr;
                }
                else if (FrcptEdgeDis.t == 1)
                {
                    this.pEnumDisMode = CEnumDisMode.FrTo;
                }
                else
                {
                    this.pEnumDisMode = CEnumDisMode.FrIn;
                }
            }
            else if (
                TocptEdgeDis.dblDis <= FrcptEdgeDis.dblDis &&
                TocptEdgeDis.dblDis <= EdgeFrcptDis.dblDis &&
                TocptEdgeDis.dblDis <= EdgeTocptDis.dblDis)
            {
                this.cptEdgeDis = TocptEdgeDis;
                if (TocptEdgeDis.t == 0)
                {
                    this.pEnumDisMode = CEnumDisMode.ToFr;
                }
                else if (TocptEdgeDis.t == 1)
                {
                    this.pEnumDisMode = CEnumDisMode.ToTo;
                }
                else
                {
                    this.pEnumDisMode = CEnumDisMode.ToIn;
                }
            }
            else if (
                EdgeFrcptDis.dblDis <= FrcptEdgeDis.dblDis &&
                EdgeFrcptDis.dblDis <= TocptEdgeDis.dblDis &&
                EdgeFrcptDis.dblDis <= EdgeTocptDis.dblDis)
            {
                this.cptEdgeDis = EdgeFrcptDis;
                if (EdgeFrcptDis.t == 0)
                {
                    this.pEnumDisMode = CEnumDisMode.FrFr;
                }
                else if (EdgeFrcptDis.t == 1)
                {
                    this.pEnumDisMode = CEnumDisMode.ToFr;
                }
                else
                {
                    this.pEnumDisMode = CEnumDisMode.InFr;
                }
            }
            else if (
                EdgeTocptDis.dblDis <= FrcptEdgeDis.dblDis &&
                EdgeTocptDis.dblDis <= TocptEdgeDis.dblDis &&
                EdgeTocptDis.dblDis <= EdgeFrcptDis.dblDis)
            {
                this.cptEdgeDis = EdgeTocptDis;
                if (EdgeTocptDis.t == 0)
                {
                    this.pEnumDisMode = CEnumDisMode.FrTo;
                }
                else if (EdgeTocptDis.t == 1)
                {
                    this.pEnumDisMode = CEnumDisMode.ToTo;
                }
                else
                {
                    this.pEnumDisMode = CEnumDisMode.InTo;
                }
            }
            else
            {
                throw new ArgumentException("impossible case!");
            }
        }


        public CEdge CEdge1
        {
            get { return _CEdge1; }
            //set { _CEdge1 = value; }
        }

        public CEdge CEdge2
        {
            get { return _CEdge2; }
            set { _CEdge2 = value; }
        }
    }
}
