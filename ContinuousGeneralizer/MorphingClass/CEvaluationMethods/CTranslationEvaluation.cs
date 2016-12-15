using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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
using MorphingClass.CEvaluationMethods;

namespace MorphingClass.CEvaluationMethods
{
    public class CTranslationEvaluation : CEvaluation
    {
        public CTranslationEvaluation()
        {

        }

        public CTranslationEvaluation(CDataRecords pDataRecords)
        {
            _pDataRecords = pDataRecords;

            SetdlgEvaluationMethod(enumEvaluationMethods.Translation);
        }

        public double CalTranslationEvaluation()
        {
            return CalEvaluationCorr();
        }


    }
}
