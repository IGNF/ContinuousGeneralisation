using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

using ESRI.ArcGIS.Geometry;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CGeometricBase<CGeo> : CBasicBase 
        where CGeo:class
    //public class CGeometricBase<TGeo> :CBasicBase where TGeo : class    //it would be better if we could do it like this, but our current version doesn't support default generic type, which we need for one of the classes inheriting from this class
    {
        
        protected int _tempID = -1;
        
        public int indexID1 { get; set; }
        public int indexID2 { get; set; }

        public bool isMatched { get; set; }
        public bool isTraversed { get; set; }
        public int indexID { get; set; }
        
        protected CEnumScale _enumScale;
        //protected int _intRed =0;
        //protected int _intGreen = 0;
        //protected int _intBlue = 0;
        //protected string _strLabel;
        protected IGeometry _pGeo;

        protected CGeo _CorrCGeo;
        protected List<CGeo> _CorrCGeoLt;

        

        //public virtual CGeometricBase()
        //{

        //}


        public virtual IGeometry JudgeAndSetAEGeometry()
        {

            return null;
        }

        public virtual void SetAEGeometryNull()
        {
        }

        //public virtual void SetAEGeometry()
        //{
        //    //return 
        //}

        public virtual IGeometry GetAEObject()  //this method would be better aInterLSo abstract, but i want to return something
        {
            return null;
        }


        public virtual CGeo Copy()
        {
            return null;

        }

        /// <summary>
        /// this funcation can only be used for a shape that implements IPointCollection4
        /// </summary>
        public virtual void JudgeAndSetZToZero()
        {
            IPointCollection4 pCol = _pGeo as IPointCollection4;
            if (pCol.PointCount > 0)
            {
                IPoint ipt0 = pCol.get_Point(0);
                if (double.IsNaN(ipt0.Z))
                {
                    SetZToZero(pCol);
                }
            }
        }

        private void SetZToZero(IPointCollection4 pCol)
        {
            for (int i = 0; i < pCol.PointCount; i++)
            {
                IPoint copyipt = pCol.get_Point(i);
                copyipt.Z = 0;
                pCol.UpdatePoint(i, copyipt);
            }

        }

        



        

        public CEnumScale enumScale
        {
            get { return _enumScale; }
            set { _enumScale = value; }
        }

        public IGeometry pGeo
        {
            get { return _pGeo; }
            set { _pGeo = value; }
        }

        public virtual CGeo CorrCGeo
        {
            get { return _CorrCGeo; }
            set { _CorrCGeo = value; }
        }

        public List<CGeo> CorrCGeoLt
        {
            get { return _CorrCGeoLt; }
            set { _CorrCGeoLt = value; }
        }
    }
}
