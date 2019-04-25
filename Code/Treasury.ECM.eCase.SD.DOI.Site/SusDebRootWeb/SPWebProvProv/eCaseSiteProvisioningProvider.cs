using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Treasury.ECM.eCase.SusDeb.DOI.Common.TimerJobs;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Treasury.ECM.eCase.SusDeb.DOI.Search;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.SusDebRootWeb
{
    public class eCaseSiteProvisioningProvider : SPWebProvisioningProvider
    {
        private static readonly string ECASE_SRCH_CTR_URL = "search";
        private static readonly string ECASE_SRCH_CTR_TITLE = "SusDeb FAST Search Center";
        private static readonly string ECASE_SRCH_CTR_DESC = "Custom FAST Search Center for SusDeb, allowing execution of both KQL & FQL queries";
        private static readonly string UPDATE_NEXT_DUE_DATE_TIMER_JOB_NAME = "eCase.UpdateNextDueDate.{0}";

        public override void Provision(SPWebProvisioningProperties props)
        {
            SPWeb eCaseRootWeb = props.Web;
            SPSite eCaseRootSite = eCaseRootWeb.Site;
            eCaseRootWeb.ApplyWebTemplate("SusDebRootWeb");

            #region Create & Configure the eCase FAST Search Site
            SPWebTemplate eCaseFastSrchCtrSiteDef = eCaseRootSite.GetWebTemplates(1033)["SusDebFastSearchSiteDef#0"];
            using (SPWeb eCaseFastSrchCtr = eCaseRootWeb.Webs.Add(ECASE_SRCH_CTR_URL, ECASE_SRCH_CTR_TITLE, ECASE_SRCH_CTR_DESC, 1033, eCaseFastSrchCtrSiteDef, false, false))
            {
                // Display Ribbon by default
                eCaseFastSrchCtr.AllProperties["__DisplayShowHideRibbonActionId"] = false.ToString();

                #region Configure Site Collection Search Center settings
                eCaseRootWeb.AllProperties["SRCH_ENH_FTR_URL"] = eCaseFastSrchCtr.Url + "/pages";
                eCaseRootWeb.AllProperties["SRCH_SITE_DROPDOWN_MODE"] = "ShowDD";
                eCaseRootWeb.AllProperties["SRCH_TRAGET_RESULTS_PAGE"] = eCaseFastSrchCtr.Url + "/pages/results.aspx";
                eCaseRootWeb.Update();
                #endregion

                #region Set MasterPage
                try
                {
                    // Get the masterpage
                    SPFile eCaseSrchMaster = eCaseRootWeb.GetFile(eCaseRootWeb.Url  + "/_catalogs/masterpage/eCase_minimal.master");
                    // eCaseFastSrchCtr.MasterUrl = eCaseSrchMaster.ServerRelativeUrl; /* DO NOT APPLY TO SYSTEM PAGES */
                    eCaseFastSrchCtr.CustomMasterUrl = eCaseSrchMaster.ServerRelativeUrl;
                    eCaseFastSrchCtr.Update();
                }
                catch (Exception x)
                {
                    Logger.Instance.Error(string.Format("Failed to set master page in SusDeb FAST Search Center at {0}", eCaseFastSrchCtr.Url),
                        x, DiagnosticsCategories.eCaseSearch);
                }
                #endregion

                #region Populate Search Tabs Lists
                try
                {
                    #region Create SearchResults Tabs List
                    Guid srchResultsGuid = eCaseFastSrchCtr.Lists.Add("Tabs in Search Results", "Use this list to store the tabs displayed in search results.",
                        "SearchResults", "285dfda8-ae65-4ac1-8f6a-39ff7187bba9", 301, "100", SPListTemplate.QuickLaunchOptions.Off);
                    SPList srchResultsList = eCaseFastSrchCtr.Lists[srchResultsGuid];
                    SPListItem resultsAspx = srchResultsList.AddItem();
                    resultsAspx[eCaseConstants.FieldGuids.TABS_LIST_TAB_NAME] = "All Sites";
                    resultsAspx[eCaseConstants.FieldGuids.TABS_LIST_PAGE] = "results.aspx";
                    resultsAspx[eCaseConstants.FieldGuids.TABS_LIST_TOOLTIP] = "Click for results from All Sites";
                    resultsAspx.Update();
                    SPListItem peopleResultsAspx = srchResultsList.AddItem();
                    peopleResultsAspx[eCaseConstants.FieldGuids.TABS_LIST_TAB_NAME] = "People";
                    peopleResultsAspx[eCaseConstants.FieldGuids.TABS_LIST_PAGE] = "peopleresults.aspx";
                    peopleResultsAspx[eCaseConstants.FieldGuids.TABS_LIST_TOOLTIP] = "Click for people results";
                    peopleResultsAspx.Update();
                    #endregion

                    #region Create SearchCenter Tabs List
                    Guid srchCenterGuid = eCaseFastSrchCtr.Lists.Add("Tabs in Search Pages", "Use this list to store the tabs displayed in the default blank search pages.",
                        "SearchCenter", "285dfda8-ae65-4ac1-8f6a-39ff7187bba9", 301, "100", SPListTemplate.QuickLaunchOptions.Off);
                    SPList srchCenterList = eCaseFastSrchCtr.Lists[srchCenterGuid];
                    SPListItem defaultAspx = srchCenterList.AddItem();
                    defaultAspx[eCaseConstants.FieldGuids.TABS_LIST_TAB_NAME] = "All Sites";
                    defaultAspx[eCaseConstants.FieldGuids.TABS_LIST_PAGE] = "default.aspx";
                    defaultAspx[eCaseConstants.FieldGuids.TABS_LIST_TOOLTIP] = "Click for results from All Sites";
                    defaultAspx.Update();
                    SPListItem advancedAspx = srchCenterList.AddItem();
                    advancedAspx[eCaseConstants.FieldGuids.TABS_LIST_TAB_NAME] = "All Sites";
                    advancedAspx[eCaseConstants.FieldGuids.TABS_LIST_PAGE] = "advanced.aspx";
                    advancedAspx[eCaseConstants.FieldGuids.TABS_LIST_TOOLTIP] = "Click for results from All Sites";
                    advancedAspx.Update();
                    SPListItem peopleAspx = srchCenterList.AddItem();
                    peopleAspx[eCaseConstants.FieldGuids.TABS_LIST_TAB_NAME] = "People";
                    peopleAspx[eCaseConstants.FieldGuids.TABS_LIST_PAGE] = "people.aspx";
                    peopleAspx[eCaseConstants.FieldGuids.TABS_LIST_TOOLTIP] = "Click for people results";
                    peopleAspx.Update();
                    #endregion
                }
                catch (Exception x)
                {
                    Logger.Instance.Error(string.Format("Failed to create Tabs lists in eCase FAST Search Center at {0}", eCaseFastSrchCtr.Url), x, DiagnosticsCategories.eCaseSearch);
                }
                #endregion
            }
            #endregion

            #region Create a Daily Schedule for the UpdateNextDueDate Timer Job
            string jobName = string.Format(UPDATE_NEXT_DUE_DATE_TIMER_JOB_NAME, eCaseRootSite.ServerRelativeUrl);
            foreach (SPJobDefinition job in eCaseRootSite.WebApplication.JobDefinitions)
            {
                if (job.Name == jobName)
                    job.Delete();
            }

            // Install the job.
            UpdateNextDueDateTimerJob unddTimerJob = new UpdateNextDueDateTimerJob(jobName, eCaseRootSite.WebApplication, null, SPJobLockType.Job, eCaseRootSite.ID.ToString());

            SPDailySchedule schedule = new SPDailySchedule();
            schedule.BeginHour = 1;
            schedule.EndHour = 2;
            unddTimerJob.Schedule = schedule;
            unddTimerJob.Update();

            #endregion
        }
    }
}
