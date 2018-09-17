using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CCorrepondObjects
{
    public class CCorrSegment
    {

        private CPolyline _CFrPolyline;
        private CPolyline _CToPolyline;

        //private List<CPoint> _CFromPtLt;
        //private List<CPoint> _CToPtLt;



        private int _intFromStartID;
        private int _intFromEndID;
        private int _intToStartID;
        private int _intToEndID;

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

        //public List<CPoint> CFromPtLt
        //{
        //    get { return _CFromBend; }
        //    set { _CFromBend = value; }
        //}

        //public List<CPoint> CToPtLt
        //{
        //    get { return _CToPtLt; }
        //    set { _CToPtLt = value; }
        //}

        public CPolyline CFrPolyline
        {
            get { return _CFrPolyline; }
            set { _CFrPolyline = value; }
        }

        public CPolyline CToPolyline
        {
            get { return _CToPolyline; }
            set { _CToPolyline = value; }
        }





        public CCorrSegment()
        {

        }

        public CCorrSegment(CBend FrBend, CBend TBend)
        {
            //_CFromPtLt = FrBend.CptLt;
            //_CToPtLt = TBend.CptLt;



            _CFrPolyline = new CPolyline(0, FrBend.CptLt);
            _CToPolyline = new CPolyline(0, TBend.CptLt);


            //_intFromStartID = FrBend.CptLt[0].ID;
            //_intFromEndID = FrBend.CptLt[FrBend.CptLt.Count - 1].ID;
            //_intToStartID = TBend.CptLt[0].ID;
            //_intToEndID = TBend.CptLt[TBend.CptLt.Count - 1].ID;
        }

        public CCorrSegment(List<CPoint> frptlt, List<CPoint> tptlt)
        {
            //_CFromPtLt = frptlt;
            //_CToPtLt = tptlt;


            _CFrPolyline = new CPolyline(0, frptlt);
            _CToPolyline = new CPolyline(0, tptlt);


            //_intFromStartID = frptlt[0].ID;
            //_intFromEndID = frptlt[frptlt.Count - 1].ID;
            //_intToStartID = tptlt[0].ID;
            //_intToEndID = tptlt[tptlt.Count - 1].ID;
        }

        public CCorrSegment(CPolyline frpl, CPolyline topl)
        {
            //_CFromPtLt = frptlt;
            //_CToPtLt = tptlt;


            _CFrPolyline = frpl;
            _CToPolyline = topl;


            //_intFromStartID = frpl.CptLt[0].ID;
            //_intFromEndID = frpl.CptLt[frpl.CptLt.Count - 1].ID;
            //_intToStartID = topl.CptLt[0].ID;
            //_intToEndID = topl.CptLt[topl.CptLt.Count - 1].ID;
        }








    }
}
