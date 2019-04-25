using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Web.UI.WebControls.WebParts;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebPartPages;

using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using System.Collections.Generic;
using Microsoft.SharePoint.Workflow;
using System.Globalization;
using Microsoft.SharePoint.Administration;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Features.CaseWebComponents
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("dfdd482f-7717-4776-b9f4-4c0ccfdabe8b")]
    public class CaseWebComponentsEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {

            SPSite curSite;
            SPWeb curWeb;
            properties.GetSiteAndWeb(out curSite, out curWeb);
            if (curSite != null && curWeb != null)
            {
                try
                {

                    #region Configure Doc Id Prefix
                    string prefix = "CCCM";
                    if (curSite.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX))
                        prefix = curSite.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX];

                    if (!curWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX))
                    {
                        curWeb.Properties.Add(eCaseConstants.PropertyBagKeys.ECASE_DOC_ID_PREFIX, prefix);
                        curWeb.Properties.Update();
                    }
                    #endregion

                    #region Configure Workflows to Associate
                    string workflowNames = eCaseConstants.PropertyBagDefaultValues.DEFAULT_WORKFLOW_NAMES;
                    if (!curSite.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_WORKFLOWS_TO_ASSOCIATE))
                    {
                        curSite.RootWeb.Properties.Add(eCaseConstants.PropertyBagKeys.ECASE_WORKFLOWS_TO_ASSOCIATE, workflowNames);
                        curSite.RootWeb.Properties.Update();
                    }
                    #endregion

                    // Activate Case Site Components feature at RootWeb if not already activated
                    try { curSite.Features.Add(eCaseConstants.FeatureIds.CASE_SITE_COMPONENTS); }
                    catch (System.Data.DuplicateNameException x)
                    {
                        Logger.Instance.Info(string.Format("CaseWebComponentsEventReceiver.FeatureActivated: {0} already activated at {1}",
                            eCaseConstants.FeatureIds.CASE_SITE_COMPONENTS.ToString(), curSite.Url), x, DiagnosticsCategories.eCaseWeb);
                    }

                    // Activate Standard Site Collection Features if not already activated
                    try { curSite.Features.Add(eCaseConstants.FeatureIds.LEGACY_WORKFLOWS); }
                    catch (System.Data.DuplicateNameException x)
                    {
                        Logger.Instance.Info(string.Format("CaseWebComponentsEventReceiver.FeatureActivated: {0} already activated at {1}",
                            eCaseConstants.FeatureIds.WORKFLOWS.ToString(), curSite.Url), x, DiagnosticsCategories.eCaseWeb);
                    }

                    // Activate 2010 Workflows feature if not already activated
                    try { curSite.Features.Add(eCaseConstants.FeatureIds.WORKFLOWS); }
                    catch (System.Data.DuplicateNameException x)
                    {
                        Logger.Instance.Info(string.Format("CaseWebComponentsEventReceiver.FeatureActivated: {0} already activated at {1}",
                            eCaseConstants.FeatureIds.WORKFLOWS.ToString(), curSite.Url), x, DiagnosticsCategories.eCaseWeb);
                    }

                    // Associate workflows from our property bag
                    if (curSite.RootWeb.Properties.ContainsKey(eCaseConstants.PropertyBagKeys.ECASE_WORKFLOWS_TO_ASSOCIATE))
                    {
                        var workFlows = curSite.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_WORKFLOWS_TO_ASSOCIATE].Split('|');
                        //ActivateWorkflowFeatures(workFlows, curSite);
                        AssociateWithWorkFlows(workFlows, curSite, curWeb);
                    }

                    #region Create RelatedLegalIssues Lookup Site Column
                    SPFieldLookup relatedLegalIssuesLookup = null;
                    if (curWeb.Fields.ContainsFieldWithStaticName("RelLglIssues"))
                    {
                        relatedLegalIssuesLookup = curWeb.Fields.GetFieldByInternalName("RelLglIssues") as SPFieldLookup;
                    }
                    else
                    {
                        SPList legalIssuesList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.LEGAL_ISSUES);
                        curWeb.Fields.AddLookup("RelLglIssues", legalIssuesList.ID, curWeb.ID, false);
                        relatedLegalIssuesLookup = curWeb.Fields["RelLglIssues"] as SPFieldLookup;
                        relatedLegalIssuesLookup.LookupField = legalIssuesList.Fields[eCaseConstants.FieldGuids.OOTB_TITLE].InternalName;
                        relatedLegalIssuesLookup.AllowMultipleValues = true;
                        relatedLegalIssuesLookup.Group = "eCases Site Columns";
                        relatedLegalIssuesLookup.Title = "Legal Issues";
                        relatedLegalIssuesLookup.Update();
                    }
                    #endregion

                    int autoRefreshInterval = 60;
                    #region Referral Documents
                    SPList caseDocsList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.REFERRAL_DOCUMENTS);
                    SPContentType caseDocCt = curSite.RootWeb.ContentTypes[eCaseConstants.ContentTypeIds.CASE_DOCUMENT];
                    SPContentType caseDocCtChild = caseDocsList.ContentTypes["Case Document"];
                    bool caseDocsCtIsNew = (caseDocCtChild == null);
                    if (caseDocsCtIsNew)
                    {
                        caseDocCtChild = new SPContentType(caseDocCt, curWeb.ContentTypes, "Case Document");
                    }
                    SPFieldLink relatedLegalIssuesFLink = new SPFieldLink(relatedLegalIssuesLookup);
                    if (caseDocCtChild.FieldLinks[relatedLegalIssuesFLink.Name] == null)
                    {
                        caseDocCtChild.FieldLinks.Add(relatedLegalIssuesFLink);
                    }
                    if (caseDocsCtIsNew)
                    {
                        caseDocsList.ContentTypes.Add(caseDocCtChild);
                        caseDocsList.Update();
                    }

                    // Enable AJAX
                    using (SPLimitedWebPartManager mgr = curWeb.GetLimitedWebPartManager(caseDocsList.DefaultViewUrl, PersonalizationScope.Shared))
                    {
                        XsltListViewWebPart xsltListViewWp = mgr.WebParts[0] as XsltListViewWebPart;
                        xsltListViewWp.AutoRefresh = true;
                        xsltListViewWp.AutoRefreshInterval = autoRefreshInterval;
                        mgr.SaveChanges(xsltListViewWp);
                    }
                    #endregion

                    #region Investigation Documents
                    SPList relatedDocsList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.INVESTIGATION_DOCUMENTS);
                    SPContentType relDocCt = curSite.RootWeb.ContentTypes[eCaseConstants.ContentTypeIds.INVESTIGATION_DOCUMENT];
                    SPContentType relDocCtChild = relatedDocsList.ContentTypes["Related Document"];
                    bool relDocCtIsNew = (relDocCtChild == null);
                    if (relDocCtIsNew)
                    {
                        relDocCtChild = new SPContentType(relDocCt, curWeb.ContentTypes, "Related Document");
                    }
                    if (relDocCtChild.FieldLinks[relatedLegalIssuesFLink.Name] == null)
                    {
                        relDocCtChild.FieldLinks.Add(relatedLegalIssuesFLink);
                    }
                    if (relDocCtIsNew)
                    {
                        relatedDocsList.ContentTypes.Add(relDocCtChild);
                        relatedDocsList.Update();
                    }

                    // Enable AJAX
                    using (SPLimitedWebPartManager mgr = curWeb.GetLimitedWebPartManager(relatedDocsList.DefaultViewUrl, PersonalizationScope.Shared))
                    {
                        XsltListViewWebPart xsltListViewWp = mgr.WebParts[0] as XsltListViewWebPart;
                        xsltListViewWp.AutoRefresh = true;
                        xsltListViewWp.AutoRefreshInterval = autoRefreshInterval;
                        mgr.SaveChanges(xsltListViewWp);
                    }
                    #endregion

                    #region SDO Documents
                    SPList finWorkProdDocsList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.SDO_DOCUMENTS);
                    SPContentType finWorkProdCt = curSite.RootWeb.ContentTypes[eCaseConstants.ContentTypeIds.SDO_DOCUMENT];
                    SPContentType finWorkProdCtChild = finWorkProdDocsList.ContentTypes["Finished Work Product"];
                    bool finWorkProdCtIsNew = (finWorkProdCtChild == null);
                    if (finWorkProdCtIsNew)
                    {
                        finWorkProdCtChild = new SPContentType(finWorkProdCt, curWeb.ContentTypes, "Finished Work Product");
                    }
                    if (finWorkProdCtChild.FieldLinks[relatedLegalIssuesFLink.Name] == null)
                    {
                        finWorkProdCtChild.FieldLinks.Add(relatedLegalIssuesFLink);
                    }
                    if (finWorkProdCtIsNew)
                    {
                        finWorkProdDocsList.ContentTypes.Add(finWorkProdCtChild);
                        finWorkProdDocsList.Update();
                    }

                    // Enable AJAX
                    using (SPLimitedWebPartManager mgr = curWeb.GetLimitedWebPartManager(finWorkProdDocsList.DefaultViewUrl, PersonalizationScope.Shared))
                    {
                        XsltListViewWebPart xsltListViewWp = mgr.WebParts[0] as XsltListViewWebPart;
                        xsltListViewWp.AutoRefresh = true;
                        xsltListViewWp.AutoRefreshInterval = autoRefreshInterval;
                        mgr.SaveChanges(xsltListViewWp);
                    }
                    #endregion

                    #region Share With External User
                    SPList shareExternalUserDocsList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.SHARE_WITH_EXTERNAL_USERS);
                    SPContentType shareExternalUserCt = curSite.RootWeb.ContentTypes[eCaseConstants.ContentTypeIds.SHARE_WITH_EXTERNAL_USER];
                    SPContentType shareExternalUserCtChild = shareExternalUserDocsList.ContentTypes["ShareWithExternalUser"];
                    bool shareExternalUserCtIsNew = (shareExternalUserCtChild == null);
                    if (shareExternalUserCtIsNew)
                    {
                        shareExternalUserCtChild = new SPContentType(shareExternalUserCt, curWeb.ContentTypes, "ShareWithExternalUser");
                    }
                    if (shareExternalUserCtChild.FieldLinks[relatedLegalIssuesFLink.Name] == null)
                    {
                        shareExternalUserCtChild.FieldLinks.Add(relatedLegalIssuesFLink);
                    }
                    if (shareExternalUserCtIsNew)
                    {
                        shareExternalUserDocsList.ContentTypes.Add(shareExternalUserCtChild);
                        shareExternalUserDocsList.Update();
                    }

                    // Enable AJAX
                    using (SPLimitedWebPartManager mgr = curWeb.GetLimitedWebPartManager(shareExternalUserDocsList.DefaultViewUrl, PersonalizationScope.Shared))
                    {
                        XsltListViewWebPart xsltListViewWp = mgr.WebParts[0] as XsltListViewWebPart;
                        xsltListViewWp.AutoRefresh = true;
                        xsltListViewWp.AutoRefreshInterval = autoRefreshInterval;
                        mgr.SaveChanges(xsltListViewWp);
                    }
                    #endregion

                    #region Related Dates -- NEVER GOT DONE PROPERLY, BUT DOES FUNCTION FOR THESE LISTS
                    //SPContentType relDatesCt = curSite.RootWeb.ContentTypes[eCaseConstants.ContentTypeIds.RELATED_DATES];
                    //SPContentType relDatesCtChild = new SPContentType(relDatesCt, curWeb.ContentTypes, "Related Date");
                    //relDatesCtChild.FieldLinks.Add(relatedLegalIssuesFLink);

                    //SPList caseRelatedDatesList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.CASE_RELATED_DATES);
                    //SPContentTypeId ootbEvent = new SPContentTypeId("0x0102");
                    //caseRelatedDatesList.ContentTypes.Add(relDatesCtChild);
                    ////caseRelatedDatesList.ContentTypes.Delete(ootbEvent);
                    //caseRelatedDatesList.Update();

                    //SPList matterRelatedDatesList = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.MATTER_RELATED_DATES);
                    //matterRelatedDatesList.ContentTypes.Add(relDatesCtChild);
                    ////matterRelatedDatesList.ContentTypes.Delete(ootbEvent);
                    //matterRelatedDatesList.Update();
                    #endregion

                    // Use Custom Logo for Site
                    curWeb.SiteLogoUrl = curSite.RootWeb.Url + "/Style%20Library/images/ecase-logo.png";

                    // Push all new content types to the SPWeb collection
                    curWeb.Update();
                }
                catch (Exception x)
                {
                    Logger.Instance.Error("CaseWebComponents FeatureActivation Failure", x, DiagnosticsCategories.eCaseWeb);
                    throw x;
                }

            }
            else // Something is very wrong -- lets throw
            {
                string obj;
                if (curSite == null)
                    obj = "SPSite";
                else
                    obj = "SPWeb";

                string exceptionMsg = string.Format("Feature activation did not complete successfully.  Unable to find {0} object", obj);
                throw new Exception(exceptionMsg);
            }

        }

        private void AssociateWithWorkFlows(string[] workFlows, SPSite curSite, SPWeb curWeb)
        {

            List<SPList> docLibs = new List<SPList>();
            SPWorkflowTemplate baseTemplate = null;
            SPWorkflowTemplate wfTemplate = null; // our template we store in PropertyBag
            SPList caseDocs = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.REFERRAL_DOCUMENTS);
            SPList relatedDocs = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.INVESTIGATION_DOCUMENTS);
            SPList finishedWorkProducts = curWeb.GetListByInternalName(eCaseConstants.ListInternalNames.SDO_DOCUMENTS);
            docLibs.Add(caseDocs);
            docLibs.Add(relatedDocs);
            docLibs.Add(finishedWorkProducts);

            var taskList = curWeb.GetListByInternalName("Tasks List");
            var historyList = curWeb.GetListByInternalName("Workflow History List");


            if (historyList == null)
            {
                var hlGuid = curWeb.Lists.Add("Workflow History List", string.Empty, SPListTemplateType.WorkflowHistory);
                historyList = curWeb.Lists.GetList(hlGuid, false);
            }

            if (taskList == null)
            {
                var tlGuid = curWeb.Lists.Add("Tasks List", string.Empty, SPListTemplateType.Tasks);
                taskList = curWeb.Lists.GetList(tlGuid, false);
            }

            foreach (string wfName in workFlows)
            {
                wfTemplate = curWeb.WorkflowTemplates.GetTemplateByName(wfName, CultureInfo.CurrentCulture);
                if (wfTemplate != null)
                {
                    baseTemplate = wfTemplate;
                    baseTemplate.StatusColumn = false;
                    foreach (SPList docLib in docLibs)
                    {
                        if (docLib.WorkflowAssociations.GetAssociationByBaseID(baseTemplate.Id) == null)
                        {
                            SPWorkflowAssociation assoc = SPWorkflowAssociation.CreateListContentTypeAssociation(baseTemplate, baseTemplate.Name, taskList, historyList);
                            assoc.AllowManual = true;
                            assoc.AutoStartCreate = false;
                            docLib.WorkflowAssociations.Add(assoc);
                            assoc.Enabled = true;
                        }
                    }
                }
            }
        }

        private void ActivateWorkflowFeatures(string[] workflows, SPSite curSite)
        {
            var siteFeatures = curSite.Features;
            SPFeatureDefinition featureDef;
            foreach (var feature in siteFeatures)
            {
                featureDef = feature.Definition;
                foreach (var s in workflows)
                {
                    if (featureDef.Name == s)
                    {
                        try { curSite.Features.Add(featureDef.Id); }
                        catch (System.Data.DuplicateNameException x)
                        {
                            Logger.Instance.Info(string.Format("CaseWebComponentsEventReceiver.FeatureActivated: {0} already activated at {1}",
                            featureDef.Name, curSite.Url), x, DiagnosticsCategories.eCaseWeb);
                        }
                    }
                }
            }
            //SPFarm mFarm = SPFarm.Local;
            //foreach (var featureDef in mFarm.FeatureDefinitions)
            //{
            //    if(featureDev.Name == wfName)

            //}
            //var wfId = string.Empty;
            //foreach (string s in workflows)
            //{
            //    if (s != string.Empty)
            //    {
            //        wfId = s.Split(',')[1];
            //        try { curSite.Features.Add(new Guid(wfId)); }
            //        catch (System.Data.DuplicateNameException x)
            //        {
            //            Logger.Instance.Info(string.Format("CaseWebComponentsEventReceiver.FeatureActivated: {0} already activated at {1}",
            //                eCaseConstants.FeatureIds.WORKFLOWS.ToString(), curSite.Url), x, DiagnosticsCategories.eCaseWeb);
            //        }
            //    }
            //}
        }

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, IDictionary<string, string> parameters)
        //{
        //    Logger.Instance.Info("Upgrading Feature CaseWebComponentsEventReceiver - Started");
        //    using (SPWeb web = properties.Feature.Parent as SPWeb)
        //    {
        //        using (System.IO.Stream fileStream = properties.Definition.GetFile("eCasePages\\AuditReport.aspx"))
        //        {
        //            web.Files.Add("AuditReport.aspx", fileStream, true);
        //        }
        //    }
        //    Logger.Instance.Info("Upgrading Feature CaseWebComponentsEventReceiver - Completed");
        //    base.FeatureUpgrading(properties, upgradeActionName, parameters);
        //}
    }
}
