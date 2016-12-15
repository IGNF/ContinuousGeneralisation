using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CCorrepondObjects
{
    

    public class CCorrCpts
    {
        public int intID;
        public CPoint FrCpt { get; set; }   //大比例尺线状要素上的点
        public CPoint ToCpt { get; set; }      //小比例尺线状要素上的点
        public CMoveVector pMoveVector { get; set; } 


        public CCorrCpts(CPoint frcpt, CPoint tocpt)
            : this(frcpt.ID, frcpt, tocpt)
        {
        }

        public CCorrCpts(int intID, CPoint frcpt, CPoint tocpt)
        {
            frcpt.PairCorrCpt = this;
            tocpt.PairCorrCpt = this;

            this.intID = intID;
            this.FrCpt = frcpt;
            this.ToCpt = tocpt;
            //this.pMoveVector = CGeometricMethods.CalMoveVector(frcpt, tocpt);
        }

        public CCorrCpts(CPoint frcpt, double dblMoveX, double dblMoveY)
        {
            CPoint tocpt = new CPoint(frcpt.ID, frcpt.X + dblMoveX, frcpt.Y + dblMoveY);
            frcpt.PairCorrCpt = this;
            tocpt.PairCorrCpt = this;

            this.intID = frcpt.ID;
            this.FrCpt = frcpt;
            this.ToCpt = tocpt;
            this.pMoveVector = new CMoveVector(frcpt.ID, dblMoveX, dblMoveY);
        }

        public CMoveVector SetMoveVector()
        {
            this.pMoveVector = CGeometricMethods.CalMoveVector(this.FrCpt, this.ToCpt);
            return this.pMoveVector;
        }

        public CPoint GetInterpolatedCpt(double dblProportion)
        {
            return CGeometricMethods.GetInterpolatedCpt(this.FrCpt, this.pMoveVector, dblProportion);
        }

        public IPoint GetInterpolatedIpt(double dblProportion)
        {
            return CGeometricMethods.GetInterpolatedIpt(this.FrCpt, this.pMoveVector, dblProportion);
        }

    }
}
