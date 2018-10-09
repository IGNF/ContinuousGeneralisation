using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ContinuousGeneralizer;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmSimilarity
{
    public partial class FrmSimDeflection : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        
        


        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmSimDeflection()
        {
            InitializeComponent();
        }

        public FrmSimDeflection(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmSimDeflection_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            CConstants.strMethod = "SimDeflection";
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelpFuncExcel.InputDataResultPtLt(OFG.FileName);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //if (SFD.FileName == null || SFD.FileName == "") return;
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);

            //List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;
            //double dblStandardX = pCorrCptsLt[0].ToCpt.X - pCorrCptsLt[0].FrCpt.X;
            //double dblStandardY = pCorrCptsLt[0].ToCpt.Y - pCorrCptsLt[0].FrCpt.Y;
            //CPoint CStandardVectorCpt = new CPoint(0, dblStandardX, dblStandardY);

            //CDeflection pDeflection = new CDeflection();
            //double dblDeflection = pDeflection.CalDeflection(pCorrCptsLt, CStandardVectorCpt);


            //double dblSimilarity = 1 - dblDeflection / (_DataRecords.ParameterResult.FromCpl.pPolyline.Length + _DataRecords.ParameterResult.ToCpl.pPolyline.Length);
            //MessageBox.Show(dblSimilarity.ToString());
        }

    }
}
