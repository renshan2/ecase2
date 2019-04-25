using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic; 
using Treasury.ECM.eCase.AuditReports;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace ECaseAuditWebPart.VisualWebPart1
{
    public partial class ECaseAuditUC : UserControl
    {
        public Treasury.ECM.eCase.AuditReports.ECaseAuditBuilder _ecAuditReportBuilder;

        protected override void OnInit(EventArgs e)
        {
            BuildDocumentLibraryList();
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _ecAuditReportBuilder = new ECaseAuditBuilder();
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
                                ddlDocumentLibraries.Items.Add(new ListItem(list.Title, list.Title));
                            }
                        }
                    }
                }
            }
            catch
            {
                if(ddlDocumentLibraries.Items.Count == 0)
                    ddlDocumentLibraries.Items.Add(new ListItem("No Libraries Found", String.Empty));
            }
        }

        protected void btnGenerate_Click(object sender, EventArgs e)
        {
            String reportSavePath = SPContext.Current.Web.Url;
            DateTime dtStartDate = calStartDate.SelectedDate;
            DateTime dtEndDate = calEndDate.SelectedDate;
            String strDocumentLibraryName = ddlDocumentLibraries.SelectedValue.Trim();

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

        protected void calStartDate_Change(object sender, EventArgs e)
        {
            txtSelectedStartDate.Text = calStartDate.SelectedDate.ToShortDateString();
        }

        protected void calEndDate_Change(object sender, EventArgs e)
        {
            txtSelectedEndDate.Text = calEndDate.SelectedDate.ToShortDateString();
        }
    }
}
