using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
//using System.Windows;

using SCG = System.Collections.Generic;
using C5;
using Microsoft.Office.Interop;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.esriSystem;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using VBClass;
//using EasyMorphing.
//using MorphingClass.

namespace MorphingClass.CUtility
{



    public static class CHelpFunc
    {
        private static string _strPath;



        public static IEnumerable<CPoint> GetTestCptEb()
        {
            yield return new CPoint(0, 0, 0);
            yield return new CPoint(1, 1, 0);
            yield return new CPoint(2, 0, 1);
            yield return new CPoint(3, 0, 0);
        }

        /// <summary>
        /// the path to the folder that contains the .mxd
        /// </summary>
        public static string strPath
        {
            get { return _strPath; }
            set { _strPath = value; }
        }

        public static string strDataFolderName { get; set; }


        public static bool InBoundOrReport<T>(T value, T lowerbound, T upperbound, IComparer<T> cmp = null)
        {
            if (cmp == null) { cmp = SCG.Comparer<T>.Default; }
            //var tst = cmp.Compare(value, lowerbound);
            if (cmp.Compare(value, lowerbound) == -1 || cmp.Compare(value, upperbound) == 1)
            {
                throw new ArgumentException("incorrect value!");
            }

            return true;
        }

        public static void testmemory()
        {
            SCG.IList<SCG.IList<double>> intltlt = new List<SCG.IList<double>>();

            //while (intltlt.Count < 40)
            //{
            //    intltlt.Add(testttt());
            //}


            while (intltlt.Count < 1000000)
            {
                intltlt.Add(testttt());
            }

            while (intltlt.Count < 2000000)
            {
                intltlt.Add(testttt());
            }

            int st = 5;
            int sq = st;
        }

        public static System.Collections.Generic.IList<double> testttt()
        {
            int intLimit = 1000000;
            System.Collections.Generic.IList<double> intlt = new List<double>(intLimit);
            while (intlt.Count < intLimit)
            {
                intlt.Add(3.0);
            }
            return intlt;
        }


        public static double GetConsumedMemoryInMB(bool blnforceFullCollection, long lngStartMemoryInByte = 0)
        {
            return Math.Round(Convert.ToDouble(GC.GetTotalMemory(blnforceFullCollection) - lngStartMemoryInByte) / 1048576, 3);
        }

        public static void SetSavePath(CParameterInitialize ParameterInitialize, bool blnCreateFileGdbWorkspace = false)
        {
            //if we have already set a path, then we simply use that path
            if (ParameterInitialize.strMxdPathBackSlash != null)
            {
                return;
            }

            //_strPath, which is different from ParameterInitialize.strPath, is defined in CHelpFunc
            if (_strPath == null)
            {
                SaveFileDialog SFD = new SaveFileDialog();
                SFD.ShowDialog();
                if (SFD.FileName == null || SFD.FileName == "") return;
                _strPath = SFD.FileName;
            }

            string strFileName = _strPath + "\\" + GetTimeStamp();
            //string strFileName = CHelpFunc.strPath + "MorphingResults";

            ParameterInitialize.strMxdPath = _strPath;
            ParameterInitialize.strMxdPathBackSlash = _strPath + "\\";
            ParameterInitialize.strSaveFolderName = System.IO.Path.GetFileNameWithoutExtension(strFileName);
            ParameterInitialize.strSavePath = strFileName;
            ParameterInitialize.strSavePathBackSlash = strFileName + "\\";
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strFileName);

            if (blnCreateFileGdbWorkspace == true)
            {
                ParameterInitialize.pFileGdbWorkspace =
                    CreateFileGdbWorkspace(ParameterInitialize.strSavePathBackSlash, "FileGdb");
            }
        }

        /// <summary>
        /// the prefix is '_'
        /// </summary>
        public static string GetTimeStampWithPrefix()
        {
            return "_" + GetTimeStamp();
        }

        public static string GetTimeStamp()
        {
            var strMonth = JudgeAndAddZero(DateTime.Now.Month);
            var strDay = JudgeAndAddZero(DateTime.Now.Day);
            var strHour = JudgeAndAddZero(DateTime.Now.Hour);
            var strMinute = JudgeAndAddZero(DateTime.Now.Minute);
            var strSecond = JudgeAndAddZero(DateTime.Now.Second);
            var strMillisecond = JudgeAndAddZero(DateTime.Now.Millisecond, 3);
            return DateTime.Now.Year.ToString() + strMonth + strDay + "_" + strHour + strMinute + strSecond + strMillisecond;
        }

        public static string JudgeAndAddZero(double dblNumber, int intDigits = 2)
        {
            var dblTest = Math.Pow(10, Convert.ToDouble(intDigits - 1));

            string strPrefix = "";
            while (dblTest > 1)
            {
                if (dblNumber < dblTest)
                {
                    strPrefix += "0";
                    dblTest = dblTest / 10;
                }
                else
                {
                    break;
                }
            }
            return strPrefix + dblNumber.ToString();
        }

