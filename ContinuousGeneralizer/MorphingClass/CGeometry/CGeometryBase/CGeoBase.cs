using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

using ESRI.ArcGIS.Geometry;

namespace MorphingClass.CGeometry.CGeometryBase
{
    public class CGeoBase : CBasicBase
    {
        public int indexID1 { get; set; }
        public int indexID2 { get; set; }

        public bool isMatched { get; set; }

        

        protected CEnumScale _enumScale;
        //protected int _intRed =0;
        //protected int _intGreen = 0;
        //protected int _intBlue = 0;
        //protected string _strLabel;
        protected IGeometry _pGeo;

        public CGeoBase CorrCGeo { get; set; }
        public List<CGeoBase> CorrCGeoLt { get; set; }
        


        public CGeoBase()
        {

        }

        public CGeoBase(int intID, IGeometry pGeo)           
        {

        }

        //public CGeoBase<CGeo> ()
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


        //public virtual CGeoBase Copy()
        //{
        //    return null;

        //}

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




    }
}
