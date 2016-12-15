using System;
using System.Collections.Generic;
using System.Text;

using MorphingClass.CCorrepondObjects ;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CEntity;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CMorphingMethods .CMorphingMethodsBase ;
using MorphingClass.CEvaluationMethods ;


using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesFile;

namespace MorphingClass.CUtility
{
    /// <summary>
    /// 结果参数类，继承于参数类
    /// </summary>
    /// <remarks></remarks>
    public class CParameterResult
    {
        private long _lngTime;     //运行时间
        //private double _dblTranslation;
        //private double _dblVtPV;

        private List<CPoint> _FromPtLt;
        private List<CPoint> _ToPtLt;
        private List<CPoint> _CResultPtLt;
        private List<CPoint> _TranlationPtLt;
        private List<List<CPoint>> _CResultPtLtLt;
        private List<CRiver> _CResultRiverLt;
        private List<CPolyline> _CInitialPlLt;
        private List<CPolyline> _CResultPlLt;
        private List<CPolyline> _CResultColorPlLt;
        private List<CPolyline> _CTrajectoryPlLt;
        private List<CPolyline> _DisplayCPlLt;
        private List<CPolyline> _FadedDisplayCPlLt;
        private List<CCorrespondRiverNet> _CResultCorrespondRiverNetLt;
        private List<CCorrCpts> _CCorrCptsLt;
        //private List<List<CCorrCpts>> _CorrCptsLtLt;
        //private List<CEdge> _CResultCCLLt;   //CCL: Characteristic Corresponding Line

        private List<CAtBd> _CBSAtBdLt;
        private List<CAtBd> _CSSAtBdLt;
        private List<CAtBd> _CSgAtBdLt;

        //private List<CPolyline> _InterLSCPlLt;
        //private List<CPolyline> _InterTransSgCPlLt;


        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private List<double> _dblIntegralLt;
        private List<double> _dblSumIntegralLt;
        private List<double> _dblTranslationLt;
        private List<double> _dblSumTranslationLt;
        private List<List<double>> _dblTranslationLtLt;
        private List<List<double>> _dblSumTranslationLtLt;

        //private LinkedList<CCorrespondSegment> _pCorrespondSegmentLk;
        //private CLIMultiA _LIMultiACorr;
        //private CLIMultiA _LIMultiASingle;

        private enumEvaluationMethods _enumEvaluationMethod;
        private string _strEvaluationMethod;
        private CMorphingBase _pMorphingBase;
        private CMorphingBaseCpl _pCMorphingBaseCpl;
        //private CMorphingBaseCpg _pCMorphingBaseCpg;
        private CEvaluation _pEvaluation;



        /// <summary>初始化函数</summary>
        public CParameterResult()
        {

            //pOutRaster = new RasterClass();
        }


        /// <summary>属性：运行时间</summary>
        public long lngTime
        {
            get { return _lngTime; }
            set { _lngTime = value; }
        }

        ///// <summary>属性：一个指标值</summary>
        //public double dblTranslation
        //{
        //    get { return _dblTranslation; }
        //    set { _dblTranslation = value; }
        //}

        ///// <summary>属性：平差中的VtPV</summary>
        //public double dblVtPV
        //{
        //    get { return _dblVtPV; }
        //    set { _dblVtPV = value; }
        //}

        public List<CPoint> FromPtLt
        {
            get { return _FromPtLt; }
            set { _FromPtLt = value; }
        }

        public List<CPoint> ToPtLt
        {
            get { return _ToPtLt; }
            set { _ToPtLt = value; }
        }

        public List<CPoint> CResultPtLt
        {
            get { return _CResultPtLt; }
            set { _CResultPtLt = value; }
        }

        public List<CCorrCpts> CCorrCptsLt
        {
            get { return _CCorrCptsLt; }
            set { _CCorrCptsLt = value; }
        }

        //public List<List<CCorrCpts>> CorrCptsLtLt
        //{
        //    get { return _CorrCptsLtLt; }
        //    set { _CorrCptsLtLt = value; }
        //}

        public List<CPoint> TranlationPtLt
        {
            get { return _TranlationPtLt; }
            set { _TranlationPtLt = value; }
        }


        public List<CCorrespondRiverNet> CResultCorrespondRiverNetLt
        {
            get { return _CResultCorrespondRiverNetLt; }
            set { _CResultCorrespondRiverNetLt = value; }
        }

        public CPolyline FromCpl
        {
            get { return _FromCpl; }
            set { _FromCpl = value; }
        }

        public CPolyline ToCpl
        {
            get { return _ToCpl; }
            set { _ToCpl = value; }
        }

