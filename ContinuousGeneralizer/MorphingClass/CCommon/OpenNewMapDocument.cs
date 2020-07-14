using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;

namespace MorphingClass.CCommon
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// Summary description for OpenNewMapDocument.
    /// </summary>
    [Guid("580e5a00-e685-4489-9bc2-63e33f6dc728")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("MorphingClass.CCommon.OpenNewMapDocument")]
    public sealed class OpenNewMapDocument : BaseCommand
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
        private ControlsSynchronizer m_controlsSynchronizer = null;
        private IHookHelper m_hookHelper;

        //添加参数
        public OpenNewMapDocument(ControlsSynchronizer controlsSynchronizer)
        {
            //
            // TODO: Define values for the public properties
            //
            //设定相关属性值
            base.m_category = "Generic"; //localizable text
            base.m_caption = "Open"; //localizable text
            base.m_message = "This should work in ArcMap/MapControl/PageLayoutControl"; //localizable text
            base.m_toolTip = "Open"; //localizable text
            base.m_name = "Generic_Open"; //unique id, non-localizable (e.g. "MyCategory_MyCommand")
            //初始化m_controlsSynchronizer
            m_controlsSynchronizer = controlsSynchronizer;
            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
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


        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public void OnClickAutomatic(string docName)
        {

            
            IMapDocument mapDoc = new MapDocumentClass();
            if (mapDoc.get_IsPresent(docName) && !mapDoc.get_IsPasswordProtected(docName))
            {
                mapDoc.Open(docName, string.Empty);
                IMap map = mapDoc.get_Map(0);
                m_controlsSynchronizer.ReplaceMap(map);
                mapDoc.Close();
            }
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add OpenNewMapDocument.OnClick implementation
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Map Documents (*.mxd)|*.mxd";
            dlg.Multiselect = false;
            dlg.Title = "Open Map Document";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string docName = dlg.FileName;
                IMapDocument mapDoc = new MapDocumentClass();
                if (mapDoc.get_IsPresent(docName) && !mapDoc.get_IsPasswordProtected(docName))
                {
                    mapDoc.Open(docName, string.Empty);
                    IMap map = mapDoc.get_Map(0);
                    m_controlsSynchronizer.ReplaceMap(map);
                    mapDoc.Close();
                }
            }
        }
        #endregion
    }
}
