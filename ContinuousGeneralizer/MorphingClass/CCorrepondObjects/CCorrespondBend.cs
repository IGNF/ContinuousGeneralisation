using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CCorrepondObjects
{
    public class CCorrespondBend
    {
        private CBend _CFromBend;
        private CBend _CToBend;

        private int _intFromStartID;
        private int _intFromEndID;
        private int _intToStartID;
        private int _intToEndID;

        private string _strSide = "Undefined";     //该对应弯曲在折线的左边"Left"或是右边"Right"，初始值为"Undefined"

        public int intFromStartID
        {
            get { return _intFromStartID; }
            set { _intFromStartID = value; }
        }

        public int intFromEndID
        {
            get { return _intFromEndID; }
            set { _intFromEndID = value; }
        }
        public int intToStartID
        {
            get { return _intToStartID; }
            set { _intToStartID = value; }
        }
        public int intToEndID
        {
            get { return _intToEndID; }
            set { _intToEndID = value; }
        }

        public CBend CFromBend
        {
            get { return _CFromBend; }
            set { _CFromBend = value; }
        }

        public CBend CToBend
        {
            get { return _CToBend; }
            set { _CToBend = value; }
        }

        /// <summary>该对应弯曲位于线状要素的哪一边</summary>
        public string strSide
        {
            get { return _strSide; }
            set { _strSide = value; }
        }

        public CCorrespondBend()
        {

        }

        public CCorrespondBend(CBend FromBend, CBend ToBend)
        {
            _CFromBend = FromBend;
            _CToBend = ToBend;

            _intFromStartID = FromBend.CptLt[0].ID;
            _intFromEndID = FromBend.CptLt[FromBend.CptLt.Count - 1].ID;
            _intToStartID = ToBend.CptLt[0].ID;
            _intToEndID = ToBend.CptLt[ToBend.CptLt.Count - 1].ID;
        }

        public CCorrespondBend(CBend FromBend, CBend ToBend, string strSide)
        {
            _CFromBend = FromBend;
            _CToBend = ToBend;
            _strSide = strSide;

            _intFromStartID = FromBend.CptLt[0].ID;
            _intFromEndID = FromBend.CptLt[FromBend.CptLt.Count - 1].ID;
            _intToStartID = ToBend.CptLt[0].ID;
            _intToEndID = ToBend.CptLt[ToBend.CptLt.Count - 1].ID;
        }

    }


}
