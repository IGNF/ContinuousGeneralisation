using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;


namespace MorphingClass.CMorphingAlgorithms
{
    /// <summary>
    /// a class based Algorithm of Linear Interpolation
    /// </summary>
    /// <remarks>the result is in InterFrCPl</remarks>
    public class CLIMultiA : CMorphingBaseCpl
    {
        public CLIMultiA(List<CPolyline> LSCPlLt, List<CPolyline> SSCPlLt)
        {
            _LSCPlLt = LSCPlLt;
            _SSCPlLt = SSCPlLt;
        }

        /// <summary>
        /// LI方法获取对应点，此函数为LI方法的入口函数(仅对一对对应线段处理的时候从此处调用，因为该函数顾及了线段的第一个顶点)
        /// </summary>
        /// <param name="CFrPolyline">大比例尺线段，可以只有一个顶点</param>
        /// <param name="CToPolyline">小比例尺线段，可以只有一个顶点</param>
        /// <param name="ResultPtLt">结果数组</param>
        /// <remarks></remarks>
        public CParameterResult CLIMulti()
        {
            List<CPolyline> LSCPlLt = _LSCPlLt;
            List<CPolyline> SSCPlLt = _SSCPlLt;

            //List<CLIA> pCLIALt = new List<CLIA>(LSCPlLt.Count);
            List<List<CCorrCpts>> pCorrCptsLtLt = new List<List<CCorrCpts>>(LSCPlLt.Count);
            for (int i = 0; i < LSCPlLt.Count; i++)
            {
                CLIA pCLIA = new CLIA(LSCPlLt[i], SSCPlLt[i]);
                pCorrCptsLtLt.Add(pCLIA.CLI().ToList());
            }
            _CorrCptsLtLt = CorrCptsLtLt;

            //the results will be recorded in _ParameterResult
            CParameterResult ParameterResult = new CParameterResult();
            ParameterResult.pMorphingBaseCpl = this as CMorphingBaseCpl;
            _ParameterResult = ParameterResult;
            return ParameterResult;
        }
    }
}
