using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.WebParts.UserSavedSearchesWebPart
{
    [ToolboxItem(false)]
    public partial class UserSavedSearchesWebPart : WebPart
    {
        // Uncomment the following SecurityPermission attribute only when doing Performance Profiling on a farm solution
        // using the Instrumentation method, and then remove the SecurityPermission attribute when the code is ready
        // for production. Because the SecurityPermission attribute bypasses the security check for callers of
        // your constructor, it's not recommended for production purposes.
        // [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Assert, UnmanagedCode = true)]
        public UserSavedSearchesWebPart()
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
                this.lbManageSavedSearches.PostBackUrl = SPContext.Current.Web.Url + "/Lists/Saved Queries";
                this.lbManageSavedResults.PostBackUrl = SPContext.Current.Web.Url + "/search/_layouts/ecasesearch/savesearchresultsets.aspx";
            }
        }

        protected void lbRunSearch_Click(object sender, EventArgs e)
        {
            if (ddlSavedSearches.SelectedValue != "0")
            {
                SPUtility.Redirect(ddlSavedSearches.SelectedValue, SPRedirectFlags.Default, System.Web.HttpContext.Current);
            }
        }


        #region private methods

            private void LoadSavedSearches()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.Url))
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
                    string cleanedUserName = null;
                    foreach (SPListItem item in list.Items)
                    {
                        //cleanedUserName = new SPFieldLookupValue(item["Author"].ToString()).LookupValue;
                        //if (cleanedUserName == SPContext.Current.Web.CurrentUser.Name)
                        //{
                        //    ddlSavedSearches.Items.Add(new ListItem(item["Title"].ToString(), item["Query"].ToString()));
                        //}
                        ddlSavedSearches.Items.Add(new ListItem(item["Title"].ToString(), item["Query"].ToString()));
                    }
                }
                else
                {
                    ddlSavedSearches.Items.Add(new ListItem("Select a Saved Search...", "0"));
                }
            }

        #endregion
    }
}