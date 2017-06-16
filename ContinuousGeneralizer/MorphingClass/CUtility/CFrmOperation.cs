using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;

using ESRI.ArcGIS.Display;

namespace MorphingClass.CUtility
{
    public class CFrmOperation
    {
        /// <summary>Constructor</summary>
        public CFrmOperation(ref CParameterInitialize pParameterInitialize)
        {
            pParameterInitialize.m_mapFeature = CHelpFunc.GetAllLayers(pParameterInitialize.m_mapControl);
            LoadTocbo(pParameterInitialize);            
        }




        private void LoadTocbo(CParameterInitialize pParameterInitialize)
        {
            if (pParameterInitialize.cboLayerLt == null)
            {
                return;
            }

            IMap pm_mapFeature = pParameterInitialize.m_mapFeature;
            IEnumLayer pEnumlayer = pm_mapFeature.Layers;
            string[] astrLayerName = new string[pm_mapFeature.LayerCount];
            int intIndex = 0;
            while (intIndex < pm_mapFeature.LayerCount)
            {
                astrLayerName[intIndex] = pEnumlayer.Next().Name;
                intIndex++;
            }

            int intSelect = 0;
            foreach (ComboBox cboLayer in pParameterInitialize.cboLayerLt)
            {
                cboLayer.Items.AddRange(astrLayerName);
                cboLayer.SelectedIndex = intSelect;
                intSelect++;
            }
        }

        #region Obsolete: loading cbolayers
        ///// <summary>实现Frm_Load的方法</summary>
        ///// <remarks>最一般的Frm_Load的方法</remarks>
        //public void FrmLoad()
        //{
        //    _ParameterInitialize.cboLayer.Items.Clear();//图层列表框

        //    for (int i = _ParameterInitialize.pMap.LayerCount - 1; i >= 0; i--) //获取所有的要素图层（由于之后的“AddLayer”方法总是将新的图层放在第一个位置，所以这里从后面开始遍历）
        //    {
        //        ILayer pLayer = _ParameterInitialize.pMap.get_Layer(i);
        //        if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
        //        {
        //            bool isVisible = pLayer.Visible;
        //            ICompositeLayer pComLayer = pLayer as ICompositeLayer;
        //            for (int j = pComLayer.Count - 1; j >= 0; j--)
        //            {
        //                if (isVisible==false)
        //                {
        //                    pComLayer.get_Layer(j).Visible = false;
        //                }

        //                if (pComLayer.get_Layer(j) is IFeatureLayer)  //是否为要素图层
        //                {
        //                    _ParameterInitialize.m_mapFeature.AddLayer(pComLayer.get_Layer(j));
        //                    _ParameterInitialize.cboLayer.Items.Insert(0, pComLayer.get_Layer(j).Name);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (pLayer is IFeatureLayer)  //是否为要素图层
        //            {
        //                _ParameterInitialize.m_mapFeature.AddLayer(pLayer);
        //                _ParameterInitialize.cboLayer.Items.Insert(0, pLayer.Name);
        //            }
        //        }
        //    }
        //    if (_ParameterInitialize.cboLayer.Items.Count > 0)
        //        _ParameterInitialize.cboLayer.SelectedIndex = 0;//默认选取第一个要素图层
        //}

        ///// <summary>实现Frm_Load的方法</summary>
        ///// <remarks>两个图层选择框</remarks>
        //public void FrmLoadMulticbo()
        //{
        //    _ParameterInitialize.cboLargerScaleLayer.Items.Clear();//图层列表框
        //    _ParameterInitialize.cboSmallerScaleLayer.Items.Clear();//图层列表框

        //    for (int i = _ParameterInitialize.pMap.LayerCount - 1; i >= 0; i--) //获取所有的要素图层（由于之后的“AddLayer”方法总是将新的图层放在第一个位置，所以这里从后面开始遍历）
        //    {
        //        ILayer pLayer = _ParameterInitialize.pMap.get_Layer(i);
        //        if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
        //        {
        //            bool isVisible = pLayer.Visible;
        //            ICompositeLayer pComLayer = pLayer as ICompositeLayer;
        //            for (int j = pComLayer.Count - 1; j >= 0; j--)
        //            {
        //                if (isVisible == false)
        //                {
        //                    pComLayer.get_Layer(j).Visible = false;
        //                }
        //                //_ParameterInitialize.m_mapAll.AddLayer(pComLayer.get_Layer(j));
        //                if (pComLayer.get_Layer(j) is IFeatureLayer)  //是否为要素图层
        //                {
        //                    IFeatureLayer pFeaturelayer = (IFeatureLayer)pComLayer.get_Layer(j);
        //                    _ParameterInitialize.cboLargerScaleLayer.Items.Insert(0, pFeaturelayer.Name);
        //                    _ParameterInitialize.cboSmallerScaleLayer.Items.Insert(0, pFeaturelayer.Name);
        //                    _ParameterInitialize.m_mapFeature.AddLayer(pFeaturelayer);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (pLayer is IFeatureLayer)
        //            {
        //                _ParameterInitialize.m_mapFeature.AddLayer(pLayer);
        //                _ParameterInitialize.cboLargerScaleLayer.Items.Insert(0, pLayer.Name);
        //                _ParameterInitialize.cboSmallerScaleLayer.Items.Insert(0, pLayer.Name);

