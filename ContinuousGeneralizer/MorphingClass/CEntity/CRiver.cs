using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CEntity
{
    public class CRiver :CLinearEntity 
    {

        private double _dblLengthSum;         //总河流长度(该河流与其各支流长度的总和)
        private double _dblLevel;            //河流的层次(主干流为1，支流则为干流的层次+1)，本应为整型，为参与计算定义为双精度
        private double _dblOrder;            //以指定方式计算的河流等级，本应为整型，为参与计算定义为双精度
        private double _dblWeightiness;      //河流的重要性
        private double _dblWeightinessUnitary;  //归一化后的河流重要性
               
        private CRiver _CMainStream;                         //干流
        private CRiver _CCorrRiver;                          //对应河流
        
        private List<CPoint> _CCorrTriJunctionPtLt;    //对应支流对应汇合点(CorrepondenceTributaryJunctionPtLt)
        private List<CRiver> _CTributaryLt;            //支流
        
        

        


         public CRiver()
        {
             
        }



        //public CRiver(int intID, IPolyline5 pPolyline)
        //{
        //    _intID = intID;

        //    IPointCollection4 pCol = new PolylineClass();
        //    pCol = (IPointCollection4)pPolyline;
        //    this.SetPointCollection(pCol);
        //    this._pPolyline = pPolyline;

        //    _CptLt = pHelperFunction.GetCPtLtByIPl(pPolyline);
        //}

        public CRiver(int intID, CPolyline cpl,double dblBuffer, double dblVerySmall)
        {
            _intID = intID;

            this .pPolyline =cpl.pPolyline;
            this.CptLt = cpl.CptLt;
            _CTributaryLt = new List<CRiver>();
            CreateBufferAndSmallBuffer( dblBuffer, dblVerySmall);
        }

        public CRiver(int intID, CPolyline cpl, double dblVerySmall)
        {
            _intID = intID;
            this.pPolyline = cpl.pPolyline;
            this.CptLt = cpl.CptLt;
            _CTributaryLt = new List<CRiver>();
            CreateSmallBufferForEnds(dblVerySmall);
            CreateSmallBufferForPolyline(dblVerySmall);
        }

        public CRiver(int intID, List<CPoint> cptlt0)
        {
            _intID = intID;
            this.CptLt = cptlt0;

            IPointCollection4 pCol = new PolylineClass();
            for (int i = 0; i < cptlt0.Count; i++)
            {
                pCol.AddPoint((IPoint)cptlt0[i]);
            }

            IPolyline5 pPolyline = pCol as IPolyline5;
            this.pPolyline = pPolyline;
            //FormPolyline(cptlt0);
            _CTributaryLt = new List<CRiver>();
        }

        /// <summary>属性：总河流长度(该河流与其各支流长度的总和)</summary>
        public double LengthSum
        {
            get { return _dblLengthSum; }
            set { _dblLengthSum = value; }
        }

       

        /// <summary>属性：对应支流对应汇合点(CorrepondenceTributaryJunctionPtLt)</summary>
        public List<CPoint> CCorrTriJunctionPtLt
        {
            get { return _CCorrTriJunctionPtLt; }
            set { _CCorrTriJunctionPtLt = value; }
        }

        /// <summary>属性：干流</summary>
        public CRiver CMainStream
        {
            get { return _CMainStream; }
            set { _CMainStream = value; }
        }

        /// <summary>属性：对应河流</summary>
        public CRiver CCorrRiver
        {
            get { return _CCorrRiver; }
            set { _CCorrRiver = value; }
        }

      


        /// <summary>属性：支流</summary>
        public List<CRiver> CTributaryLt
        {
            get { return _CTributaryLt; }
            set { _CTributaryLt = value; }
        }

         /// <summary>属性：河流的层次(主干流为1，支流则为干流的层次+1)，本应为整型，为参与计算定义为双精度</summary>
        public double dblLevel
        {
            get { return _dblLevel; }
            set { _dblLevel = value; }
        }

        /// <summary>属性：以指定方式计算的河流等级，本应为整型，为参与计算定义为双精度</summary>
        public double dblOrder
        {
            get { return _dblOrder; }
            set { _dblOrder = value; }
        }

        /// <summary>属性：河流的重要性</summary>
        public double dblWeightiness
        {
            get { return _dblWeightiness; }
            set { _dblWeightiness = value; }
        }

        /// <summary>属性：归一化后的河流重要性</summary>
        public double dblWeightinessUnitary
        {
            get { return _dblWeightinessUnitary; }
            set { _dblWeightinessUnitary = value; }
        }



        #region IEquatable<dEdge> Members

        /// <summary>
        /// Checks whether two cedges are equal disregarding the direction of the cedges
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CRiver other)
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


   
}
