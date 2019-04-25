using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.WebParts.MgrDashboardDueDates
{
    [ToolboxItem(false)]
    public partial class MgrDashboardDueDates : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]

        public int ItemId
        {
            get { return ViewState["ItemId"] != null ? (int)ViewState["ItemId"] : 0; }
            set { ViewState["ItemId"] = value; }
        }

        public MgrDashboardDueDates()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var lists = GetDueDateLists(int.Parse(ddlNumOfDays.SelectedValue));
            BindGrid(lists);
        }

        private void BindGrid(List<SPList> lists)
        {
            List<SPListItemCollection> filteredLists = GetFilteredLists(lists);



            DataTable tbl = new DataTable();
            tbl.Columns.Add("CaseName", typeof(string));
            tbl.Columns.Add("Title", typeof(string));
            tbl.Columns.Add("EventDate", typeof(string));
            tbl.Columns.Add("EndDate", typeof(string));
            tbl.Columns.Add("StartDate", typeof(string));
            tbl.Columns.Add("DueDate", typeof(string));
            DataRow row;

            if (filteredLists.Count > 0)
            {
                // add our first seperator row
                string currentCaseName = filteredLists[0].List.ParentWeb.Name;
                row = tbl.Rows.Add();
                row["CaseName"] = filteredLists[0].List.ParentWeb.Name;

                foreach (SPListItemCollection itemCollection in filteredLists)
                {
                    // if we are dealing with a new Case name then add a new seperator row
                    if (itemCollection.List.ParentWeb.Name != currentCaseName)
                    {
                        row = tbl.Rows.Add();
                        row["CaseName"] = itemCollection.List.ParentWeb.Name;
                        currentCaseName = row["CaseName"].ToString();
                    }

                    // get our datatable from our listitemcollection
                    var dt = itemCollection.GetDataTable();

                    // add our data to our grid
                    foreach (DataRow r in dt.Rows)
                    {
                        row = tbl.Rows.Add();
                        row["Title"] = r["Title"].ToString();
                        //if (dt.TableName == "Case Related Dates")
                        //{
                        //    row["EventDate"] = r["EventDate"] == null ? string.Empty : r["EventDate"].ToString();
                        //    row["EndDate"] = r["EndDate"] == null ? string.Empty : r["EndDate"].ToString();
                        //}
                        //else
                        //{
                        row["StartDate"] = r["StartDate"] == null ? string.Empty : r["StartDate"].ToString();
                        row["DueDate"] = r["DueDate"] == null ? string.Empty : r["DueDate"].ToString();
                        //}
                    }
                }
            }

            GridViewDates.DataSource = tbl.DefaultView;
            GridViewDates.DataBind();
        }

        private List<SPListItemCollection> GetFilteredLists(List<SPList> lists)
        {
            List<SPListItemCollection> listItemCollections = new List<SPListItemCollection>();
            SPListItemCollection filteredCaseRelatedDates;
            SPListItemCollection filteredTasksAndActivities;
            var offSetDays = ddlNumOfDays.SelectedValue;

            foreach (SPList list in lists)
            {
                if (list.RootFolder.Name == eCaseConstants.ListInternalNames.CASE_RELATED_DATES)
                {
                    SPQuery spQueryCRD = new SPQuery();
                    string crdQuery = "<Where><And><Geq><FieldRef Name='EndDate' /><Value Type='DateTime'><Today /></Value></Geq><Leq><FieldRef Name='EndDate' /><Value Type='DateTime'><Today OffsetDays=" + offSetDays + "/></Value></Leq></And></Where>";
                    spQueryCRD.Query = crdQuery;
                    filteredCaseRelatedDates = list.GetItems(spQueryCRD);

                    if (filteredCaseRelatedDates.Count > 0)
                        listItemCollections.Add(filteredCaseRelatedDates);
                }
                else if (list.RootFolder.Name == eCaseConstants.ListInternalNames.ACTIVITIES_AND_TASKS)
                {
                    SPQuery spQueryTAA = new SPQuery();
                    string tasksQuery = "<Where><And><Geq><FieldRef Name='DueDate' /><Value Type='DateTime'><Today /></Value></Geq><Leq><FieldRef Name='DueDate' /><Value Type='DateTime'><Today OffsetDays=" + offSetDays + "/></Value></Leq></And></Where>";
                    spQueryTAA.Query = tasksQuery;
                    filteredTasksAndActivities = list.GetItems(spQueryTAA);

                    if (filteredTasksAndActivities.Count > 0)
                        listItemCollections.Add(filteredTasksAndActivities);
                }
            }

            return listItemCollections;
        }

        private List<SPList> GetDueDateLists(int offSetDays)
        {
            List<SPList> spLists = new List<SPList>();

            // get our filtered list of cases 
            var parentWeb = SPContext.Current.Site.RootWeb;
            var eCases = parentWeb.GetList(parentWeb.Url + "/Lists/Cases");

            SPQuery myeCaseQuery = new SPQuery();
            //string eCaseQuery = "<Where><Eq><FieldRef Name='AssignedToSupervisor' /><Value Type='Integer'><UserID/></Value></Eq></Where>";
            string eCaseQuery = "<Where><Eq><FieldRef Name='AssignedTo' /><Value Type='Integer'><UserID/></Value></Eq></Where>";
            myeCaseQuery.Query = eCaseQuery;
            var filteredCases = eCases.GetItems(myeCaseQuery);

            foreach (SPListItem caseItem in filteredCases)
            {
                // get our lists
                using (SPSite caseSite = new SPSite(parentWeb.Url + "/" + caseItem["UniqueCaseID"].ToString()))
                {
                    using (SPWeb caseWeb = caseSite.OpenWeb())
                    {
                        if (!caseWeb.IsRootWeb)
                        {
                            // case related dates
                            var caseRelatedDates = caseWeb.GetList(caseWeb.Url + "/Lists/CaseRelatedDates");
                            if (caseRelatedDates != null)
                            {
                                spLists.Add(caseRelatedDates);
                            }

                            // tasks and activities
                            var tasksAndActivities = caseWeb.GetList(caseWeb.Url + "/Lists/TasksAndActivities");
                            if (tasksAndActivities != null)
                            {
                                spLists.Add(tasksAndActivities);
                            }
                        }
                    }
                }
            }

            return spLists;

        }

        protected void ddlNumOfDays_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listData = GetDueDateLists(int.Parse(ddlNumOfDays.SelectedValue));
            BindGrid(listData);
        }
    }
}

