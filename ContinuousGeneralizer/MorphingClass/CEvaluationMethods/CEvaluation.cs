using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Linq;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

using MorphingClass.CGeometry;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CMorphingAlgorithms;

namespace MorphingClass.CEvaluationMethods
{

    //public delegate double DlgEvaluationMethodCorrCptsEb(CCorrCpts LastCorrCpts, CCorrCpts CurrentCorrCpts);
    public delegate double DlgEvaluationMethod(CCorrCpts LastCorrCpts, CCorrCpts CurrentCorrCpts, double dblFrTotalLength = 0, double dblToTotalLength = 0);

    public class CEvaluation
    {
        protected CDataRecords _pDataRecords;
        //protected CPoint _StandardVectorCpt;
        protected List<List<double>> _dblEvaluationLtLt = new List<List<double>>();
        protected List<List<double>> _dblSumEvaluationLtLt = new List<List<double>>();
        public double dblCorrection { set; get; }  //using optcor, we don't want to divide the evaluation by 2 each time we compare a evaluation. Instead, we do it for the whole evaluation, thus we set this value to 2

        public DlgEvaluationMethod dlgEvaluationMethod { set; get; }
        //public DlgEvaluationMethodCorrCptsEb dlgEvaluationMethodCorrCptsEb { set; get; }

        public CEvaluation()
        {
            dblCorrection = 0;
        }


        public CEvaluation(int intSelectedIndex)
        {
            switch (intSelectedIndex)
            {
                case 0:
                    this.dlgEvaluationMethod = CEvaluation.CalDeflectionCLIA;
                    dblCorrection = 2;
                    break;
                case 1:
                    this.dlgEvaluationMethod = CEvaluation.CalTranslation;
                    dblCorrection=1;
                    break;
                case 2:
                    this.dlgEvaluationMethod = CEvaluation.CalDeflection;   //this is about the weight of the cost between two line segments; sometimes the weight = (frlength + tolength)/2, we can actually divide the sum by 2 at last only once
                    dblCorrection = 2;
                    break;
                default: throw new ArgumentException("An undefined method! ");
            }
        }

        public CEvaluation(CDataRecords pDataRecords, enumEvaluationMethods enumEvaluationMethod = enumEvaluationMethods.Translation)
        {
            _pDataRecords = pDataRecords;

            SetdlgEvaluationMethod(enumEvaluationMethod);
        }

        protected void SetdlgEvaluationMethod(enumEvaluationMethods enumEvaluationMethod)
        {
            switch (enumEvaluationMethod)
            {
                case enumEvaluationMethods.DeflectionCLIA:
                    this.dlgEvaluationMethod = CalDeflectionCLIA;
                    dblCorrection = 2;
                    break;
                case enumEvaluationMethods.Translation:
                    this.dlgEvaluationMethod = CalTranslation;
                    dblCorrection=1;
                    break;
                case enumEvaluationMethods.Deflection:
                    this.dlgEvaluationMethod = CalDeflection;
                    dblCorrection = 2;
                    break;
                default:
                    MessageBox.Show("Undefined morphing method!  In:" + this.ToString() + ".cs   ");
                    break;
            }
        }


        #region CalEvaluation


        /// <summary>Calculate Evaluation values for a List<List<CCorrCpts>> which has been recorded in ParameterResult</summary>
        /// <remarks></remarks>
        protected double CalEvaluationCorr()
        {
            CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            double dblSUM = CalEvaluationCorr(ParameterResult.pMorphingBase, _dblEvaluationLtLt, _dblSumEvaluationLtLt);
            ParameterResult.pEvaluation = this;

            return dblSUM;
        }

        private double CalEvaluationCorr(CMorphingBase pMorphingBase, List<List<double>> dblEvaluationLtLt, List<List<double>> dblSumEvaluationLtLt)
        {
            List<List<CCorrCpts>> CorrCptsLtLt = pMorphingBase.CorrCptsLtLt;

            if (dblEvaluationLtLt == null)
            {
                dblEvaluationLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            }
            if (dblSumEvaluationLtLt == null)
            {
                dblSumEvaluationLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            }

            double dblSUM = 0;
            foreach (List<CCorrCpts> CorrCptsLt in CorrCptsLtLt)
            {
                List<double> dblEvaluationLt = new List<double>(CorrCptsLt.Count);
                List<double> dblSumEvaluationLt = new List<double>(CorrCptsLt.Count);

                CalEvaluationCorr(CorrCptsLt, this.dlgEvaluationMethod, dblEvaluationLt, dblSumEvaluationLt);
                dblEvaluationLtLt.Add(dblEvaluationLt);
                dblSumEvaluationLtLt.Add(dblSumEvaluationLt);
                dblSUM += dblSumEvaluationLt[dblSumEvaluationLt.Count - 1];
            }

            return dblSUM;
        }



