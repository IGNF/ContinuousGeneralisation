using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Office.Interop;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;
using VBClass;


namespace MorphingClass.CUtility
{
    public static class CHelpFuncExcel
    {

        public static object[][] ReadDataFromExcel(string strPath = null)
        {
            if (strPath == null || strPath == "")
            {
                strPath = OpenAnExcel();
                if (strPath == null || strPath == "")
                {
                    return null;
                }
            }

            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Open(strPath);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;


            int intRow = pWorksheet.UsedRange.Rows.Count;
            int intCol = pWorksheet.UsedRange.Columns.Count;
            System.Array values = (System.Array)pWorksheet.UsedRange.Formula;
            object[][] aObj = new object[intRow][];
            for (int i = 0; i < intRow ; i++)
            {
                aObj[i] = new object[intCol];
                for (int j = 0; j < intCol; j++)
                {
                    aObj[i][j] = values.GetValue(i + 1, j + 1);
                }
            }
            pExcelAPP.Quit();
            return aObj;
        }

        public static string OpenAnExcel()
        {
            OpenFileDialog OFG = new OpenFileDialog();
            
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            return OFG.FileName;
        }

        //public static Worksheet GetWorksheetFromPath(string strEntireFileName = null)
        //{


        //}


        #region ExportEvaluationToExcel for ResultPtLt
        /// <summary>
        /// 将指标值导出到Excel中
        /// </summary>
        /// <param name="pParameterResultToExcel">指标结果信息</param>
        /// <param name="strSavePath">保存路径</param>
        /// <remarks>导出时，Excel显示的指标信息为到该点时的指标信息</remarks>
        public static void ExportEvaluationToExcel(CParameterResult pParameterResultToExcel, CParameterInitialize pParameterInitialize, string strSuffix)
        {
            ////为应付Excel的bug，加入以下两行代码
            //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            StatusStrip ststMain = pParameterInitialize.ststMain;
            ToolStripStatusLabel tsslTime = pParameterInitialize.tsslTime;
            ToolStripStatusLabel tsslMessage = pParameterInitialize.tsslMessage;
            ToolStripProgressBar tspbMain = pParameterInitialize.tspbMain;
            tsslMessage.Text = "正在将指标值导出到Excel...";
            ststMain.Refresh();
            long lngStartTime = System.Environment.TickCount;

            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
            string strSavePath = pParameterInitialize.strSavePath;
            string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
            switch (pParameterResultToExcel.strEvaluationMethod)
            {
                case "Integral":
                    {
                        ExportIntegralEvaluation(pParameterResultToExcel, ref pWorksheet, ref tspbMain);
                        strfilename = strfilename + "Integral";
                        break;
                    }
                case "Translation":
                    {
                        ExportTranslationEvaluation(pParameterResultToExcel, ref pWorksheet, ref tspbMain);
                        strfilename = strfilename + "Translation";
                        break;
                    }
                case "Deflection":
                    {
                        ExportTranslationEvaluation(pParameterResultToExcel, ref pWorksheet, ref tspbMain);
                        strfilename = strfilename + "Deflection";
                        break;
                    }
                default: throw new ArgumentException("调用的评价指标值导出方法不存在！");
            }


            pWorksheet.Columns.AutoFit();
            pExcelAPP.DisplayAlerts = false;
            pWorkBook.SaveAs(strSavePath + "\\" + strfilename + strSuffix, AccessMode: XlSaveAsAccessMode.xlNoChange);
            pExcelAPP.Quit();

            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            tsslTime.Text = "导出运行时间：" + Convert.ToString(lngTime) + "ms";  //显示运行时间
            tsslMessage.Text = "详细信息已导出到Excel！";
        }

        /// <summary>
        /// 导出Integral指标值
        /// </summary>
        /// <param name="pParameterResultToExcel">需保存的数据</param>
        /// <param name="pWorksheet">Excel的Sheet</param>
        private static void ExportIntegralEvaluation(CParameterResult pParameterResultToExcel, ref Worksheet pWorksheet, ref ToolStripProgressBar tspbMain)
        {
            ////为应付Excel的bug，加入以下两行代码
            //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            pWorksheet.Cells[1, 1] = "FrID";
            pWorksheet.Cells[1, 2] = "FrX";
            pWorksheet.Cells[1, 3] = "FrY";
            pWorksheet.Cells[1, 4] = "ToID";
            pWorksheet.Cells[1, 5] = "ToX";
            pWorksheet.Cells[1, 6] = "ToY";
            pWorksheet.Cells[1, 7] = "Integral";
            pWorksheet.Cells[1, 8] = "SumIntegral";

            int intRowCount = pParameterResultToExcel.FromPtLt.Count;

            for (int i = 0; i < intRowCount; i++)
            {
                pWorksheet.Cells[i + 2, 1] = pParameterResultToExcel.FromPtLt[i].ID;
                pWorksheet.Cells[i + 2, 2] = pParameterResultToExcel.FromPtLt[i].X;
                pWorksheet.Cells[i + 2, 3] = pParameterResultToExcel.FromPtLt[i].Y;
                pWorksheet.Cells[i + 2, 4] = pParameterResultToExcel.ToPtLt[i].ID;
                pWorksheet.Cells[i + 2, 5] = pParameterResultToExcel.ToPtLt[i].X;
                pWorksheet.Cells[i + 2, 6] = pParameterResultToExcel.ToPtLt[i].Y;
                pWorksheet.Cells[i + 2, 7] = pParameterResultToExcel.dblIntegralLt[i];
                pWorksheet.Cells[i + 2, 8] = pParameterResultToExcel.dblSumIntegralLt[i];

                tspbMain.Value = (i + 1) * 100 / (intRowCount);
            }
        }

