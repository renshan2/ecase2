using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.SaveSearchResultsWebPart
{
    [ToolboxItem(false)]
    public partial class SaveSearchResultsWebPart : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SaveSearchResultsWebPart()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void manageSavedSearchResultsButton_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect("ecasesearch/savesearchresultsets.aspx", SPRedirectFlags.RelativeToLayoutsPage, Context);
        //    SPUtility.Redirect(
        //SPUrlUtility.CombineUrl(
        //    SPContext.Current.Web.Url, String.Format("?query={0}", Context.Request.QueryString["k"])),
        //    SPRedirectFlags.Default,
        //    Context
        //    );
        }

        protected void saveSearchResultsButton_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect(
                    String.Format("ecasesearch/savesearchresults.aspx?query={0}", Context.Request.QueryString["k"]),
                    SPRedirectFlags.RelativeToLayoutsPage,
                    Context
            );
        }
    }
}
