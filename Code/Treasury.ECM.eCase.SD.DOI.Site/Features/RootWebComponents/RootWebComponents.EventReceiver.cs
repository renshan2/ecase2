using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web.UI.WebControls.WebParts;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.Features.RootWebComponents
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("3d9c7728-f286-4094-9463-8d65a26b127f")]
    public class RootWebComponentsEventReceiver : SPFeatureReceiver
    {
        
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite curSite;
            SPWeb curWeb;
            properties.GetSiteAndWeb(out curSite, out curWeb);

            #region Alter Enterprise Keywords' Display Name at Site Column level and push changes down
            SPField taxKeywords = curSite.RootWeb.Fields[eCaseConstants.FieldGuids.OOTB_TAXKEYWORD];
            taxKeywords.Title = "Tags";
            taxKeywords.PushChangesToLists = true;
            taxKeywords.Update();
            #endregion

            // Use Custom Logo for Site
            curWeb.SiteLogoUrl = curWeb.Url + "/Style%20Library/images/ecase-logo.png";
            curWeb.Update();

            #region Configure Managed Metadata Properties

            //Attempt to connect the Managed Metadata Properties to the Correct Term Store

            //Associate UIL Field to Managed Metadata Store
            AssociateTaxonomyFieldToMetadataStore(curSite,
                eCaseConstants.FieldGuids.ECASES_LIST_UIL,
                eCaseConstants.Taxonomy.METADATA_GROUP_NAME,
                eCaseConstants.Taxonomy.METADATA_TERMSET_TAX_LIL);

            //Associate Judge Field to Managed Metadata Store
            //AssociateTaxonomyFieldToMetadataStore(curSite,
            //    eCaseConstants.FieldGuids.ECASES_LIST_JUDGE,
            //    eCaseConstants.Taxonomy.METADATA_GROUP_NAME,
            //    eCaseConstants.Taxonomy.METADATA_TERMSET_TAX_JUDGE);

            //Add Columns to ContentType and List
            AddSiteColumnToListContentType(curSite,
                eCaseConstants.ListInternalNames.ECASES_LIST, 
                eCaseConstants.ContentTypeNames.CASE,
                eCaseConstants.FieldGuids.ECASES_LIST_UIL);
            //AddSiteColumnToListContentType(curSite,
            //    eCaseConstants.ListInternalNames.ECASES_LIST,
            //    eCaseConstants.ContentTypeNames.CASE,
            //    eCaseConstants.FieldGuids.ECASES_LIST_JUDGE);

            #endregion
        }

        private void AssociateTaxonomyFieldToMetadataStore(SPSite curSite, Guid fieldId, string taxGroup, string taxTermSet)
        {
            Logging.Logger.Instance.Info(
                    String.Format("Connecting Site Column to Managed Metadata Service: Field Guid: {0}; Group: {1}; TermSet: {2}", fieldId, taxGroup, taxTermSet),
                   Logging.DiagnosticsCategories.eCaseSite);

            if (curSite.RootWeb.Fields.Contains(fieldId))
            {
                TaxonomySession session = new TaxonomySession(curSite);

                if (session.TermStores.Count != 0)
                {
                    TermStore termStore = session.DefaultKeywordsTermStore;
                    Group group = termStore.Groups[taxGroup];
                    TermSet termSet = group.TermSets[taxTermSet];

                    TaxonomyField field = curSite.RootWeb.Fields[fieldId] as TaxonomyField;

                    // Connect to MMS 
                    field.SspId = termSet.TermStore.Id;
                    field.TermSetId = termSet.Id;
                    field.PushChangesToLists = true;
                    field.Update();
                }
            }
        }

        //TODO: Error Handling, and move these methods to a more central location if appropriate
        private void AddSiteColumnToListContentType(SPSite curSite, string listName, string contentTypeName, Guid fieldId)
        {
            Logging.Logger.Instance.Info(
                String.Format("Adding Site Column to List Content Type if it Already Exists: List Name: {0}; Content Type Name: {1}; Field: {2}", listName, contentTypeName, fieldId),
                Logging.DiagnosticsCategories.eCaseSite);

            //Get Fresh Site and Web each time to avoid exceptions due to multiple concurrent updates
            using (SPSite freshSite = new SPSite(curSite.ID))
            {
                SPList list = freshSite.RootWeb.Lists.TryGetList(listName);
                if (list != null)
                {
                    SPContentType contentType = list.ContentTypes[contentTypeName];
                    if (!contentType.Fields.Contains(fieldId))
                    {
                        //The field doesn't appear in the Fields collection yet, first see if the field link is there.
                        SPFieldLink fl = contentType.FieldLinks[fieldId];
                        SPField siteColumn = freshSite.RootWeb.Fields[fieldId] as SPField;
                        if (fl == null)
                        {
                            contentType.FieldLinks.Add(new SPFieldLink(siteColumn));
                            contentType.Update();
                            list.Update();
                        }

                        //The field is in the FieldLinks collection, now ensure it's in the fields collection.                       
                        if (!list.Fields.Contains(fieldId) && !siteColumn.Hidden)
                        {
                            list.Fields.Add(siteColumn);
                            list.Update();
                        }
                    }
                }
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        //public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        //{
        //}


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