        /// <summary>
        /// 导出Translation指标值
        /// </summary>
        /// <param name="pParameterResultToExcel">需保存的数据</param>
        /// <param name="pWorksheet">Excel的Sheet</param>
        private static void ExportTranslationEvaluation(CParameterResult pParameterResultToExcel, ref Worksheet pWorksheet, ref ToolStripProgressBar tspbMain)
        {
            ////为应付Excel的bug，加入以下两行代码
            //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            pWorksheet.Cells[1, 1] = "FrID";
            pWorksheet.Cells[1, 2] = "FrX";
            pWorksheet.Cells[1, 3] = "FrY";
            pWorksheet.Cells[1, 4] = "ToID";
            pWorksheet.Cells[1, 5] = "ToX";
            pWorksheet.Cells[1, 6] = "ToY";
            pWorksheet.Cells[1, 7] = "TrID";
            pWorksheet.Cells[1, 8] = "TrX";
            pWorksheet.Cells[1, 9] = "TrY";
            pWorksheet.Cells[1, 10] = "Translation";
            pWorksheet.Cells[1, 11] = "SumTranslation";

            int intRowCount = pParameterResultToExcel.FromPtLt.Count;
            for (int i = 0; i < intRowCount; i++)
            {
                pWorksheet.Cells[i + 2, 1] = pParameterResultToExcel.FromPtLt[i].ID;
                pWorksheet.Cells[i + 2, 2] = pParameterResultToExcel.FromPtLt[i].X;
                pWorksheet.Cells[i + 2, 3] = pParameterResultToExcel.FromPtLt[i].Y;
                pWorksheet.Cells[i + 2, 4] = pParameterResultToExcel.ToPtLt[i].ID;
                pWorksheet.Cells[i + 2, 5] = pParameterResultToExcel.ToPtLt[i].X;
                pWorksheet.Cells[i + 2, 6] = pParameterResultToExcel.ToPtLt[i].Y;
                pWorksheet.Cells[i + 2, 7] = pParameterResultToExcel.TranlationPtLt[i].ID;
                pWorksheet.Cells[i + 2, 8] = pParameterResultToExcel.TranlationPtLt[i].X;
                pWorksheet.Cells[i + 2, 9] = pParameterResultToExcel.TranlationPtLt[i].Y;
                pWorksheet.Cells[i + 2, 10] = pParameterResultToExcel.dblTranslationLt[i];
                pWorksheet.Cells[i + 2, 11] = pParameterResultToExcel.dblSumTranslationLt[i];

                tspbMain.Value = (i + 1) * 100 / (intRowCount);
            }
        }
        #endregion

        #region ExportEvaluationToExcel for ResultPtLt
        //public static void ExportEvaluationToExcel(CDataRecords pDataRecords, string strSuffix)
        //{
        //    CParameterInitialize pParameterInitialize = pDataRecords.ParameterInitialize;
        //    CParameterResult pParameterResult = pDataRecords.ParameterResult;

        //    StatusStrip ststMain = pParameterInitialize.ststMain;
        //    ToolStripStatusLabel tsslTime = pParameterInitialize.tsslTime;
        //    ToolStripStatusLabel tsslMessage = pParameterInitialize.tsslMessage;
        //    ToolStripProgressBar tspbMain = pParameterInitialize.tspbMain;
        //    tsslMessage.Text = "正在将指标值导出到Excel...";
        //    ststMain.Refresh();
        //    long lngStartTime = System.Environment.TickCount;

        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strSavePath = pParameterInitialize.strSavePath;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    switch (pParameterResult.strEvaluationMethod)
        //    {
        //        case "Integral":
        //            {
        //                ExportIntegralEvaluation(pParameterResult, ref pWorksheet, ref tspbMain);
        //                strfilename = strfilename + "Integral";
        //                break;
        //            }
        //        case "Translation":
        //            {
        //                ExportTranslationEvaluation(pParameterResult, ref pWorksheet, ref tspbMain);
        //                strfilename = strfilename + "Translation";
        //                break;
        //            }
        //        case "Deflection":
        //            {
        //                ExportTranslationEvaluation(pParameterResult, ref pWorksheet, ref tspbMain);
        //                strfilename = strfilename + "Deflection";
        //                break;
        //            }
        //        default: throw new ArgumentException("调用的评价指标值导出方法不存在！");
        //    }


