//ÒÆ³ýÍ¼²ãÀàRemoveLayer´úÂë
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

namespace MorphingClass.CCommon
{
    /// <summary>
    /// É¾³ýÍ¼²ã
    /// </summary>
    public sealed class RemoveLayer : BaseCommand
    {
        private IMapControl4 m_mapControl;
        public RemoveLayer()
        {
            base.m_caption = "Remove Layer";
        }
        public override void OnClick()
        {
            ILayer layer = (ILayer)m_mapControl.CustomProperty;
            m_mapControl.Map.DeleteLayer(layer);
        }
        public override void OnCreate(object hook)
        {
            m_mapControl = (IMapControl4)hook;
        }
    }
}
