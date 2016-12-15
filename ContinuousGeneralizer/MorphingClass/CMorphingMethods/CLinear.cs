using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS .Carto ;

using MorphingClass.CUtility;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CMorphingMethods
{
    public class CLinear : CMorphingBaseCpl
    {
        //_pEvaluation = new CEvaluation();

        public CLinear(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline, CPolyline>(ParameterInitialize);
            _LSCPlLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolyline, object>().ToList();
            _SSCPlLt = this.ObjCGeoLtLt[1].ToExpectedClass<CPolyline, object>().ToList();
        }

        public CParameterResult LIMorphing()
        {
            List<CPolyline> pLSCPlLt = _LSCPlLt;
            List<CPolyline> pSSCPlLt = _SSCPlLt;

            long lngStartTime1 = System.Environment.TickCount;  //lngTime1
            pLSCPlLt.ForEach(cpl => cpl.SetEdgeLengthOnToCpt());
            pSSCPlLt.ForEach(cpl => cpl.SetEdgeLengthOnToCpt());
            long lngTime1 = System.Environment.TickCount - lngStartTime1;  //lngTime1

            double dblX = pSSCPlLt[0].FrCpt.X - pLSCPlLt[0].FrCpt.X;
            double dblY = pSSCPlLt[0].FrCpt.Y - pLSCPlLt[0].FrCpt.Y;
            CPoint StandardVectorCpt = new CPoint(0, dblX, dblY);
            _StandardVectorCpt = StandardVectorCpt;
            double dblStandardLength = CGeometricMethods.CalDis(0, 0, dblX, dblY);

            List<List<CCorrCpts>> pCorrCptsLtLt = new List<List<CCorrCpts>>(pLSCPlLt.Count);

            long lngStartTime2 = System.Environment.TickCount;  //lngTime2

            pCorrCptsLtLt = new List<List<CCorrCpts>>(pLSCPlLt.Count);   //initialize
            //pCtrlCptLtLt = new List<List<CCorrCpts>>(pLSCPlLt.Count);    //initialize

            for (int i = 0; i < LSCPlLt.Count; i++)
            {
                CLIA pCLIA = new CLIA(LSCPlLt[i], SSCPlLt[i]);
                pCorrCptsLtLt.Add(pCLIA.CLI().ToList ());
            }

            long lngTime2 = System.Environment.TickCount - lngStartTime2;   //lngTime2
            _ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime2) + "ms";  //显示运行时间

            CHelperFunction.SaveCorrLine(pCorrCptsLtLt, "Linear", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            _CorrCptsLtLt = pCorrCptsLtLt;

            //the results will be recorded in _ParameterResult
            CParameterResult ParameterResult = new CParameterResult();
            ParameterResult.pMorphingBase = this as CMorphingBase;
            _ParameterResult = ParameterResult;
            return ParameterResult;
        }




        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }

    }
}