        //    pWorksheet.Columns.AutoFit();
        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.SaveAs(strSavePath + "\\" + strfilename + strSuffix, AccessMode: XlSaveAsAccessMode.xlNoChange);
        //    pExcelAPP.Quit();

        //    long lngEndTime = System.Environment.TickCount;
        //    long lngTime = lngEndTime - lngStartTime;
        //    tsslTime.Text = "导出运行时间：" + Convert.ToString(lngTime) + "ms";  //显示运行时间
        //    tsslMessage.Text = "详细信息已导出到Excel！";
        //}


        public static void ExportEvaluationToExcelCorr(CParameterResult pParameterResult, string strPath, string strSuffix = null)
        {
            string strEvaluationMethod = pParameterResult.enumEvaluationMethod.ToString();
            string strfilename = System.IO.Path.GetFileNameWithoutExtension(strPath);

            List<List<CCorrCpts>> CorrCptsLtLt = pParameterResult.pMorphingBase.CorrCptsLtLt;
            List<List<double>> dblEvaluationLtLt = pParameterResult.pEvaluation.dblEvaluationLtLt;
            List<List<double>> dblSumEvaluationLtLt = pParameterResult.pEvaluation.dblSumEvaluationLtLt;

            for (int i = 0; i < CorrCptsLtLt.Count; i++)
            {
                string strEntireFileName = strPath + "\\" + strfilename + strEvaluationMethod + 
                    CHelpFunc.JudgeAndAddZero(i, 4) + "__" + dblSumEvaluationLtLt[i].Last().ToString();

                //it's better if we can output a file with ".xlsx", but that causes problems
                var sw = new StreamWriter(strEntireFileName + ".xls", false, Encoding.GetEncoding(-0));  

                string strColNames = "FrID\t" + "FrX\t" + "FrY\t" + "ToID\t" + "ToX\t" + "ToY\t" + "MoveID\t" + "MoveX\t" + "MoveY\t" 
                    + strEvaluationMethod + "\t" + "Sum" + strEvaluationMethod;
                sw.WriteLine(strColNames);

                int intJ = 0;
                foreach (CCorrCpts CorrCpt in CorrCptsLtLt[i])
                {
                    string strTemp = CorrCpt.FrCpt.ID.ToString() + "\t"
                                            + CorrCpt.FrCpt.X.ToString() + "\t"
                                            + CorrCpt.FrCpt.Y.ToString() + "\t"
                                            + CorrCpt.ToCpt.ID.ToString() + "\t"
                                            + CorrCpt.ToCpt.X.ToString() + "\t"
                                            + CorrCpt.ToCpt.Y.ToString() + "\t"
                                            + CorrCpt.pMoveVector.ID.ToString() + "\t"
                                            + CorrCpt.pMoveVector.X.ToString() + "\t"
                                            + CorrCpt.pMoveVector.Y.ToString() + "\t"
                                            + dblEvaluationLtLt[i][intJ].ToString() + "\t"
                                            + dblSumEvaluationLtLt[i][intJ].ToString();
                    sw.WriteLine(strTemp);
                    intJ++;
                }
                sw.Close();
            }


            //output the last values
            var dblSumEvaluationLt = new List<double>(dblSumEvaluationLtLt.Count);
            for (int i = 0; i < dblSumEvaluationLtLt.Count; i++)
            {
                dblSumEvaluationLt.Add(dblSumEvaluationLtLt[i].Last());
            }
            ExportDataltToExcel(dblSumEvaluationLt, "SumEachFeature", strPath);

            //KillExcel();
        }

        #endregion

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="objDataEbEb">the object should be something that we can use the method .ToString()</param>
        ///// <param name="objHeadEb"></param>
        ///// <param name="strName"></param>
        ///// <param name="strSavePath"></param>
        //public static string ExportToExcelSW(IEnumerable<IEnumerable<object>> objDataEbEb, IEnumerable<object> objHeadEb, string strName, string strSavePath)
        //{
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);

        //    //int intColumn = 1;
        //    //foreach (var objHead in objHeadEb)
        //    //{
        //    //    pWorksheet.Cells[1, intColumn++] = objHead.ToString ();
        //    //}

