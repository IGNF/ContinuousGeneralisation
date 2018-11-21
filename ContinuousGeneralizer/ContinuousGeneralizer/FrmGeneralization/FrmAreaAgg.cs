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
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeneralizationMethods;
using MorphingClass.CGeometry;

using ILOG.Concert;
using ILOG.CPLEX;

namespace ContinuousGeneralizer.FrmMorphing
{
    /// <summary>
    /// Continuous Aggregation of Maps based on Dijkstra: AreaAgg_AStar
    /// </summary>
    public partial class FrmAreaAgg : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        

        private CAreaAgg_Base _pCAreaAgg_Base;
        private CAreaAgg_Greedy _pCAreaAgg_Greedy;
        private CAreaAgg_AStar _pCAreaAgg_AStar;
        private CAreaAgg_ILP _pCAreaAgg_ILP;


        private double _dblProp = 0.5;


        public FrmAreaAgg()
        {
            InitializeComponent();
        }
        
        public FrmAreaAgg(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            pDataRecords.ParameterInitialize.frmCurrentForm = this;
        }

        private void FrmAreaAgg_Load(object sender, EventArgs e)
        {
            var ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(2);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);
            CConstants.strMethod = "AStar";
            ParameterInitialize.cboShapeConstraint = this.cboShapeConstraint;
            ParameterInitialize.chkSmallest = this.chkSmallest;

