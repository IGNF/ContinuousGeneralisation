using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;


namespace MorphingClass.CEntity
{
    public class CRiverNet
    {
        private int _intRNID = -1;                        //线段号
       
        private CRiver _CMasterStream;                         //主干流
        private List<CRiver> _CRiverLt;            //河流
        private CRiverNet _CCorrRiverNet;              //对应河网

        public CRiverNet()
        {

        }

        public CRiverNet(int pintRNID, CRiver pCMasterStream)
        {
            _intRNID = pintRNID;
            _CMasterStream = pCMasterStream;
            _CRiverLt = new List<CRiver>();
            _CRiverLt.Add(pCMasterStream);
        }


        /// <summary>属性：河网号</summary>
        public int RNID
        {
            get { return _intRNID; }
            set { _intRNID = value; }
        }

        /// <summary>属性：主干流</summary>
        public CRiver CMasterStream
        {
            get { return _CMasterStream; }
            set { _CMasterStream = value; }
        }

        /// <summary>属性：河流</summary>
        public List<CRiver> CRiverLt
        {
            get { return _CRiverLt; }
            set { _CRiverLt = value; }
        }

        /// <summary>属性：河流</summary>
        public CRiverNet CCorrRiverNet
        {
            get { return _CCorrRiverNet; }
            set { _CCorrRiverNet = value; }
        }


    }
}