        public double CalEvaluationCorr(IEnumerable<CCorrCpts> CorrCptsEb, DlgEvaluationMethod pdlgEvaluationMethod, List<double> dblEvaluationLt, List<double> dblSumEvaluationLt)
        {
            double dblSumLength = 0;

            dblEvaluationLt.Add(0);
            dblSumEvaluationLt.Add(0);

            var CorrCptsEt = CorrCptsEb.GetEnumerator();
            CorrCptsEt.MoveNext();   //we may need to test whether this is successful

            var preCorrCpts = CorrCptsEt.Current;
            while (CorrCptsEt.MoveNext())
            {
                var currentCorrCpts = CorrCptsEt.Current;
                double dblLength = pdlgEvaluationMethod(preCorrCpts, currentCorrCpts) / dblCorrection;
                dblSumLength += dblLength;

                dblEvaluationLt.Add(dblLength);
                dblSumEvaluationLt.Add(dblSumLength);

                preCorrCpts = currentCorrCpts;
            }

            return dblSumLength;
        }

        /// <summary>
        /// compute the cost between correspondences
        /// </summary>
        /// <param name="CorrCptsEb"></param>
        /// <param name="pdlgEvaluationMethod"></param>
        /// <param name="pStandardVectorCpt"></param>
        /// <returns></returns>
        public double CalEvaluationCorr(IEnumerable<CCorrCpts> CorrCptsEb, DlgEvaluationMethod pdlgEvaluationMethod, double dblFrTotalLength=0, double dblToTotalLength=0)
        {
            double dblSumLength = 0;
            var CorrCptsEt = CorrCptsEb.GetEnumerator();
            CorrCptsEt.MoveNext();   //we may need to test whether this is successful

            var preCorrCpts = CorrCptsEt.Current;
            while (CorrCptsEt.MoveNext())
            {
                var currentCorrCpts = CorrCptsEt.Current;
                double dblLength = pdlgEvaluationMethod(preCorrCpts, currentCorrCpts, dblFrTotalLength, dblToTotalLength);
                dblSumLength += dblLength;

                preCorrCpts = currentCorrCpts;
            }

            return dblSumLength;
        }

        private static double CalTranslation(CCorrCpts LastCorrCpts, CCorrCpts CurrentCorrCpts, double dblFrTotalLength = 0, double dblToTotalLength = 0)
        {
            LastCorrCpts.SetMoveVector();
            CurrentCorrCpts.SetMoveVector();
            return LastCorrCpts.pMoveVector.DistanceTo(CurrentCorrCpts.pMoveVector); ;
        }

        /// <summary>
        /// Deflection (i.e. the integral distance between two line segments)
        /// </summary>
        /// <param name="LastCorrCpts">the last pair of corresponding points</param>
        /// <param name="CurrentCorrCpts">the current pair of corresponding points</param>
        /// <param name="pStandardVectorCpt"></param>
        /// <returns>the integral distance between the two line segments</returns>
        private static double CalDeflection(CCorrCpts LastCorrCpts, CCorrCpts CurrentCorrCpts, double dblFrTotalLength, double dblToTotalLength)
        {
            double dblfrlength = CGeoFunc.CalDis(LastCorrCpts.FrCpt, CurrentCorrCpts.FrCpt);
            double dbltolength = CGeoFunc.CalDis(LastCorrCpts.ToCpt, CurrentCorrCpts.ToCpt);

            return CalDeflectionSub(LastCorrCpts, CurrentCorrCpts, dblfrlength, dbltolength);
        }


        /// <summary>
        /// Deflection (i.e. the integral distance between two line segments)
        /// </summary>
        /// <param name="LastCorrCpts">the last pair of corresponding points</param>
        /// <param name="CurrentCorrCpts">the current pair of corresponding points</param>
        /// <param name="pStandardVectorCpt"></param>
        /// <returns>the integral distance between the two line segments</returns>
        private static double CalDeflectionCLIA(CCorrCpts LastCorrCpts, CCorrCpts CurrentCorrCpts, double dblFrTotalLength, double dblToTotalLength)
        {
            double dblfrlength = dblFrTotalLength * (CurrentCorrCpts.FrCpt.dblRatioLengthFromStart - LastCorrCpts.FrCpt.dblRatioLengthFromStart);
            double dbltolength = dblToTotalLength * (CurrentCorrCpts.ToCpt.dblRatioLengthFromStart - LastCorrCpts.ToCpt.dblRatioLengthFromStart);

            return CalDeflectionSub(LastCorrCpts, CurrentCorrCpts, dblfrlength, dbltolength);
        }

