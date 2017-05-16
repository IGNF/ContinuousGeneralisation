using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
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
    public class CExcelOperator
    {
        //protected StreamWriter sw { get; set; }
        protected Excel.Application pExcelAPP { get; set; }
        protected Workbook pWorkBook { get; set; }
        protected Worksheet pWorksheet { get; set; }
        protected string strEntireFileName { get; set; }

        public CExcelOperator(string strEntireFileName)
        {
            this.strEntireFileName = strEntireFileName;
            if (strEntireFileName == null || strEntireFileName == "")
            {
                this.strEntireFileName = CHelpFuncExcel.OpenAnExcel();
            }

            this.pExcelAPP = new Excel.Application();
            pExcelAPP.Visible = false;

        }


        //public CExcelOperator(string strEntireFileName, bool blnAppend = false, int intWorkSheets=1)
        //{
        //    //if (strEntireFileName == null || strEntireFileName == "")
        //    //{
        //    //    strEntireFileName = CHelpFuncExcel.OpenAnExcel();
        //    //}

        //    //this.pExcelAPP = new Excel.Application();
        //    //pExcelAPP.Visible = false;
        //    //Workbook pWorkBook = pExcelAPP.Workbooks.Open(strEntireFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //    //Worksheet pWorksheet = pWorkBook.Worksheets[intWorkSheets] as Worksheet;
        //    //this.sw = new StreamWriter(strEntireFileName + ".xls", blnAppend, Encoding.GetEncoding(-0));  //it's better if we can output a file with ".xlsx", but that causes problems
        //}

        public void Create()
        {
            //if (strSavePath == null || strSavePath == "")
            //{
            //    strSavePath = CHelpFuncExcel.OpenAnExcel();
            //}

            Workbook pWorkBook = pExcelAPP.Workbooks.Add(true);
            Worksheet pWorksheet = pWorkBook.Worksheets[1] as Worksheet;
            //string strfilename = System.IO.Path.GetFileNameWithoutExtension(strSavePath);
            //strfilename = strfilename + strName;
            //string strEntireFileName = strSavePath + "\\" + strfilename;
            pExcelAPP.DisplayAlerts = false;
            pWorkBook.SaveAs(this.strEntireFileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            pWorkBook.Close(false, false, false);

            Open();
        }

        public void Open(int intWorkSheets = 1)
        {
            this. pWorkBook = pExcelAPP.Workbooks.Open(this.strEntireFileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            this. pWorksheet = pWorkBook.Worksheets[intWorkSheets] as Worksheet;            
        }


        ///// <summary>
        ///// doesn't work
        ///// </summary>
        ///// <param name="intWorkSheets"></param>
        //public void AutoFitColumns(int intWorkSheets =1 )
        //{
        //    pExcelAPP.DisplayAlerts = false;


        //    string strNewFileName = Path.GetFileNameWithoutExtension(strEntireFileName) + "test.xls";
        //    pWorkBook.SaveAs(strNewFileName, 56, Type.Missing, Type.Missing, Type.Missing, Type.Missing, XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        //    pWorkBook.Close(false, false, false);

        //    int intRow = this.pWorksheet.UsedRange.Rows.Count;
        //    int intCol = this.pWorksheet.UsedRange.Columns.Count;
        //    System.Array values = (System.Array)this.pWorksheet.UsedRange.Formula;

        //    object obj = values.GetValue(2, 3);



        //    Range aRange = this.pWorksheet.get_Range("A1", "F3");
        //    aRange.Columns.AutoFit();
        //    Open(intWorkSheets);
        //    this.pWorksheet.Cells[5, 5] = 10;
        //    this.pWorksheet.Cells["B:C"].Columns.AutoFit();
        //    this.pWorksheet.UsedRange.Columns.AutoFit();
        //}

        public void Close()
        {
            //this.pWorksheet.sa
            this.pWorkBook.Save();
            this.pWorkBook.Close(false, false, false);
            this.pExcelAPP.Quit();
        }











    }
}
