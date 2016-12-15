using System;
using System.Collections.Generic;
using System.Text;

using MorphingClass.CEntity;

namespace MorphingClass.CCorrepondObjects
{
    public class CCorrespondRiver
    {
        private CRiver _CFromRiver;           //大比例尺表达河流
        private CRiver _CToRiver;             //小比例尺表达河流
        private bool _blnCorr;                //是否有对应河流
       

        public CCorrespondRiver()
        {

        }

        public CCorrespondRiver(CRiver pCFromRiver, CRiver pCToRiver)
        {
            _CFromRiver = pCFromRiver;
            _CToRiver = pCToRiver;
        }


        /// <summary>属性：大比例尺表达河流</summary>
        public CRiver CFromRiver
        {
            get { return _CFromRiver; }
            set { _CFromRiver = value; }
        }

        /// <summary>属性：小比例尺表达河流</summary>
        public CRiver CToRiver
        {
            get { return _CToRiver; }
            set { _CToRiver = value; }
        }

        /// <summary>属性：是否有对应河流</summary>
        public bool blnCorr
        {
            get { return _blnCorr; }
            set { _blnCorr = value; }
        }


    }
}