        public static void Displaytspb(double dblValue, double dblTotal)
        {
            CConstants.ParameterInitialize.tspbMain.Value = Convert.ToInt32(dblValue * 100 / dblTotal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lngTimeMs"></param>
        /// <param name="strName"></param>
        /// <param name="lngTimeMs2"></param>
        /// <param name="strName2"></param>
        /// <remarks>set CConstants.ParameterInitialize = ParameterInitialize in CMorphingBase.cs</remarks>
        public static void DisplayRunTime(long lngTimeMs, string strName = "", long lngTimeMs2 = 0, string strName2 = "")
        {
            if (strName == "")
            {
                strName = "RunTime";
            }
            CConstants.ParameterInitialize.tsslTime.Text = strName + ": " + lngTimeMs + " ms";

            if (lngTimeMs2 != 0)
            {
                CConstants.ParameterInitialize.tsslTime.Text += "; " + strName2 + ": " + lngTimeMs2 + " ms";
            }
        }

        public static List<object> GetObjLtByFeatureLayer(IFeatureLayer pFeatureLayer, out List<List<object>> pobjectValueLtLt,
            string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！
            int intFieldCount = pFeatureClass.Fields.FieldCount;
            pobjectValueLtLt = new List<List<object>>(intFeatureCount);


            int intSpecifiedFieldIndex = -1;
            if (strSpecifiedFieldName != null)
            {
                intSpecifiedFieldIndex = pFeatureClass.FindField(strSpecifiedFieldName);
                pobjectValueLtLt = new List<List<object>>();
            }

            var ObjShapeLt = new List<object>(intFeatureCount);
            for (int i = 0; i < intFeatureCount; i++)
            {
                IFeature pFeature = pFeatureCursor.NextFeature();
                if (strSpecifiedFieldName != null)  //to only get the specified feature
                {
                    if (pFeature.get_Value(intSpecifiedFieldIndex).ToString() != strSpecifiedValue)
                    {
                        continue;
                    }
                }

                //TestIGeoAccordingToInput(pFeature.Shape, i);
                ObjShapeLt.Add(pFeature.Shape);

                var ObjValueLt = new List<object>(intFieldCount - 2);  //we don't need the first two values, i.e., Id and shape
                for (int j = 2; j < intFieldCount; j++)
                {
                    ObjValueLt.Add(pFeature.get_Value(j));
                }
                pobjectValueLtLt.Add(ObjValueLt);
            }

            return ObjShapeLt;
        }

        public static IEnumerable<CGeoBase> GenerateCGeoEbAccordingToInputLt(List<object> pObjIGeoLt, double dblFactor = 1)
        {
            for (int i = 0; i < pObjIGeoLt.Count; i++)
            {
                yield return GenerateCGeoAccordingToInput((IGeometry)pObjIGeoLt[i], i, dblFactor);
            }
        }

        public static IEnumerable<T> GenerateCGeoEbAccordingToGeoEb<T>(IEnumerable<IGeometry> IGeoEb)
            where T: CGeoBase
        {
            var IGeoEt = IGeoEb.GetEnumerator();
            int intCount = 0;
            while (IGeoEt.MoveNext())
            {
                yield return (T)GenerateCGeoAccordingToInput(IGeoEt.Current, intCount++);
            }
        }



        public static void TestIGeoAccordingToInput(IGeometry pGeo, int intIndex)
        {
            switch (pGeo.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:               //point******************
                    break;
                case esriGeometryType.esriGeometryPolygon:             //polygon******************
                    var ipg = pGeo as IPolygon4;
                    if (ipg.Length <= 0)
                    {
                        throw new ArgumentNullException("Polygon " + intIndex + " has length 0!");
                    }

                    var pArea = ipg as IArea;
                    if (pArea.Area <= 0)
                    {
                        throw new ArgumentNullException("Polygon " + intIndex + " has area 0!");
                    }
                    break;
                case esriGeometryType.esriGeometryPolyline:            //polyline******************
                    var ipl = pGeo as IPolyline5;
                    if (ipl.Length <= 0)
                    {
                        throw new ArgumentNullException("Polyline " + intIndex + " has length 0!");
                    }
                    break;
                default:
                    break;
            }
        }

        public static CGeoBase GenerateCGeoAccordingToInput(IGeometry pGeo, int intID = -2, double dblFactor = 1)
        {
            CGeoBase obj = null;
            switch (pGeo.GeometryType)
            {
                case esriGeometryType.esriGeometryPoint:               //point******************
                    obj = new CPoint(intID, (IPoint)pGeo);
                    break;
                case esriGeometryType.esriGeometryPolygon:             //polygon******************
                    obj = new CPolygon(intID, (IPolygon4)pGeo, dblFactor);
                    break;
                case esriGeometryType.esriGeometryPolyline:            //polyline******************
                    obj = new CPolyline(intID, (IPolyline5)pGeo);
                    break;
                default:
                    break;
            }
            return obj;
        }




        /// <summary>
        /// 通过线要素图层获取线数组
        /// </summary>
        /// <param name="pFeatureLayer">线要素图层</param>
        /// <returns>线数组</returns>
        public static List<CPolyline> GetCPlLtByFeatureLayer(IFeatureLayer pFeatureLayer)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！
            List<CPolyline> CPlLt = new List<CPolyline>(intFeatureCount);    //先创建线数组，以获取图层中的线段
            for (int i = 0; i < intFeatureCount; i++)
            {
                IFeature pFeature = pFeatureCursor.NextFeature();
                IPolyline5 pPolyline = new PolylineClass();
                pPolyline = (IPolyline5)pFeature.Shape;
                SetZCoordinates(pPolyline as IPointCollection4);  //set the z coordinates, it may be used in Constructing TIN


                CPolyline cpl = new CPolyline(i, pPolyline);
                CPlLt.Add(cpl);
            }

            return CPlLt;
        }


        public static List<CPoint> GetCPtLtFromPointFeatureLayer(IFeatureLayer pFeatureLayer)
        {

            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！
            List<CPoint> CPtLt = new List<CPoint>(intFeatureCount);
            for (int i = 0; i < intFeatureCount; i++)
            {
                IFeature pFeature = pFeatureCursor.NextFeature();
                IPoint ipt = (IPoint)pFeature.Shape;
                CPoint cpt = new CPoint(i, ipt);
                CPtLt.Add(cpt);
            }

            return CPtLt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCol"></param>
        /// <remarks>note that if we set z coordinates, then we may lose many information related to IPolygon (ConnectedComponentBag, InteriorRingBag, ExteriorRingBag, etc.) </remarks>
        public static void SetZCoordinates(IPointCollection4 pCol)
        {
            for (int i = 0; i < pCol.PointCount; i++)
            {
                IPoint copyipt = pCol.get_Point(i);
                copyipt.Z = 0;
                pCol.UpdatePoint(i, copyipt);
            }
        }
        //public static void PreviousWorkCplLt(ref List<CPolyline> cpllt, CEnumScale enumScale)
        //{
        //    foreach (CPolyline cpl in cpllt)
        //    {
        //        CPolyline cpli = cpl;
        //        PreviousWorkCpl(ref cpli, enumScale);
        //    }
        //}

        //public static void PreviousWorkCSeLt(ref C5.LinkedList<CCorrSegment> CorrespondSegmentLk)
        //{
        //    foreach (CCorrSegment pCorrespondSegment in CorrespondSegmentLk)
        //    {
        //        CPolyline frcpl = pCorrespondSegment.CFrPolyline;
        //        CPolyline tocpl = pCorrespondSegment.CToPolyline;
        //        PreviousWorkCpl(ref frcpl, CEnumScale.Larger);
        //        PreviousWorkCpl(ref tocpl, CEnumScale.Smaller);
        //    }
        //}

        public static void SetAbsAndRatioLength(ref CPolyline cpl, CEnumScale enumScale)
        {
            //cpl.enumScale = enumScale;
            //cpl.SetCptBelongedPolyline();
            //cpl.SetEdgeLength();
            cpl.SetAbsLengthFromStart();
            cpl.SetRatioLengthFromStart();

        }


        //public static void SetCEdgeCEdgeTwinBelongedCpl(ref List<CPolyline> cpllt)
        //{
        //    foreach (CPolyline cpl in cpllt)
        //    {
        //        cpl.SetCEdgeBelongedPolyline();
        //        cpl.SetCEdgeTwinBelongedPolyline();
        //    }
        //}


        ///// <summary>
        ///// transform some items into a list
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks >we allow at most three items here. the number of items could be increased if necessary</remarks>
        //public static List<T> MakeLt<T>(T item)
        //   where T : class
        //{
        //    return MakeLt(1, item);
        //}


        /// <summary>
        /// transform some items into a list
        /// </summary>
        /// <returns></returns>
        /// <remarks >we allow at most three items here. the number of items could be increased if necessary</remarks>
        public static List<T> MakeLt<T>(T item1 = null, T item2 = null, T item3 = null)
           where T : class
        {
            List<T> TLt = new List<T>();

            if (item1 != null)
            {
                TLt.Add(item1);
            }
            if (item2 != null)
            {
                TLt.Add(item2);
            }
            if (item3 != null)
            {
                TLt.Add(item3);
            }

            return TLt;
        }

        ///// <summary>
        ///// transform some items into a list
        ///// </summary>
        ///// <returns></returns>
        ///// <remarks >we allow at most three items here. the number of items could be increased if necessary</remarks>
        //public static IEnumerable<T> MakeEb<T>(int intCount, T item1, T item2 = null, T item3 = null, T item4 = null, T item5 = null, T item6 = null)
        //   where T : class
        //{

        //    if (intCount >= 1)
        //    {
        //        yield return item1;
        //    }
        //    if (intCount >= 2)
        //    {
        //        yield return item2;
        //    }
        //    if (intCount >= 3)
        //    {
        //        yield return item3;
        //    }
        //    if (intCount >= 4)
        //    {
        //        yield return item4;
        //    }
        //    if (intCount >= 5)
        //    {
        //        yield return item5;
        //    }
        //    if (intCount >= 6)
        //    {
        //        yield return item6;
        //    }

        //}

        /// <summary>
        /// 通过线要素(IPolyline5)获取点数组
        /// </summary>
        /// <param name="ipl">线要素</param>
        /// <returns>点数组</returns>
        public static List<CPoint> GetCPtLtByIPl(IPolyline5 ipl)
        {
            IGeometryCollection pGeoCol = ipl as IGeometryCollection;
            if (pGeoCol.GeometryCount > 1)
            {
                throw new ArgumentException("I didn't consider the problem of multiple polylines!");
            }

            return GetCptEbByICol((IPointCollection4)ipl).ToList();
        }

        //static int intCount = 0;

        /// <summary>
        /// 通过面要素(IPolygon4)获取点数组
        /// </summary>
        /// <param name="ipg">面要素</param>
        /// <returns>点数组</returns>
        /// <remarks>currently, we assume that there is only one exterior ring for ipg</remarks>
        public static IEnumerable<CPoint> GetIpgExteriorCptLt(IPolygon4 ipg, double dblFactor = 1)
        {
            //ipg.Close();
            if (ipg.ExteriorRingCount != 1)
            {
                throw new ArgumentException(
                    "I have not considered such a complicated case! A hole contains other holes! ");
            }

            IRing2 pExteriorRing = (ipg.ExteriorRingBag as IGeometryCollection).get_Geometry(0) as IRing2;
            return GetCptEbByICol(pExteriorRing as IPointCollection4, dblFactor).ToList();
        }

        public static IEnumerable<List<CPoint>> GetIpgInteriorCptLtEb(IPolygon4 ipg, double dblFactor = 1)
        {
            //ipg.Close();
            if (ipg.ExteriorRingCount != 1)
            {
                throw new ArgumentException(
                    "I have not considered such a complicated case! A hole contains other holes! ");
            }

            IRing2 pExteriorRing = (ipg.ExteriorRingBag as IGeometryCollection).get_Geometry(0) as IRing2;
            IGeometryCollection pGeoColInteriorRing = (IGeometryCollection)ipg.get_InteriorRingBag(pExteriorRing);

            for (int i = 0; i < pGeoColInteriorRing.GeometryCount; i++)
            {
                yield return GetCptEbByICol(pGeoColInteriorRing.get_Geometry(i) as IPointCollection4, dblFactor).ToList();
            }
        }


        public static IEnumerable<IEnumerable<CPoint>> GetCptEbEbByIColEb<T>(IEnumerable<T> TEb, int intFactor = 1)
        {
            foreach (var item in TEb)
            {
                yield return GetCptEbByICol(item as IPointCollection4, intFactor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCol">for a polygon, the fisrt point and the last point are identical</param>
        /// <param name="dblFactor"></param>
        /// <returns></returns>
        public static IEnumerable<CPoint> GetCptEbByICol(IPointCollection4 pCol, double dblFactor = 1)
        {
            for (int i = 0; i < pCol.PointCount; i++)
            {
                yield return new CPoint(i, pCol.get_Point(i), dblFactor);
            }
        }



        public static void ReverseCptLt(ref List<CPoint> cptlt)
        {
            cptlt.Reverse();
            for (int i = 0; i < cptlt.Count - 1; i++)
            {
                cptlt[i].ID = i;
                if (cptlt[i].pPoint != null)
                {
                    cptlt[i].pPoint.ID = i;
                }
            }
        }


        public static List<CPoint> CopyCptLt(List<CPoint> cptlt)
        {
            List<CPoint> copiedcptlt = new List<CPoint>(cptlt.Count);
            cptlt.ForEach(cpt => copiedcptlt.Add(cpt.Copy()));

            return copiedcptlt;
        }


        public static SortedDictionary<CPoint, CPoint> TestCloseCpts(List<CPoint> cptlt)
        {
            var cptSD = cptlt.ToSD(cpt => cpt, CCmpCptYX_VerySmall.sComparer);
            if (cptSD.Count == cptlt.Count)
            {
                return cptSD;
            }
            else
            {
                throw new ArgumentException("some points are too close to each other!");
                //return false;
            }
        }



        private static void RecursivelyGetLayers(ILayer pLayer, ref IMap mapFeature,
            List<bool> blnVisibleLt, bool blnVisible)
        {
            if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
            {
                ICompositeLayer pComLayer = pLayer as ICompositeLayer;
                for (int j = pComLayer.Count - 1; j >= 0; j--)
                {
                    ILayer psubLayer = pComLayer.get_Layer(j);
                    if (pLayer.Visible == false) //if any level is invisible, the lower levels are invisible
                    {
                        blnVisible = false;
                    }
                    RecursivelyGetLayers(psubLayer, ref mapFeature, blnVisibleLt, blnVisible);
                }
            }
            else
            {
                if (pLayer is IFeatureLayer)  //是否为要素图层
                {
                    mapFeature.AddLayer(pLayer);
                    blnVisibleLt.Add(blnVisible && pLayer.Visible);    //we do this because it's helpful for exporting data to Ipe

                }
            }
        }



        //红色线 
        public static void ViewPolyline(IMapControl4 pMapControl, CPolyline cpl)
        {
            //设置线段属性
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            ILineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
            pSimpleLineSymbol.Width = 2;
            pSimpleLineSymbol.Color = pColor;

            //生成线段
            ILineElement pLineElement = new LineElementClass();
            pLineElement.Symbol = pSimpleLineSymbol;
            IElement pElement = pLineElement as IElement;
            pElement.Geometry = cpl.pPolyline;

            //显示线段
            IGraphicsContainer pGra = pMapControl.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            pGra.AddElement(pElement, 0);
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);  //此语句必不可少，否则添加的要素不会实时显示
        }

        //红色线 
        public static void ViewPolylines(IMapControl4 pMapControl, List<CPolyline> cpllt)
        {
            //设置线段属性
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            ILineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
            pSimpleLineSymbol.Width = 1;
            pSimpleLineSymbol.Color = pColor;


            //生成线段
            IElementCollection pEleCol = new ElementCollectionClass();

            for (int i = 0; i < cpllt.Count; i++)
            {
                ILineElement pLineElement = new LineElementClass();
                pLineElement.Symbol = pSimpleLineSymbol;
                IElement pElement = pLineElement as IElement;
                pElement.Geometry = cpllt[i].pPolyline;
                pEleCol.Add(pElement, 0);
            }

            //显示线段
            IGraphicsContainer pGra = pMapControl.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            pGra.AddElements(pEleCol, 5);
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }


        /// <summary>
        /// Get the type and the index of this type for every Cgb 
        /// </summary>
        /// <typeparam name="CGeo"></typeparam>
        /// <param name="TEb"></param>
        /// <param name="objltlt"></param>
        /// <param name="intValueIndex"></param>
        /// <param name="intTypeIndexSD"></param>
        public static void GetCgbTypeAndTypeIndex(IEnumerable<CPolygon> TEb, List<List<object>> objltlt,
            int intValueIndex, CValMap_Dt<int, int> TypePVDt)
        //where CGeo : class
        {
            IEnumerator<CPolygon> TEt = TEb.GetEnumerator();
            //IEnumerator<object> objEt = objEb.GetEnumerator();

            int intCount = 0;
            while (TEt.MoveNext())
            {
                CPolygon TCurrent = TEt.Current;

                //get intType
                TCurrent.intType = Convert.ToInt32(objltlt[intCount++][intValueIndex]);

                //get intTypeIndex
                int intTypeIndex;
                if (TypePVDt.Dt.TryGetValue(TCurrent.intType, out intTypeIndex) == true)
                {
                    TCurrent.intTypeIndex = intTypeIndex;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("failed to get type index for a type!");
                    //MessageBox.Show("failed to get type index for a type!   In: " + "CHelpFunc.cs");
                }
            }
        }


        //public static void GetCgbTypeAndTypeIndex<CGeo>(IEnumerable<CGeoBase<CGeo>> TEb, List<List<object>> objltlt, int intValueIndex, CValMap_Dt<int, int> TypePVDt)
        //    where CGeo : class
        //{
        //    IEnumerator<CGeoBase<CGeo>> TEt = TEb.GetEnumerator();
        //    //IEnumerator<object> objEt = objEb.GetEnumerator();

        //    int intCount = 0;
        //    while (TEt.MoveNext())
        //    {
        //        CGeoBase<CGeo> TCurrent = TEt.Current;

        //        //get intType
        //        TCurrent.intType = Convert.ToInt32(objltlt[intCount++][intValueIndex]);

        //        //get intTypeIndex
        //        int intTypeIndex;
        //        if (TypePVDt.Dt.TryGetValue(TCurrent.intType, out intTypeIndex) == true)
        //        {
        //            TCurrent.intTypeIndex = intTypeIndex;
        //        }
        //        else
        //        {
        //            throw new ArgumentOutOfRangeException("failed to get type index for a type!");
        //            //MessageBox.Show("failed to get type index for a type!   In: " + "CHelpFunc.cs");
        //        }
        //    }



        public static IEnumerable<object> JudgeAndSetAEGeometry<T>(IEnumerable<T> pCGeoEb)
            where T : CGeoBase
        {
            foreach (var pCGeo in pCGeoEb)
            {
                yield return (pCGeo.JudgeAndSetAEGeometry() as object);
            }
        }

        public static void SetAEGeometryNull<T>(IEnumerable<T> pCGeoEb)
            where T : CGeoBase
        {
            foreach (var pCGeo in pCGeoEb)
            {
                pCGeo.SetAEGeometryNull();
            }
        }


        /// <summary>
        /// 保存对应线
        /// </summary>
        /// <param name="CResultPtLt">点数组（包含对应线信息）</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="strPath">保存路径</param>
        /// <param name="m_mapControl">地图控件</param>
        public static void SaveCtrlLine(List<List<CCorrCpts>> CtrlCptLtLt, string strFileName, double dblStandardLength, IWorkspace pWorkspace, IMapControl4 m_mapControl)
        {
            List<CPolyline> CtrlCplLt = GenerateCplLt(CtrlCptLtLt);
            int intCount = CtrlCptLtLt.GetCountItem();
            List<CPolyline> cpllt1 = new List<CPolyline>(intCount);
            List<CPolyline> cpllt2 = new List<CPolyline>(intCount);

            foreach (CPolyline CtrlCpl in CtrlCplLt)
            {
                CtrlCpl.SetPolyline();

                if (CCmpMethods.CmpDbl_CoordVerySmall(CtrlCpl.pPolyline.Length, dblStandardLength) == 0)
                {
                    cpllt1.Add(CtrlCpl);
                }
                else
                {
                    cpllt2.Add(CtrlCpl);
                }
            }

            SaveCPlLt(cpllt2, strFileName + "_UnPrecise", pWorkspace, m_mapControl, intRed: 255);
            SaveCPlLt(cpllt1, strFileName + "_Precise", pWorkspace, m_mapControl, intRed: 255);
        }

        /// <summary>
        /// 保存对应线
        /// </summary>
        /// <param name="CResultPtLt">点数组（包含对应线信息）</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="strPath">保存路径</param>
        /// <param name="m_mapControl">地图控件</param>
        public static void SaveCorrLine(List<List<CCorrCpts>> CtrlCptLtLt, string strFileName, IWorkspace pWorkspace, IMapControl4 m_mapControl)
        {
            List<CPolyline> CtrlCplLt = GenerateCplLt(CtrlCptLtLt);
            SaveCPlLt(CtrlCplLt, strFileName, pWorkspace, m_mapControl, intRed: 255);
        }


        public static List<CPolyline> GenerateCplLt(List<List<CCorrCpts>> CorrCptsLtLt)
        {
            var cpllt = new List<CPolyline>(CorrCptsLtLt.GetCountItem());
            foreach (var CorrCptsLt in CorrCptsLtLt)
            {
                foreach (var CorrCpts in CorrCptsLt)
                {
                    cpllt.Add(new CPolyline(CorrCpts));
                }
            }

            return cpllt;
        }

        public static List<CPolyline> GenerateCplLtByCEdgeLt(List<CEdge> CEdgeLt)
        {
            var cpllt = new List<CPolyline>(CEdgeLt.Count);
            for (int i = 0; i < CEdgeLt.Count; i++)
            {
                cpllt.Add(new CPolyline(i, CEdgeLt[i]));
            }

            return cpllt;
        }


    /// <summary>
    /// 保存线数据
    /// </summary>
    /// <param name="cpllt">线数组</param>
    /// <param name="strFileName">保存的文件名</param>
    /// <param name="strPath">保存路径</param>
    /// <param name="m_mapControl">地图控件</param>
    public static IFeatureLayer SaveCPlLt(List<CPolyline> cpllt, string strFileName, IWorkspace pWorkspace, IMapControl4 m_mapControl, int intRed = 0, int intGreen = 0, int intBlue = 0, double dblWidth = 1)
        {
            return CSaveFeature.SaveCGeoEb(cpllt, esriGeometryType.esriGeometryPolyline, strFileName, null, null, null, intRed, intGreen, intBlue, dblWidth);
        }


        public static void PrintStart(string strName)
        {
            string str = "\n-----------------------Start to print " + strName + "-----------------------";
            Console.WriteLine(str);
        }

        public static void PrintEnd(string strName)
        {
            string str = "************************End of printing " + strName + "************************\n\n";
            Console.WriteLine(str);
        }

        public static void PrintArray2D<T>(T[,] array2D, int intLength)
        {
            string strFormat = "{0," + intLength.ToString() + "}";  //like "{0,intLength}"

            Console.WriteLine();
            for (int i = 0; i < array2D.GetLength(0); i++)
            {
                for (int j = 0; j < array2D.GetLength(1); j++)
                {
                    Console.Write(string.Format(strFormat, array2D[i, j]));
                }
                Console.WriteLine();
            }

        }

        public static string GetUnits(esriUnits pMapUnits)
        {
            //don't need the first four characters, which are "esri"
            return pMapUnits.ToString().Substring(4);
        }



        /// <summary>
        /// Coder:   梁  爽
        /// Date:    2008-10-16
        /// Content: 该函数根据传入的路径参数打开一个工作空间，如果传入的路径不存在，则创建一个路径
        /// </summary>
        /// <param name="path">用来传入工作空间的路径</param>
        /// <returns></returns>
        /// <remarks>please consider using CHelpFunc.SetSavePath(ParameterInitialize)</remarks>
        public static IWorkspace OpenWorkspace(string path)
        {



            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspace pWorkspace;

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
            pWorkspace = pWorkspaceFactory.OpenFromFile(path, 0);
            return pWorkspace;
        }

        public static IWorkspace CreateFileGdbWorkspace(string strSavePathBackSlash, string strName)
        {
            // Instantiate a file geodatabase workspace factory and create a new file geodatabase.
            // The Create method returns a workspace name object.
            IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
            //IWorkspaceName workspaceName = workspaceFactory.Create("C:\\temp\\",
            //  "Sample.gdb", null, 0);
            IWorkspaceName workspaceName = workspaceFactory.Create(strSavePathBackSlash,
  strName, null, 0);


            // Cast the workspace name object to the IName interface and open the workspace.
            IName name = (IName)workspaceName;
            IWorkspace workspace = (IWorkspace)name.Open();
            return workspace;
        }


        public static long GetDirectoryLength(string dirPath)
        {
            //判断给定的路径是否存在,如果不存在则退出
            if (!Directory.Exists(dirPath))
                return 0;
            long len = 0;

            //定义一个DirectoryInfo对象
            DirectoryInfo di = new DirectoryInfo(dirPath);

            //通过GetFiles方法,获取di目录中的所有文件的大小
            foreach (FileInfo fi in di.GetFiles())
            {
                len += fi.Length;
            }

            //获取di中所有的文件夹,并存到一个新的对象数组中,以进行递归
            DirectoryInfo[] dis = di.GetDirectories();
            if (dis.Length > 0)
            {
                for (int i = 0; i < dis.Length; i++)
                {
                    len += GetDirectoryLength(dis[i].FullName);
                }
            }
            return len;
        }


        public static IEnumerable<T> GetTEbFromIGeoCol<T>(IGeometryCollection pGeoCol)
        {
            for (int i = 0; i < pGeoCol.GeometryCount; i++)
            {
                yield return (T)pGeoCol.get_Geometry(i);
            }
        }


        /// <summary>
        /// 显示并返回单个插值线段
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static CPolyline DisplayInterpolation(CDataRecords pDataRecords, double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }
            List<CPoint> CResultPtLt = pDataRecords.ParameterResult.CResultPtLt;
            CPolyline cpl = CGeoFunc.GetTargetcpl(CResultPtLt, dblProp);
            cpl.SetPolyline();
            // 清除绘画痕迹
            IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            //m_mapControl.ActiveView.Refresh();   //由于在下一步“ViewPolyline”中有刷新的命令，此语句可省略
            ViewPolyline(m_mapControl, cpl);  //显示生成的线段
            return cpl;
        }

        /// <summary>
        /// 显示并返回多个插值线段
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static List<CPolyline> DisplayInterpolations(CDataRecords pDataRecords, double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }
            List<List<CPoint>> CResultPtLtLt = pDataRecords.ParameterResult.CResultPtLtLt;

            List<CPolyline> cpllt = new List<CPolyline>();
            for (int i = 0; i < CResultPtLtLt.Count; i++)
            {
                CPolyline cpl = CGeoFunc.GetTargetcpl(CResultPtLtLt[i], dblProp);
                cpllt.Add(cpl);
            }


            // 清除绘画痕迹
            IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            //m_mapControl.ActiveView.Refresh();   //由于在下一步“ViewPolyline”中有刷新的命令，此语句可省略
            ViewPolylines(m_mapControl, cpllt);  //显示生成的线段
            return cpllt;
        }

        /// <summary>
        /// we set move vector so that we can easily get intermediate results and save to excel
        /// </summary>
        public static void SetMoveVectorForCorrCptsLtLt(List<List<CCorrCpts>> CorrCptsLtLt)
        {
            CorrCptsLtLt.ForEach(CorrCptsLt => CorrCptsLt.ForEach(CorrCpts => CorrCpts.SetMoveVector()));
        }

        public static void SetMoveVectorForCorrCptsLt(List<CCorrCpts> CorrCptsLt)
        {
            CorrCptsLt.ForEach(CorrCpts => CorrCpts.SetMoveVector());
        }

        /// <summary>
        /// 显示并返回单个插值线段
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static List<CPolyline> GetAndSaveInterpolation(CDataRecords pDataRecords, double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("the parameter t is not acceptable!");
                return null;
            }

            CMorphingBaseCpl pMorphingBaseCpl = pDataRecords.ParameterResult.pMorphingBaseCpl;
            List<CPolyline> pInterpolatedCPlLt = pMorphingBaseCpl.GenerateInterpolatedCplLt(dblProp);

            CParameterInitialize pParameterInitialize = pDataRecords.ParameterInitialize;
            CHelpFunc.SaveCPlLt(pInterpolatedCPlLt, pParameterInitialize.strSaveFolderName + "____" + dblProp.ToString(), pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);

            return pInterpolatedCPlLt;
        }


        ///// <summary>
        ///// 显示并返回单个插值线段
        ///// </summary>
        ///// <param name="pDataRecords">数据记录</param>
        ///// <param name="dblProp">差值参数</param>
        ///// <returns>插值线段</returns>
        //public static List<CRiver> DisplayRiverLt(CDataRecords pDataRecords, double dblProp)
        //{
        //    if (dblProp < 0 || dblProp > 1)
        //    {
        //        MessageBox.Show("请输入正确参数！");
        //        return null;
        //    }
        //    List<CRiver> CResultRiverLt = pDataRecords.ParameterResult.CResultRiverLt;
        //    List<CRiver> CDisplayRiverLt = new List<CRiver>();
        //    List<CPolyline> CDisplayPlLt = new List<CPolyline>();
        //    for (int i = 0; i < CResultRiverLt.Count; i++)
        //    {
        //        if (CResultRiverLt[i].dblWeightinessUnitary > dblProp)
        //        {
        //            CDisplayRiverLt.Add(CResultRiverLt[i]);
        //            CPolyline cpl = new CPolyline(CResultRiverLt[i]);
        //            CDisplayPlLt.Add(cpl);
        //        }
        //    }

        //    // 清除绘画痕迹
        //    IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
        //    IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
        //    pGra.DeleteAllElements();
        //    m_mapControl.ActiveView.Refresh();
        //    ViewPolylines(m_mapControl, CDisplayPlLt);  //显示生成的线段
        //    return CDisplayRiverLt;
        //}




        /// <summary>
        /// 提取对应线段
        /// </summary>
        /// <param name="frcpl">大比例尺线状要素</param>
        /// <param name="tocpl">小比例尺线状要素</param>
        /// <param name="pCorrespondBendLt">对应弯曲列表</param>
        /// <returns>对应线段</returns>
        /// <remarks></remarks>
        public static SCG.LinkedList<CCorrSegment> DetectCorrespondSegment(CPolyline frcpl, CPolyline tocpl, List<CCorrespondBend> pCorrespondBendLt)
        {
            //提取对应弯曲的首尾点为对应特征点
            SortedList<double, CCorrCpts> pCorrespondingCptSlt = new SortedList<double, CCorrCpts>(new CCmpDbl());

            CCorrCpts pStartCorrespondingCpt0 = new CCorrCpts(frcpl.CptLt[0], tocpl.CptLt[0]);//第一对对应特征点
            CCorrCpts pEndCorrespondingCpt0 = new CCorrCpts(frcpl.CptLt[frcpl.CptLt.Count - 1], tocpl.CptLt[tocpl.CptLt.Count - 1]);//第二对对应特征点
            pCorrespondingCptSlt.Add(0, pStartCorrespondingCpt0);
            pCorrespondingCptSlt.Add(1, pEndCorrespondingCpt0);

            //其它对应特征点
            for (int i = 0; i < pCorrespondBendLt.Count; i++)
            {
                CCorrCpts pStartCorrespondingCpt = new CCorrCpts(pCorrespondBendLt[i].CFromBend.CptLt[0], pCorrespondBendLt[i].CToBend.CptLt[0]);
                CCorrCpts pEndCorrespondingCpt = new CCorrCpts(pCorrespondBendLt[i].CFromBend.CptLt[pCorrespondBendLt[i].CFromBend.CptLt.Count - 1], pCorrespondBendLt[i].CToBend.CptLt[pCorrespondBendLt[i].CToBend.CptLt.Count - 1]);

                pCorrespondingCptSlt.Add(pCorrespondBendLt[i].CFromBend.dblStartRL, pStartCorrespondingCpt);
                pCorrespondingCptSlt.Add(pCorrespondBendLt[i].CFromBend.dblEndRL, pEndCorrespondingCpt);
            }


            //查找并删除重复对应特征点
            for (int i = pCorrespondingCptSlt.Count - 1; i > 0; i--)
            {
                CPoint frcpt2 = pCorrespondingCptSlt.Values[i].FrCpt;
                CPoint tocpt2 = pCorrespondingCptSlt.Values[i].ToCpt;
                CPoint frcpt1 = pCorrespondingCptSlt.Values[i - 1].FrCpt;
                CPoint tocpt1 = pCorrespondingCptSlt.Values[i - 1].ToCpt;
                if (frcpt1.Equals2D(frcpt2) && tocpt1.Equals2D(tocpt2))
                {
                    pCorrespondingCptSlt.RemoveAt(i);
                }
            }

            //以对应特征点为断点分割原始线段，得到对应线段
            SCG.LinkedList<CCorrSegment> pCorrespondSegmentLk = new SCG.LinkedList<CCorrSegment>();

            //中间的对应线段
            for (int i = 0; i < pCorrespondingCptSlt.Count - 1; i++)
            {
                CPolyline frSegment = frcpl.GetSubPolyline(pCorrespondingCptSlt.Values[i].FrCpt, pCorrespondingCptSlt.Values[i + 1].FrCpt);
                CPolyline toSegment = tocpl.GetSubPolyline(pCorrespondingCptSlt.Values[i].ToCpt, pCorrespondingCptSlt.Values[i + 1].ToCpt);
                pCorrespondSegmentLk.AddLast(new CCorrSegment(frSegment, toSegment));
            }

            return pCorrespondSegmentLk;
        }

        /// <summary>
        /// 将ResultPtlt形式的结果转换为CorrespondPtlt形式的结果
        /// </summary>
        /// <param name="cresultptlt">ResultPtlt形式的结果</param>
        /// <returns>CorrespondPtlt形式的结果</returns>
        /// <remarks></remarks>
        public static List<CCorrCpts> TransferResultptltToCorrCptsLt(List<CPoint> cresultptlt)
        {
            List<CCorrCpts> pCorrCptslt = new List<CCorrCpts>(cresultptlt.Count);
            for (int i = 0; i < cresultptlt.Count; i++)
            {
                CPoint frcpt = cresultptlt[i];
                for (int j = 0; j < cresultptlt[i].CorrespondingPtLt.Count; j++)
                {
                    CPoint tocpt = cresultptlt[i].CorrespondingPtLt[j];

                    //为了跟原来的点断绝关系，以产生不必要的麻烦，此处重新生成点
                    CPoint newfrcpt = new CPoint(frcpt.ID, frcpt.X, frcpt.Y, frcpt.Z);
                    CPoint newtocpt = new CPoint(tocpt.ID, tocpt.X, tocpt.Y, frcpt.Z);
                    CCorrCpts pCorrCpts = new CCorrCpts(newfrcpt, newtocpt);
                    pCorrCptslt.Add(pCorrCpts);
                }
            }
            return pCorrCptslt;
        }


        public static IComparer<T> SetOrDefaultCmp<T>(IComparer<T> cmp)
        {
            if (cmp == null)
            {
                cmp = SCG.Comparer<T>.Default;
            }
            return cmp;
        }


        public static void CompareAndOrder<T, TOrder>(T T1, T T2, Func<T, TOrder> orderFunc,
            out T minT, out T maxT, IComparer<TOrder> cmp = null)
        {
            if (cmp == null) { cmp = SCG.Comparer<TOrder>.Default; }
            if (cmp.Compare(orderFunc(T1), orderFunc(T2)) <= 0)
            {
                minT = T1;
                maxT = T2;
            }
            else
            {
                minT = T2;
                maxT = T1;
            }
        }

        public static void Swap<T>(T T1, T T2)
        {
            T TempT = T1;
            T1 = T2;
            T2 = TempT;
        }

        /// <summary>
        /// compare two variables according to a specified attribute and return the smaller one 
        /// </summary>
        /// <typeparam name="T">the type of the variables</typeparam>
        /// <typeparam name="TOrder">the type of the specified attribute</typeparam>
        /// <param name="T1">variable 1</param>
        /// <param name="T2">variable 2</param>
        /// <param name="orderFunc">the specified attribute of type T</param>
        /// <param name="cmp">compare function</param>
        /// <returns>the smaller varaible according to the specified attribute</returns>
        public static T Min<T, TOrder>(T T1, T T2, Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null)
        {
            if (cmp == null) { cmp = SCG.Comparer<TOrder>.Default; }

            if (cmp.Compare(orderFunc(T1), orderFunc(T2)) < 0)
            {
                return T1;
            }
            else
            {
                return T2;
            }
        }


        public static T Max<T, TOrder>(T T1, T T2, Func<T, TOrder> orderFunc, Func<T, TOrder> orderFunc2 = null,
             IComparer<TOrder> cmp = null, IComparer<TOrder> cmp2 = null)
        {
            if (cmp == null) { cmp = SCG.Comparer<TOrder>.Default; }
            int intResult1 = cmp.Compare(orderFunc(T1), orderFunc(T2));
            if (intResult1 > 0)
            {
                return T1;
            }
            else if (intResult1 < 0)
            {
                return T2;
            }
            else
            {
                if (orderFunc2 == null)
                {
                    return T1;
                }
                else
                {
                    if (cmp2 == null) { cmp2 = SCG.Comparer<TOrder>.Default; }
                    int intResult2 = cmp2.Compare(orderFunc2(T1), orderFunc2(T2));
                    if (intResult2 > 0)
                    {
                        return T1;
                    }
                    else if (intResult2 < 0)
                    {
                        return T2;
                    }
                    else
                    {
                        return T1;
                    }
                }
            }
        }


        public static void FrmOperation(ref CParameterInitialize pParameterInitialize)
        {
            pParameterInitialize.m_mapFeature = CHelpFunc.GetAllLayers(pParameterInitialize.m_mapControl);
            LoadTocbo(pParameterInitialize);
        }

        private static void LoadTocbo(CParameterInitialize pParameterInitialize)
        {
            if (pParameterInitialize.cboLayerLt == null)
            {
                return;
            }

            IMap pm_mapFeature = pParameterInitialize.m_mapFeature;
            IEnumLayer pEnumlayer = pm_mapFeature.Layers;            
            var astrLayerName = new string[pm_mapFeature.LayerCount];
            int intIndex = 0;
            while (intIndex < pm_mapFeature.LayerCount)
            {
                astrLayerName[intIndex] = pEnumlayer.Next().Name;
                intIndex++;
            }

            int intSelect = 0;
            foreach (ComboBox cboLayer in pParameterInitialize.cboLayerLt)
            {
                cboLayer.Items.AddRange(astrLayerName);
                cboLayer.SelectedIndex = intSelect;
                intSelect++;
            }
        }

        public static List<IFeatureLayer> GetVisibleLayers(CParameterInitialize ParameterInitialize)
        {
            var blnVisibleLt = new List<bool>();
            var m_mapFeature = CHelpFunc.GetAllLayers(ParameterInitialize.m_mapControl, blnVisibleLt);

            var pFLayerLt = new List<IFeatureLayer>(m_mapFeature.LayerCount);
            for (int i = 0; i < m_mapFeature.LayerCount; i++)
            {
                var pFLayer = m_mapFeature.get_Layer(i) as IFeatureLayer;

                if (blnVisibleLt[i] == false)
                {
                    continue;
                }
                else
                {
                    pFLayerLt.Add(pFLayer);
                }
            }

            return pFLayerLt;
        }

        public static IMap GetAllLayers(IMapControl4 m_mapControl, List<bool> blnVisibleLt = null)
        {
            var pMap = m_mapControl.Map;
            IMap mapFeature = new MapClass();
            if (blnVisibleLt == null)
            {
                blnVisibleLt = new List<bool>(pMap.LayerCount);
            }

            for (int i = pMap.LayerCount - 1; i >= 0; i--) //获取所有的要素图层（由于之后的“AddLayer”方法总是将新的图层放在第一个位置，所以这里从后面开始遍历）
            {
                ILayer pLayer = pMap.get_Layer(i);
                RecursivelyGetLayers(pLayer, ref mapFeature, blnVisibleLt, true);
            }
            blnVisibleLt.Reverse(); //由于之后的“AddLayer”方法总是将新的图层放在第一个位置，so we reverse
            return mapFeature;
        }
    }
}
