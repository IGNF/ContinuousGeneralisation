using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;

namespace ContinuousGeneralizer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!RuntimeManager.Bind(ProductCode.Desktop))
            {
                if (!RuntimeManager.Bind(ProductCode.Engine))
                {
                    MessageBox.Show("Unable to bind to ArcGIS runtime. Application will be shut down.");
                    return;
                }
            }

            ESRI.ArcGIS.esriSystem.IAoInitialize ao = new ESRI.ArcGIS.esriSystem.AoInitialize();
            ao.Initialize(ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeAdvanced);

            Enable3DAnalysis(); //avoid "The 3D Analyst extension has not been enabled"

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmContinuousGeneralizer());
        }

        public static void Enable3DAnalysis()
        {
            ESRI.ArcGIS.esriSystem.IExtensionManagerAdmin iExtensionManagerAdmin = new ESRI.ArcGIS.esriSystem.ExtensionManagerClass();
            UID iUid3D = new UIDClass();
            iUid3D.Value = "{94305472-592E-11D4-80EE-00C04FA0ADF8}";
            object o = new object();
            iExtensionManagerAdmin.AddExtension(iUid3D, ref o);
            IExtensionConfig iExtensionConfig = (IExtensionConfig)(iExtensionManagerAdmin as IExtensionManager).FindExtension(iUid3D);
            iExtensionConfig.State = esriExtensionState.esriESEnabled;
        }
    }
}