using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities
{
    public static class CasesNextDueDate
    {
        private static readonly string DUE_DATE_URL_FORMAT = "{0}, {1}: {2} ({3} @ {4})"; // Url, Type/Prefix, Title, Date, Time
        private static readonly string CASE_RELATED_DATE_PREFIX = "Event";
        private static readonly string ACTIVITIES_TASKS_PREFIX = "Task";

        /// <summary>
        /// This method will run with elevated privileges and execute the method that takes an SPWeb object
        /// </summary>
        /// <param name="properties"></param>
        public static void UpdateNextDueDate(SPItemEventProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(properties.SiteId))
                {
                    using (SPWeb web = site.OpenWeb(properties.Web.ID))
                    {
                        CasesNextDueDate.UpdateNextDueDate(web);
                    }
                }
            });
        }

        /// <summary>
        /// This method will retrieve the top item (ORDER BY DueDate/EndDate) from the Activities & Tasks list, 
        /// as well as the Case Related Dates list.  It then determines which of those items is most pressing and
        /// edits the parent Cases list item's Due Date and Due Date Url fields.
        /// </summary>
        /// <param name="caseWeb"></param>
        public static void UpdateNextDueDate(SPWeb caseWeb)
        {
            Guid caseWebParentListItemGuid, activitiesTasksGuid, caseRelatedDatesGuid;
            SPList casesList;
            SPListItem caseWebParentListItem;

            #region Get Necessary Data to Update Next Due Date
            try
            {
                caseWebParentListItemGuid = new Guid(caseWeb.AllProperties[eCaseConstants.PropertyBagKeys.ECASE_CASE_LIST_ITEM_GUID].ToString());
                casesList = caseWeb.Site.RootWeb.GetListByInternalName(eCaseConstants.ListInternalNames.ECASES_LIST);
                caseWebParentListItem = casesList.GetItemByUniqueId(caseWebParentListItemGuid);
                activitiesTasksGuid = caseWeb.GetListByInternalName(eCaseConstants.ListInternalNames.ACTIVITIES_AND_TASKS).ID;
                caseRelatedDatesGuid = caseWeb.GetListByInternalName(eCaseConstants.ListInternalNames.CASE_RELATED_DATES).ID;
            }
            catch (Exception x)
            {
                Logger.Instance.Error(string.Format("Failed to get data while updating Next Due Date at {0}", caseWeb.Url), x, Logging.DiagnosticsCategories.eCaseCommon);
                return; // Already logged, no need to do more
            }
            #endregion

            try
            {
                string itemUrl = string.Empty;
                string itemType = string.Empty;
                string itemTitle = string.Empty;
                string itemDate = string.Empty;
                string itemTime = string.Empty;
                bool allDayEvent = false;

                #region Get Activities & Tasks item
                SPQuery atQuery = new SPQuery();
                atQuery.SetViewFields(new string[] { "Title", "DueDate" });
                atQuery.RowLimit = 1;
                #region Query
                atQuery.Query = string.Format(@"
                    <Where>
                        <Geq>
                            <FieldRef Name='DueDate'/>
                            <Value Type='DateTime' IncludeTimeValue='TRUE'>
                                {0}
                            </Value>
                        </Geq>
                    </Where>
                    <OrderBy>
                        <FieldRef Name='DueDate'/>
                        <FieldRef Name='Title'/>
                    </OrderBy>"
                    , SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now)); // <Now/> element doesn't appear to work
                #endregion
                SPList atList = caseWeb.Lists[activitiesTasksGuid];
                SPListItemCollection atItems = atList.GetItems(atQuery);
                SPListItem atItem = null;
                DateTime atDt;
                if (atItems.Count > 0)
                {
                    atItem = atItems[0];
                    DateTime.TryParse(atItem[eCaseConstants.FieldGuids.OOTB_DUE_DATE].ToString(), out atDt);
                }
                else
                {
                    atDt = DateTime.MaxValue;
                }
                #endregion

                #region Get Case Related Dates item
                SPList crdList = caseWeb.Lists[caseRelatedDatesGuid];

                // Get item with most recent End Date that is not a recurrence
                SPQuery crdQuery = new SPQuery();
                crdQuery.SetViewFields(new string[] { "Title", "EventDate", "EndDate", "RecurrenceID", "fAllDayEvent", "fRecurrence" });
                crdQuery.RowLimit = 1;
                crdQuery.CalendarDate = DateTime.Now;
                #region Query
                crdQuery.Query = string.Format(@"
                    <Where>
                        <And>
                            <Neq>
                                <FieldRef Name='fRecurrence'/>
                                <Value Type='Recurrence'>1</Value>
                            </Neq>                            
                            <Geq>
                                <FieldRef Name='EndDate'/>
                                <Value Type='DateTime' IncludeTimeValue='TRUE'>
                                    {0}
                                </Value>
                            </Geq>
                        </And>
                    </Where>
                    <OrderBy>
                        <FieldRef Name='EndDate'/>
                        <FieldRef Name='Title'/>
                    </OrderBy>",
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime(DateTime.Now)); // <Now/> element doesn't appear to work
                #endregion
                SPListItemCollection crdItems = crdList.GetItems(crdQuery);

                // Get non-Deleted recurrence items (Event Type != 3) that occur within the year
                SPQuery crdExpandedQuery = new SPQuery();
                crdExpandedQuery.SetViewFields(new string[] 
                { "Title", "EventDate", "EndDate", "RecurrenceID", "fAllDayEvent", "fRecurrence", "EventType" });
                crdExpandedQuery.ExpandRecurrence = true; // This is a Calendar list, and can contain recurrence events
                crdExpandedQuery.CalendarDate = DateTime.Now;
                #region Query
                crdExpandedQuery.Query = @"
                    <Where>
                        <And>
                            <And>
                                <Eq>
                                    <FieldRef Name='fRecurrence'/>
                                    <Value Type='Recurrence'>1</Value>
                                </Eq>
                                <Neq>
                                    <FieldRef Name='EventType'/>
                                    <Value Type='Integer'>3</Value>
                                </Neq>
                            </And>
                            <DateRangesOverlap>
                                <FieldRef Name='EventDate'/>
                                <FieldRef Name='EndDate'/>
                                <FieldRef Name='RecurrenceID'/>
                                <Value Type='DateTime' IncludeTimeValue='TRUE'>
                                    <Year/>
                                </Value>
                            </DateRangesOverlap>
                        </And>
                    </Where>
                    <OrderBy>
                        <FieldRef Name='EventDate'/>
                        <FieldRef Name='Title'/>
                    </OrderBy>";
                #endregion
                SPListItemCollection crdExpandedItems = crdList.GetItems(crdExpandedQuery);

                SPListItem crdItem = null;
                if (crdItems.Count > 0)
                    crdItem = crdItems[0];
                SPListItem crdExpandedItem = null;
                if (crdExpandedItems.Count > 0)
                {
                    foreach (SPListItem item in crdExpandedItems) // Iterate over each item and ignore those that occur before NOW
                    {
                        DateTime dt;
                        if (DateTime.TryParse(item[eCaseConstants.FieldGuids.OOTB_EVENT_DATE].ToString(), out dt))
                        {
                            if (dt >= DateTime.Now) // Grab first item that occurs NOW or later
                            {
                                crdExpandedItem = item;
                                break; // We found the next item; no need to keep looking
                            }
                        }
                    }
                }

                DateTime crdDt;
                if (crdItem != null && crdExpandedItem != null)
                {
                    DateTime expandedDt, dt;
                    DateTime.TryParse(crdExpandedItem[eCaseConstants.FieldGuids.OOTB_EVENT_DATE].ToString(), out expandedDt);
                    DateTime.TryParse(crdItem[eCaseConstants.FieldGuids.OOTB_END_DATE].ToString(), out dt);
                    if (expandedDt <= dt)
                    {
                        crdItem = crdExpandedItem;
                        crdDt = expandedDt;
                    }
                    else
                        crdDt = dt;
                }
                else if (crdExpandedItems.Count > 0)
                {
                    crdItem = crdExpandedItem;
                    DateTime.TryParse(crdItem[eCaseConstants.FieldGuids.OOTB_EVENT_DATE].ToString(), out crdDt);
                }
                else if (crdItems.Count > 0)
                {
                    crdItem = crdItems[0];
                    DateTime.TryParse(crdItem[eCaseConstants.FieldGuids.OOTB_END_DATE].ToString(), out crdDt);
                }
                else
                {
                    crdDt = DateTime.MaxValue;
                }

                if (crdItem != null && crdItem[eCaseConstants.FieldGuids.OOTB_ALL_DAY_EVENT] != null)
                    allDayEvent = (bool)crdItem[eCaseConstants.FieldGuids.OOTB_ALL_DAY_EVENT];
                #endregion

                SPListItem nextDueDateItem = null;
                DateTime? nextDueDate = null;
                #region Compare Activities & Tasks next date vs Case Related Dates
                if (atItem != null && crdItem != null)
                {
                    if (atDt <= crdDt)
                    {
                        nextDueDateItem = atItem;
                        nextDueDate = atDt;
                        itemType = ACTIVITIES_TASKS_PREFIX;
                    }
                    else
                    {
                        nextDueDateItem = crdItem;
                        nextDueDate = crdDt;
                        itemType = CASE_RELATED_DATE_PREFIX;
                    }
                }
                else if (atItem != null)
                {
                    DateTime.TryParse(atItem[eCaseConstants.FieldGuids.OOTB_DUE_DATE].ToString(), out atDt);
                    nextDueDateItem = atItem;
                    nextDueDate = atDt;
                    itemType = ACTIVITIES_TASKS_PREFIX;
                }
                else if (crdItem != null)
                {
                    DateTime.TryParse(crdItem[eCaseConstants.FieldGuids.OOTB_END_DATE].ToString(), out crdDt);
                    nextDueDateItem = crdItem;
                    nextDueDate = crdDt;
                    itemType = CASE_RELATED_DATE_PREFIX;
                }
                #endregion

                if (nextDueDateItem != null)
                {
                    // SharePoint does not know how to route SPListItem.Url to the Display Form
                    itemUrl = nextDueDateItem.GetDisplayFormUrl();

                    itemTitle = nextDueDateItem[eCaseConstants.FieldGuids.OOTB_TITLE].ToString();
                    itemDate = ((DateTime)nextDueDate).ToShortDateString();
                    if (allDayEvent)
                        itemTime = "All Day";
                    else
                        itemTime = ((DateTime)nextDueDate).ToShortTimeString();

                    string oldValue = string.Empty;
                    if (caseWebParentListItem[eCaseConstants.FieldGuids.ECASES_LIST_NEXTDUEDATEURL] != null)
                        oldValue = caseWebParentListItem[eCaseConstants.FieldGuids.ECASES_LIST_NEXTDUEDATEURL].ToString();

                    string newValue = string.Format(DUE_DATE_URL_FORMAT, itemUrl, itemType, itemTitle, itemDate, itemTime);

                    if (!oldValue.Contains(newValue)) // Old value has an absolute URL, but newValue has a server-relative URL
                    {
                        caseWebParentListItem[eCaseConstants.FieldGuids.ECASES_LIST_NEXTDUEDATEURL] = newValue;
                        caseWebParentListItem[eCaseConstants.FieldGuids.ECASES_LIST_TASKDUEDATE] = nextDueDate;
                        caseWebParentListItem.Update();
                    }
                }
                else // Ensure the value in Due Date and Due Date Url is properly cleared
                {
                    itemUrl = caseWeb.Url;
                    itemTitle = "No Upcoming Dates or Tasks";
                    itemDate = string.Empty;
                    itemTime = string.Empty;
                    string newValue = string.Format("{0}, {1}", itemUrl, itemTitle);
                    caseWebParentListItem[eCaseConstants.FieldGuids.ECASES_LIST_NEXTDUEDATEURL] = newValue;
                    caseWebParentListItem[eCaseConstants.FieldGuids.ECASES_LIST_TASKDUEDATE] = null;
                    caseWebParentListItem.Update();
                }
            }
            catch (Exception x)
            { Logger.Instance.Error(string.Format("Failed to update Next Due Date at {0}", caseWeb.Url), x, DiagnosticsCategories.eCaseCommon); }
        }
    }
}