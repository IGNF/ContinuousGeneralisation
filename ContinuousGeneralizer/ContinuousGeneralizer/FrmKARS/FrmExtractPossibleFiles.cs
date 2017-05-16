using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;

using MorphingClass.CUtility;

namespace ContinuousGeneralizer.FrmKARS
{
    public partial class FrmExtractPossibleFiles : Form
    {
        public FrmExtractPossibleFiles()
        {
            InitializeComponent();
        }

        /// <summary>
        /// for the methods that we want to check, we find their positions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnRun_Click(object sender, EventArgs e)
        {
            var aObj_CheckingMethods = CHelpFuncExcel.ReadDataFromExcel
                (@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out\checkingmethods");

            List<List<object>> resultobjltlt = new List<List<object>>(aObj_CheckingMethods.GetLength(0));
            int intMaxOccurency = 0;
            for (int i = 0; i < aObj_CheckingMethods.GetLength(0); i++)
            //for (int i = 0; i < 1; i++)
            {
                var strMethod = aObj_CheckingMethods[i][0].ToString();
                string[] filenames = Directory.GetFiles(@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Eclipse\fsvw_common_KARS_DM", "*.*", SearchOption.TopDirectoryOnly);
                List<object> resultobjlt = new List<object> { strMethod };
                int intOccurency = 0;
                foreach (var filename in filenames)
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(filename);
                    string strline = "";
                    int intlinecounter = 1;
                    while ((strline = file.ReadLine()) != null)
                    {
                        //strline.
                        //if (intlinecounter==60)
                        //{
                        //    int ss = 5;
                        //}

                        if (strline.Contains(strMethod + "(") && !strline.Contains("<td>" + strMethod)) //if strMethod is used as a method, we try to find the end of this using
                        {

                            int intStartline = intlinecounter;


                            while (strline != null)
                            {
                                if (strline.Contains(";"))
                                {
                                    break;
                                }

                                if (strline.Contains("{") && strline[0] == '{')
                                {
                                    break ;
                                }

                                strline = file.ReadLine();
                                intlinecounter++;
                            }

                            if (strline.Contains(";") && !(strline.Contains("{") && strline[0] == '{'))
                            {
                                //the strMethod is called here, we don't care
                            }
                            else if (!strline.Contains(";") && (strline.Contains("{") && strline[0] == '{'))
                            {
                                resultobjlt.Add(System.IO.Path.GetFileName(filename));
                                resultobjlt.Add(intStartline);
                                while (!(strline.Contains("}") && strline[0] == '}'))
                                {
                                    strline = file.ReadLine();
                                    intlinecounter++;
                                }
                                resultobjlt.Add(intlinecounter);
                                intOccurency++;
                            }
                            else
                            {
                                MessageBox.Show("impossible case!");
                            }
                        }
                        intlinecounter++;
                    }
                    file.Close();
                }
                resultobjltlt.Add(resultobjlt);
                intMaxOccurency = Math.Max(intMaxOccurency, intOccurency);
            }

            var strHeadLt = new List<string>(1 + intMaxOccurency * 3);
            strHeadLt.Add("Methods");
            for (int i = 0; i < intMaxOccurency; i++)
            {
                strHeadLt.Add("File");
                strHeadLt.Add("Start");
                strHeadLt.Add("End");
            }

            CHelpFuncExcel.ExportToExcel(resultobjltlt,
                CHelpFunc.GetTimeStampWithPrefix() + "_methods_position",
                @"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out", strHeadLt);

            MessageBox.Show("done!");
        }

        /// <summary>
        /// output the information that we want 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRun2_Click(object sender, EventArgs e)
        {
            var aObj_Out_kars = CHelpFuncExcel.ReadDataFromExcel
                (@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out\kars2");
            var aObj_Met_pos = CHelpFuncExcel.ReadDataFromExcel
                (@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out\methods_position");

            //char[] charSeparators = new char[] { ' ', ';' };
            //var result = str.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

            //var strPos_fileSD = new SortedDictionary<string, string>();
            var CheckRangeSS = new SortedSet<CCheckRange>();
            for (int i = 1; i < aObj_Met_pos.GetLength(0); i++)
            {
                int j = 1;
                while (j < aObj_Met_pos[i].GetLength(0) && aObj_Met_pos[i][j].ToString() != "" && aObj_Met_pos[i][j].ToString() != null)
                {
                    CCheckRange pchkrng = new CCheckRange(aObj_Met_pos[i][j++].ToString(),
                        Convert.ToInt32(aObj_Met_pos[i][j++]),
                        Convert.ToInt32(aObj_Met_pos[i][j++]),
                        aObj_Met_pos[i][0].ToString());

                    CheckRangeSS.Add(pchkrng);
                }
            }

            //var astrRemainLt = new List<object[]>();
            var astrPasteLt = new List<object[]>();

            //from 'possible_files', we know that whether a function is at top-level
            var aObj_Pos_fil = CHelpFuncExcel.ReadDataFromExcel
                (@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out\possible_files");
            var strMethodFilesSD = new SortedDictionary<string, object[]>();
            for (int i = 1; i < aObj_Pos_fil.GetLength(0); i++)
            {
                strMethodFilesSD.Add(aObj_Pos_fil[i][0].ToString(), aObj_Pos_fil[i]);
            }

            for (int i = 1; i < aObj_Out_kars.GetLength(0); i++)
            {
                CCheckRange pchkrng = new CCheckRange(aObj_Out_kars[i][0].ToString(),
                        Convert.ToInt32(aObj_Out_kars[i][1]),
                        Convert.ToInt32(aObj_Out_kars[i][1]));
                CCheckRange pchkrngstart = new CCheckRange(aObj_Out_kars[i][0].ToString(), 0, 0);
                var predecessor = CheckRangeSS.GetViewBetween(pchkrngstart, pchkrng).Max;

                if (predecessor != null && predecessor.intEndline >= pchkrng.intEndline)
                {
                    //astrRemainLt.Add(aObj_Out_kars[i]);

                    var aPaste = new object[]
                    {
                       predecessor.strFileName,
                       predecessor.strMethod,
                       "",   //Mangled Name
                       "",   //Line, actual
                       pchkrng.intStartline,
                       GetColumn(@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Eclipse\fsvw_common_KARS_DM\"+predecessor.strFileName, pchkrng.intStartline),
                       "",   //Stmt-No
                       IsCalled(strMethodFilesSD,predecessor.strMethod),
                       aObj_Out_kars[i][2],
                       aObj_Out_kars[i][3],
                       aObj_Out_kars[i][4]                   
                    };
                    astrPasteLt.Add(aPaste);
                }
            }

            //CHelpFuncExcel.ExportToExcel(astrRemainLt,
            //    CHelpFunc.GetTimeStamp() + "_out_kars_Extracted_" + astrRemainLt.Count,
            //    @"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out");
            CHelpFuncExcel.ExportToExcel(astrPasteLt,
                CHelpFunc.GetTimeStampWithPrefix() + "_out_kars_ExtractedProcessed_" + astrPasteLt.Count,
                @"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out");
        }

        private int GetColumn(string strFileName, int intLine)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(strFileName);
            string strline = "";
            int intlinecounter = 1;
            
            while ((strline = file.ReadLine()) != null)
            {
                if (intLine == intlinecounter)
                {
                    int intColumn = 1;
                    for (int j = 0; j < strline.Length; j++)
                    {
                        if (strline[j]=='\t')
                        {
                            intColumn += 4;
                        }
                        else if (strline[j] == ' ')
                        {
                             intColumn++;
                        }
                        else
                        {
                            return intColumn;
                        }
                    }

                    throw new ArgumentNullException("cannot find the column!");
                }
                intlinecounter++;
            }

            return -1;
        }

        private string IsCalled(SortedDictionary<string, object[]> strMethodFilesSD, string strMethod)
        {
            object[] aobjValue;
            var blnGetValue = strMethodFilesSD.TryGetValue(strMethod, out aobjValue);
            if (blnGetValue == false)
            {
                throw new ArgumentNullException("method doesn't exist!");
            }
            else
            {
                if (aobjValue[3].ToString() == "")
                {
                    return "y";
                }
                else
                {
                    return "n";
                }
            }

        }

        private void btnRun3_Click(object sender, EventArgs e)
        {
            string strFileNameCheckingMethods = @"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out\checkingmethods";
            var aObj_CheckingMethods = CHelpFuncExcel.ReadDataFromExcel(strFileNameCheckingMethods);

            List<List<object>> resultobjltlt = new List<List<object>>(aObj_CheckingMethods.GetLength(0));
            int intMaxOccurency = 0;
            for (int i = 0; i < aObj_CheckingMethods.GetLength(0); i++)
            //for (int i = 0; i < 1; i++)
            {
                var strMethod = aObj_CheckingMethods[i][0].ToString();
                string[] filenames = Directory.GetFiles(@"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Eclipse\fsvw_common_KARS_DM", "*.*", SearchOption.TopDirectoryOnly);
                List<object> resultobjlt = new List<object> { strMethod };
                int intOccurency = 0;
                foreach (var filename in filenames)
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(filename);
                    string strline = "";
                    int intlinecounter = 1;
                    while ((strline = file.ReadLine()) != null)
                    {
                        if (strline.Contains(strMethod))
                        {
                            resultobjlt.Add(System.IO.Path.GetFileName(filename));
                            if (strline.Contains(strMethod + "("))  //if strMethod is used as a method, we try to find the end of this using
                            {
                                resultobjlt.Add(intlinecounter);

                                while (strline != null && !strline.Contains(";"))
                                {
                                    strline = file.ReadLine();
                                    intlinecounter++;
                                }
                                resultobjlt.Add(intlinecounter);
                            }
                            else  //a special announce case
                            {
                                resultobjlt.Add(intlinecounter - 1);
                                resultobjlt.Add(intlinecounter + 2);
                            }

                            intOccurency++;
                        }
                        intlinecounter++;
                    }
                    file.Close();
                }
                resultobjltlt.Add(resultobjlt);
                intMaxOccurency = Math.Max(intMaxOccurency, intOccurency);
            }

            var strHeadLt = new List<string>(1 + intMaxOccurency * 3);
            strHeadLt.Add("Methods");
            for (int i = 0; i < intMaxOccurency; i++)
            {
                strHeadLt.Add("File");
                strHeadLt.Add("Start");
                strHeadLt.Add("End");
            }

            CHelpFuncExcel.ExportToExcel(resultobjltlt,
                CHelpFunc.GetTimeStampWithPrefix() + "_methods_position",
                @"C:\Study\MyWork\CodesChecking\fsvw_common_KARS_DM_Out", strHeadLt);

            MessageBox.Show("done!");
        }
    }
}
