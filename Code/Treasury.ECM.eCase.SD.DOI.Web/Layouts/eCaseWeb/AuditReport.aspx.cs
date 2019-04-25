using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts
{
    public partial class AuditReport : LayoutsPageBase
    {
        public eCaseAuditBuilder _ecAuditReportBuilder;

        protected override SPBasePermissions RightsRequired
        {
            get
            {
                return base.RightsRequired & SPBasePermissions.ManagePermissions;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (SPContext.Current.Web.WebTemplate.ToLower().Contains("eCase"))
            { }
            //this.MasterPageFile = SPContext.Current.Site.ServerRelativeUrl + "/Style Library/MasterPages/ecase_main.master";
        }

        protected override void OnInit(EventArgs e)
        {
            BuildDocumentLibraryList();
            base.OnInit(e);
        }

        /// <summary>
        /// On Page Load, find literal controls, then populate with content
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            _ecAuditReportBuilder = new eCaseAuditBuilder(); 
            
            //if (!IsPostBack)
            //{
            //    SPQuery caseUniqueIdQuery = new SPQuery();
            //    caseUniqueIdQuery.ViewFields = "<FieldRef Name='UniqueCaseID' />";
            //    caseUniqueIdQuery.Query = String.Format("<Where><Eq><FieldRef Name='CaseUrl' /><Value Type='Url'>{0}</Value></Eq></Where>", SPContext.Current.Web.Url);
            //    SPListItemCollection caseRow = SPContext.Current.Site.RootWeb.Lists["Cases"].GetItems(caseUniqueIdQuery);
            //    caseIdLabel.Text = caseRow.Count > 0 ? caseRow[0]["UniqueCaseId"] as string : string.Empty;
            //}
        }

        private void BuildDocumentLibraryList()
        {
            try
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        foreach (SPList list in web.Lists)
                        {
                            if (list.BaseType == SPBaseType.DocumentLibrary
                                && list.DoesUserHavePermissions(SPBasePermissions.AddListItems))
                            {
                                documentLibrariesDropDown.Items.Add(new ListItem(list.Title, list.Title));
                            }
                        }
                    }
                }
            }
            catch
            {
                if (documentLibrariesDropDown.Items.Count == 0)
                    documentLibrariesDropDown.Items.Add(new ListItem("No Libraries Found", String.Empty));
            }
        }

        protected void startDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            selectedStartDateTextBox.Text = startDateCalendar.SelectedDate.ToShortDateString();
        }

        protected void endDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            selectedEndDateTextBox.Text = endDateCalendar.SelectedDate.ToShortDateString();
        }

        protected void generateButton_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                String reportSavePath = SPContext.Current.Web.Url;
                DateTime dtStartDate = startDateCalendar.SelectedDate;
                DateTime dtEndDate = endDateCalendar.SelectedDate;
                String strDocumentLibraryName = documentLibrariesDropDown.SelectedValue.Trim();

                if (!String.IsNullOrEmpty(strDocumentLibraryName))
                {
                    SPLongOperation.Begin(delegate(SPLongOperation longOperation)
                    {
                        _ecAuditReportBuilder.StartDate = dtStartDate;
                        _ecAuditReportBuilder.EndDate = dtEndDate;
                        _ecAuditReportBuilder.DocumentLibraryName = strDocumentLibraryName;
                        Boolean bSuccessfulProcess = _ecAuditReportBuilder.ProcessAuditReport();

                        if ((bSuccessfulProcess)
                            && _ecAuditReportBuilder.ReportSavePath != null
                            && _ecAuditReportBuilder.ReportSavePath != String.Empty)
                            reportSavePath = _ecAuditReportBuilder.ReportSavePath;

                        longOperation.End(reportSavePath);
                    }
                    );
                }
            }
        }

        protected void miscValidationLogicValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (startDateCalendar.SelectedDate == DateTime.MinValue)
            {
                args.IsValid = false;
                miscValidationLogicValidator.ErrorMessage += "\nStart Date must be selected.";
            }
            if (endDateCalendar.SelectedDate == DateTime.MinValue)
            {
                args.IsValid = false;
                miscValidationLogicValidator.ErrorMessage += "\nEnd Date must be selected.";
            }
            //end date must be greater than or equal to start date and not equal to minimum date
            if (endDateCalendar.SelectedDate < startDateCalendar.SelectedDate)
            {
                args.IsValid = false;
                miscValidationLogicValidator.ErrorMessage += "\nEnd Date Must be greater than or equal to Start Date.";
            }
        }

    }
}
