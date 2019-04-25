using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Taxonomy;
using System.Globalization;
using System.Text.RegularExpressions;
using Treasury.ECM.eCase.SusDeb.DOI.Common.DocIdProvider;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;
using System.Data;
using System.Data.SqlClient;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Web
{
    public partial class DocumentIdApply : LayoutsPageBase
    {

        protected List<SPListItem> listItems = new List<SPListItem>();
        internal const int MAX_LENGTH = 255;
        internal static readonly Regex SpaceRegex = new Regex("[ ]{2,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public DocumentIdApply()
        {
            this.RightsCheckMode = RightsCheckModes.None;
        }

        [Serializable()]
        public class SelectedItem
        {
            public SelectedItem() { }

            public SelectedItem(string itemId)
            {
                Id = itemId;
            }
            public string Id { get; set; }
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static void SendDocuments(string siteId, string webId, string listId, List<SelectedItem> items)
        {
            Logging.Logger.Instance.Info("Receiving selected documents from Apply Document IDs Custom Action");
            //send these values to session for later use
            HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_SITE_ID] = siteId;
            HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_WEB_ID] = webId;
            HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_LIST_ID] = listId;
            HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_ITEMS] = items;
        }


        protected override bool RequireSiteAdministrator
        {
            get
            {
                return false;
            }
        }



        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                string siteId = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_SITE_ID] as string;
                string webId = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_WEB_ID] as string;
                string listId = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_LIST_ID] as string;
                List<SelectedItem> items = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_ITEMS] as List<SelectedItem>;

                instructionsLabel.Text = String.Format("Document IDs will be applied to the {0} item(s) you have selected.", items.Count.ToString());
            }
        }

        protected void applyButton_Click(object sender, EventArgs e)
        {
            bool errorEncountered = false;
            try
            {
                string siteId = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_SITE_ID] as string;
                string webId = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_WEB_ID] as string;
                string listId = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_LIST_ID] as string;
                List<SelectedItem> selectedItems = HttpContext.Current.Session[eCaseConstants.SessionKeys.DOC_ID_RENUMBER_ITEMS] as List<SelectedItem>;
                List<SelectedItem> unselectedItems = new List<SelectedItem>();

                //reset the instructions label
                instructionsLabel.Text = string.Empty;

                //A validator is making sure the field is populated (Required) - Now ensure textbox is uppercase and value is trimmed    
                prefixTextBox.Text = prefixTextBox.Text.Trim().ToUpper();

                //TODO: Determine if the set of chosen items is the complete set of items, else display an error message.
                string connectionString = SPContext.Current.Site.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
                if (String.IsNullOrEmpty(connectionString))
                {
                    itemsLabel.Text = "Unable to obtain connection information for the eCase Database.";
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "GetDocIdListByPrefix";
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter param = new SqlParameter();
                        param.ParameterName = "@SiteGuid";
                        param.Value = siteId;
                        cmd.Parameters.Add(param);

                        param = new SqlParameter();
                        param.ParameterName = "@WebGuid";
                        param.Value = webId;
                        cmd.Parameters.Add(param);

                        param = new SqlParameter();
                        param.ParameterName = "@Prefix";
                        param.Value = prefixTextBox.Text;
                        cmd.Parameters.Add(param);

                        List<SelectedItem> existingItems = new List<SelectedItem>();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (reader.GetValue(0).ToString() != string.Empty)
                                    existingItems.Add(new SelectedItem(reader.GetValue(0).ToString()));
                            }
                        }

                        //unselectedItems = existingItems.Except(selectedItems).ToList();
                        unselectedItems = (from ei in existingItems where !selectedItems.Select(si => si.Id).Contains(ei.Id) select ei).ToList();
                    }
                }

                if (unselectedItems.Count > 0)
                {
                    itemsLabel.Text = String.Format(@"The prefix '{0}' is already in use. You may not apply document ids 
                                                            to an existing prefix unless you select all of the 
                                                            documents currently using that prefix. Please choose an alternate prefix.
                                                            There are {1} items that you did not select.", prefixTextBox.Text, unselectedItems.Count);
                    return;
                }

                //TODO: The set of items is either complete, or the prefix is not in use on this site, apply an id to each document.
                List<SPItem> selectedListItems = new List<SPItem>();

                //This could likely be much more efficient, but approached this way to minimize dev work while still avoiding .Items
                using (SPSite site = new SPSite(new Guid(siteId)))
                {
                    using (SPWeb web = site.OpenWeb(new Guid(webId)))
                    {
                        SPList list = web.Lists[new Guid(listId)];
                        SPQuery query = new SPQuery();
                        //query.Query = String.Format("<Where><In><FieldRef Name='ID'/><Values><Value Type='Number'>{0}</Value></Values></In></Where>", selectedItems[0].Id);
                        System.Text.StringBuilder queryBuilder = new System.Text.StringBuilder();
                        queryBuilder.Append("<Where><In><FieldRef Name='ID'/><Values>");
                        foreach (SelectedItem item in selectedItems)
                        {
                            queryBuilder.AppendFormat("<Value Type='Number'>{0}</Value>", item.Id);
                        }
                        queryBuilder.Append("</Values></In></Where>");
                        query.Query = queryBuilder.ToString();
                        query.ItemIdQuery = true;
                        SPListItemCollection workItems = list.GetItems(query);

                        Microsoft.Office.DocumentManagement.DocumentIdProvider currentProvider = Microsoft.Office.DocumentManagement.DocumentId.GetProvider(site);
                        eCaseDocIdProvider eCaseProvider = null;
                        if (currentProvider is eCaseDocIdProvider)
                        {
                            eCaseProvider = currentProvider as eCaseDocIdProvider;
                        }

                        if (eCaseProvider != null)
                        {
                            AssignDocumentIDsToItems(eCaseProvider, workItems);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorEncountered = true;
                Logging.Logger.Instance.Error("Error occurred while assigning document IDs.", ex, Logging.DiagnosticsCategories.eCaseWeb);
            }

            //TODO: Display a success message and allow the user to return to their original page
            if (errorEncountered)
            {
                instructionsLabel.Text = "An error occurred while applying document IDs. If this error continues to occur please review logs for additional details.";
            }
            else
            {
                instructionsLabel.Text = "Document IDs were successfully applied to all selected items.";
                returnToSource.Visible = true;
            }
        }

        protected bool AssignDocumentIDsToItems(eCaseDocIdProvider eCaseProvider, SPListItemCollection workItems)
        {
            bool errorEncountered = false;
            foreach (SPListItem workItem in workItems)
            {
                System.Diagnostics.Debug.WriteLine("Assigning new Document ID to Item: {0}", workItem.Url);
                //TODO: Special Cases = Folder, Other?
                if (workItem.Folder == null) //Docs state that if the folder property is null, this is not a folder
                {
                    try
                    {
                        eCaseProvider.AssignDocumentId(workItem, prefixTextBox.Text);
                    }
                    catch (Exception ex)
                    {
                        errorEncountered = true;
                        Logging.Logger.Instance.Error("Error occured while assigning Document ID", ex, Logging.DiagnosticsCategories.eCaseWeb);
                    }
                }
                else
                {
                    SPQuery childItemsQuery = new SPQuery();
                    childItemsQuery.Folder = workItem.Folder;
                    SPListItemCollection childItems = workItem.ParentList.GetItems(childItemsQuery);
                    errorEncountered = AssignDocumentIDsToItems(eCaseProvider, childItems);
                }
            }
            return errorEncountered;
        }

        protected void returnToSource_Click(object sender, EventArgs e)
        {
            Microsoft.SharePoint.Utilities.SPUtility.Redirect(string.Empty, Microsoft.SharePoint.Utilities.SPRedirectFlags.UseSource, Context);
        }
    }
}