        private static double CalDeflectionSub(CCorrCpts LastCorrCpts, CCorrCpts CurrentCorrCpts, double dblfrlength, double dbltolength)
        {
            CPoint frfrcpt = LastCorrCpts.FrCpt;         //vertex1
            CPoint frtocpt = CurrentCorrCpts.FrCpt;      //vertex2
            CPoint tofrcpt = LastCorrCpts.ToCpt;         //vertex3
            CPoint totocpt = CurrentCorrCpts.ToCpt;      //vertex4

            //double dblfrfrX = LastCorrCpts.FrCpt.X + pStandardVectorCpt.X;        //x1
            //double dblfrfrY = LastCorrCpts.FrCpt.Y + pStandardVectorCpt.Y;        //y1
            //double dblfrtoX = CurrentCorrCpts.FrCpt.X + pStandardVectorCpt.X;     //x2
            //double dblfrtoY = CurrentCorrCpts.FrCpt.Y + pStandardVectorCpt.Y;     //y2

            double dblfftfx = frfrcpt.X - tofrcpt.X;    //x1-x3
            double dblfftfy = frfrcpt.Y - tofrcpt.Y;    //y1-y3

            double dblXPart = dblfftfx - frtocpt.X + totocpt.X;    //x1-x2-x3+x4
            double dblYPart = dblfftfy - frtocpt.Y + totocpt.Y;    //y1-y2-y3+y4


            //Integral of sqrt(ax^2+bx+c); X=ax^2+bx+c, delta=4ac-b^2; k=4a/delta
            double a = dblXPart * dblXPart + dblYPart * dblYPart;
            double b = -2 * (dblfftfx * dblXPart + dblfftfy * dblYPart);
            double c = dblfftfx * dblfftfx + dblfftfy * dblfftfy;

            double dblTwoA = a + a;
            double dblFourA = dblTwoA + dblTwoA;

            CQuadraticEquation pQuadraticEquation = new CQuadraticEquation(a, b, c);    //since this is a function of distance, dblDelta <= 0
            double dblIntegral = 0;

            //we have a>=0
            if (CCmpMethods.CmpSquare(a, 0) == 0) //then b==0
            {
                dblIntegral = Math.Sqrt(c);
            }
            else
            {
                //f(x)=ax^2+bx+c, f(0)=c, f(1)=a+b+c
                double X_0 = c;
                double X_1 = CMath.JudgeNegtiveReturn0(a + b + c, "X1");

                double SqrtX_0 = Math.Sqrt(X_0);
                double SqrtX_1 = Math.Sqrt(X_1);


                double dblTwoAUplusB_0 = b;               //U = 0
                double dblTwoAUplusB_1 = dblTwoA + b;       //U = 1
                //double dblDelta = CMath 

                double dblFirstPart = (dblTwoAUplusB_1 * SqrtX_1 - dblTwoAUplusB_0 * SqrtX_0) / dblFourA;   //(2 * a * 1 + b) * SqrtX_1 / (4 * a) - (2 * a * 0 + b) * SqrtX_0 / (4 * a)
                double dblSecondPart = 0;

                //two line segments are parallel to each other is a special case of Delta=0, notice that the distance between the two parallel segments is changing
                if (CCmpMethods.CmpPower4(pQuadraticEquation.dblDelta, 0) != 0)
                {
                    double dblSqrtA = Math.Sqrt(a);
                    double dblTwoSqrtA = 2 * dblSqrtA;
                    //double dblk = dblFourA / (-pQuadraticEquation.dblDelta);
                    double dblfisrtpart = dblTwoSqrtA * SqrtX_1 + dblTwoAUplusB_1;    //the part in ln when u=1
                    double dblsecondpart = dblTwoSqrtA * SqrtX_0 + dblTwoAUplusB_0;    //the part in ln when u=0
                    double dblAbsPart = dblfisrtpart / dblsecondpart;    //the ln part
                    double dblLnPart = Math.Log(dblAbsPart);  //Math.Log(2 * dblSqrtA * SqrtX_1 + 2 * a * 1 + b) - * Math.Log(2 * dblSqrtA * SqrtX_0 + 2 * a * 0 + b);
                    dblSecondPart = -pQuadraticEquation.dblDelta * dblLnPart / dblFourA / dblTwoSqrtA;

                    //dblSecondPart = dblSubSecondPart / (2 * dblk);
                    if (double.IsNaN(dblSecondPart))  //sometimes, the ComparePower4 doesn't work well
                    {
                        dblSecondPart = 0;
                    }
                }
                dblIntegral = dblFirstPart + dblSecondPart;
            }

            double dblResult = dblIntegral * (dblfrlength + dbltolength);

            return dblResult;
        }


        #endregion

        public CDataRecords pDataRecords
        {
            get { return _pDataRecords; }
            set { _pDataRecords = value; }
        }

        public List<List<double>> dblEvaluationLtLt
        {
            get { return _dblEvaluationLtLt; }
            set { _dblEvaluationLtLt = value; }
        }

        public List<List<double>> dblSumEvaluationLtLt
        {
            get { return _dblSumEvaluationLtLt; }
            set { _dblSumEvaluationLtLt = value; }
        }


        //public CPoint StandardVectorCpt
        //{
        //    get { return _StandardVectorCpt; }
        //    set { _StandardVectorCpt = value; }
        //}
    }

    public enum enumEvaluationMethods
    {
        Translation,
        Deflection,
        DeflectionCLIA


    }
}
