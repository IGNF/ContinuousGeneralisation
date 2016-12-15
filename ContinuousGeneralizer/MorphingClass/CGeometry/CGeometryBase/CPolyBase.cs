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
        
        protected List<CEdge> _CEdgeLt;     // cedges(clockwise)???
        
        
        // vertices(clockwise)??? ; for a polygon, the first point and the last point in cptlt must have the same coordinates
        protected List<List<CPoint>> _CptLtLt;
        public double dblLengthSimple { get; set; }


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
            _FrCpt = fcptlt[0];
            _ToCpt = fcptlt.GetLast_T();

            this.CptLt = fcptlt;
            //_CEdgeLt = CGeometricMethods.FormCEdgeLt(fcptlt);
        }

        /// <summary>
        /// 按照原线段方向生成弯曲
        /// </summary>
        /// <remarks>如果已经生成过边，则直接返回</remarks>
        public virtual void FormCEdgeLt()
        {
            _CEdgeLt = CGeometricMethods.FormCEdgeLt(this.CptLt);
        }

        public virtual double SetLengthSimple()
        {
            this.dblLengthSimple = CGeometricMethods.CalLengthForCptEb(this.CptLt);

            if (this.dblLengthSimple == 0)
            {
                throw new ArgumentNullException("mightbe forgot to set _CptLt for a polygon!");
            }
            return this.dblLengthSimple;
        } 

        public virtual void SetCEdgeLength()
        {
            _CEdgeLt.ForEach(cedge => cedge.SetLength());
        }

        public void JudgeAndFormCEdgeLt()
        {
            if (_CEdgeLt == null)
            {
                _CEdgeLt = CGeometricMethods.FormCEdgeLt(this.CptLt);
            }
        }

        /// <summary>
        /// compute the length of every edge, and store the value in the "ToCPoint"
        /// </summary>
        public void SetEdgeLengthOnToCpt()
        {
            CGeometricMethods.SetEdgeLengthOnToCpt(this.CptLt);
        }

        /// <summary>
        /// we have to run GenerateDataAreaCEdgeLt first
        /// </summary>
        /// <param name="pParameterInitialize"></param>
        /// <param name="str"></param>
        public void SaveCEdgeLt(CParameterInitialize pParameterInitialize, string str)
        {
            CSaveFeature.SaveCGeoEb(_CEdgeLt, esriGeometryType.esriGeometryPolyline, str, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
        }

        public void PrintCptlt()
        {
            CHelperFunction.PrintStart("cptlt of cpolybase " + this.ID);
            foreach (var cpt in this.CptLt)
            {
                cpt.PrintMySelf();
            }
            CHelperFunction.PrintEnd("cptlt of cpolybase " + this.ID);
        }

        /// <summary>
        /// Start of cedge
        /// </summary>
        public override CPoint FrCpt
        {
            get { return this.CptLt.GetFirstT(); }
            set { this.CptLt.SetFirstT(value); }
        }

        /// <summary>
        /// End of cedge
        /// </summary>
        public override CPoint ToCpt
        {
            get { return this.CptLt.GetLast_T(); }
            set { this.CptLt.SetLast_T(value); }
        }


        /// <summary>
        /// cedges(clockwise)???
        /// </summary>
        public List<CEdge> CEdgeLt
        {
            get { return _CEdgeLt; }
            set { _CEdgeLt = value; }
        }

        /// <summary>
        /// vertices(clockwise)??? ; 
        /// for a polygon, the first point and the last point in cptlt must have the same coordinates
        /// for a polygon, CptLt must be CptLtLt[0]
        /// </summary>
        public List<CPoint> CptLt
        {
            get { return this.CptLtLt[0]; }
            set
            {
                this.CptLtLt = new List<List<CPoint>>(1);
                this.CptLtLt.Add(value);
            }
        }

        //CptLtLt[0] indicates the only exterior ring of the polygon
        public List<List<CPoint>> CptLtLt
        {
            get { return _CptLtLt; }
            set { _CptLtLt = value; }
            //set
            //{
            //    _CptLtLt = value;
            //    this.CptLt = _CptLtLt[0];
            //}
        }


        //public void AddToCptLtLt(List <CPoint> cptlt)
        //{
        //    _CptLtLt.Add(cptlt);
        //}

        //public void SetCptLtLt(List<List<CPoint>> cptltlt)
        //{
        //    _CptLtLt = cptltlt;
        //}
    }
}
