using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods ;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>COptCorSimplified</summary>
    /// <remarks>
    /// </remarks>
    public class COptCorSimplified
    {
        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        //private CHelperFunction _pHelperFunction = new CHelperFunction();
        //private CHelperFunctionExcel _pHelperFunctionExcel = new CHelperFunctionExcel();
        //private CMathStatistic _MathStatistic = new CMathStatistic();
        private CLinearInterpolationA _LinearInterpolationA = new CLinearInterpolationA();
        private CParameterResult _ParameterResult;
        private CTranslation _Translation;
        private CDeflection _Deflection;
        private CPoint _StandardVetorCpt;


        private CParameterInitialize _ParameterInitialize;

        public COptCorSimplified()
        {

        }

        public COptCorSimplified(CParameterInitialize ParameterInitialize)
        {

            //获取当前选择的点要素图层
            //大比例尺要素图层
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            //小比例尺要素图层
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           
            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;
            ParameterInitialize.intMaxBackK = Convert.ToInt32(ParameterInitialize.txtMaxBackK.Text);
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            List<CPolyline> _BSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pBSFLayer);
            List<CPolyline> _SSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pSSFLayer);

            _FromCpl = _BSCPlLt[0];
            _ToCpl = _SSCPlLt[0];
        }

        public void OptCorSimplifiedMorphing()
        {

            //CPolyline frcpl = _FromCpl;
            //CPolyline tocpl = _ToCpl;

            //frcpl.SetEdgeLength();
            //tocpl.SetEdgeLength();

            //int intMaxBackK = _ParameterInitialize.intMaxBackK;
            //double dblSmallDis = CGeometricMethods.CalSmallDis(frcpl);

            //List<CPolyline> CFrEdgeLt = CGeometricMethods.CreateCplLt(frcpl.cptlt);
            //List<CPolyline> CToEdgeLt = CGeometricMethods.CreateCplLt(tocpl.cptlt);
           
            //double dblX = tocpl.cptlt[0].X - frcpl.cptlt[0].X;
            //double dblY = tocpl.cptlt[0].Y - frcpl.cptlt[0].Y;
            //CPoint StandardVetorCpt = new CPoint(0, dblX, dblY);

            ////List<CPolyline> CFrEdgeLt = CGeometricMethods.CreateCplLt(frcpl.cptlt);
            ////List<CPolyline> CToEdgeLt = CGeometricMethods.CreateCplLt(tocpl.cptlt);

            ////CTable[,] T = CreatT(frcpl, tocpl, CFrEdgeLt, CToEdgeLt, intMaxBackK, StandardVetorCpt);  //创建T矩阵
            ////C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = FindCorrespondSegmentLk(T, frcpl, tocpl, CFrEdgeLt, CToEdgeLt);

            //long lngStartTime = System.Environment.TickCount;  //开始时间
            //List<CPolyline> frlastcpllt = new List<CPolyline>();
            //List<CPolyline> tolastcpllt = new List<CPolyline>();
            //CTable[,] T = CreatT(frcpl, tocpl, CFrEdgeLt, CToEdgeLt, intMaxBackK, StandardVetorCpt);  //创建T矩阵
            //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = FindCorrespondSegmentLk(T, frcpl, tocpl, CFrEdgeLt, CToEdgeLt);

            ////按指定方式对对应线段进行点匹配，提取对应点
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////保存对应线
            //CHelperFunction.SaveCtrlLine(pCorrespondSegmentLk, "OptCorSimplifiedControlLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCorrespondLine(pResultPtLt, "OptCorSimplifiedCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;

            //ParameterResult.CResultPtLt = pResultPtLt;
            //ParameterResult.lngTime = lngTime;
            //_ParameterResult = ParameterResult;

        }

        /// <summary>多个结果的Morphing方法</summary>
        /// <remarks>对于指定的回溯参数K，不断计算增大的结果，直到Translation值稳定</remarks>
        public void OptCorSimplifiedMultiResultsMorphing()
        {

            //CPolyline frcpl = _FromCpl;
            //CPolyline tocpl = _ToCpl;

            //int intMaxBackK = _ParameterInitialize.intMaxBackK;

            //List<CPolyline> CFrEdgeLt = CGeometricMethods.CreateCplLt(frcpl.cptlt);
            //List<CPolyline> CToEdgeLt = CGeometricMethods.CreateCplLt(tocpl.cptlt);

            //long lngStartTime = System.Environment.TickCount;  //开始时间
            //List<CPolyline> frlastcpllt = new List<CPolyline>();
            //List<CPolyline> tolastcpllt = new List<CPolyline>();

            //int intUpperbound = frcpl.cptlt.Count;
            //int intLowerbound = tocpl.cptlt.Count;

            //List<double> dblTranslationLt = new List<double>();
            //CTable[,] T = new CTable[intUpperbound, intLowerbound];
            //for (int i = 0; i < 5; i++)
            //{
            //    T = CreatT(frcpl, tocpl, CFrEdgeLt, CToEdgeLt, intMaxBackK);  //创建T矩阵                
            //    dblTranslationLt.Add(T[intUpperbound - 1, intLowerbound - 1].dblEvaluation);
            //    //if (dblTranslationLt.Count >= 5)
            //    //{
            //    //    int intCount = dblTranslationLt.Count;
            //    //    if ((dblTranslationLt[intCount - 1] == dblTranslationLt[intCount - 2]) && (dblTranslationLt[intCount - 1] == dblTranslationLt[intCount - 3]) &&
            //    //        (dblTranslationLt[intCount - 1] == dblTranslationLt[intCount - 4]) && (dblTranslationLt[intCount - 1] == dblTranslationLt[intCount - 5]))
            //    //    {
            //    //        break;
            //    //    }
            //    //}
            //    //break;
            //    intMaxBackK = intMaxBackK + 1;
            //}
            
            //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = FindCorrespondSegmentLk(T, frcpl, tocpl, CFrEdgeLt, CToEdgeLt);

            ////按指定方式对对应线段进行点匹配，提取对应点
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

            ////保存对应线
            //CHelperFunctionExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", _ParameterInitialize.strSavePath);
            //CHelperFunction.SaveCtrlLine(pCorrespondSegmentLk, "OptCorSimplifiedControlLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCorrespondLine(pResultPtLt, "OptCorSimplifiedCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;

            //ParameterResult.CResultPtLt = pResultPtLt;
            //ParameterResult.lngTime = lngTime;
            //_ParameterResult = ParameterResult;

        }


        public C5.LinkedList<CCorrespondSegment> DWByOptCorSimplified(CPolyline frcpl, CPolyline tocpl, int intMaxBackK, CPoint StandardVetorCpt)
        {
            double dblSmallDis = CGeometricMethods.CalSmallDis(frcpl);

            List<CPolyline> CFrEdgeLt = CGeometricMethods.CreateCplLt(frcpl.cptlt);
            List<CPolyline> CToEdgeLt = CGeometricMethods.CreateCplLt(tocpl.cptlt);

            CTable[,] T = CreatT(frcpl, tocpl, CFrEdgeLt, CToEdgeLt, intMaxBackK, StandardVetorCpt);  //创建T矩阵
            C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = FindCorrespondSegmentLk(T, frcpl, tocpl, CFrEdgeLt, CToEdgeLt);

            return pCorrespondSegmentLk;

        }
        /// <summary>创建T矩阵</summary>
        /// <param name="frcpl">大比例尺线状要素</param>
        /// <param name="tocpl">小比例尺线状要素</param> 
        /// <param name="CFrEdgeLt">大比例尺线段</param>  
        ///  <param name="CToEdgeLt">小比例尺线段</param> 
        /// <param name="frlastcpllt">大比例尺终点线段（只有一个点）</param> 
        /// <param name="tolastcpllt">小比例尺终点线段（只有一个点）</param>
        /// <param name="intMaxBackK">回溯系数</param> 
        /// <returns>T矩阵</returns>
        public CTable[,] CreatT(CPolyline frcpl, CPolyline tocpl, List<CPolyline> CFrEdgeLt, List<CPolyline> CToEdgeLt, int intMaxBackK, CPoint StandardVetorCpt)
        {

            //注意：T矩阵中的序号跟原文算法中的序号是统一的，但线数组中的序号则应减1
            CTable[,] T = new CTable[frcpl.cptlt.Count, tocpl.cptlt.Count];  //线段数量为顶点数量减1

            //数组的第一行及第一列初始化
            T[0, 0] = new CTable();
            T[0, 0].dblEvaluation = 0;

            CPolyline frfirstcpl = new CPolyline(0, frcpl.cptlt[0]);  //以线状要素的第一个点作为线段
            for (int j = 1; j <= CToEdgeLt.Count; j++)
            {
                T[0, j] = new CTable();
                T[0, j].dblEvaluation =-1;
            }

            for (int i = 1; i <= CFrEdgeLt.Count; i++)
            {
                T[i, 0] = new CTable();
                T[i, 0].dblEvaluation = -1;
            }


            int intI = CFrEdgeLt.Count;
            int intJ = CToEdgeLt.Count;
            if (intJ == 1)
            {
                T[intI, intJ] = new CTable();
                T[intI, intJ].frfrId = 1;
                T[intI, intJ].frtoId = intI;
                T[intI, intJ].tofrId = intJ;
                T[intI, intJ].intBackK = intI;
                CPolyline frcpli = frcpl.GetSubPolyline(CFrEdgeLt[intI - intI].cptlt[0], CFrEdgeLt[intI - 1].cptlt[1]);
                T[intI, intJ].dblEvaluation = T[0, 0].dblEvaluation + CalTDistance(frcpli, CToEdgeLt[intJ - 1], StandardVetorCpt);               
            }
            else if (intJ > 1)
            {
                //循环填满二维数组T中的各个值
                //注意：T中的序号1指定第一个元素，而各LT中（如CFrEdgeLt,tolastcpllt）的序号1则指定第二个元素
                //前提：仅存在的对应关系为每个“较小比例尺线状要素上的线段”对应一个或多个“较大比例尺线状要素上的线段”，即不存在“点对应线段”或“较大比例尺线状要素上的一个线段对应多个较小比例尺上的线段”
                for (int i = 1; i <= CFrEdgeLt.Count - 1; i++)               //计算各空格值
                {
                    //特殊情况（“较小比例尺线状要素上第一个线段”对应“较大比例尺线状要素前i个线段”）：基于“前提”，“较大比例尺线状要素上的第一个线段”必需“隶属于”“某个较小比例尺线状要素上的线段”，因此该步骤不能放入接下来的循环中
                    //此处以及下面关于j的循环中，似乎忘记考虑这个情况： CFrEdgeLt.Count-i>= CToEdgeLt.Count-j
                    T[i, 1] = new CTable();
                    T[i, 1].frfrId = 1;
                    T[i, 1].frtoId = i;
                    T[i, 1].tofrId = 1;
                    T[i, 1].intBackK = i;
                    CPolyline frcpli = frcpl.GetSubPolyline(CFrEdgeLt[0].cptlt[0], CFrEdgeLt[i - 1].cptlt[1]);
                    T[i, 1].dblEvaluation = T[0, 0].dblEvaluation + CalTDistance(frcpli, CToEdgeLt[0], StandardVetorCpt);

                    //在该循环的两个条件中：
                    //    1、“j = CToEdgeLt.Count”为特殊情况，“较小比例尺线状要素上的最后一个线段”必需有相应的对应线段，因此该步骤不能放入该循环中
                    //    2、“j <= i”，基于前提，必需在j <= i时，计算 T[i, j]才有意义
                    for (int j = 2; (j <= CToEdgeLt.Count - 1 && j <= i); j++)
                    {
                        SortedDictionary<double, CTable> dblCTableSlt = new SortedDictionary<double, CTable>(new CDblDecCompare());
                        for (int k = 1; k <= intMaxBackK; k++)
                        {
                            //if语句的两个条件中：
                            //    1、(i - k >= 1)：程序刚开始执行时，之前已遍历过线段数较少，可能小于intMaxBackK
                            //    2、基于“前提”，在较大比例尺线状要素上位于回溯范围之前的线段数量(i - k)必需大于较小比例尺线状要素上目标线段j之前的线段数量(j - 1)
                            //因此，限制条件应该为(i - k >= 1) && ((i - k) >=(j-1))，考虑到j>=2，因此第二个条件比第一个条件更加严格，可以省去第一个条件
                            if ((i - k) >= (j - 1))
                            {
                                CTable table5i = new CTable();
                                table5i.frfrId = i - k + 1;
                                table5i.frtoId = i;
                                table5i.tofrId = j;
                                table5i.intBackK = k;
                                CPolyline frcplik = frcpl.GetSubPolyline(CFrEdgeLt[i - k].cptlt[0], CFrEdgeLt[i - 1].cptlt[1]);
                                table5i.dblEvaluation = T[i - k, j - 1].dblEvaluation + CalTDistance(frcplik, CToEdgeLt[j - 1], StandardVetorCpt);
                                dblCTableSlt.Add(table5i.dblEvaluation, table5i);
                            }
                            else
                            {
                                break;
                            }
                        }
                        T[i, j] = dblCTableSlt.ElementAt(0).Value;
                    }
                }

                //最后一个元素 
                SortedDictionary<double, CTable> dblCTableSlt2 = new SortedDictionary<double, CTable>(new CDblDecCompare());
                for (int k = 1; k <= intMaxBackK; k++)
                {
                    //if语句的两个条件中：
                    //    1、(i - k >= 1)：程序刚开始执行时，之前已遍历过线段数较少，可能小于intMaxBackK
                    //    2、基于“前提”，在较大比例尺线状要素上位于回溯范围之前的线段数量(i - k)必需大于较小比例尺线状要素上目标线段j之前的线段数量(j - 1)
                    //因此，限制条件应该为(i - k >= 1) && ((i - k) >=(j-1))，考虑到j>=2，因此第二个条件比第一个条件更加严格，可以省去第一个条件
                    if ((intI - k) >= (intJ - 1))
                    {
                        CTable table5i = new CTable();
                        table5i.frfrId = intI - k + 1;
                        table5i.frtoId = intI;
                        table5i.tofrId = intJ;
                        table5i.intBackK = k;
                        CPolyline frcpli = frcpl.GetSubPolyline(CFrEdgeLt[intI - k].cptlt[0], CFrEdgeLt[intI - 1].cptlt[1]);
                        table5i.dblEvaluation = T[intI - k, intJ - 1].dblEvaluation + CalTDistance(frcpli, CToEdgeLt[intJ - 1], StandardVetorCpt);
                        dblCTableSlt2.Add(table5i.dblEvaluation, table5i);
                    }
                    else
                    {
                        break;
                    }
                }
                T[intI, intJ] = dblCTableSlt2.ElementAt(0).Value;
            }
           

            double dblTranslation = T[intI, intJ].dblEvaluation;
            return T;
        }


        /// <summary>以回溯的方式寻找对应线段</summary>
        /// <param name="frcpl">大比例尺线状要素</param>
        /// <param name="tocpl">小比例尺线状要素</param> 
        /// <param name="CFrEdgeLt">大比例尺线段</param>  
        ///  <param name="CToEdgeLt">小比例尺线段</param> 
        /// <param name="frlastcpllt">大比例尺终点线段（只有一个点）</param> 
        /// <param name="tolastcpllt">小比例尺终点线段（只有一个点）</param>  
        /// <returns>对应线段数组</returns>
        public C5.LinkedList<CCorrespondSegment> FindCorrespondSegmentLk(CTable[,] T,  CPolyline frcpl, CPolyline tocpl, List<CPolyline> CFrEdgeLt, List<CPolyline> CToEdgeLt)
        {
            C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            int i = CFrEdgeLt.Count;
            int j = CToEdgeLt.Count;

            while (i >= 0 && j >= 0)
            {
                CPolyline frcplw = new CPolyline();
                CPolyline tocplw = new CPolyline();
                if (i == 0 && j == 0)
                {
                    break;
                }               
                else  //其它行列的情况
                {
                    frcplw = frcpl.GetSubPolyline(CFrEdgeLt[i - T[i, j].intBackK].cptlt[0], CFrEdgeLt[i - 1].cptlt[1]);
                    tocplw = CToEdgeLt[j - 1];
                    i = i - T[i, j].intBackK;
                    j = j - 1;
                }

                CCorrespondSegment pCorrespondSegment = new CCorrespondSegment();
                pCorrespondSegment = new CCorrespondSegment(frcplw, tocplw);
                CorrespondSegmentLk.Insert(0,pCorrespondSegment);
            }

            return CorrespondSegmentLk;

        }






        /// <summary>计算距离值(Translation指标值)</summary>
        /// <param name="frcpl">大比例尺线段，可以只有一个顶点</param>
        /// <param name="tocpl">小比例尺线段，可以只有一个顶点</param> 
        /// <returns>距离值</returns>
        public double CalTDistance(CPolyline frcpl, CPolyline tocpl,CPoint StandardVetorCpt)
        {
            //List<CPoint> cresultptlt = _LinearInterpolationA.CLI(frcpl, tocpl);  //每次都相当于处理新的线段，因此使用CLI
            ////_Translation = new CTranslation();
            ////double dblTranslation = _Translation.CalTranslation(cresultptlt);
            ////return dblTranslation;


            //double dblDeflection = _Deflection.CalDeflection(cresultptlt, StandardVetorCpt);
            //return dblDeflection;
            return 0;
        }

        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
