using System;
using System.Collections.Generic;
using System.Text;

using MorphingClass.CEntity;
using MorphingClass.CGeometry ;

namespace MorphingClass.CCorrepondObjects
{
    public class CCorrespondRiverNet
    {


        private CRiverNet _CBSRiverNet;
        private CRiverNet _CSSRiverNet;
        private bool _blnCorr;
        private List<CCorrespondRiver> _CCorrespondRiverLt = new List<CCorrespondRiver>();
        private List<List<CPoint>> _CResultPtLtLt;
        private List<CRiver> _CResultRiverLt;

        public CCorrespondRiverNet()
        {

        }

        public CCorrespondRiverNet(CRiverNet pCBSRiverNet, CRiverNet pCToRiverNet)
        {
            _CBSRiverNet = pCBSRiverNet;
            _CSSRiverNet = pCToRiverNet;
        }



        public CRiverNet CBSRiverNet
        {
            get { return _CBSRiverNet; }
            set { _CBSRiverNet = value; }
        }

        public CRiverNet CSSRiverNet
        {
            get { return _CSSRiverNet; }
            set { _CSSRiverNet = value; }
        }

        public List<List<CPoint>> CResultPtLtLt
        {
            get { return _CResultPtLtLt; }
            set { _CResultPtLtLt = value; }
        }


        public List<CRiver> CResultRiverLt
        {
            get { return _CResultRiverLt; }
            set { _CResultRiverLt = value; }
        }




        public List<CCorrespondRiver> CCorrespondRiverLt
        {
            get { return _CCorrespondRiverLt; }
            set { _CCorrespondRiverLt = value; }
        }

        public bool blnCorr
        {
            get { return _blnCorr; }
            set { _blnCorr = value; }
        }

    }
}
