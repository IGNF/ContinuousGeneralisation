using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CMorphingExtend;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingLSA
{
    public partial class FrmALALMulti : FrmALLMulti
    {
        private CALALMulti _pALALMulti = new CALALMulti();
        private int _intI = 0;

        public FrmALALMulti()
        {
            InitializeComponent();
            //CConstants.strMethod = "LandingTime";
        }

        public FrmALALMulti(CDataRecords pDataRecords)
        {
            InitializeComponent();

            CParameterInitialize ParameterInitialize = pDataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer = this.cboLayer;
            ParameterInitialize.txtInterpolationNum = this.txtInterpolatedNum;
            ParameterInitialize.txtIterationNum = this.txtIterationNum;
            CConstants.strMethod = "ALALMulti";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoad();
            _DataRecords = pDataRecords;

        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);
            _pALALMulti = new CALALMulti(_DataRecords);
            _pALALMulti.ALALMultiMorphing();

            CHelperFunction.SaveCPlLt(_DataRecords.ParameterResult.CResultPlLt, "ALALMulti", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        public override void btnInputResults_Click(object sender, EventArgs e)
        {
            //获取当前选择的点要素图层
            //大比例尺要素图层
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            IFeatureLayer pFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayer.SelectedIndex);
                                                                      
            ParameterInitialize.pFeatureLayer = pFLayer;
            List<CPolyline> cpllt = CHelperFunction.GetCPlLtByFeatureLayer(pFLayer);

            CParameterResult pParameterResult = new CParameterResult();
            pParameterResult.CResultPlLt = cpllt;
            pParameterResult.FromCpl = cpllt[0];
            pParameterResult.ToCpl = cpllt[cpllt.Count - 1];

            _DataRecords.ParameterResult = pParameterResult;
        }

        public override void timerReduce_Tick(object sender, EventArgs e)
        {
            List<CPolyline> cpllt = _DataRecords.ParameterResult.CResultPlLt;
            if (_intI >= 0 && _intI < cpllt.Count)
            {
                pbScale.Value = Convert.ToInt16(100 * _intI / (cpllt.Count - 1));
                IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
                IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
                pGra.DeleteAllElements();
                CHelperFunction.ViewPolyline(m_mapControl, cpllt[_intI]);  //显示生成的线段
                _intI -= 1;
            }
            else if (_intI < 0)
            {
                this.timerReduce.Enabled = false;
                _intI = 0;
            }
            else
            {
                _intI = cpllt.Count-1;
            }
        }

        public override void timerAdd_Tick(object sender, EventArgs e)
        {
            List<CPolyline> cpllt = _DataRecords.ParameterResult.CResultPlLt;
            if (_intI >= 0 && _intI < cpllt.Count)
            {
                pbScale.Value = Convert.ToInt16(100 * _intI / (cpllt.Count - 1));
                IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
                IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
                pGra.DeleteAllElements();
                CHelperFunction.ViewPolyline(m_mapControl, cpllt[_intI]);  //显示生成的线段
                _intI += 1;
            }
            else if (_intI >= cpllt.Count)
            {
                this.timerAdd.Enabled = false;
                _intI = cpllt.Count - 1;
            }
            else
            {
                _intI = 0;
            }
        }






    }
}
