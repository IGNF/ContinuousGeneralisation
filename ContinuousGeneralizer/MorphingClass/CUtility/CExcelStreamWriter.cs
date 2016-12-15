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
    public class CExcelStreamWriter
    {
        protected StreamWriter sw { get; set; }
        //protected Excel.Application pExcelAPP { get; set; }
        //protected Workbook pWorkBook { get; set; }
        //protected Worksheet pWorksheet { get; set; }

        public CExcelStreamWriter()
        {
        }


        public CExcelStreamWriter(string strEntireFileName, bool blnAppend = false, int intWorkSheets = 1)
        {
            if (strEntireFileName == null || strEntireFileName == "")
            {
                strEntireFileName = CHelperFunctionExcel.OpenAnExcel();
            }

            this.sw = new StreamWriter(strEntireFileName + ".xls", blnAppend, Encoding.GetEncoding(-0));  //it's better if we can output a file with ".xlsx", but that causes problems
        }

        public void WriteLine(IEnumerable<object> pObjDataEb)
        {
            string strtemp = "";
            foreach (var obj in pObjDataEb)
            {
                strtemp += obj.ToString() + "\t";
            }
            this.sw.WriteLine(strtemp);
        }

        public void Close()
        {
            this.sw.Close();
        }











    }
}
