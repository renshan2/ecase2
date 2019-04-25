using Microsoft.SharePoint;
using Microsoft.SharePoint.Portal.WebControls;
using Microsoft.SharePoint.Utilities;
using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.SaveSearchCriteriaWebPart
{
    [ToolboxItem(false)]
    public partial class SaveSearchCriteriaWebPart : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public SaveSearchCriteriaWebPart()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadSavedSearches();
            }

            var listNewFormUrl = string.Empty;
            using (SPSite site = new SPSite(SPContext.Current.Site.RootWeb.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    listNewFormUrl = Page.Request.Url.ToString();
                }
            }

            hdnSearchUrl.Value = listNewFormUrl;
        }

        protected void btnRunSearch_Click(object sender, EventArgs e)
        {
            if (ddlSavedSearches.SelectedValue != "0")
            {
                SPUtility.Redirect(ddlSavedSearches.SelectedValue, SPRedirectFlags.Default, System.Web.HttpContext.Current);
            }
        }

        private void LoadSavedSearches()
        {
            using (SPSite site = new SPSite(SPContext.Current.Site.RootWeb.Url))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists["Saved Queries"];
                    BindSavedSearchDropdown(list);
                }
            }
        }

        private void BindSavedSearchDropdown(SPList list)
        {
            if (list.Items.Count > 0)
            {
                ddlSavedSearches.Items.Add(new ListItem("Select a Saved Search...", "0"));
                foreach (SPListItem item in list.Items)
                {
                    ddlSavedSearches.Items.Add(new ListItem(item["Title"].ToString(), item["Query"].ToString()));
                }
            }
            else
            {
                ddlSavedSearches.Items.Add(new ListItem("Select a Saved Search...", "0"));
            }
        }
    }
}
