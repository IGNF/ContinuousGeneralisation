using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CUtility;
using MorphingClass.CEntity;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CPolygon : CPolyBase<CPolygon>
    {
        //public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 
        /// 要使用边（多边形）进行空间分析，需执行函数FormCEdge（FormPolygon），生成边（多边形）之后，方可使用
        /// </summary>
        /// <remarks></remarks>

        private static int _intStaticGID;

        private CEdge _OuterCmptCEdge;    //if a face has OuterCmptCEdge == null, then this face is super face
        private CEdge _cedgeStartAtLeftMost;
        private List<CEdge> _InnerCmptCEdgeLt;     //counter clockwise???

        public double dblAreaSimple { get; set; }
        public bool WasTooSmall { get; set; }

        public int intType { get; set; }
        public int intTypeIndex { get; set; } //the index (0, 1, 2, ...) of a type; used for access type distance directly

        public CPolygon ParentCpg { get; set; }
        public CPolygon ClipCpg { get; set; }

        public List<CPolygon> HoleCpgLt { get; set; }
        public CPoint CentroidCptSimple { get; set; }

        public List<CPolygon> MergedSubCpgLt { get; set; }
        public List<CEdge> BridgeCEdgeLt { get; set; }

        //public CPolygon AssigningFace { get; set; }
        //public List<CPolygon> AssignedFaceLt { get; set; }

        private IPolygon4 _pPolygon;

        //private CPoint _CentroidCpt;
        private CPoint _LeftMostCpt;
        private bool _IsHole;
        private bool _IsMerged;

        //private Func<IEnumerable<List<CPoint>>> getHoleCptLtEb;

        /// <summary>
        /// Initializes a new instance of a triangle
        /// </summary>
        //public CPolygon(int intID=-2)
        //    : this(intID, CHelpFunc.MakeLt<CPoint>(0))
        //{
        //}

        ///// <summary>
        ///// Initializes a new instance of a triangle
        ///// </summary>
        //public CPolygon()
        //    : this(-2, CHelpFunc.MakeLt<List<CPoint>>(0))
        //{
        //}



        public CPolygon(int intID, IPolygon4 pPolygon, double dblFactor = 1)
            : this(intID, CHelpFunc.GetIpgExteriorCptLt(pPolygon, dblFactor).ToList(), 
                  CHelpFunc.GetIpgInteriorCptLtEb(pPolygon, dblFactor))
        {
            this.pPolygon = pPolygon;
        }

        //public CPolygon(int intID, List<CPoint> cptlt)
        //    : this(intID, CHelpFunc.MakeLt(1, cptlt))
        //{
        //}

        /// <summary>
        /// Initializes a new instance of a CPolygon, the first point and the last point in cptlt must have the same coordinates
        /// </summary>
        public CPolygon(int intID, List<CPoint> cptlt, IEnumerable<List<CPoint>> holecptltlt)
            :this(intID,cptlt,CGeoFunc.GenerateHoleCpgLt(holecptltlt).ToList())
        {
        }

        /// <summary>
        /// Initializes a new instance of a CPolygon, the first point and the last point in cptlt must have the same coordinates
        /// </summary>
        public CPolygon(int intID = -2, List<CPoint> cptlt = null, List<CPolygon> holecpglt = null)
        {
            this.GID = _intStaticGID++;
            _intID = intID;
            this.strShape = "Polygon";
            FormPolyBase(cptlt);


            //if (holecpglt!=null)
            //{
            //    foreach (var holecpg in holecpglt)
            //    {
            //        holecpg.ParentCpg = this;
            //    }
            //}
            this.HoleCpgLt = holecpglt;
        }

        //public CPolygon(int intID = -2, List<CPoint> cptlt = null, List<CPolygon> holecpglt = null, Func<IEnumerable<List<CPoint>>> getHoleCptLtEb = null) : this(intID, cptlt, holecpglt)
        //{
        //    this.getHoleCptLtEb = getHoleCptLtEb;
        //}

        public override void FormCEdgeLt()
        {
            this.CEdgeLt = CGeoFunc.FormCEdgeEb(this.CptLt).ToList();

            if (this.HoleCpgLt!=null)
            {
                foreach (var holecpg in this.HoleCpgLt)
                {
                    holecpg.FormCEdgeLt();
                }
            }
        }

        public override void SetCEdgeLtLength()
        {
            this.CEdgeLt.ForEach(cedge => cedge.SetLength());
           
            if (this.HoleCpgLt != null)
            {
                foreach (var holecpg in this.HoleCpgLt)
                {
                    holecpg.SetCEdgeLtLength();
                }
            }
        }

        public override void SetCEdgeLtAxisAngle()
        {
            this.CEdgeLt.ForEach(cedge => cedge.SetAxisAngle());

            if (this.HoleCpgLt != null)
            {
                foreach (var holecpg in this.HoleCpgLt)
                {
                    holecpg.SetCEdgeLtAxisAngle();
                }
            }
        }

        public void SetAreaSimple()
        {
            var cpgSK = new Stack<CPolygon>();
            cpgSK.Push(this);

            do
            {
                var cpg = cpgSK.Pop();
                cpg.dblAreaSimple = CGeoFunc.CalAreaForCptEb(cpg.CptLt);

                //add the holes
                if (cpg.HoleCpgLt != null)
                {
                    foreach (var holecpg in cpg.HoleCpgLt)
                    {
                        cpgSK.Push(holecpg);
                    }
                }
            } while (cpgSK.Count > 0);
        }

        public override void SetAngleDiffLt()
        {
            var cpgSK = new Stack<CPolygon>();
            cpgSK.Push(this);

            do
            {
                var cpg = cpgSK.Pop();
                SetCpbAngleDiffLt(cpg as CPolyBase<CPolygon>);

                //add the holes
                if (cpg.HoleCpgLt != null)
                {
                    foreach (var holecpg in cpg.HoleCpgLt)
                    {
                        cpgSK.Push(holecpg);
                    }
                }
            } while (cpgSK.Count>0);
        }

        

        public override void JudgeAndFormCEdgeLt()
        {
            if (this.CEdgeLt == null)
            {
                FormCEdgeLt();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>SetPolygon will first set IPoint</remarks>
        public IPolygon4 SetPolygon()
        {
            //Build a polygon segment-by-segment.
            IPolygon4 polygon = new PolygonClass();

            IGeometryCollection geometryCollection = (IGeometryCollection)polygon;
            geometryCollection.AddGeometry(CGeoFunc.GetIrgFromCptLt(this.CptLt));
            //add the holes
            if (this.HoleCpgLt != null)
            {
                foreach (var holecpg in this.HoleCpgLt)
                {
                    geometryCollection.AddGeometry(CGeoFunc.GetIrgFromCptLt(holecpg.CptLt));
                }
            }

            this.pPolygon = polygon;
            return polygon;
        }

        public IPolygon4 JudgeAndSetPolygon()
        {
            if (_pPolygon == null)
            {
                return SetPolygon();
            }
            else
            {
                return _pPolygon;
            }
        }

        public override IGeometry JudgeAndSetAEGeometry()
        {
            return JudgeAndSetPolygon();
        }

        public override void SetAEGeometryNull()
        {
            _pPolygon = null;
        }

        public override IGeometry GetAEObject()
        {
            return _pPolygon;
        }

        public void SetGeometricProperties()
        {
            this.FormCEdgeLt();
            this.SetCEdgeLtLength();
            this.SetCEdgeLtAxisAngle();
            this.SetAngleDiffLt();
        }

        public void RemoveClosePoints()
        {
            var cpgSK = new Stack<CGeometry.CPolygon>();
            cpgSK.Push(this);

            do
            {
                var cpg = cpgSK.Pop();
                cpg.CptLt = CGeoFunc.RemoveClosePointsForCptEb(cpg.CptLt, true).ToList();

                if (cpg.HoleCpgLt != null)
                {
                    foreach (var holecpg in HoleCpgLt)
                    {
                        cpgSK.Push(holecpg);
                    }
                }
            } while (cpgSK.Count > 0);
        }

        public IEnumerable<List<CPoint>> GetHoleCptLtEb()
        {
            if (this.HoleCpgLt != null)
            {
                foreach (var holecpg in this.HoleCpgLt)
                {
                    yield return holecpg.CptLt;
                }
            }
        }

        public IEnumerable<CPoint> GetOuterCptEb(bool clockwise = true, bool blnIdentical = true)
        {
            var pOuterCmptCEdge = _OuterCmptCEdge;
            if (pOuterCmptCEdge == null)
            {
                throw new ArgumentException("Super face does not have a outer ring!");
            }

            if (clockwise == true)   //for an outer path, the edges are stored counter-clockwise in DCEL
            {
                foreach (var cpt in TraverseToGetCptEb(pOuterCmptCEdge, false))
                {
                    yield return cpt;
                }
            }
            else
            {
                foreach (var cpt in TraverseToGetCptEb(pOuterCmptCEdge, true))
                {
                    yield return cpt;
                }
            }

            if (blnIdentical == true)
            {
                yield return pOuterCmptCEdge.FrCpt;
            }
        }

        public IEnumerable<CPoint> GetInnerCptEb(CEdge cedgeComponent, bool clockwise = true, bool blnIdentical = true)
        {
            var pOuterCmptCEdge = cedgeComponent;

            if (clockwise == true)  //for an inner path, the edges are stored clockwise in DCEL
            {
                foreach (var cpt in TraverseToGetCptEb(pOuterCmptCEdge, true))
                {
                    yield return cpt;
                }
            }
            else
            {
                foreach (var cpt in TraverseToGetCptEb(pOuterCmptCEdge, false))
                {
                    yield return cpt;
                }
            }

            if (blnIdentical == true)
            {
                yield return pOuterCmptCEdge.FrCpt;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedgeComponent"></param>
        /// <param name="blnNext"></param>
        /// <returns></returns>
        private static IEnumerable<CPoint> TraverseToGetCptEb(CEdge cedgeComponent, bool blnNext)
        {
            var currentcedge = cedgeComponent;
            do
            {
                yield return currentcedge.FrCpt;

                if (blnNext == true)
                {
                    currentcedge = currentcedge.cedgeNext;
                }
                else
                {
                    currentcedge = currentcedge.cedgePrev;
                }

            } while (currentcedge.GID != cedgeComponent.GID);
        }


        public List<List<CPoint>> GetInnerCptLtLt(bool clockwise = true, bool blnIdentical = true)
        {
            var innercptltlt = new List<List<CPoint>>();
            if (_InnerCmptCEdgeLt != null && _InnerCmptCEdgeLt.Count > 0)
            {
                foreach (var cedgeInnerComponent in _InnerCmptCEdgeLt)
                {

                    innercptltlt.Add(GetInnerCptEb(cedgeInnerComponent, clockwise, blnIdentical).ToList());
                }
            }
            else
            {
                throw new ArgumentException("This face does not have inner components!");
            }
            return innercptltlt;
        }

        public List<CPoint> GetOnlyInnerCptLt(bool clockwise = true, bool blnIdentical = true)
        {
            //var innercptltlt = new List<List<CPoint>>();
            if (_InnerCmptCEdgeLt != null && _InnerCmptCEdgeLt.Count == 1)
            {
                return GetInnerCptEb(_InnerCmptCEdgeLt.GetFirstT(), clockwise, blnIdentical).ToList();
            }
            else
            {
                throw new ArgumentException("This face has no or more than one inner components!");
            }
        }


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="cedgeComponent"></param>
        ///// <returns></returns>
        ///// <remarks>we don't just use the FrCpt of OuterCmptCEdge as the start vertex, because a pair of corresponding faces may have different OuterCmptCEdge
        /////          Instead, we use the Cpt having the smallest indexID as the start vertex</remarks>
        //private List <CPoint> TraverseToGenerateCptLt(CEdge cedgeComponent)
        //{
        //    var MinFrIndexIDCEdge = cedgeComponent;
        //    var CurrentCEdge = cedgeComponent.cedgeNext;
        //    int intCount = 1;
        //    do
        //    {
        //        intCount++;
        //        if (CurrentCEdge.FrCpt.indexID < MinFrIndexIDCEdge.FrCpt.indexID)
        //        {
        //            MinFrIndexIDCEdge = CurrentCEdge;
        //        }
        //        //Console.WriteLine(CurrentCEdge.indexID + "___" + CurrentCEdge.indexID1 + "   " + CurrentCEdge.indexID2);
        //        CurrentCEdge = CurrentCEdge.cedgeNext;
        //    } while (CurrentCEdge.indexID != cedgeComponent.indexID);


        //    var cptlt = new List<CPoint>(intCount + 1);
        //    cptlt.Add(MinFrIndexIDCEdge.FrCpt);
        //    cptlt.Add(MinFrIndexIDCEdge.ToCpt);
        //    CurrentCEdge = MinFrIndexIDCEdge.cedgeNext;
        //    do
        //    {
        //        cptlt.Add(CurrentCEdge.ToCpt);
        //        CurrentCEdge = CurrentCEdge.cedgeNext;
        //    } while (CurrentCEdge.indexID != MinFrIndexIDCEdge.indexID);
        //    this.CptLt = cptlt;
        //    return cptlt;
        //}

        /// <summary>
        /// (counter clockwise???)
        /// </summary>
        /// <remarks>inner components</remarks>
        public List<CEdge> InnerCmptCEdgeLt
        {
            get { return _InnerCmptCEdgeLt; }
            set { _InnerCmptCEdgeLt = value; }
        }



        /// <summary>
        /// 
        /// </summary>
        /// <remarks>outer component</remarks>
        public CEdge OuterCmptCEdge
        {
            get { return _OuterCmptCEdge; }
            set { _OuterCmptCEdge = value; }
        }

        public CEdge cedgeStartAtLeftMost
        {
            get { return _cedgeStartAtLeftMost; }
            set { _cedgeStartAtLeftMost = value; }
        }



        /// <summary>
        /// 中心点
        /// </summary>
        public CPoint SetCentroidCptSimple()
        {
            double dblSumX = 0;
            double dblSumY = 0;
            var cptlt = this.CptLt;
            for (int i = 0; i < cptlt.Count - 1; i++)
            {
                dblSumX += cptlt[i].X;
                dblSumY += cptlt[i].Y;
            }

            this.CentroidCptSimple = new CPoint(0, dblSumX / (cptlt.Count - 1), dblSumY / (cptlt.Count - 1));
            return this.CentroidCptSimple;
        }

        //public void Clear()
        //{
        //    _InnerCmptCEdgeLt = null;
        //    this.CEdgeLt = null;
        //    _OuterCmptCEdge = null;
        //    _cedgeStartAtLeftMost = null;
        //    _CorrCGeo = null;
        //    _CorrCGeoLt = null;
        //    this.CptLt = null;
        //    _LeftMostCpt = null;
        //    //_pGeo = null;





        //}



        public CPoint LeftMostCpt
        {
            get { return _LeftMostCpt; }
            set { _LeftMostCpt = value; }
        }



        //public CPatch cpatch
        //{
        //    get { return _cpatch; }
        //    set { _cpatch = value; }
        //}

        public IPolygon4 pPolygon
        {
            get { return _pPolygon; }
            set
            {
                _pPolygon = value;
                _pGeo = value;
            }
        }

        public bool IsHole
        {
            get { return _IsHole; }
            set { _IsHole = value; }
        }

        public bool IsMerged
        {
            get { return _IsMerged; }
            set { _IsMerged = value; }
        }



        //public void SetPolygonAndEdge()
        //{
        //    if (this .pPolygon ==null)
        //    {
        //        IPointCollection4 pPntCtl = CHelpFunc.GetPointCollectionFromCptLt(cptlt);
        //        this.pPolygon = pPntCtl as IPolygon4;
        //        this.pPolygon.Close();
        //    }

        //    if (true)
        //    {
        //        ISegmentCollection pSegmentCollection = this.pPolygon as ISegmentCollection;
        //        List<CEdge> pedgelt = _CEdgeLt;
        //        for (int i = 0; i < pedgelt.Count ; i++)
        //        {
        //            pedgelt[i].pLine = pSegmentCollection.get_Segment(i) as ILine;
        //        }
        //    }

        //}

        //public void SetEdgeIncidentFace()
        //{
        //    foreach (CEdge  cedge in _CEdgeLt)
        //    {
        //        cedge.cpgIncidentFace = this;
        //    }
        //}



    }
}
