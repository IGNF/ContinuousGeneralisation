using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
//using ESRI.ArcGIS.ADF.local

namespace ContinuousGeneralizer.CCommon
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("fbcc164d-94f2-496a-b10d-9e2fb3376ff4")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("ContinuousGeneralizer.CCommon.OpenAttributeTable")]

    public sealed class OpenAttributeTable : ESRI.ArcGIS.ADF.BaseClasses.BaseCommand
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            ControlsCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper;
        private ILayer m_pLayer;
        private IMapControl4 m_mapControl;

        //public OpenAttributeTable()
        //{
        //    //
        //    // TODO: Define values for the public properties
        //    //
        //    base.m_category = ""; //localizable text
        //    base.m_caption = "";  //localizable text
        //    base.m_message = "";  //localizable text 
        //    base.m_toolTip = "";  //localizable text 
        //    base.m_name = "";   //unique id, non-localizable (e.g. "MyCategory_MyCommand")

        //    try
        //    {
        //        //
        //        // TODO: change bitmap name if necessary
        //        //
        //        string bitmapResourceName = GetType().Name + ".bmp";
        //        base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Trace.WriteLine(ex.Message, "InvaID Bitmap");
        //    }
        //}

        public OpenAttributeTable(ref IMapControl4 mapControl, ILayer pLayer)
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text
            base.m_caption = "Open Attribute Table"; //localizable text
            base.m_message = "Open Attribute Table"; //localizable text 
            base.m_toolTip = "Open Attribute Table"; //localizable text 
            base.m_name = "Open Attribute Table"; //unique id, non-localizable (e.g. "MyCategory_MyCommand")
            m_pLayer = pLayer;
            m_mapControl = mapControl;
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "InvaID Bitmap");
            }
        }


        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            if (m_hookHelper == null)
                m_hookHelper = new HookHelperClass();

            m_hookHelper.Hook = hook;

            // TODO:  Add other initialization code
        }

        //再在On_Click函数中添加如下代码，以创建并打开属性表窗体。
        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add OpenAttributeTable.OnClick implementation

            ContinuousGeneralizer.FrmGISCommand.FrmAttributeTable attributeTable = new ContinuousGeneralizer.FrmGISCommand.FrmAttributeTable(ref m_mapControl, m_pLayer);
            
            //EasyInterpolator.FrmGISCommand.FrmAttributeTable2 attributeTable = new EasyInterpolator.FrmGISCommand.FrmAttributeTable2(ref m_mapControl, m_pLayer);
            attributeTable.CreateAttributeTable();
            attributeTable.Show();
        }

        #endregion
    }
}
