using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CEntity
{
    public class CAtBd : CLinearEntity
    {
        //
        private CAtBd _pCorrAtBd;
        private CAtBd _pFrAtBd;
        private CAtBd _pToAtBd;
        private List<CAtBd> _pCorrAtBdLt;
        private SortedList<double, CCorrAtBdInfo> _pCorrAtBdInfoSLt;

        public CAtBd()
        {
            
        }

        public CAtBd(int intID, CPolyline cpl)
        {
            _intID = intID;
            this.pPolyline = cpl.pPolyline;
            FormPolyBase(cpl.CptLt);
        }

        public CAtBd(int intID, CPolyline cpl, double dblVerySmall, enumBuffer penmuBuffer)
        {
            _intID = intID;
            this.pPolyline = cpl.pPolyline;
            FormPolyBase(cpl.CptLt);
            switch (penmuBuffer)
            {
                case enumBuffer.Pls: CreateSmallBufferForPolyline(dblVerySmall);
                    break;
                case enumBuffer.Ends: CreateSmallBufferForEnds(dblVerySmall);
                    break;
                case enumBuffer.PlsEnds:
                    CreateSmallBufferForPolyline(dblVerySmall);
                    CreateSmallBufferForEnds(dblVerySmall);
                    break;
                default:
                    break;
            }
        }

        public void SetCptBelongedPolyline()
        {
            foreach (CPoint cpt in this.CptLt)
            {
                cpt.BelongedCPolyline = this;
            }
        }

        //public CAtBd(int intID, CPolyline cpl,double dblBuffer, double dblVerySmall)
        //{
        //    _intID = intID;
        //    FormPolyline(cpl.CptLt);
        //    CreateBufferAndSmallBuffer(dblBuffer, dblVerySmall);
        //}

        //public CAtBd(int intID, List <CPoint > cptlt0, double dblBuffer, double dblVerySmall)
        //{
        //    _intID = intID;
        //    FormPolyline(cptlt0);
        //    CreateBufferAndSmallBuffer(dblBuffer, dblVerySmall);
        //}

        //public void UnionAtBd(CAtBd other, double dblBuffer, double dblVerySmall, ref bool isUnioned)
        //{
        //    this.UnionCpl(other, dblVerySmall, ref isUnioned);
        //    if (isUnioned == true)
        //    {
        //        CreateBufferAndSmallBuffer(dblBuffer, dblVerySmall);
        //    }
        //}


        //public CAtBd(int intID, List<CPoint> cptlt0)
        //{

        //    _intID = intID;
        //    this.CptLt = cptlt0;
        //    FormPolyline(cptlt0);
        //}

        public CAtBd pFrAtBd
        {
            get { return _pFrAtBd; }
            set { _pFrAtBd = value; }
        }

        public CAtBd pToAtBd
        {
            get { return _pToAtBd; }
            set { _pToAtBd = value; }
        }

        /// <summary>属性：对应边界</summary>
        public CAtBd pCorrAtBd
        {
            get { return _pCorrAtBd; }
            set { _pCorrAtBd = value; }
        }

        /// <summary>属性：对应边界列</summary>
        public List<CAtBd> pCorrAtBdLt
        {
            get { return _pCorrAtBdLt; }
            set { _pCorrAtBdLt = value; }
        }

        /// <summary>属性：对应边界列</summary>
        public SortedList<double, CCorrAtBdInfo> pCorrAtBdInfoSLt
        {
            get { return _pCorrAtBdInfoSLt; }
            set { _pCorrAtBdInfoSLt = value; }
        }

        public void SetCptBelongedObject()
        {
            List<CPoint> fcptlt = this.CptLt;
            foreach (CPoint cpt in fcptlt)
            {
                cpt.BelongedObject  = this;
            }
        }

        #region IEquatable<dEdge> Members

        /// <summary>
        /// Checks whether two cedges are equal disregarding the direction of the cedges
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CAtBd other)
        {
            IPointCollection4 pThisCol = new PolylineClass();
            pThisCol = (IPointCollection4)this.pPolyline;

            IPointCollection4 potherCol = new PolylineClass();
            potherCol = (IPointCollection4)other.pPolyline;

            if (pThisCol.PointCount != potherCol.PointCount)
                return false;


            bool blnEqual = true;
            for (int i = 0; i < pThisCol.PointCount; i++)
            {
                if ((pThisCol.get_Point(i).X != potherCol.get_Point(i).X) || (pThisCol.get_Point(i).Y != potherCol.get_Point(i).Y))
                {
                    blnEqual = false;
                    break;
                }
            }
            return blnEqual;


        }

        #endregion


    }

    public class CCorrAtBdInfo
    {
        private CAtBd _pCorrAtBd;
        private double _dblOverlapArea;
        private double _dblOverlapRatio;

        public CCorrAtBdInfo(CAtBd CorrAtBd, double pdblOverlapRatio, double pdblOverlapArea)
        {
            _pCorrAtBd = CorrAtBd;
            _dblOverlapRatio = pdblOverlapRatio;
            _dblOverlapArea = pdblOverlapArea;
        }


        /// <summary>属性：对应边界</summary>
        public CAtBd pCorrAtBd
        {
            get { return _pCorrAtBd; }
            set { _pCorrAtBd = value; }
        }

        /// <summary>属性：</summary>
        public double dblOverlapArea
        {
            get { return _dblOverlapArea; }
            set { _dblOverlapArea = value; }
        }

        /// <summary>属性：</summary>
        public double dblOverlapRatio
        {
            get { return _dblOverlapRatio; }
            set { _dblOverlapRatio = value; }
        }
    }

    public enum enumBuffer
    {
        Pls,
        Ends,
        PlsEnds

    }

}