        //                //IFeatureLayer pFeaturelayer = (IFeatureLayer)pLayer;
        //                //if (pFeaturelayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline)
        //                //{
        //                //    _ParameterInitialize.cboLargerScaleLayer.Items.Add(pFeaturelayer.Name);
        //                //    _ParameterInitialize.cboSmallerScaleLayer.Items.Add(pFeaturelayer.Name);
        //                //    _ParameterInitialize.m_mapPolyline.AddLayer(pFeaturelayer);
        //                //}
        //            }
        //        }
        //    }

        //    if (_ParameterInitialize.cboLargerScaleLayer.Items.Count > 1 && _ParameterInitialize.cboSmallerScaleLayer.Items.Count > 1)
        //    {
        //        _ParameterInitialize.cboLargerScaleLayer.SelectedIndex = 0;//默认选取第一个点要素图层
        //        _ParameterInitialize.cboSmallerScaleLayer.SelectedIndex = 1;//默认选取第一个点要素图层
        //    }


        //}

        ///// <summary>实现Frm_Load的方法</summary>
        ///// <remarks>两个图层选择框</remarks>
        //public void FrmLoadThreecbo()
        //{
        //    _ParameterInitialize.cboLargerScaleLayer.Items.Clear();//图层列表框
        //    _ParameterInitialize.cboSmallerScaleLayer.Items.Clear();//图层列表框
        //    _ParameterInitialize.cboLayer.Items.Clear();

        //    for (int i = _ParameterInitialize.pMap.LayerCount - 1; i >= 0; i--) //获取所有的要素图层
        //    {
        //        ILayer pLayer = _ParameterInitialize.pMap.get_Layer(i);
        //        if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
        //        {
        //            bool isVisible = pLayer.Visible;
        //            ICompositeLayer pComLayer = pLayer as ICompositeLayer;
        //            for (int j = pComLayer.Count - 1; j >= 0; j--)
        //            {
        //                if (isVisible == false)
        //                {
        //                    pComLayer.get_Layer(j).Visible = false;
        //                }
        //                if (pComLayer.get_Layer(j) is IFeatureLayer)  //是否为要素图层
        //                {
        //                    IFeatureLayer pFeaturelayer = (IFeatureLayer)pComLayer.get_Layer(j);
        //                    _ParameterInitialize.m_mapFeature.AddLayer(pFeaturelayer);

        //                    _ParameterInitialize.cboLargerScaleLayer.Items.Insert(0, pFeaturelayer.Name);
        //                    _ParameterInitialize.cboSmallerScaleLayer.Items.Insert(0, pFeaturelayer.Name);
        //                    _ParameterInitialize.cboLayer.Items.Insert(0, pFeaturelayer.Name);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (pLayer is IFeatureLayer)
        //            {
        //                IFeatureLayer pFeaturelayer = (IFeatureLayer)pLayer;
        //                _ParameterInitialize.m_mapFeature.AddLayer(pFeaturelayer);

        //                _ParameterInitialize.cboLargerScaleLayer.Items.Insert(0, pFeaturelayer.Name);
        //                _ParameterInitialize.cboSmallerScaleLayer.Items.Insert(0, pFeaturelayer.Name);
        //                _ParameterInitialize.cboLayer.Items.Insert(0, pFeaturelayer.Name);
        //            }
        //        }
        //    }

        //    if (_ParameterInitialize.cboLargerScaleLayer.Items.Count > 1 && _ParameterInitialize.cboSmallerScaleLayer.Items.Count > 1)
        //    {
        //        _ParameterInitialize.cboLargerScaleLayer.SelectedIndex = 0;//默认选取第一个点要素图层
        //        _ParameterInitialize.cboSmallerScaleLayer.SelectedIndex = 1;//默认选取第一个点要素图层
        //        _ParameterInitialize.cboLayer.SelectedIndex = 2;
        //    }

        //}
        #endregion

    }
}
