using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using Microsoft.SharePoint;
using Microsoft.Office.DocumentManagement;
using Treasury.ECM.eCase.SusDeb.DOI.Common.DocIdProvider;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.Features.SiteComponents
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("597b9a33-877d-428f-9700-75cfd981945b")]
    public class SiteComponentsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite curSite;
            SPWeb curWeb;
            properties.GetSiteAndWeb(out curSite, out curWeb);
            
            #region Turn on auditing flags.
            curSite.Audit.AuditFlags = SPAuditMaskType.All;
            curSite.Audit.Update();
            #endregion

            #region Configure eCase DB Connection String
            string connectionString = "Initial Catalog=WSS_Content_Apps_eCase_DOI;Data Source=ECMINTSPSQL1.INTECM.GOV.TESTECM.GOV,1433;User ID=eCaseUser;Password=Devise!!!"; 
            if (!curSite.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING))
            {
                curSite.RootWeb.Properties.Add(eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING, connectionString);
                curSite.RootWeb.Properties.Update();
            }
            #endregion

            #region Configure Doc Id Prefix
            string prefix = "CCCM";
            if (!curSite.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX))
            {
                curSite.RootWeb.Properties.Add(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX, prefix);
                curSite.RootWeb.Properties.Update();
            }
            #endregion

              // Configure Doc ID Service
            DocumentId.SetProvider(curSite, new eCaseDocIdProvider()); 

        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite curSite;
            SPWeb curWeb;
            properties.GetSiteAndWeb(out curSite, out curWeb);
            DocumentId.SetDefaultProvider(curSite);
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
