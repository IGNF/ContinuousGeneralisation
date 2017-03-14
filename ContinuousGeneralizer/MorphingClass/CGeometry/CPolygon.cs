using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CUtility;
using MorphingClass.CEntity;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CPolygon : CPolyBase<CPolygon>
    {
        //public event PropertyChangedEventHandler PropertyChanged;
         /// <summary>
        /// 
        /// 要使用边（多边形）进行空间分析，需执行函数FormCEdge（FormPolygon），生成边（多边形）之后，方可使用
        /// </summary>
        /// <remarks></remarks>

        private static int _intStaticGID;

        private CEdge _cedgeOuterComponent;    //if a face has cedgeOuterComponent == null, then this face is super face
        private CEdge _cedgeStartAtLeftMost;
        private LinkedList<CEdge> _cedgeLkInnerComponents;     //counter clockwise???
        private SortedDictionary<CPolygon, LinkedList<CEdge>> _AdjacentSD;    //Why SortedDictionary? Because we may need to unite the adjacent elements of two elements, and we also need to consider the shared borders
        //private CPatch _cpatch;

        public double dblAreaSimple { get; set; }

        public int intType { get; set; }
        public int intTypeIndex { get; set; } //the index (0, 1, 2, ...) of a type; used for access type distance directly

        public List<List<CEdge>> CEdgeLtLt { get; set; }
        

        public CPoint CentroidCptSimple { get; set; }

        //public CPolygon AssigningFace { get; set; }
        //public List<CPolygon> AssignedFaceLt { get; set; }

        private IPolygon4 _pPolygon;

        //private CPoint _CentroidCpt;
        private CPoint _LeftMostCpt;
          private bool _IsHole;
        private bool _IsMerged;

        /// <summary>
        /// Initializes a new instance of a triangle
        /// </summary>
        public CPolygon(int intID)
            : this(intID, CHelperFunction.MakeLt<List<CPoint>>(0))
        {
        }

        /// <summary>
        /// Initializes a new instance of a triangle
        /// </summary>
        public CPolygon()
            : this(-2, CHelperFunction.MakeLt<List<CPoint>>(0))
        {
        }



        public CPolygon(int intID, IPolygon4 pPolygon, double dblFactor = 1)
            : this(intID, CHelperFunction.GetCPtLtLtByIPG(pPolygon, dblFactor))
        {
            this.pPolygon = pPolygon;
        }

        public CPolygon(int intID, List<CPoint> cptlt)
            : this(intID, CHelperFunction.MakeLt(1, cptlt))
        {
        }

        /// <summary>
        /// Initializes a new instance of a CPolygon, the first point and the last point in cptlt must have the same coordinates
        /// </summary>
        public CPolygon(int intID, List<List<CPoint>> cptltlt)
        {
            this.GID = _intStaticGID++;
            _intID = intID;
            FormPolyBase(cptltlt);
        }



        public  void FormPolyBase(List<List<CPoint>> fcptltlt)
        {
            if (fcptltlt.Count>0)
            {
                if (fcptltlt[0].Count>0)
                {
                    _FrCpt = fcptltlt[0][0];
                    _ToCpt = fcptltlt[0].GetLast_T();
                }
            }


            this.CptLtLt = fcptltlt;
            //this.SetCptLtLt(fcptltlt);
            //_CEdgeLt = CGeometricMethods.FormCEdgeLt(fcptlt);
        }


        public void FormCEdgeLtLt()
        {
            this.CEdgeLtLt = CGeometricMethods.FormCEdgeLtLt(this.CptLtLt);
            this.CEdgeLt = this.CEdgeLtLt[0];
        }

        public void SetCEdgeLtLtLength()
        {
            this.CEdgeLtLt.ForEach(cedgelt => cedgelt.ForEach(cedge => cedge.SetLength()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>SetPolygon will first set IPoint</remarks>
        public IPolygon4 SetPolygon()
        {
            this.pPolygon = CGeometricMethods.GetPolygonFromCptLt(this.CptLt);           
            return this.pPolygon;
        }

        public IPolygon4 JudgeAndSetPolygon()
        {
            if (_pPolygon == null)
            {
                return SetPolygon();
            }
            else
            {
                return _pPolygon;
            }
        }

        public override IGeometry JudgeAndSetAEGeometry()
        {
            return JudgeAndSetPolygon();
        }

        public override void SetAEGeometryNull()
        {
            _pPolygon = null;
        }

        public override IGeometry GetAEObject()
        {
            return _pPolygon;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <remarks>we suppose that there is no </remarks>
        public List<CPoint> TraverseFaceToGenerateCptLt()
        {
            if (_cedgeOuterComponent != null)  //this is a normal face
            {
                return TraverseToGenerateCptLt(_cedgeOuterComponent);
            }
            else //this is a super face
            {
                return TraverseToGenerateCptLt(_cedgeLkInnerComponents.First.Value);  //we suppose that there is only one InnerComponent
            }
        }


        public List<CPoint> GetOuterCptLt(bool clockwise = true)
        {
            var outercptlt = new List<CPoint>();
            if (_cedgeOuterComponent != null)
            {
               outercptlt= TraverseToGenerateCptLt(_cedgeOuterComponent);

               //
               if (clockwise == true)
               {
                   outercptlt.Reverse();
               }
            }
            return outercptlt;
        }

        public List<List<CPoint>> GetInnerCptLtLt(bool clockwise = true)
        {
            var innercptltlt = new List<List<CPoint>>();
            if (_cedgeLkInnerComponents != null)
            {
                foreach (var cedgeInnerComponent in _cedgeLkInnerComponents)
                {
                    innercptltlt.Add(GetInnerCptLt(cedgeInnerComponent, clockwise));
                }
            }
            return innercptltlt;
        }

        public List<CPoint> GetInnerCptLt(CEdge cedgeInnerComponent, bool clockwise = true)
        {
            var innercptlt = TraverseToGenerateCptLt(cedgeInnerComponent);

            if (clockwise == false)
            {
                innercptlt.Reverse();
            }

            return innercptlt;
        }

        //public List < List<CPoint>> TraverseFaceToGenerateCptLtLt(bool blnOuter=true, bool clockwise=true )
        //{
        //    if (blnOuter==true)
        //    {
        //        cedgeLkInnerComponents
        //    }




        //    if (_cedgeOuterComponent != null)  //this is a normal face
        //    {
        //        return TraverseToGenerateCptLt(_cedgeOuterComponent);
        //    }
        //    else //this is a super face
        //    {
        //        return TraverseToGenerateCptLt(_cedgeLkInnerComponents.First.Value);  //we suppose that there is only one InnerComponent
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedgeComponent"></param>
        /// <returns></returns>
        /// <remarks>we don't just use the FrCpt of cedgeOuterComponent as the start vertex, because a pair of corresponding faces may have different cedgeOuterComponent
        ///          Instead, we use the Cpt having the smallest indexID as the start vertex</remarks>
        private List <CPoint> TraverseToGenerateCptLt(CEdge cedgeComponent)
        {
            var MinFrIndexIDCEdge = cedgeComponent;
            var CurrentCEdge = cedgeComponent.cedgeNext;
            int intCount = 1;
            do
            {
                intCount++;
                if (CurrentCEdge.FrCpt.indexID < MinFrIndexIDCEdge.FrCpt.indexID)
                {
                    MinFrIndexIDCEdge = CurrentCEdge;
                }
                //Console.WriteLine(CurrentCEdge.indexID + "___" + CurrentCEdge.indexID1 + "   " + CurrentCEdge.indexID2);
                CurrentCEdge = CurrentCEdge.cedgeNext;
            } while (CurrentCEdge.indexID != cedgeComponent.indexID);


            var cptlt = new List<CPoint>(intCount + 1);
            cptlt.Add(MinFrIndexIDCEdge.FrCpt);
            cptlt.Add(MinFrIndexIDCEdge.ToCpt);
            CurrentCEdge = MinFrIndexIDCEdge.cedgeNext;
            do
            {
                cptlt.Add(CurrentCEdge.ToCpt);
                CurrentCEdge = CurrentCEdge.cedgeNext;
            } while (CurrentCEdge.indexID != MinFrIndexIDCEdge.indexID);
            this.CptLt = cptlt;
            return cptlt;
        }

        /// <summary>
        /// (counter clockwise???)
        /// </summary>
        public LinkedList<CEdge> cedgeLkInnerComponents
        {
            get { return _cedgeLkInnerComponents; }
            set { _cedgeLkInnerComponents = value; }
        }


        public CEdge cedgeOuterComponent
        {
            get { return _cedgeOuterComponent; }
            set { _cedgeOuterComponent = value; }
        }

        public CEdge cedgeStartAtLeftMost
        {
            get { return _cedgeStartAtLeftMost; }
            set { _cedgeStartAtLeftMost = value; }
        }
        
        public SortedDictionary<CPolygon, LinkedList<CEdge>> AdjacentSD
        {
            get { return _AdjacentSD; }
            set { _AdjacentSD = value; }
        }



        /// <summary>
        /// 中心点
        /// </summary>
        public CPoint SetCentroidCptSimple()
        {
            double dblSumX = 0;
            double dblSumY = 0;
            var cptlt = this.CptLtLt[0];
            for (int i = 0; i < cptlt.Count - 1; i++)
            {
                dblSumX += cptlt[i].X;
                dblSumY += cptlt[i].Y;
            }

            this.CentroidCptSimple = new CPoint(0, dblSumX / (cptlt.Count - 1), dblSumY / (cptlt.Count - 1));
            return this.CentroidCptSimple;
        }

        public void Clear()
        {
            _cedgeLkInnerComponents = null;
            _CEdgeLt = null;
            _cedgeOuterComponent = null;
            _cedgeStartAtLeftMost = null;
            _CorrCGeo = null;
            _CorrCGeoLt = null;
            this.CptLt = null;
            _LeftMostCpt = null;
            //_pGeo = null;
            




        }

        public double SetAreaSimple()
        {
            this.dblAreaSimple = CGeometricMethods.CalAreaForCptEb(this.CptLtLt[0]);
            return this.dblAreaSimple;
        }

        public CPoint LeftMostCpt
        {
            get { return _LeftMostCpt; }
            set { _LeftMostCpt = value; }
        }
        


        //public CPatch cpatch
        //{
        //    get { return _cpatch; }
        //    set { _cpatch = value; }
        //}

        public IPolygon4 pPolygon
        {
            get { return _pPolygon; }
            set 
            { 
                _pPolygon = value;
                _pGeo = value;
            }
        }

        public bool IsHole
        {
            get { return _IsHole; }
            set { _IsHole = value; }
        }

        public bool IsMerged
        {
            get { return _IsMerged; }
            set { _IsMerged = value; }
        }
        

       
        //public void SetPolygonAndEdge()
        //{
        //    if (this .pPolygon ==null)
        //    {
        //        IPointCollection4 pPntCtl = CHelperFunction.GetPointCollectionFromCptLt(cptlt);
        //        this.pPolygon = pPntCtl as IPolygon4;
        //        this.pPolygon.Close();
        //    }

        //    if (true)
        //    {
        //        ISegmentCollection pSegmentCollection = this.pPolygon as ISegmentCollection;
        //        List<CEdge> pedgelt = _CEdgeLt;
        //        for (int i = 0; i < pedgelt.Count ; i++)
        //        {
        //            pedgelt[i].pLine = pSegmentCollection.get_Segment(i) as ILine;
        //        }
        //    }

        //}

        //public void SetEdgeIncidentFace()
        //{
        //    foreach (CEdge  cedge in _CEdgeLt)
        //    {
        //        cedge.cpgIncidentFace = this;
        //    }
        //}



    }
}
