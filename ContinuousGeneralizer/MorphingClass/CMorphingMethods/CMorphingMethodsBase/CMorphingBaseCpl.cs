using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CMorphingMethods.CMorphingMethodsBase
{
    public class CMorphingBaseCpl:CMorphingBase
    {


        //protected List<List<CPolyline>> _CPlLtLt;
        protected List<CPolyline> _CPlLt;  //this is usually used for normal case (one-scale case)
        protected List<CPolyline> _AllReadCPlLt;
        protected List<CPolyline> _LSCPlLt;
        protected List<CPolyline> _SSCPlLt;
        protected List<CPolyline> _InterLSCPlLt;
        protected List<CPolyline> _InterSSCPlLt;
        protected List<CPolyline> _SgCPlLt;
        protected List<CPolyline> _SpSgCPlLt;   //Sp: simplify




        public List<CPolyline> GetAllReadCPlLt()
        {

            List<CPolyline> pAllReadCPlLt = new List<CPolyline>();
            var pObjCGeoLtLt = this.ObjCGeoLtLt;
            foreach (var CGeoLt in pObjCGeoLtLt)
            {
                pAllReadCPlLt.AddRange(CGeoLt.AsExpectedClassEb<CPolyline, CGeoBase>());
            }
            _AllReadCPlLt = pAllReadCPlLt;
            return pAllReadCPlLt;

            //MessageBox.Show("to be improved!");
            //return null;
        }


        public  List<CPolyline> GenerateInterpolatedCplLt(double dblPropotion)
        {
            return GenerateInterpolatedCplLt(dblPropotion, _CorrCptsLtLt);
        }

        public  List<CPolyline> GenerateInterpolatedCplLt(double dblPropotion, List<List<CCorrCpts>> pCorrCptsLtLt)
        {
            List<CPolyline> pInterpolatedCPlLt = new List<CPolyline>(pCorrCptsLtLt.Count);

            for (int i = 0; i < pCorrCptsLtLt.Count; i++)
            {
                pInterpolatedCPlLt.Add(GenerateInterpolatedCPl(i, pCorrCptsLtLt[i], dblPropotion));
            }
            return pInterpolatedCPlLt;
        }

        public CPolyline GenerateInterpolatedCPl(int intIndex, List<CCorrCpts> pCorrCptsLt, double dblPropotion)
        {
            List<CPoint> pcptlt = new List<CPoint>(pCorrCptsLt.Count);
            foreach (CCorrCpts CorrCpt in pCorrCptsLt)
            {
                pcptlt.Add(CorrCpt.GetInterpolatedCpt(dblPropotion));
            }
            return new CPolyline(intIndex, pcptlt);
        }

        public IEnumerable<IPolyline5> GenerateInterpolatedIPl(double dblPropotion, List<List<CCorrCpts>> pCorrCptsLtLt)
        {
            for (int i = 0; i < pCorrCptsLtLt.Count; i++)
            {
                yield return GenerateInterpolatedIPl(pCorrCptsLtLt[i], dblPropotion);
            }
        }

        public IPolyline5 GenerateInterpolatedIPl(List<CCorrCpts> pCorrCptsLt, double dblPropotion)
        {
            IPointCollection4 pCol = new PolylineClass();
            foreach (CCorrCpts CorrCpt in pCorrCptsLt)
            {
                IPoint ipt = CorrCpt.GetInterpolatedIpt(dblPropotion);
                pCol.AddPoint(ipt);
            }
            return (pCol as IPolyline5) ;
        }


        public IEnumerable<IPolyline5> GenerateCorrIPlEb(IEnumerable<CCorrCpts> pCorrCptsEb)
        {
            foreach (var pCorrCpts in pCorrCptsEb)
            {
                yield return GenerateCorrIPl(pCorrCpts);
            }
        }

        public IPolyline5 GenerateCorrIPl(CCorrCpts pCorrCpts)
        {
            IPointCollection4 pCol = new PolylineClass();
            pCol.AddPoint(pCorrCpts.FrCpt.JudgeAndSetAEGeometry() as IPoint);
            pCol.AddPoint(pCorrCpts.ToCpt.JudgeAndSetAEGeometry() as IPoint);
            
            return (pCol as IPolyline5);
        }

        //public List<List<CPolyline>> CPlLtLt
        //{
        //    get { return _CPlLtLt; }
        //    //set { _InterLSCPlLt = value; }
        //}

        public List<CPolyline> AllReadCPlLt
        {
            get { return _AllReadCPlLt; }
            //set { _InterLSCPlLt = value; }
        }

        public List<CPolyline> LSCPlLt
        {
            get { return _InterLSCPlLt; }
            //set { _InterLSCPlLt = value; }
        }

        public List<CPolyline> SSCPlLt
        {
            get { return _SSCPlLt; }
            //set { _SSCPlLt = value; }
        }

        public List<CPolyline> InterLSCPlLt
        {
            get { return _InterLSCPlLt; }
            //set { _InterLSCPlLt = value; }
        }

        public List<CPolyline> InterSSCPlLt
        {
            get { return _InterSSCPlLt; }
            //set { _InterSSCPlLt = value; }
        }

        public List<CPolyline> SgCPlLt
        {
            get { return _SgCPlLt; }
            //set { _SgCPlLt = value; }
        }

        public List<CPolyline> SpSgCPlLt
        {
            get { return _SpSgCPlLt; }
            //set { _SpSgCPlLt = value; }
        }


    }
}