        public List<double> dblIntegralLt
        {
            get { return _dblIntegralLt; }
            set { _dblIntegralLt = value; }
        }

        public List<double> dblSumIntegralLt
        {
            get { return _dblSumIntegralLt; }
            set { _dblSumIntegralLt = value; }
        }

        public List<double> dblTranslationLt
        {
            get { return _dblTranslationLt; }
            set { _dblTranslationLt = value; }
        }

        public List<double> dblSumTranslationLt
        {
            get { return _dblSumTranslationLt; }
            set { _dblSumTranslationLt = value; }
        }

        public List<List<double>> dblTranslationLtLt
        {
            get { return _dblTranslationLtLt; }
            set { _dblTranslationLtLt = value; }
        }

        public List<List<double>> dblSumTranslationLtLt
        {
            get { return _dblSumTranslationLtLt; }
            set { _dblSumTranslationLtLt = value; }
        }

        public enumEvaluationMethods enumEvaluationMethod
        {
            get { return _enumEvaluationMethod; }
            set { _enumEvaluationMethod = value; }
        }

        public string strEvaluationMethod
        {
            get { return _strEvaluationMethod; }
            set { _strEvaluationMethod = value; }
        }

        
        public List<List<CPoint>> CResultPtLtLt
        {
            get { return _CResultPtLtLt; }
            set { _CResultPtLtLt = value; }
        }

        public List<CPolyline> CInitialPlLt
        {
            get { return _CInitialPlLt; }
            set { _CInitialPlLt = value; }
        }

        public List<CPolyline> FadedDisplayCPlLt
        {
            get { return _FadedDisplayCPlLt; }
            set { _FadedDisplayCPlLt = value; }
        }

        public List<CPolyline> DisplayCPlLt
        {
            get { return _DisplayCPlLt; }
            set { _DisplayCPlLt = value; }
        }

        public List<CRiver> CResultRiverLt
        {
            get { return _CResultRiverLt; }
            set { _CResultRiverLt = value; }
        }

        public List<CPolyline> CResultPlLt
        {
            get { return _CResultPlLt; }
            set { _CResultPlLt = value; }
        }

        //public List<CPolyline> CResultColorPlLt
        //{
        //    get { return _CResultColorPlLt; }
        //    set { _CResultColorPlLt = value; }
        //}

        public List<CPolyline> CTrajectoryPlLt
        {
            get { return _CTrajectoryPlLt; }
            set { _CTrajectoryPlLt = value; }
        }

        //public LinkedList<CCorrespondSegment> pCorrespondSegmentLk
        //{
        //    get { return _pCorrespondSegmentLk; }
        //    set { _pCorrespondSegmentLk = value; }
        //}

        //public List<CEdge> CResultCCLLt
        //{
        //    get { return _CResultCCLLt; }
        //    set { _CResultCCLLt = value; }
        //}

        public List<CAtBd> CBSAtBdLt
        {
            get { return _CBSAtBdLt; }
            set { _CBSAtBdLt = value; }
        }

        public List<CAtBd> CSSAtBdLt
        {
            get { return _CSSAtBdLt; }
            set { _CSSAtBdLt = value; }
        }

        public List<CAtBd> CSgAtBdLt
        {
            get { return _CSgAtBdLt; }
            set { _CSgAtBdLt = value; }
        }

        //public List<CPolyline> InterLSCPlLt
        //{
        //    get { return _InterLSCPlLt; }
        //    set { _InterLSCPlLt = value; }
        //}

        //public List<CPolyline> InterTransSgCPlLt
        //{
        //    get { return _InterTransSgCPlLt; }
        //    set { _InterTransSgCPlLt = value; }
        //}

        //public CLIMultiA LIMultiACorr
        //{
        //    get { return _LIMultiACorr; }
        //    set { _LIMultiACorr = value; }
        //}

        //public CLIMultiA LIMultiASingle
        //{
        //    get { return _LIMultiASingle; }
        //    set { _LIMultiASingle = value; }
        //}


        public CMorphingBaseCpl pMorphingBaseCpl
        {
            get { return _pCMorphingBaseCpl; }
            set { _pCMorphingBaseCpl = value; }
        }

        //public CMorphingBaseCpg pMorphingBaseCpg
        //{
        //    get { return _pCMorphingBaseCpg; }
        //    set { _pCMorphingBaseCpg = value; }
        //}





        public CMorphingBase pMorphingBase
        {
            get { return _pMorphingBase; }
            set { _pMorphingBase = value; }
        }

        public CEvaluation pEvaluation
        {
            get { return _pEvaluation; }
            set { _pEvaluation = value; }
        }

    }
}
