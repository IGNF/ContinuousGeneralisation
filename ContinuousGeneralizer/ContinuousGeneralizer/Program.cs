using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESRI.ArcGIS;

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

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmContinuousGeneralizer());
        }
    }
}