            //0 NonShape
            //1 MinimizeInteriorBoundaries
            //2 MaximizeMinComp_EdgeNumber
            //3 MaximizeMinComp_Combine
            //4 MaximizeAvgComp_EdgeNumber
            //5 MaximizeAvgComp_Combine
            ParameterInitialize.cboShapeConstraint.SelectedIndex = 1;
            //ParameterInitialize.chkSmallest.Checked = false;


            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }

        public void btnGreedy_Click(object sender, EventArgs e)
        {
            var ParameterInitialize = _DataRecords.ParameterInitialize;
            CConstants.strMethod = "Greedy";
            var objResultSD = new SortedDictionary<string, List<object>>();
            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);
            //Read Datasets
            var pCAreaAgg_Greedy = new CAreaAgg_Greedy(ParameterInitialize);
            pCAreaAgg_Greedy.AreaAggregation();

            this.txtEvaluation.Text = pCAreaAgg_Greedy.dblCost.ToString();
            StrObjLtDt.Merge(pCAreaAgg_Greedy.StrObjLtDt);
            CAreaAgg_Base.SaveData(StrObjLtDt, ParameterInitialize, "Greedy");
            _pCAreaAgg_Base = pCAreaAgg_Greedy as CAreaAgg_Base;
            MessageBox.Show("Done!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            var ParameterInitialize = _DataRecords.ParameterInitialize;
            //var objDataLtLt = new List<List<object>>();
            //var objResultSD = new SortedDictionary<string, List<object>>();
            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);
            //Read Datasets
            _pCAreaAgg_AStar = new CAreaAgg_AStar(ParameterInitialize);
            _pCAreaAgg_AStar.AreaAggregation(Convert.ToInt32(txtNodes.Text));

            this.txtEvaluation.Text = _pCAreaAgg_AStar.dblCost.ToString();
            StrObjLtDt.Merge(_pCAreaAgg_AStar.StrObjLtDt);
            CAreaAgg_Base.SaveData(StrObjLtDt, ParameterInitialize, "AStar", txtNodes.Text);

            _pCAreaAgg_Base = _pCAreaAgg_AStar as CAreaAgg_Base;
            MessageBox.Show("Done!");
        }

        private void btnRunSpecified_Click(object sender, EventArgs e)
        {
            //List<int> intSpecifiedIDLt = new List<int>();
            #region intSpecifiedIDLt
            //intSpecifiedIDLt.Add(543);
            //intSpecifiedIDLt.Add(94);
            //intSpecifiedIDLt.Add(190);
            //intSpecifiedIDLt.Add(301);
            //intSpecifiedIDLt.Add(436);
            //intSpecifiedIDLt.Add(523);
            //intSpecifiedIDLt.Add(553);
            //intSpecifiedIDLt.Add(397);
            //intSpecifiedIDLt.Add(586);
            //intSpecifiedIDLt.Add(551);
            //intSpecifiedIDLt.Add(492);
            //intSpecifiedIDLt.Add(194);
            //intSpecifiedIDLt.Add(177);
            //intSpecifiedIDLt.Add(506);
            //intSpecifiedIDLt.Add(165);
            //intSpecifiedIDLt.Add(215);
            //intSpecifiedIDLt.Add(590);
            //intSpecifiedIDLt.Add(386);
            //intSpecifiedIDLt.Add(112);
            //intSpecifiedIDLt.Add(452);
            //intSpecifiedIDLt.Add(531);
            //intSpecifiedIDLt.Add(298);
            //intSpecifiedIDLt.Add(477);
            //intSpecifiedIDLt.Add(53);
            //intSpecifiedIDLt.Add(252);
            //intSpecifiedIDLt.Add(503);
            //intSpecifiedIDLt.Add(343);
            //intSpecifiedIDLt.Add(462);
            //intSpecifiedIDLt.Add(516);
            //intSpecifiedIDLt.Add(400);
            //intSpecifiedIDLt.Add(179);
            //intSpecifiedIDLt.Add(424);
            //intSpecifiedIDLt.Add(410);
            //intSpecifiedIDLt.Add(537);
            //intSpecifiedIDLt.Add(272);
            //intSpecifiedIDLt.Add(1);
            //intSpecifiedIDLt.Add(463);
            //intSpecifiedIDLt.Add(418);
            //intSpecifiedIDLt.Add(155);
            #endregion



            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);
            //for (int i = 0; i < intSpecifiedIDLt.Count; i++)
            //{ 
            //    _pCAreaAgg_AStar = new CAreaAgg_AStar(ParameterInitialize, "RegionNum", intSpecifiedIDLt[i].ToString());
            //    _pCAreaAgg_AStar.AreaAggregation(Convert.ToInt32(txtNodes.Text));

            //    if (chkOutput.Checked==true)
            //    {
            //        btnMultiResults_Click(sender, e);
            //    }                

            //    StrObjLtDt.Merge(_pCAreaAgg_AStar.StrObjLtDt);
            //    CHelpFunc.Displaytspb(i, intSpecifiedIDLt.Count);
            //}

            //CAreaAgg_AStar.SaveData(StrObjLtDt, ParameterInitialize, "AStar", Convert.ToInt32(txtNodes.Text));

            //_pCAreaAgg_Base = _pCAreaAgg_AStar as CAreaAgg_Base;
            //MessageBox.Show("Done!");
        }



        public void btnRunILP_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboShapeConstraint.SelectedIndex = 1;

            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);

            //Read Datasets
            _pCAreaAgg_ILP = new CAreaAgg_ILP(ParameterInitialize);
            _pCAreaAgg_ILP.AreaAggregation();

            //this.txtEvaluation.Text = _pCAreaAgg_ILP.dblCost.ToString();
            StrObjLtDt.Merge(_pCAreaAgg_ILP.StrObjLtDt);
            CAreaAgg_AStar.SaveData(StrObjLtDt, ParameterInitialize, "ILP", Convert.ToInt32(_pCAreaAgg_ILP.dblTimeLimit).ToString());


            _pCAreaAgg_Base = _pCAreaAgg_ILP as CAreaAgg_Base;
            MessageBox.Show("Done!");
        }






        private void btnRunILPSpecified_Click(object sender, EventArgs e)
        {
            List<int> intSpecifiedIDLt = new List<int>();
            #region intSpecifiedIDLt
            intSpecifiedIDLt.Add(215);
            intSpecifiedIDLt.Add(94);
            intSpecifiedIDLt.Add(590);
            intSpecifiedIDLt.Add(301);
            intSpecifiedIDLt.Add(436);
            intSpecifiedIDLt.Add(386);
            intSpecifiedIDLt.Add(112);
            intSpecifiedIDLt.Add(523);
            intSpecifiedIDLt.Add(452);
            intSpecifiedIDLt.Add(531);
            intSpecifiedIDLt.Add(553);
            intSpecifiedIDLt.Add(397);
            intSpecifiedIDLt.Add(586);
            intSpecifiedIDLt.Add(190);
            intSpecifiedIDLt.Add(179);
            intSpecifiedIDLt.Add(298);
            intSpecifiedIDLt.Add(551);
            intSpecifiedIDLt.Add(492);
            intSpecifiedIDLt.Add(194);
            intSpecifiedIDLt.Add(177);
            intSpecifiedIDLt.Add(477);
            intSpecifiedIDLt.Add(53);
            intSpecifiedIDLt.Add(358);
            intSpecifiedIDLt.Add(252);
            intSpecifiedIDLt.Add(506);
            intSpecifiedIDLt.Add(424);
            intSpecifiedIDLt.Add(543);
            intSpecifiedIDLt.Add(165);
            intSpecifiedIDLt.Add(410);
            intSpecifiedIDLt.Add(97);
            intSpecifiedIDLt.Add(257);
            intSpecifiedIDLt.Add(154);
            intSpecifiedIDLt.Add(248);
            intSpecifiedIDLt.Add(544);
            intSpecifiedIDLt.Add(403);
            intSpecifiedIDLt.Add(537);
            intSpecifiedIDLt.Add(503);
            intSpecifiedIDLt.Add(429);
            intSpecifiedIDLt.Add(343);
            intSpecifiedIDLt.Add(67);
            intSpecifiedIDLt.Add(462);
            intSpecifiedIDLt.Add(272);
            intSpecifiedIDLt.Add(1);
            intSpecifiedIDLt.Add(525);
            intSpecifiedIDLt.Add(463);
            intSpecifiedIDLt.Add(391);
            intSpecifiedIDLt.Add(516);
            intSpecifiedIDLt.Add(186);
            intSpecifiedIDLt.Add(420);
            intSpecifiedIDLt.Add(418);
            intSpecifiedIDLt.Add(371);
            intSpecifiedIDLt.Add(77);
            intSpecifiedIDLt.Add(216);
            intSpecifiedIDLt.Add(14);
            intSpecifiedIDLt.Add(647);
            intSpecifiedIDLt.Add(545);
            intSpecifiedIDLt.Add(444);
            intSpecifiedIDLt.Add(441);
            intSpecifiedIDLt.Add(542);
            intSpecifiedIDLt.Add(381);
            intSpecifiedIDLt.Add(2);
            intSpecifiedIDLt.Add(479);
            intSpecifiedIDLt.Add(133);
            intSpecifiedIDLt.Add(327);
            intSpecifiedIDLt.Add(400);
            intSpecifiedIDLt.Add(569);
            intSpecifiedIDLt.Add(155);
            intSpecifiedIDLt.Add(153);
            intSpecifiedIDLt.Add(521);
            intSpecifiedIDLt.Add(500);
            intSpecifiedIDLt.Add(123);
            intSpecifiedIDLt.Add(349);
            intSpecifiedIDLt.Add(245);
            intSpecifiedIDLt.Add(29);
            intSpecifiedIDLt.Add(520);
            intSpecifiedIDLt.Add(560);
            intSpecifiedIDLt.Add(467);
            intSpecifiedIDLt.Add(33);
            intSpecifiedIDLt.Add(422);
            intSpecifiedIDLt.Add(324);
            intSpecifiedIDLt.Add(566);
            intSpecifiedIDLt.Add(684);
            intSpecifiedIDLt.Add(289);
            intSpecifiedIDLt.Add(559);
            intSpecifiedIDLt.Add(222);
            intSpecifiedIDLt.Add(679);
            intSpecifiedIDLt.Add(451);
            intSpecifiedIDLt.Add(426);
            intSpecifiedIDLt.Add(101);
            intSpecifiedIDLt.Add(47);
            intSpecifiedIDLt.Add(450);
            intSpecifiedIDLt.Add(533);
            intSpecifiedIDLt.Add(207);
            intSpecifiedIDLt.Add(572);
            intSpecifiedIDLt.Add(417);
            intSpecifiedIDLt.Add(217);
            intSpecifiedIDLt.Add(548);
            intSpecifiedIDLt.Add(368);
            intSpecifiedIDLt.Add(119);
            intSpecifiedIDLt.Add(639);
            intSpecifiedIDLt.Add(490);
            intSpecifiedIDLt.Add(437);
            intSpecifiedIDLt.Add(193);
            intSpecifiedIDLt.Add(487);
            intSpecifiedIDLt.Add(447);
            intSpecifiedIDLt.Add(329);
            intSpecifiedIDLt.Add(306);
            intSpecifiedIDLt.Add(243);
            intSpecifiedIDLt.Add(653);
            intSpecifiedIDLt.Add(394);
            intSpecifiedIDLt.Add(379);
            intSpecifiedIDLt.Add(315);
            intSpecifiedIDLt.Add(288);
            intSpecifiedIDLt.Add(224);
            intSpecifiedIDLt.Add(720);
            intSpecifiedIDLt.Add(561);
            intSpecifiedIDLt.Add(491);
            intSpecifiedIDLt.Add(0);
            intSpecifiedIDLt.Add(389);
            intSpecifiedIDLt.Add(72);
            intSpecifiedIDLt.Add(562);
            intSpecifiedIDLt.Add(475);
            intSpecifiedIDLt.Add(582);
            intSpecifiedIDLt.Add(261);
            intSpecifiedIDLt.Add(137);
            intSpecifiedIDLt.Add(42);
            intSpecifiedIDLt.Add(26);
            intSpecifiedIDLt.Add(515);
            intSpecifiedIDLt.Add(364);
            intSpecifiedIDLt.Add(234);
            intSpecifiedIDLt.Add(24);
            intSpecifiedIDLt.Add(469);
            intSpecifiedIDLt.Add(372);
            intSpecifiedIDLt.Add(348);
            intSpecifiedIDLt.Add(267);
            intSpecifiedIDLt.Add(577);
            intSpecifiedIDLt.Add(517);
            intSpecifiedIDLt.Add(405);
            intSpecifiedIDLt.Add(350);
            intSpecifiedIDLt.Add(221);
            intSpecifiedIDLt.Add(58);
            intSpecifiedIDLt.Add(712);
            intSpecifiedIDLt.Add(578);
            intSpecifiedIDLt.Add(555);
            intSpecifiedIDLt.Add(61);
            intSpecifiedIDLt.Add(278);
            intSpecifiedIDLt.Add(180);
            intSpecifiedIDLt.Add(128);
            intSpecifiedIDLt.Add(240);
            intSpecifiedIDLt.Add(466);
            intSpecifiedIDLt.Add(412);
            intSpecifiedIDLt.Add(375);
            intSpecifiedIDLt.Add(113);
            intSpecifiedIDLt.Add(75);
            intSpecifiedIDLt.Add(342);
            intSpecifiedIDLt.Add(135);
            intSpecifiedIDLt.Add(286);
            intSpecifiedIDLt.Add(68);
            intSpecifiedIDLt.Add(23);
            intSpecifiedIDLt.Add(693);
            intSpecifiedIDLt.Add(651);
            intSpecifiedIDLt.Add(539);
            intSpecifiedIDLt.Add(407);
            intSpecifiedIDLt.Add(274);
            intSpecifiedIDLt.Add(725);
            intSpecifiedIDLt.Add(607);
            intSpecifiedIDLt.Add(377);
            intSpecifiedIDLt.Add(370);
            intSpecifiedIDLt.Add(208);
            intSpecifiedIDLt.Add(65);
            intSpecifiedIDLt.Add(556);
            intSpecifiedIDLt.Add(355);
            intSpecifiedIDLt.Add(71);
            intSpecifiedIDLt.Add(52);
            intSpecifiedIDLt.Add(10);
            intSpecifiedIDLt.Add(43);
            intSpecifiedIDLt.Add(78);
            intSpecifiedIDLt.Add(702);
            intSpecifiedIDLt.Add(127);
            intSpecifiedIDLt.Add(579);
            intSpecifiedIDLt.Add(352);
            intSpecifiedIDLt.Add(235);
            intSpecifiedIDLt.Add(141);
            intSpecifiedIDLt.Add(13);
            intSpecifiedIDLt.Add(713);
            intSpecifiedIDLt.Add(518);
            intSpecifiedIDLt.Add(307);
            intSpecifiedIDLt.Add(209);
            intSpecifiedIDLt.Add(114);
            intSpecifiedIDLt.Add(50);
            intSpecifiedIDLt.Add(730);
            intSpecifiedIDLt.Add(714);
            intSpecifiedIDLt.Add(616);
            intSpecifiedIDLt.Add(528);
            intSpecifiedIDLt.Add(468);
            intSpecifiedIDLt.Add(465);
            intSpecifiedIDLt.Add(285);
            intSpecifiedIDLt.Add(271);
            intSpecifiedIDLt.Add(122);
            intSpecifiedIDLt.Add(473);
            intSpecifiedIDLt.Add(259);
            intSpecifiedIDLt.Add(669);
            intSpecifiedIDLt.Add(454);
            intSpecifiedIDLt.Add(369);
            intSpecifiedIDLt.Add(640);
            intSpecifiedIDLt.Add(488);
            intSpecifiedIDLt.Add(263);
            intSpecifiedIDLt.Add(82);
            intSpecifiedIDLt.Add(434);
            intSpecifiedIDLt.Add(385);
            intSpecifiedIDLt.Add(602);
            intSpecifiedIDLt.Add(93);
            intSpecifiedIDLt.Add(612);
            intSpecifiedIDLt.Add(411);
            intSpecifiedIDLt.Add(104);
            intSpecifiedIDLt.Add(249);
            intSpecifiedIDLt.Add(439);
            intSpecifiedIDLt.Add(482);
            intSpecifiedIDLt.Add(625);
            intSpecifiedIDLt.Add(354);
            intSpecifiedIDLt.Add(446);
            intSpecifiedIDLt.Add(461);
            intSpecifiedIDLt.Add(287);
            intSpecifiedIDLt.Add(88);
            intSpecifiedIDLt.Add(685);
            intSpecifiedIDLt.Add(320);
            intSpecifiedIDLt.Add(74);
            intSpecifiedIDLt.Add(605);
            intSpecifiedIDLt.Add(296);
            intSpecifiedIDLt.Add(242);
            intSpecifiedIDLt.Add(17);
            intSpecifiedIDLt.Add(710);
            intSpecifiedIDLt.Add(192);
            intSpecifiedIDLt.Add(353);
            intSpecifiedIDLt.Add(637);
            intSpecifiedIDLt.Add(606);
            intSpecifiedIDLt.Add(510);
            intSpecifiedIDLt.Add(608);
            intSpecifiedIDLt.Add(170);
            intSpecifiedIDLt.Add(673);
            intSpecifiedIDLt.Add(175);
            intSpecifiedIDLt.Add(408);
            intSpecifiedIDLt.Add(597);
            intSpecifiedIDLt.Add(157);
            intSpecifiedIDLt.Add(299);
            intSpecifiedIDLt.Add(641);
            intSpecifiedIDLt.Add(536);
            intSpecifiedIDLt.Add(229);
            intSpecifiedIDLt.Add(225);
            intSpecifiedIDLt.Add(659);
            intSpecifiedIDLt.Add(629);
            intSpecifiedIDLt.Add(648);
            intSpecifiedIDLt.Add(79);
            intSpecifiedIDLt.Add(388);
            intSpecifiedIDLt.Add(387);
            intSpecifiedIDLt.Add(610);
            intSpecifiedIDLt.Add(638);
            intSpecifiedIDLt.Add(48);
            intSpecifiedIDLt.Add(90);
            intSpecifiedIDLt.Add(557);
            intSpecifiedIDLt.Add(655);
            intSpecifiedIDLt.Add(156);
            intSpecifiedIDLt.Add(431);
            intSpecifiedIDLt.Add(728);
            intSpecifiedIDLt.Add(255);
            intSpecifiedIDLt.Add(524);
            intSpecifiedIDLt.Add(39);
            intSpecifiedIDLt.Add(664);
            intSpecifiedIDLt.Add(617);
            intSpecifiedIDLt.Add(581);
            intSpecifiedIDLt.Add(213);
            intSpecifiedIDLt.Add(696);
            intSpecifiedIDLt.Add(309);
            intSpecifiedIDLt.Add(715);
            intSpecifiedIDLt.Add(260);
            intSpecifiedIDLt.Add(615);
            intSpecifiedIDLt.Add(269);
            intSpecifiedIDLt.Add(35);
            intSpecifiedIDLt.Add(313);
            #endregion





            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);
            _pCAreaAgg_ILP = new CAreaAgg_ILP(ParameterInitialize);
            _pCAreaAgg_ILP.AreaAggregation(intSpecifiedIDLt);
            StrObjLtDt.Merge(_pCAreaAgg_ILP.StrObjLtDt);

            //for (int i = 0; i < intSpecifiedIDLt.Count; i++)
            //{
                
            //    _pCAreaAgg_ILP.AreaAggregation();
            //    StrObjLtDt.Merge(_pCAreaAgg_ILP.StrObjLtDt);
                
            //}

            _pCAreaAgg_Base = _pCAreaAgg_ILP as CAreaAgg_Base;
            CAreaAgg_AStar.SaveData(StrObjLtDt, ParameterInitialize, "ILP", Convert.ToInt32(_pCAreaAgg_ILP.dblTimeLimit).ToString());
            MessageBox.Show("Done!");
        }



        private void btnRunILP_Extend_Click(object sender, EventArgs e)
        {
            throw new ArgumentException("need to be implemented!");


            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);

            ////Read Datasets
            //_pCAreaAgg_AStar = new CAreaAgg_AStar(ParameterInitialize);
            //_pCAreaAgg_AStar.AreaAggregation(Convert.ToInt32(txtNodes.Text), "ILP_Extend");

            //this.txtEvaluation.Text = _pCAreaAgg_AStar.dblCost.ToString();
            //StrObjLtDt.Merge(_pCAreaAgg_AStar.StrObjLtDt);
            //CAreaAgg_AStar.SaveData(StrObjLtDt, ParameterInitialize, Convert.ToInt32(txtNodes.Text));





        }

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            int intQuitCount = Convert.ToInt32(txtNodes.Text);
            int intTwiceCount = intQuitCount * 2;
            //int intUnlimited = 2000000000;

            //0: NonShape
            //1: MinimizeInteriorBoundaries
            //2: MaximizeMinComp
            //3: MaximizeMinComp_Combine
            //4: MaximizeAvgComp_EdgeNumber
            //5: MaximizeAvgComp_Combine
            //true: involving smallest
            //false: not necessarily involving smallest
            //RunGreedy(ParameterInitialize, 4, true);
            //RunGreedy(ParameterInitialize, 1, true);

            RunAStar(ParameterInitialize, intQuitCount, 4, true);
            RunAStar(ParameterInitialize, intTwiceCount, 4, true);
            RunAStar(ParameterInitialize, intQuitCount, 1, true);

            //RunILP(ParameterInitialize, 112, true);
            //RunILP(ParameterInitialize, 300, true);

            MessageBox.Show("Done!");
        }

        private void RunGreedy(CParameterInitialize ParameterInitialize,
    int cboShapeConstraintSelectedIndex, bool chkSmallestChecked)
        {
            ParameterInitialize.cboShapeConstraint.SelectedIndex = cboShapeConstraintSelectedIndex;
            ParameterInitialize.chkSmallest.Checked = chkSmallestChecked;

            //var objResultSD = new SortedDictionary<string, List<object>>();
            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);

            _pCAreaAgg_Greedy = new CAreaAgg_Greedy(ParameterInitialize);
            _pCAreaAgg_Greedy.AreaAggregation();
            StrObjLtDt.Merge(_pCAreaAgg_Greedy.StrObjLtDt);
            CAreaAgg_Base.SaveData(StrObjLtDt, ParameterInitialize, "Greedy");
        }

        private void RunAStar(CParameterInitialize ParameterInitialize, 
            int intQuitCount, int cboShapeConstraintSelectedIndex, bool chkSmallestChecked)
        {
            ParameterInitialize.cboShapeConstraint.SelectedIndex = cboShapeConstraintSelectedIndex;
            ParameterInitialize.chkSmallest.Checked = chkSmallestChecked;

            //var objResultSD = new SortedDictionary<string, List<object>>();
            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);

            _pCAreaAgg_AStar = new CAreaAgg_AStar(ParameterInitialize);
            _pCAreaAgg_AStar.AreaAggregation(intQuitCount);
            StrObjLtDt.Merge(_pCAreaAgg_AStar.StrObjLtDt);
            CAreaAgg_Base.SaveData(StrObjLtDt, ParameterInitialize, "AStar", intQuitCount.ToString());
        }

        private void RunILP(CParameterInitialize ParameterInitialize, double dblTimeLimit, bool chkSmallestChecked)
        {
            ParameterInitialize.cboShapeConstraint.SelectedIndex = 1;
            ParameterInitialize.chkSmallest.Checked = chkSmallestChecked;

            //var objResultSD = new SortedDictionary<string, List<object>>();
            var StrObjLtDt = new CStrObjLtDt(CAreaAgg_Base.strKeyLt);

            _pCAreaAgg_ILP = new CAreaAgg_ILP(ParameterInitialize);
            _pCAreaAgg_ILP.dblTimeLimit = dblTimeLimit;
            _pCAreaAgg_ILP.AreaAggregation();
            StrObjLtDt.Merge(_pCAreaAgg_ILP.StrObjLtDt);
            CAreaAgg_Base.SaveData(StrObjLtDt, ParameterInitialize, "ILP", Convert.ToInt32(_pCAreaAgg_ILP.dblTimeLimit).ToString());
        }


        private void btnResultFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@_DataRecords.ParameterInitialize.strSavePath);
        }



        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _pCAreaAgg_Base.Output(_dblProp);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _pCAreaAgg_Base.Output(_dblProp);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _pCAreaAgg_Base.Output(_dblProp);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProp = _dblProp - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _pCAreaAgg_Base.Output(_dblProp);
            }
            catch (System.Exception)
            {
                MessageBox.Show("不能再减小了！");
            }

        }

        IEnumerator<IFeatureLayer> _pFLayerEt;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            //_dblProp = _dblProp + 0.02;
            //pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _pFLayerEt = _pCAreaAgg_Base.AggregateStepByStep().GetEnumerator();
            this.timerAdd.Interval = 500;
           this.timerAdd.Enabled = true;

        }


        private void timerAdd_Tick(object sender, EventArgs e)
        {
            if (_pFLayerEt.MoveNext()==false)
            {
                this.timerAdd.Enabled = false;
            }
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //CParameterResult ParameterResult = _DataRecords.ParameterResult;
            //string strFileName = _dblProp.ToString();
            //CHelpFunc.SaveCPlLt(ParameterResult.DisplayCPlLt, strFileName+"_morphing", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCPlLt(ParameterResult.FadedDisplayCPlLt, strFileName + "_DPFade", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);





            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //List<CPolyline> cpllt = new List<CPolyline>();
            //for (int i = 0; i < _RelativeInterpolationCplLt.Count; i++)
            //{
            //    cpllt.Add(_RelativeInterpolationCplLt[i]);
            //}
            //string strFileName = _dblProp.ToString();
            //CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        private void btnMultiResults_Click(object sender, EventArgs e)
        {
            var dblMultiCount = Convert.ToDouble(this.txtMultiResults.Text);
            for (int i = 0; i < dblMultiCount; i++)
            {
                _dblProp = i / (dblMultiCount - 1);
                //i++;
                _pCAreaAgg_Base.Output(_dblProp);
            }
        }

        private void btnDetailToIpe_Click(object sender, EventArgs e)
        {
            _pCAreaAgg_Base.DetailToIpe();
        }



        //private static void()
        //    {



        //    }
    }
}