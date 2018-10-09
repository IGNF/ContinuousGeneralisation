using System;
using System.Collections.Generic;
using System.Text;

using MorphingClass.CEntity;

namespace MorphingClass.CCorrepondObjects
{
    public class CCorrAtBds
    {
        private CAtBd _pBSAtBd;           //大比例尺表达河流
        private CAtBd _pSSAtBd;             //小比例尺表达河流
        private bool _blnCorr;                //是否有对应河流
       

        public CCorrAtBds()
        {

        }

        public CCorrAtBds(CAtBd fBSAtBd, CAtBd fSSAtBd)
        {
            _pBSAtBd = fBSAtBd;
            _pSSAtBd = fSSAtBd;
        }


        /// <summary>属性：大比例尺表达河流</summary>
        public CAtBd pBSAtBd
        {
            get { return _pBSAtBd; }
            set { _pBSAtBd = value; }
        }

        /// <summary>属性：小比例尺表达河流</summary>
        public CAtBd pSSAtBd
        {
            get { return _pSSAtBd; }
            set { _pSSAtBd = value; }
        }

        /// <summary>属性：是否有对应河流</summary>
        public bool blnCorr
        {
            get { return _blnCorr; }
            set { _blnCorr = value; }
        }


    }
}
