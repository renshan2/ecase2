using System;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Search;
using Microsoft.SharePoint.WebControls;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch
{
    public partial class SaveSearchResults : LayoutsPageBase
    {
        protected bool IsNew = false;

        [System.Web.Services.WebMethod()]
        public static string SaveSearchResultData(string id, string field, string value)
        {
            Logging.Logger.Instance.Info(String.Format("Saving Results Data For Field: {0}; Row ID: {1}; Value:{2};", id, field, value));
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string updateCommandText = String.Format(@"UPDATE SavedSearchResultItems
                                                SET {0} = @Value
                                                WHERE Id = @Id", field);

                using (SqlCommand cmd = new SqlCommand(updateCommandText, conn))
                {
                    cmd.Parameters.AddWithValue("@Value", value);
                    cmd.Parameters.AddWithValue("@Id", id);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return "Data successfully saved.";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //If the Id key does not exist this is a new request
            IsNew = !Request.QueryString.AllKeys.Contains("id");
            this.copySelectedResults.Visible = !IsNew;
            this.copySelectedResultsNote.Visible = !IsNew;

            if (!IsPostBack)
            {
                ViewState["PreviousPage"] = Request.UrlReferrer;

                if (!IsNew)
                {
                    PopulateSavedSearchResultsSetsInfo();
                }
                else
                {
                    queryLabel.Text = Request.QueryString["query"];
                    ExecuteSearchQuery(queryLabel.Text);
                    ownerPeopleEditor.CommaSeparatedAccounts = SPContext.Current.Web.CurrentUser.LoginName;                    
                }
                
                long queryId;
                if (!String.IsNullOrEmpty(Request.QueryString["id"]) && long.TryParse(Request.QueryString["id"], out queryId))
                {
                    savedSearchResultsIdHidden.Value = queryId.ToString();
                    BindSearchResultsDataTableById(queryId);
                    savedSearchesDropDown.SelectedValue = queryId.ToString();
                    copySelectedResults.NavigateUrl = String.Format("javascript:CopySelectedSearchResults('{0}');", 
                                                                    queryId.ToString());
                }
            }

            SetFormDisplayMode(IsNew);
        }

        protected void ExecuteSearchQuery(string searchQueryText)
        {
            Logging.Logger.Instance.Info("Executing Search Query");
            //TODO: Refactor this to allow reuse with the btnSave code below
            SPServiceContext serviceContext = SPServiceContext.Current;
            SPServiceApplicationProxy proxy = serviceContext.GetDefaultProxy(typeof(SearchServiceApplicationProxy));
            SearchServiceApplicationProxy searchAppProxy = proxy as SearchServiceApplicationProxy;
            string scopeName = String.Empty;
            if (SPContext.Current.Site.RootWeb.AllProperties.Contains(SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_SCOPE))
            {
                scopeName = SPContext.Current.Site.RootWeb.AllProperties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_SCOPE] as string;
            }
            
            Logging.Logger.Instance.Info(String.Format("Scope retrieved from property bag setting. Scope: {0}; Setting Name: {1} ; Site: {2}",
                scopeName,
                SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_SCOPE,
                SPContext.Current.Site.RootWeb.Url), Logging.DiagnosticsCategories.eCaseSearch);


            if (!String.IsNullOrEmpty(searchQueryText))
            {
                using (KeywordQuery query = new KeywordQuery(SPContext.Current.Site))
                {
                    int rowsPerSet = 50;
                    query.QueryText = queryLabel.Text;
                    query.ResultsProvider = SearchProvider.Default;
                    query.ResultTypes = ResultType.RelevantResults;
                    query.RowLimit = rowsPerSet;
                    query.TrimDuplicates = false;
                    query.EnableStemming = true;
                    if (!String.IsNullOrEmpty(scopeName))
                    {
                        query.HiddenConstraints = "scope:\"" + scopeName + "\"";
                    }
                    ResultTableCollection resultsTableCollection = query.Execute();
                    rowCountSpan.InnerText = resultsTableCollection[ResultType.RelevantResults].TotalRows.ToString();
                }
            }
        }

        protected void PopulateSavedSearchResultsSetsInfo()
        {
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            using (System.Data.DataTable savedSearches = new System.Data.DataTable())
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    Logging.Logger.Instance.Info(String.Format("Opening database connection to: {0}", connectionString));
                    conn.Open();
                    //Insert the Saved Search Results Parent Entry
                    using (System.Data.SqlClient.SqlCommand selectCommand = new System.Data.SqlClient.SqlCommand())
                    {
                        selectCommand.Connection = conn;
                        //selectCommand.CommandText = "SET NOCOUNT ON; SELECT Id, Name, Description FROM SavedSearchResults WHERE Owner = @Owner;";
                        selectCommand.CommandType = CommandType.StoredProcedure;
                        selectCommand.CommandText = "GetSavedSearchResultSetsById";
                        selectCommand.Parameters.AddWithValue("@Id", Request.QueryString["id"]);

                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter())
                        {
                            da.SelectCommand = selectCommand;
                            da.Fill(savedSearches);
                        }
                    }
                    using (System.Data.SqlClient.SqlCommand permissionsSelectCommand = new System.Data.SqlClient.SqlCommand())
                    {
                        using (DataTable permsTable = new DataTable())
                        {
                            permissionsSelectCommand.Connection = conn;
                            permissionsSelectCommand.CommandType = CommandType.Text;
                            permissionsSelectCommand.CommandText = @"
                                                                    SELECT SavedSearchResultsId, PermissionName FROM SavedSearchResultPermissions
                                                                    WHERE SavedSearchResultsId = @SavedSearchResultsId
                                                                 ";
                            permissionsSelectCommand.Parameters.AddWithValue("@SavedSearchResultsId", Request.QueryString["id"]);
                            permsTable.Load(permissionsSelectCommand.ExecuteReader());

                            System.Text.StringBuilder permissionsString = new System.Text.StringBuilder();
                            foreach (DataRow row in permsTable.Rows)
                            {
                                permissionsString.Append(row["PermissionName"]).Append(",");
                            }
                            shareWithPeopleEditor.CommaSeparatedAccounts = permissionsString.ToString();
                        }
                    }
                }

                savedSearchNameTextBox.Text = savedSearches.Rows[0]["Name"].ToString();
                savedSearchDescriptionTextBox.Text = savedSearches.Rows[0]["Description"].ToString();
                queryLabel.Text = savedSearches.Rows[0]["OriginalQuery"].ToString();
                ownerPeopleEditor.CommaSeparatedAccounts = savedSearches.Rows[0]["Owner"].ToString();

                SetFormDisplayMode(IsNew);
            }
        }

        protected void BindSearchResultsDataTableById(long queryId)
        {
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];

            using (DataTable savedSearchResults = new DataTable())
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    Logging.Logger.Instance.Info(String.Format("Retrieving Saved Search Results:{0} for Owner: {1}", savedSearchNameTextBox.Text, connectionString));
                    conn.Open();
                    //Insert the Saved Search Results Parent Entry
                    using (System.Data.SqlClient.SqlCommand selectCommand = new System.Data.SqlClient.SqlCommand())
                    {
                        selectCommand.Connection = conn;
                        selectCommand.CommandType = CommandType.StoredProcedure;
                        selectCommand.CommandText = "GetSavedSearchResultsById";
                        selectCommand.Parameters.AddWithValue("@Id", queryId);

                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            da.SelectCommand = selectCommand;
                            da.Fill(savedSearchResults);

                            savedSearchResultsRepeater.DataSource = savedSearchResults;
                            savedSearchResultsRepeater.DataBind();

                            rowCountSpan.InnerText = savedSearchResults.Rows.Count.ToString();
                        }
                    }
                }
            }
        }

        protected void retrieveSavedSearchButton_Click(object sender, EventArgs e)
        {
            if (savedSearchesDropDown.SelectedIndex > 0)
            {
                long queryId;
                if (long.TryParse(savedSearchesDropDown.SelectedValue, out queryId))
                {
                    BindSearchResultsDataTableById(queryId);
                }
            }
        }

        protected void savedSearchResultsRepeater_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                System.Data.DataRowView currentRow = e.Item.DataItem as System.Data.DataRowView;
                string itemLinkValue = currentRow["HitHighlightedPropertiesXml"] as string;

                System.Xml.Linq.XElement xEl = System.Xml.Linq.XElement.Parse(itemLinkValue);
                System.Xml.Linq.XElement hhTitleElement =
                    (from node in xEl.Elements()
                     where node.Name == "HHTitle"
                     select node).FirstOrDefault();

                System.Xml.Linq.XElement hhUrlElement =
                    (from node in xEl.Elements()
                     where node.Name == "HHUrl"
                     select node).FirstOrDefault();

                string hhTitle = hhTitleElement != null ? hhTitleElement.Value : string.Empty;
                string hhUrl = hhUrlElement != null ? hhUrlElement.Value : string.Empty; ;
                string iconUrl = Microsoft.SharePoint.Publishing.Fields.LinkFieldValue.GetDefaultIconUrl(currentRow["Url"] as string, SPContext.Current.Web);

                System.Web.UI.WebControls.Image docTypeImage = e.Item.FindControl("docTypeImage") as System.Web.UI.WebControls.Image;
                if (docTypeImage != null)
                {
                    docTypeImage.ImageUrl = iconUrl;
                }

                System.Web.UI.WebControls.HyperLink titleLink = e.Item.FindControl("titleLink") as System.Web.UI.WebControls.HyperLink;
                if (titleLink != null)
                {
                    titleLink.NavigateUrl = hhUrl;
                    titleLink.Text = hhTitle;
                }

                System.Web.UI.WebControls.CheckBox reviewedCheckBox = e.Item.FindControl("reviewedCheckBox") as System.Web.UI.WebControls.CheckBox;
                if (reviewedCheckBox != null)
                {
                    reviewedCheckBox.InputAttributes.Add("onclick", 
                        String.Format("SaveSearchResultData({0}, {1}, {2});", 
                        currentRow["Id"], 
                        "'reviewed'", 
                        "$('#" + reviewedCheckBox.ClientID +"').prop('checked')" )
                        );
                    reviewedCheckBox.Checked = (currentRow.Row.IsNull("Reviewed")) ? false: (bool)currentRow["Reviewed"];
                }

                System.Web.UI.WebControls.CheckBox includeInSetCheckBox = e.Item.FindControl("includeInSetCheckBox") as System.Web.UI.WebControls.CheckBox;
                if (includeInSetCheckBox != null)
                {
                    includeInSetCheckBox.InputAttributes.Add("onclick",
                        String.Format("SaveSearchResultData({0}, {1}, {2});",
                        currentRow["Id"],
                        "'IncludeInSet'",
                        "$('#" + includeInSetCheckBox.ClientID + "').prop('checked')")
                        );
                    includeInSetCheckBox.Checked = (currentRow.Row.IsNull("IncludeInSet")) ? false : (bool)currentRow["IncludeInSet"];
                }        
            }
        }

        protected void returnToResultSetsLink_Click(object sender, EventArgs e)
        {
            Response.Redirect(
                Microsoft.SharePoint.Utilities.SPUrlUtility.CombineUrl(
                    SPContext.Current.Web.Url,
                    "_layouts/ecasesearch/SaveSearchResultSets.aspx"));
        }

        protected void returnToSearch_Click(object sender, EventArgs e)
        {
            Response.Redirect(
                Microsoft.SharePoint.Utilities.SPUrlUtility.CombineUrl(
                    SPContext.Current.Site.Url,
                    "search"));
        }

        [System.Web.Services.WebMethod(EnableSession = true)]
        public static string SendSelectedItemsToSession(string resultSetId)
        {
            //Get the selected results for the current result set
            Logging.Logger.Instance.Info("Sending Selected Items Data to Session");
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            long count = 0;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string selectCommandText = @"SELECT Url 
                                                FROM SavedSearchResultItems
                                                WHERE
                                                    SavedSearchResultId = @Id AND
                                                    IncludeInSet = 1 AND
                                                    IsDocument = 1";

                using (SqlCommand cmd = new SqlCommand(selectCommandText, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", resultSetId);
                    conn.Open();
                    List<string> itemUrls = new List<string>();
                    string itemUrlsJoined;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            itemUrls.Add(dr.GetString(0));
                        }

                        itemUrlsJoined = string.Join("|", itemUrls.ToArray());
                        count = itemUrls.LongCount();

                        //Place that list of results in Session
                        System.Web.HttpContext.Current.Session[SusDeb.DOI.Common.Utilities.eCaseConstants.SessionKeys.BATCH_COPY_ITEMS_SESSION_KEY_NAME] = itemUrlsJoined;
                    }
                }
            }

            //Let the client know the work is complete/successful so that it can direct the user to the next page...
            return count.ToString();
        }

        protected void saveSearchResultsButton_Click(object sender, EventArgs e)
        {
            Logging.Logger.Instance.Info("Begin Saving Search Results");

            string scopeName = String.Empty;
            if (SPContext.Current.Site.RootWeb.AllProperties.Contains(SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_SCOPE))
            {
                scopeName = SPContext.Current.Site.RootWeb.AllProperties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_SCOPE] as string;
            }
            Logging.Logger.Instance.Info(String.Format("Scope retrieved from property bag setting. Scope: {0}; Setting Name: {1} ; Site: {2}",
                scopeName,
                SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_SCOPE,
                SPContext.Current.Site.RootWeb.Url), Logging.DiagnosticsCategories.eCaseSearch);

            SPServiceContext serviceContext = SPServiceContext.Current;
            SPServiceApplicationProxy proxy = serviceContext.GetDefaultProxy(typeof(SearchServiceApplicationProxy));
            SearchServiceApplicationProxy searchAppProxy = proxy as SearchServiceApplicationProxy;

            if (!String.IsNullOrEmpty(savedSearchNameTextBox.Text)) //&& !String.IsNullOrEmpty(Request.QueryString["query"]))
            {
                using (KeywordQuery query = new KeywordQuery(SPContext.Current.Site))
                {
                    int rowsPerSet = 50;
                    query.QueryText = queryLabel.Text;
                    query.ResultsProvider = SearchProvider.Default;
                    query.ResultTypes = ResultType.RelevantResults;
                    query.TrimDuplicates = false;
                    query.EnableStemming = true;
                    if (!String.IsNullOrEmpty(scopeName))
                    {
                        Logging.Logger.Instance.Info(String.Format("Adding scope to hidden constraints: {0}", scopeName), Logging.DiagnosticsCategories.eCaseSearch);
                        query.HiddenConstraints = "scope:\"" + scopeName + "\"";
                    }
                    query.RowLimit = rowsPerSet;

                    ResultTableCollection resultsTableCollection = query.Execute();

                    if (resultsTableCollection.Count > 0)
                    {
                        //save search result entry
                        string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
                        Int64 searchRowIdentity = 0;
                        using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                        {
                            Logging.Logger.Instance.Info(String.Format("Opening database connection to: {0}", connectionString));
                            conn.Open();
                            using (System.Data.SqlClient.SqlTransaction trans = conn.BeginTransaction())
                            {
                                //Insert the Saved Search Results Parent Entry
                                using (System.Data.SqlClient.SqlCommand parentInsertCommand = new System.Data.SqlClient.SqlCommand())
                                {
                                    parentInsertCommand.Connection = conn;
                                    parentInsertCommand.Transaction = trans;
                                    parentInsertCommand.CommandType = CommandType.StoredProcedure;
                                    parentInsertCommand.CommandText = "CreateSavedSearchResult";

                                    if (!IsNew)
                                    {
                                        parentInsertCommand.Parameters.AddWithValue("@Id", Request.QueryString["id"]);
                                    }
                                    parentInsertCommand.Parameters.AddWithValue("@Name", savedSearchNameTextBox.Text);
                                    parentInsertCommand.Parameters.AddWithValue("@Description", savedSearchDescriptionTextBox.Text);
                                    parentInsertCommand.Parameters.AddWithValue("@Query", queryLabel.Text);
                                    parentInsertCommand.Parameters.AddWithValue("@Owner", SPContext.Current.Web.CurrentUser.LoginName);
                                    searchRowIdentity = (Int64)parentInsertCommand.ExecuteScalar();
                                }

                                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter())
                                {
                                    if (IsNew) //skip updating the results if this isn't a new result set to save time and effort
                                    {
                                        string maxResultsString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_SAVED_SEARCH_RESULTS_MAX_RESULTS];
                                        int maxResults;
                                        if (!int.TryParse(maxResultsString, out maxResults))
                                        {
                                            int.TryParse(SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagDefaultValues.ECASE_SAVED_SEARCH_RESULTS_MAX_RESULTS, out maxResults);
                                        }
                                        Logging.Logger.Instance.Info(String.Format("Results limit: {0}", maxResults), Logging.DiagnosticsCategories.eCaseSearch);


                                        ResultTable results = resultsTableCollection[ResultType.RelevantResults];
                                        int startRow = 0;
                                        int rowsFound = 0;
                                        int lastRow = startRow + results.RowCount;

                                        using (System.Data.SqlClient.SqlCommand childInsertCommand = new System.Data.SqlClient.SqlCommand())
                                        {
                                            childInsertCommand.Connection = conn;
                                            childInsertCommand.Transaction = trans;
                                            childInsertCommand.CommandText = @"INSERT INTO SavedSearchResultItems
                                                                    (
                                                                        SavedSearchResultId, WorkId, Rank, Author, Size, Path, Description,
                                                                        SiteName, HitHighlightedSummary, HitHighlightedProperties, ContentClass,
                                                                        IsDocument, PictureThumbnailUrl, Url, ServerRedirectedUrl, FileExtension, SpSiteUrl,
                                                                        docvector, fcocount, fcoid, PictureWidth, PictureHeight
                                                                    )
                                                                    VALUES
                                                                    (
                                                                        @SavedSearchResultId, @WorkId, @Rank, @Author, @Size, @Path, @Description,
                                                                        @SiteName, @HitHighlightedSummary, @HitHighlightedProperties, @ContentClass,
                                                                        @IsDocument, @PictureThumbnailUrl, @Url, @ServerRedirectedUrl, @FileExtension, @SpSiteUrl,
                                                                        @docvector, @fcocount, @fcoid, @PictureWidth, @PictureHeight
                                                                    )";
                                            childInsertCommand.Parameters.Add("@SavedSearchResultId", System.Data.SqlDbType.BigInt);
                                            childInsertCommand.Parameters["@SavedSearchResultId"].Value = searchRowIdentity;
                                            childInsertCommand.Parameters.Add("@WorkId", System.Data.SqlDbType.NVarChar, 50, "WorkId");
                                            childInsertCommand.Parameters.Add("@Rank", System.Data.SqlDbType.Int, 0, "Rank");
                                            childInsertCommand.Parameters.Add("@Author", System.Data.SqlDbType.NVarChar, 50, "Author");
                                            childInsertCommand.Parameters.Add("@Size", System.Data.SqlDbType.Int, 50, "Size");
                                            childInsertCommand.Parameters.Add("@Path", System.Data.SqlDbType.NVarChar, 500, "Path");
                                            childInsertCommand.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar, 500000, "Description");
                                            childInsertCommand.Parameters.Add("@SiteName", System.Data.SqlDbType.NVarChar, 500, "SiteName");
                                            childInsertCommand.Parameters.Add("@HitHighlightedSummary", System.Data.SqlDbType.NVarChar, 500000, "HitHighlightedSummary");
                                            childInsertCommand.Parameters.Add("@HitHighlightedProperties", System.Data.SqlDbType.NVarChar, 500000, "HitHighlightedProperties");
                                            childInsertCommand.Parameters.Add("@ContentClass", System.Data.SqlDbType.NVarChar, 50, "ContentClass");
                                            childInsertCommand.Parameters.Add("@IsDocument", System.Data.SqlDbType.Bit, 0, "IsDocument");
                                            childInsertCommand.Parameters.Add("@PictureThumbnailUrl", System.Data.SqlDbType.NVarChar, 500, "PictureThumbnailUrl");
                                            childInsertCommand.Parameters.Add("@Url", System.Data.SqlDbType.NVarChar, 500, "Url");
                                            childInsertCommand.Parameters.Add("@ServerRedirectedUrl", System.Data.SqlDbType.NVarChar, 500, "ServerRedirectedUrl");
                                            childInsertCommand.Parameters.Add("@FileExtension", System.Data.SqlDbType.NVarChar, 500, "FileExtension");
                                            childInsertCommand.Parameters.Add("@SpSiteUrl", System.Data.SqlDbType.NVarChar, 500, "SpSiteUrl");
                                            childInsertCommand.Parameters.Add("@docvector", System.Data.SqlDbType.NVarChar, 500, "docvector");
                                            childInsertCommand.Parameters.Add("@fcocount", System.Data.SqlDbType.Int, 0, "fcocount");
                                            childInsertCommand.Parameters.Add("@fcoid", System.Data.SqlDbType.NVarChar, 50, "fcoid");
                                            childInsertCommand.Parameters.Add("@PictureWidth", System.Data.SqlDbType.Int, 0, "PictureWidth");
                                            childInsertCommand.Parameters.Add("@PictureHeight", System.Data.SqlDbType.Int, 0, "PictureHeight");
                                            da.InsertCommand = childInsertCommand;

                                            //if we've found a number of rows <= the total rows in the result set AND
                                            //the current result set contains > 0 results (there are still new results being found) AND
                                            //we've found <= the maximum number of rows we're allowing to be saved
                                            while (rowsFound <= results.TotalRows && results.RowCount > 0 && rowsFound <= maxResults)
                                            {
                                                da.Update(results.Table);

                                                //set the start row = the last row we found
                                                query.StartRow = lastRow;
                                                //increment the last row we found by the number of results we retrieved
                                                lastRow += results.RowCount;
                                                rowsFound += results.RowCount;

                                                Logging.Logger.Instance.Info(String.Format("Results Found: {0}; Last Result Found: {1}", rowsFound, query.StartRow), Logging.DiagnosticsCategories.eCaseSearch);

                                                resultsTableCollection = query.Execute();
                                                results = resultsTableCollection[ResultType.RelevantResults];
                                            }
                                        }
                                    }

                                    using (System.Data.SqlClient.SqlDataAdapter permsAdapter = new System.Data.SqlClient.SqlDataAdapter())
                                    {
                                        //for permissions, always remove all items and then add them back
                                        using (System.Data.SqlClient.SqlCommand permsDeleteCommand = new System.Data.SqlClient.SqlCommand())
                                        {
                                            permsDeleteCommand.Connection = conn;
                                            permsDeleteCommand.Transaction = trans;
                                            permsDeleteCommand.CommandText = @"DELETE FROM SavedSearchResultPermissions
                                                                               WHERE SavedSearchResultsId = @SavedSearchResultsId
                                                                                ";
                                            permsDeleteCommand.Parameters.Add("@SavedSearchResultsId", System.Data.SqlDbType.BigInt);
                                            permsDeleteCommand.Parameters["@SavedSearchResultsId"].Value = searchRowIdentity;
                                            permsDeleteCommand.ExecuteNonQuery();
                                        }

                                        using (System.Data.SqlClient.SqlCommand permsInsertCommand = new System.Data.SqlClient.SqlCommand())
                                        {
                                            permsInsertCommand.Connection = conn;
                                            permsInsertCommand.Transaction = trans;
                                            permsInsertCommand.CommandText = @"INSERT INTO SavedSearchResultPermissions
                                                                    (
                                                                        SavedSearchResultsId, PermissionName
                                                                    )
                                                                    VALUES
                                                                    (
                                                                        @SavedSearchResultsId, @PermissionName
                                                                    )";
                                            permsInsertCommand.Parameters.Add("@SavedSearchResultsId", System.Data.SqlDbType.BigInt);
                                            permsInsertCommand.Parameters["@SavedSearchResultsId"].Value = searchRowIdentity;
                                            permsInsertCommand.Parameters.Add("@PermissionName", System.Data.SqlDbType.NVarChar, 100, "PermissionName");
                                            foreach (object account in shareWithPeopleEditor.Accounts)
                                            {
                                                permsInsertCommand.Parameters["@PermissionName"].Value = account.ToString();
                                                permsInsertCommand.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                trans.Commit();
                                Microsoft.SharePoint.Utilities.SPUtility.Redirect(
                                            Microsoft.SharePoint.Utilities.SPUtility.GetPageUrlPath(Context) +
                                            String.Format("?id={0}", searchRowIdentity),
                                            Microsoft.SharePoint.Utilities.SPRedirectFlags.Default, Context
                                    );
                            }
                        }
                    }
                }
            }

            PopulateSavedSearchResultsSetsInfo();
            SetFormDisplayMode(false);

        }

        protected void editSearchResultsButton_Click(object sender, EventArgs e)
        {
            //put the form in edit mode
            PopulateSavedSearchResultsSetsInfo();
            SetFormDisplayMode(true);
        }

        protected void cancelEditButton_Click(object sender, EventArgs e)
        {
            if (IsNew == true)
            {
                if (ViewState["PreviousPage"] != null)	//Check if the ViewState contains Previous page URL
                {
                    Response.Redirect(ViewState["PreviousPage"].ToString()); //Redirect to previous page by retrieving the PreviousPage Url from ViewState.
                }
            }
            else
            {
                //put the form back in display mode
                PopulateSavedSearchResultsSetsInfo();
                SetFormDisplayMode(false);
            }
        }

        private void SetFormDisplayMode(bool isEditable)
        {
            savedSearchNameTextBox.Enabled = isEditable;
            savedSearchDescriptionTextBox.Enabled = isEditable;
            ownerPeopleEditor.Enabled = isEditable;
            shareWithPeopleEditor.Enabled = isEditable;
            cancelEditButton.Visible = isEditable;
            editSearchResultsButton.Visible = !isEditable;
            saveSearchResultsButton.Visible = isEditable;
        }
    }
}