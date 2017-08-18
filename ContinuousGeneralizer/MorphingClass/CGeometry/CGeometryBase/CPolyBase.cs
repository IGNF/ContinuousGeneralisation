using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

using MorphingClass.CUtility;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CPolyBase<CGeo> : CLineBase<CGeo>
        where CGeo : class
    {

        /// <summary>
        /// Whether the last vertex is the first vertex: My Cpg (yes, clockwise), Ipolygon (yes, clockwise), clipper (no, counter), Ipe (no)
        /// </summary>
        /// <remarks>
        /// for a polygon, the first point and the last point in cptlt must have the same coordinates, which is the same as in IPolygon
        /// for polygons, exterior rings have a clockwise orientation; Interior Rings have a counterclockwise orientation
        /// the first point and the last point of a output path are not identical
            //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon
        /// </remarks>
        public List<CPoint> CptLt { get; set; }
        public List<CEdge> CEdgeLt { get; set; }
        public List <double> dblAngleDiffLt { get; set; }  //we may need to extend this attribute to dblCornerAngleLtLt
        public double dblLengthSimple { get; set; }

        /// <summary>"Polyline" or "Polygon"</summary>
        public string strShape { get; set; }


        public CPolyBase()
        {

        }

        public CPolyBase(int fintID, List <CPoint> fcptlt)
        {

        }

        /// <summary>
        /// 按照原线段方向生成弯曲
        /// </summary>
        /// <remarks>如果已经生成过边，则直接返回</remarks>
        public virtual void FormPolyBase(List<CPoint> fcptlt)
        { 
            this.CptLt = fcptlt;
            //this.CEdgeLt = CGeoFunc.FormCEdgeEb(fcptlt).ToList();
        }

        public override void SetEnvelope()
        {
            this.CEnv = CGeoFunc.GetEnvelope(this.CptLt);
        }

        public virtual bool IsIntersectWith(CPolyBase<CGeo> pGeo, 
            bool blnTouchBothEnds = false, bool blnTouchEndEdge = false, bool blnOverlap = false)
        {
            foreach (var cedge in this.CEdgeLt)
            {
                foreach (var othercedge in pGeo.CEdgeLt)
                {
                    if (cedge.IsIntersectWith(othercedge, blnTouchBothEnds, blnTouchEndEdge, blnOverlap))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual CptEdgeDis CalMinDis(CPolyBase<CGeo> pGeo, bool blnCheckIntersect=true,
            bool blnTouchBothEnds = false, bool blnTouchEndEdge = false, bool blnOverlap = false)
        {
            if (blnCheckIntersect == true)
            {
                if (IsIntersectWith(pGeo, blnTouchBothEnds, blnTouchEndEdge, blnOverlap))
                {
                    throw new ArgumentOutOfRangeException("I haven't considered this case!");
                }
            }

            return CHelpFunc.Min(
                CalMinDisCptLtCEdgeLt(this.CptLt, pGeo.CEdgeLt),
                CalMinDisCptLtCEdgeLt(pGeo.CptLt, this.CEdgeLt), 
                ptedgedis => ptedgedis.dblDis);
        }

        private static CptEdgeDis CalMinDisCptLtCEdgeLt(IEnumerable<CPoint> cptlt, IEnumerable<CEdge> cedgelt)
        {
            var PtEdgeDis =new CGeometry.CptEdgeDis(double.MaxValue);
            foreach (var cpt in cptlt)
            {
                foreach (var cedge in cedgelt)
                {
                    PtEdgeDis = CHelpFunc.Min(PtEdgeDis, cpt.DistanceTo(cedge), ptedgedis => ptedgedis.dblDis);                    
                }
            }
            return PtEdgeDis;
        }

        

        //public void FormPolyBase(List<List<CPoint>> fcptltlt)
        //{
        //    if (fcptltlt.Count > 0)
        //    {
        //        if (fcptltlt[0].Count > 0)
        //        {
        //            _FrCpt = fcptltlt[0][0];
        //            _ToCpt = fcptltlt[0].GetLastT();
        //        }
        //    }


        //    this.CptLtLt = fcptltlt;
        //    //this.CEdgeLtLt = new List<List<CEdge>>
        //    //{new List<CEdge>() };

        //    //this.CEdgeLtLt[0] = new List<CEdge>();
        //    //this.SetCptLtLt(fcptltlt);
        //    //_CEdgeLt = CGeoFunc.FormCEdgeLt(fcptlt);
        //}

        //public void FormPolyBase(List<CPoint> fcptlt)
        //{
        //    var fcptltlt = new List<List<CPoint>> { fcptlt };
        //    FormPolyBase(fcptltlt);
        //}

        /// <summary>
        /// 按照原线段方向生成弯曲
        /// </summary>
        /// <remarks>如果已经生成过边，则直接返回</remarks>
        public virtual void FormCEdgeLt()
        {
            this.CEdgeLt = CGeoFunc.FormCEdgeEb(this.CptLt).ToList();

            //FormCEdgeLtLt();
        }

        public virtual int GetEdgeCount()
        {
            return this.CEdgeLt.Count;
        }

        //public virtual void FormCEdgeLtLt()
        //{
        //    this.CEdgeLtLt = CGeoFunc.FormCEdgeLtLt(this.CptLtLt);
        //}

        public virtual void SetCEdgeToCpts()
        {
            this.CEdgeLt.ForEach(cedge => cedge.SetCEdgeToCpts());
        }

        public virtual void SetCEdgeLtLength()
        {
            this.CEdgeLt.ForEach(cedge => cedge.SetLength());
        }



        //public virtual void SetCEdgeLtLtLength()
        //{
        //    this.CEdgeLtLt.ForEach(cedgelt => cedgelt.ForEach(cedge => cedge.SetLength()));
        //}



        public virtual double SetLengthSimple()
        {
            this.dblLengthSimple = CGeoFunc.CalLengthForCptEb(this.CptLt);

            if (this.dblLengthSimple == 0)
            {
                throw new ArgumentNullException("mightbe forgot to set _CptLt for a polygon!");
            }
            return this.dblLengthSimple;
        } 

        public virtual void SetCEdgeLtAxisAngle()
        {
            this.CEdgeLt.ForEach(cedge => cedge.SetAxisAngle());
        }

        public virtual void SetAngleDiffLt()
        {
            SetCpbAngleDiffLt(this as CPolyBase<CGeo>);
        }

        protected static void SetCpbAngleDiffLt(CPolyBase<CGeo> cpb)
        {
            var cedgelt = cpb.CEdgeLt;
            var dblAngleDiffLt = new List<double>(cedgelt.Count);
            dblAngleDiffLt.Add(CGeoFunc.CalAngle_Counterclockwise
                (cedgelt.GetLastT().dblAxisAngle, cedgelt[0].dblAxisAngle));
            for (int i = 0; i < cedgelt.Count - 1; i++)
            {
                dblAngleDiffLt.Add(CGeoFunc.CalAngle_Counterclockwise
                (cedgelt[i].dblAxisAngle, cedgelt[i + 1].dblAxisAngle));
            }
            cpb.dblAngleDiffLt= dblAngleDiffLt;
        }


        public virtual void JudgeAndFormCEdgeLt()
        {
            if (this.CEdgeLt == null)
            {
                FormCEdgeLt();
            }
        }

        /// <summary>
        /// compute the length of every edge, and store the value in the "ToCPoint"
        /// </summary>
        public void SetEdgeLengthOnToCpt()
        {
            CGeoFunc.SetEdgeLengthOnToCpt(this.CptLt);
        }

        ///// <summary>
        ///// we have to run GenerateDataAreaCEdgeLt first
        ///// </summary>
        ///// <param name="pParameterInitialize"></param>
        ///// <param name="str"></param>
        //public void SaveCEdgeLt(CParameterInitialize pParameterInitialize, string str)
        //{
        //    CSaveFeature.SaveCGeoEb(this.CEdgeLt, esriGeometryType.esriGeometryPolyline, str, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
        //}

        public void PrintCptlt()
        {
            CHelpFunc.PrintStart("cptlt of cpolybase " + this.ID);
            foreach (var cpt in this.CptLt)
            {
                cpt.PrintMySelf();
            }
            CHelpFunc.PrintEnd("cptlt of cpolybase " + this.ID);
        }

        /// <summary>
        /// Start of cedge
        /// </summary>
        public override CPoint FrCpt
        {
            get { return this.CptLt.First(); }
            set { this.CptLt.SetFirstT(value); }
        }

        /// <summary>
        /// End of cedge
        /// </summary>
        public override CPoint ToCpt
        {
            get { return this.CptLt.GetLastT(); }
            set { this.CptLt.SetLastT(value); }
        }


        /// <summary>
        /// cedges(clockwise)???
        /// </summary>
        //public List<CEdge> CEdgeLt
        //{
        //    //get { return _CEdgeLt; }
        //    //set { _CEdgeLt = value; }

        //    get { return this.CEdgeLtLt[0]; }
        //    set
        //    {
        //        this.CEdgeLtLt[0] = value;
        //    }
        //}

        /// <summary>
        /// vertices(clockwise)??? ; 
        /// for a polygon, the first point and the last point in cptlt must have the same coordinates
        /// for a polygon, CptLt must be CptLtLt[0]
        /// </summary>
        //public List<CPoint> CptLt
        //{
        //    get { return this.CptLtLt[0]; }
        //    set
        //    {
        //        this.CptLtLt[0] = value;
        //    }
        //}
    }
}