        //    //pWorksheet.Columns.AutoFit();
        //    strfilename = strfilename + strName;
        //    string strEntireFileName = strSavePath + "\\" + strfilename + ".xls";
        //    pWorkBook.SaveAs(strEntireFileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive);
        //    pWorkBook.Close(false, false, false);
        //    pExcelAPP.Quit();


        //    StreamWriter sw = new StreamWriter(strEntireFileName, false  , Encoding.GetEncoding(-0));

        //    //write the first line into the file, i.e., head line
        //    string strHead = "";
        //    foreach (var objHead in objHeadEb)
        //    {
        //        strHead += (objHead.ToString() + "\t");
        //    }
        //    sw.WriteLine(strHead);

        //    //write the data into the file
        //    foreach (var objDataEb in objDataEbEb)
        //    {
        //        string tempStr = "";
        //        foreach (var objData in objDataEb)
        //        {
        //            tempStr += (objData + "\t");
        //        }
        //        sw.WriteLine(tempStr);
        //    }
        //    sw.Close();
            
        //    return strEntireFileName;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objDataEbEb">the object should be something that we can use the method .ToString()</param>
        /// <param name="objHeadEb"></param>
        /// <param name="strName"></param>
        /// <param name="strSavePath"></param>
        public static void ExportToExcel(IEnumerable<IEnumerable<object>> objDataEbEb,
            string strName, string strSavePath, IEnumerable<object> objHeadEb=null)
        {
            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
            //string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);

            int intCurrentRow = 1;
            int intCurrentCol = 1;

            if (objHeadEb!=null)
            {
                foreach (var objHead in objHeadEb)
                {
                    pWorksheet.Cells[intCurrentRow, intCurrentCol++] = objHead.ToString();
                }
                intCurrentRow++;
                intCurrentCol = 1;
            }


            foreach (var objDataEb in objDataEbEb)
            {
                foreach (var objData in objDataEb)
                {
                    pWorksheet.Cells[intCurrentRow, intCurrentCol++] = objData.ToString();
                }

                intCurrentRow++;
                intCurrentCol = 1;
            }

            pWorksheet.Columns.AutoFit();
            string strEntireFileName = strSavePath + "\\" + strName + ".xls";
            pWorkBook.SaveAs(strEntireFileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            pWorkBook.Close(false, false, false);
            pExcelAPP.Quit();
        }

        //private static string CreateExcelFile(string strName, string strSavePath)
        //{
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;
        //    string strEntireFileName = strSavePath + "\\" + strfilename;
        //    pWorkBook.SaveAs(strEntireFileName, XlFileFormat.xlWorkbookNormal, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive);
        //    pWorkBook.Close(false, false, false);
        //    pExcelAPP.Quit();
        //    KillExcel();

        //    return strEntireFileName;
        //    return null;
        //}


























        ///// <summary>
        ///// 计算标准差
        ///// </summary>
        ///// <param name="dblDatalt">数据列</param>
        ///// <returns>标准差</returns>
        //public static double ECalStdev(List<double> dblDatalt)
        //{
        //    ////为应付Excel的bug，加入以下两行代码
        //    //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    object Missing = Type.Missing;
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;

        //    //添加值
        //    for (int i = 0; i < dblDatalt.Count; i++)
        //    {
        //        pWorksheet.Cells[i + 1, 1] = dblDatalt[i];
        //    }

        //    //确定计算范围
        //    string objCell2 = "A" + dblDatalt.Count.ToString();
        //    Range rng = pWorksheet.get_Range("A1", objCell2);
        //    WorksheetFunction OptFun = pWorksheet.Application.WorksheetFunction;

        //    //计算标准差
        //    double dblStDev = OptFun.StDev(rng, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing);

        //    //KillExcel();
        //    return dblStDev;
        //}


        ///// <summary>
        ///// 计算标准差(整体)
        ///// </summary>
        ///// <param name="dblDatalt">数据列</param>
        ///// <returns>标准差</returns>
        //public static double ECalStdevP(List<double> dblDatalt)
        //{
        //    ////为应付Excel的bug，加入以下两行代码
        //    //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    object Missing = Type.Missing;
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;

        //    //添加值
        //    for (int i = 0; i < dblDatalt.Count; i++)
        //    {
        //        pWorksheet.Cells[i + 1, 1] = dblDatalt[i];
        //    }

        //    //确定计算范围
        //    string objCell2 = "A" + dblDatalt.Count.ToString();
        //    Range rng = pWorksheet.get_Range("A1", objCell2);
        //    WorksheetFunction OptFun = pWorksheet.Application.WorksheetFunction;

        //    //计算标准差
        //    double dblStDevP = OptFun.StDevP(rng, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing,
        //        Missing, Missing, Missing, Missing, Missing, Missing);

        //    //KillExcel();
        //    return dblStDevP;
        //}



        /// <summary>
        /// 输出数据到Excel中
        /// </summary>
        /// <param name="pParameterResultToExcel">指标结果信息</param>
        /// <param name="strName">文件名称</param>
        /// <param name="strSavePath">保存路径</param>
        /// <remarks>一维数列</remarks>
        public static void ExportDataltToExcel(List<double> dblDatalt, string strName, string strSavePath)
        {
            //为应付Excel的bug，加入以下两行代码
            //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
            string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
            strfilename = strfilename + "____" + strName;

            int intRowCount = dblDatalt.Count;
            for (int i = 0; i < intRowCount; i++)
            {
                pWorksheet.Cells[i + 1, 1] = dblDatalt[i];
            }

            pWorksheet.Columns.AutoFit();
            pExcelAPP.DisplayAlerts = false;
            pWorkBook.SaveAs(strSavePath + "\\" + strfilename, AccessMode: XlSaveAsAccessMode.xlNoChange);
            pExcelAPP.Quit();
            //KillExcel();
        }

        ///// <summary>
        ///// 输出数据到Excel中
        ///// </summary>
        ///// <param name="pParameterResultToExcel">指标结果信息</param>
        ///// <param name="strName">文件名称</param>
        ///// <param name="strSavePath">保存路径</param>
        ///// <remarks>一维数组</remarks>
        //public static void ExportDataToExcel(double[] dblData, string strName, string strSavePath)
        //{
        //    ////为应付Excel的bug，加入以下两行代码
        //    //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;

        //    int intRowCount = dblData.GetUpperBound(0);
        //    for (int i = 0; i < intRowCount; i++)
        //    {
        //        pWorksheet.Cells[i + 1, 1] = dblData[i];
        //    }

        //    pWorksheet.Columns.AutoFit();
        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.SaveAs(strSavePath + "\\" + strfilename, AccessMode: XlSaveAsAccessMode.xlNoChange);
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //}


        /// <summary>
        /// 输出数据到Excel中
        /// </summary>
        /// <param name="pParameterResultToExcel">指标结果信息</param>
        /// <param name="strName">文件名称</param>
        /// <param name="strSavePath">保存路径</param>
        /// <remarks>二维数列</remarks>
        public static void ExportDataltltToExcel(List<List<double>> dblDataltlt, string strName, string strSavePath)
        {
            //    //为应付Excel的bug，加入以下两行代码
            //    System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
            string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
            strfilename = strfilename + strName;

            int intRowCount = dblDataltlt.Count;
            for (int i = 0; i < intRowCount; i++)
            {
                int intColCount = dblDataltlt[i].Count;
                for (int j = 0; j < intColCount; j++)
                {
                    pWorksheet.Cells[i + 1, j + 1] = dblDataltlt[i][j];
                }
            }

            pWorksheet.Columns.AutoFit();
            pExcelAPP.DisplayAlerts = false;
            pWorkBook.SaveAs(strSavePath + "\\" + strfilename + ".xlsx", AccessMode: XlSaveAsAccessMode.xlNoChange);
            pExcelAPP.Quit();
            //KillExcel();
        }

        ///// <summary>
        ///// 输出数据到Excel中
        ///// </summary>
        ///// <param name="pParameterResultToExcel">指标结果信息</param>
        ///// <param name="strName">文件名称</param>
        ///// <param name="strSavePath">保存路径</param>
        ///// <remarks>二维数列</remarks>
        //public static void ExportDataltltToExcelSW(List<List<double>> dblDataltlt, string strName, string strSavePath)
        //{
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;
        //    string strEntireFileName = strSavePath + "\\" + strfilename;
        //    pWorkBook.SaveAs(strEntireFileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive);
        //    //pWorkBook 
        //    //pWorksheet.close
        //    pWorkBook.Close(false, false, false);
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //    //SaveFileDialog saveFileDialog = new SaveFileDialog();
        //    //saveFileDialog.FileName = strEntireFileName;
        //    //Stream myStream = saveFileDialog.OpenFile();
        //    //SaveFileDialog sfg = new SaveFileDialog();
        //    //sfg.FileName = strEntireFileName + ".xls";
        //    //Stream myStream = sfg.OpenFile();
        //    //StreamWriter sw = new StreamWriter(myStream, Encoding.GetEncoding(-0));


        //    StreamWriter sw = new StreamWriter(strEntireFileName + ".xls", false, Encoding.GetEncoding(-0));


        //    long lngStart = System.Environment.TickCount;
        //    int intRowCount = dblDataltlt.Count;

        //    for (int i = 0; i < intRowCount; i++)
        //    {
        //        string tempStr = "";
        //        int intColCount = dblDataltlt[i].Count;
        //        for (int j = 0; j < intColCount; j++)
        //        {
        //            if (j > 0)
        //            {
        //                tempStr += "\t";
        //            }
        //            tempStr += dblDataltlt[i][j];

        //        }
        //        //tempStr.TrimEnd(charend);
        //        sw.WriteLine(tempStr);
        //    }
        //    sw.Close();


        //    long lngEnd = System.Environment.TickCount;
        //    long lngTime = lngEnd - lngStart;
        //    //myStream.Close();
        //    //ofg.clo
        //    //pWorksheet.Columns.AutoFit();
        //    //pExcelAPP.DisplayAlerts = false;
        //    //pWorkBook.SaveAs(strSavePath + "\\" + strfilename, AccessMode: XlSaveAsAccessMode.xlNoChange);
        //    //pExcelAPP.Quit();
        //    //KillExcel();
        //}


        ///// <summary>
        ///// 输出数据到Excel中
        ///// </summary>
        ///// <param name="pParameterResultToExcel">指标结果信息</param>
        ///// <param name="strName">文件名称</param>
        ///// <param name="strSavePath">保存路径</param>
        ///// <remarks>二维数列</remarks>
        //public static void ExportColDataltltToExcel(List<List<double>> dblDataltlt, string strName, string strSavePath)
        //{
        //    //    //为应付Excel的bug，加入以下两行代码
        //    //    System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    //    System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;

        //    int intRowCount = dblDataltlt[0].Count;
        //    int intColCount = dblDataltlt.Count;
        //    for (int i = 0; i < intRowCount; i++)
        //    {
        //        for (int j = 0; j < intColCount; j++)
        //        {
        //            pWorksheet.Cells[i + 1, j + 1] = dblDataltlt[j][i];
        //        }
        //    }

        //    pWorksheet.Columns.AutoFit();
        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.SaveAs(strSavePath + "\\" + strfilename, AccessMode: XlSaveAsAccessMode.xlNoChange);
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //}

        ///// <summary>
        ///// 输出数据到Excel中
        ///// </summary>
        ///// <param name="pParameterResultToExcel">指标结果信息</param>
        ///// <param name="strName">文件名称</param>
        ///// <param name="strSavePath">保存路径</param>
        ///// <remarks>二维数组</remarks>
        //public static  void ExportDataToExcel2(double[,] dbldata, string strName, string strSavePath)
        //{


        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;

        //    int intRowCount = dbldata.GetUpperBound(0);
        //    int intColCount = dbldata.GetUpperBound(1);

        //    for (int i = 0; i < intRowCount; i++)
        //    {
        //        for (int j = 0; j < intColCount; j++)
        //        {
        //            pWorksheet.Cells[i + 1, j + 1] = dbldata[i,j];
        //        }
        //    }

        //    pWorksheet.Columns.AutoFit();
        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.SaveAs(strSavePath + "\\" + strfilename, AccessMode: XlSaveAsAccessMode.xlNoChange);
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //}


        /// <summary>
        /// 输出数据到Excel中
        /// </summary>
        /// <param name="pParameterResultToExcel">指标结果信息</param>
        /// <param name="strName">文件名称</param>
        /// <param name="strSavePath">保存路径</param>
        /// <remarks>二维矩阵</remarks>
        public static void ExportDataToExcel2(VBMatrix maxData, string strName, string strSavePath)
        {
            //为应付Excel的bug，加入以下两行代码
            //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
            string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
            strfilename = strfilename + strName;

            int intRowCount = maxData.Row;
            int intColCount = maxData.Col;

            for (int i = 0; i < intRowCount; i++)
            {
                for (int j = 0; j < intColCount; j++)
                {
                    pWorksheet.Cells[i + 1, j + 1] = maxData[i, j];
                }
            }

            pWorksheet.Columns.AutoFit();
            pExcelAPP.DisplayAlerts = false;
            pWorkBook.SaveAs(strSavePath + "\\" + strfilename, AccessMode: XlSaveAsAccessMode.xlNoChange);
            pExcelAPP.Quit();
            //KillExcel();
        }

        ///// <summary>
        ///// 输出系数矩阵A
        ///// </summary>
        ///// <param name="maxData">系数矩阵A(注意：该矩阵从第0行到第i行为对角矩阵，i行之后为全矩阵</param>
        ///// <param name="strName">文件名称</param>
        ///// <param name="strSavePath">保存路径</param>
        ///// <remarks>二维矩阵</remarks>
        //public static void ExportDataToExcelA(VBMatrix maxData, string strName, string strSavePath)
        //{
        //    //为应付Excel的bug，加入以下两行代码
        //    //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;

        //    int intRowCount = maxData.Row;
        //    int intColCount = maxData.Col;

        //    for (int i = 0; i < intColCount; i++)
        //    {
        //        pWorksheet.Cells[i + 1, i + 1] = maxData[i, i];
        //    }

        //    for (int i = intColCount; i < intRowCount; i++)
        //    {
        //        for (int j = 0; j < intColCount; j++)
        //        {
        //            pWorksheet.Cells[i + 1, j + 1] = maxData[i, j];
        //        }
        //    }

        //    pWorksheet.Columns.AutoFit();
        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.SaveAs(strSavePath + "\\" + strfilename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange);
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //}

        ///// <summary>
        ///// 输出权重矩阵P(注意：P为对角矩阵)
        ///// </summary>
        ///// <param name="maxData">权重矩阵P(注意：P为对角矩阵)</param>
        ///// <param name="strName">文件名称</param>
        ///// <param name="strSavePath">保存路径</param>
        ///// <remarks>二维矩阵</remarks>
        //public static void ExportDataToExcelP(VBMatrix maxData, string strName, string strSavePath)
        //{
        //    //为应付Excel的bug，加入以下两行代码
        //    //System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
        //    //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
        //    string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
        //    strfilename = strfilename + strName;

        //    int intRowCount = maxData.Row;
        //    int intColCount = maxData.Col;

        //    for (int i = 0; i < intRowCount; i++)
        //    {
        //        pWorksheet.Cells[i + 1, i + 1] = maxData[i, i];

        //    }

        //    pWorksheet.Columns.AutoFit();
        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.SaveAs(strSavePath + "\\" + strfilename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlNoChange);
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //}

        ///// <summary>
        ///// insert a data into an existed excel file at specified cell
        ///// </summary>
        ///// <param name="strPath">the path of the file</param>
        //public static void InsertData(string strPath, int intRow, int intCol, double dblData)
        //{
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Open(strPath);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;

        //    pWorksheet.Cells[intRow, intCol] = dblData;

        //    pExcelAPP.DisplayAlerts = false;
        //    pWorkBook.Save();
        //    pExcelAPP.Quit();
        //    //KillExcel();
        //}

        /// <summary>
        /// 从Excel中读入数据
        /// </summary>
        /// <param name="strPath">文件路径</param>
        /// <returns>pParameterResult：较大比例尺线状要素上、较小比例尺线状要素、对应点列</returns>
        public static CParameterResult InputDataResultPtLt(string strPath, bool blnReverse = false)
        {
            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Open(strPath);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;

            List<CCorrCpts> pCorrCptsLt = new List<CCorrCpts>();
            List<CPoint> frcptlt = new List<CPoint>();
            List<CPoint> tocptlt = new List<CPoint>();
            int intRow = pWorksheet.UsedRange.Rows.Count;
            System.Array values = (System.Array)pWorksheet.UsedRange.Formula;
            for (int i = 0; i < intRow - 1; i++)
            {
                CPoint frcpt = new CPoint(Convert.ToInt32(values.GetValue(i + 2, 1)), 
                    Convert.ToDouble(values.GetValue(i + 2, 2)), Convert.ToDouble(values.GetValue(i + 2, 3)));
                CPoint tocpt = new CPoint(Convert.ToInt32(values.GetValue(i + 2, 4)), 
                    Convert.ToDouble(values.GetValue(i + 2, 5)), Convert.ToDouble(values.GetValue(i + 2, 6)));

                if (blnReverse == true)
                {
                    CHelpFunc.Swap(frcpt, tocpt);
                }

                if ((frcpt.ID != -1) && (tocpt.ID != -1))
                {
                    frcpt.isCtrl = true;
                    tocpt.isCtrl = true;
                }
                frcpt.CorrespondingPtLt = new List<CPoint>();
                frcpt.CorrespondingPtLt.Add(tocpt);
                frcptlt.Add(frcpt);
                tocptlt.Add(tocpt);
                CCorrCpts pCorrCpts = new CCorrCpts(frcpt, tocpt);
                pCorrCptsLt.Add(pCorrCpts);
            }
            pExcelAPP.Quit();

            //删除重复点，以生成frcpl
            for (int i = frcptlt.Count - 2; i >= 0; i--)
            {
                if (frcptlt[i].Equals2D(frcptlt[i + 1]))
                {
                    frcptlt.RemoveAt(i + 1);
                }
            }

            //删除重复点，以生成tocpl
            for (int i = tocptlt.Count - 2; i >= 0; i--)
            {
                if (tocptlt[i].Equals2D(tocptlt[i + 1]))
                {
                    tocptlt.RemoveAt(i + 1);
                }
            }

            //生成结果
            CPolyline frcpl = new CPolyline(0, frcptlt);
            CPolyline tocpl = new CPolyline(1, tocptlt);
            CParameterResult pParameterResult = new CParameterResult();
            pParameterResult.FromCpl = frcpl;
            pParameterResult.ToCpl = tocpl;
            double dblfr = frcpl.pPolyline.Length;
            double dblto = tocpl.pPolyline.Length;

            pParameterResult.CCorrCptsLt = pCorrCptsLt;
            //pParameterResult.CResultPtLt = ResultPtLt;

            return pParameterResult;
        }



        ///// <summary>
        ///// 从Excel中读入数据
        ///// </summary>
        ///// <param name="strPath">文件路径</param>
        ///// <returns>pParameterResult：较大比例尺线状要素上、较小比例尺线状要素、对应点列</returns>
        ///// <remarks >数据格式：第一行为表头，即各列的名称；
        /////                     第一列为数据序号；
        /////                     第二列为时间；
        /////                     第三、四、五列分别为X、Y、Z坐标</remarks>
        //public static CPolyline InputDataXYZT(string strPath)
        //{
        //    Excel.Application pExcelAPP = new Excel.Application();
        //    pExcelAPP.Visible = false;
        //    Workbook pWorkBook = pExcelAPP.Workbooks.Open(strPath);
        //    Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;

        //    int intRow = pWorksheet.UsedRange.Rows.Count;
        //    System.Array values = (System.Array)pWorksheet.UsedRange.Formula;
        //    List<CPoint> cptlt = new List<CPoint>();
        //    for (int i = 0; i < intRow - 1; i++)
        //    {
        //        CPoint cpt = new CPoint(i, Convert.ToDouble(values.GetValue(i + 2, 3)), 
        //              Convert.ToDouble(values.GetValue(i + 2, 4)), Convert.ToDouble(values.GetValue(i + 2, 5)));
        //        cpt.dblTime = Convert.ToDouble(values.GetValue(i + 2, 2));
        //        cptlt.Add(cpt);
        //    }
        //    pExcelAPP.Quit();

        //    CPolyline cpl = new CPolyline(0, cptlt);
        //    return cpl;
        //}

        /// <summary>
        /// 从Excel中读入数据
        /// </summary>
        /// <param name="strPath">文件路径</param>
        /// <returns>pParameterResult：较大比例尺线状要素上、较小比例尺线状要素、对应点列</returns>
        /// <remarks >数据格式：第一行为表头，即各列的名称；
        ///                     第一列为数据序号；
        ///                     第二列为时间；
        ///                     第三、四、五列分别为X、Y、Z坐标</remarks>
        public static List<CPolyline> InputDataLtXYZT(string strPath)
        {
            Excel.Application pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;
            Workbook pWorkBook = pExcelAPP.Workbooks.Open(strPath);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;

            //读取所有数据
            int intRow = pWorksheet.UsedRange.Rows.Count;
            System.Array values = (System.Array)pWorksheet.UsedRange.Formula;
            List<CPoint> cptlt = new List<CPoint>();
            for (int i = 0; i < intRow - 1; i++)
            {
                CPoint cpt = new CPoint(i, Convert.ToDouble(values.GetValue(i + 2, 3)), 
                    Convert.ToDouble(values.GetValue(i + 2, 4)), Convert.ToDouble(values.GetValue(i + 2, 5)));
                cpt.intTrajectory = Convert.ToInt32(values.GetValue(i + 2, 1));
                cpt.dblTime = Convert.ToDouble(values.GetValue(i + 2, 2));
                cptlt.Add(cpt);
            }
            pExcelAPP.Quit();

            //以序号为依据生成线状要素
            List<CPolyline> cpllt = new List<CPolyline>();
            int intLastTrajectory = cptlt[0].intTrajectory;
            List<CPoint> subcptlt = new List<CPoint>();
            subcptlt.Add(cptlt[0]);
            for (int i = 1; i < cptlt.Count; i++)
            {
                if (intLastTrajectory == cptlt[i].intTrajectory)
                {
                    subcptlt.Add(cptlt[i]);
                }
                else
                {
                    cpllt.Add(new CPolyline(intLastTrajectory, subcptlt));
                    intLastTrajectory = cptlt[i].intTrajectory;
                    subcptlt = new List<CPoint>();
                    subcptlt.Add(cptlt[i]);
                }
            }

            //注意：最后一个线段并没有添加，因此在此处添加
            cpllt.Add(new CPolyline(intLastTrajectory, subcptlt));

            //返回
            return cpllt;
        }


        /// <summary>
        /// 关闭有关Excel的进程
        /// </summary>
        public static void KillExcel()
        {
            try
            {
                Process[] pProcess;
                pProcess = System.Diagnostics.Process.GetProcessesByName("Excel");
                for (int i = pProcess.Length - 1; i >= 0; i--)
                {
                    pProcess[i].Kill();
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }












    }
}
