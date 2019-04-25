using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.ServiceLocation;
using Microsoft.Practices.SharePoint.Common.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Logging.Features.LoggingConfigurator
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("8951417b-d6e5-4485-84c0-898522d29dbf")]
    public class LoggingConfiguratorEventReceiver : SPFeatureReceiver
    {
        private List<DiagnosticsArea> BuildDiagnosticAreas()
        {
            List<DiagnosticsArea> retVal = new List<DiagnosticsArea>();

            DiagnosticsArea treasuryEcmArea = new DiagnosticsArea("Treasury.ECM.DOI");
            foreach (DiagnosticsCategories category in Enum.GetValues(typeof(DiagnosticsCategories)))
                treasuryEcmArea.DiagnosticsCategories.Add(new DiagnosticsCategory(Enum.GetName(typeof(DiagnosticsCategories), category), EventSeverity.Error, TraceSeverity.Unexpected));

            retVal.Add(treasuryEcmArea);

            return retVal;
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            LoggingManager lMgr = new LoggingManager();
            foreach (DiagnosticsArea area in BuildDiagnosticAreas())
                lMgr.AddDiagnosticArea(area);
            lMgr.Save();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            LoggingManager lMgr = new LoggingManager();
            foreach (DiagnosticsArea area in BuildDiagnosticAreas())
                lMgr.RemoveDiagnosticArea(area);
            lMgr.Save();

        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
