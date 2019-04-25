using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Globalization;
using System.Text;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch
{
    public partial class SaveSearchQuery : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (SPSite site = new SPSite(Request.QueryString["searchUrl"].ToString()))
            //using (SPSite site = new SPSite(SPContext.Current.Site.RootWeb.Url))
            {
                SPListItem itemToAdd = null;
                SPWeb web = site.RootWeb;
                web.AllowUnsafeUpdates = true;
                SPList queryList = web.Lists["Saved Queries"];
                itemToAdd = queryList.Items.Add();
                itemToAdd["Title"] = txtTitle.Text.Trim();
                itemToAdd["Description"] = txtDescription.Text.Trim();
                itemToAdd["Query"] = Request.QueryString["searchUrl"].ToString();
                // itemToAdd["Author"] = web.CurrentUser;
                itemToAdd["Author1"] = web.CurrentUser;
                if (spPeoplePicker.ResolvedEntities.Count > 0)
                {
                    itemToAdd["SharedWith"] = GetUserValues(web);
                }
                itemToAdd.Update();
                web.AllowUnsafeUpdates = false;

                //close the modal
                Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, '{0}');</script>", itemToAdd.ID));
                Response.Flush();
                Response.End();
            }
        }

        private SPFieldUserValueCollection GetUserValues(SPWeb web)
        {
            SPFieldUserValueCollection values = new SPFieldUserValueCollection();

            foreach (PickerEntity entity in spPeoplePicker.ResolvedEntities)
            {
                SPUser user = web.EnsureUser(entity.Key);
                SPFieldUserValue fuv = new SPFieldUserValue(web, user.ID, user.LoginName);
                values.Add(fuv);
            }

            return values;
        }
    }
}
