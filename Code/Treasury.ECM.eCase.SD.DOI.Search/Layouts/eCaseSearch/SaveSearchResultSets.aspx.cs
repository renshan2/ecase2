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
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch
{
    public partial class SaveSearchResultSets : LayoutsPageBase
    {
        private const string DATA_SOURCE_VIEWSTATE_LOCATION = "SavedResultSetsDataSource";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateSavedSearchResultSetsGrid();
            }
        }

        protected void PopulateSavedSearchResultSetsGrid()
        {
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            using (System.Data.DataTable savedSearches = new System.Data.DataTable())
            {
                using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    Logging.Logger.Instance.Info(String.Format("Populating Saved Search Result Set. Opening database connection to: {0}", connectionString));
                    conn.Open();
                    //Insert the Saved Search Results Parent Entry
                    using (System.Data.SqlClient.SqlCommand selectCommand = new System.Data.SqlClient.SqlCommand())
                    {
                        selectCommand.Connection = conn;
                        //selectCommand.CommandText = "SET NOCOUNT ON; SELECT Id, Name, Description, OriginalQuery, Owner FROM SavedSearchResults WHERE Owner = @Owner;";
                        selectCommand.CommandType = CommandType.StoredProcedure;
                        selectCommand.CommandText = "GetSavedSearchResultSetsByUser";
                        selectCommand.Parameters.AddWithValue("@User", SPContext.Current.Web.CurrentUser.LoginName);

                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter())
                        {
                            da.SelectCommand = selectCommand;
                            da.Fill(savedSearches);
                        }
                    }
                }

                ViewState[DATA_SOURCE_VIEWSTATE_LOCATION] = savedSearches;
                
                savedSearchResultSetGrid.EnableViewState = true;
                savedSearchResultSetGrid.AllowPaging = true;
                savedSearchResultSetGrid.PageSize = 20;
                BindSavedSearchResultSetGrid();
            }
        }

        protected void BindSavedSearchResultSetGrid()
        {
            savedSearchResultSetGrid.DataSource = ViewState[DATA_SOURCE_VIEWSTATE_LOCATION];
            savedSearchResultSetGrid.DataBind();
        }

        protected void savedSearchResultSetGrid_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            Logging.Logger.Instance.Info(String.Format("Row Editing"));
            savedSearchResultSetGrid.EditIndex = e.NewEditIndex;
            BindSavedSearchResultSetGrid();
        }

        protected void savedSearchResultSetGrid_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            Logging.Logger.Instance.Info(String.Format("Row Deleting"));

            DataControlFieldCell cell;
            int tableCellsCount = savedSearchResultSetGrid.Rows[e.RowIndex].Cells.Count;

            for (int i = 0; i < tableCellsCount; i++)
            {
                cell = savedSearchResultSetGrid.Rows[e.RowIndex].Cells[i] as DataControlFieldCell;
                cell.ContainingField.ExtractValuesFromCell(
                e.Values,
                cell,
                DataControlRowState.Edit,
                true);
            }
            
            object idObject = e.Values["Id"];
            long idLong;
            bool parsed = long.TryParse(idObject.ToString(), out idLong);

            DeleteSearchResultSet(idLong);
            PopulateSavedSearchResultSetsGrid();
        }

        protected void DeleteSearchResultSet(long id)
        {
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            using (System.Data.SqlClient.SqlConnection conn = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                Logging.Logger.Instance.Info(String.Format("Deleting Saved Search Result Set. Opening database connection to: {0}", connectionString));
                conn.Open();

                //Delete the Saved Search Results Parent Entry
                using (SqlCommand deleteCommand = new SqlCommand())
                {
                    deleteCommand.Connection = conn;
                    deleteCommand.CommandType = CommandType.StoredProcedure;
                    deleteCommand.CommandText = "DeleteSavedSearchResultsById";
                    //deleteParentCommand.Transaction = trans;
                    //deleteParentCommand.CommandText = "SET NOCOUNT ON; DELETE FROM SavedSearchResults WHERE Id = @Id;";
                    deleteCommand.Parameters.AddWithValue("@Id", id);
                    deleteCommand.ExecuteNonQuery();
                }

            }
        }

        protected void UpdateSearchResultSet(System.Collections.Specialized.IOrderedDictionary newValues)
        {
            Logging.Logger.Instance.Info(String.Format("Updating Result Set"));
            string connectionString = SPContext.Current.Site.RootWeb.Properties[SusDeb.DOI.Common.Utilities.eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
            long searchRowIdentity;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                Logging.Logger.Instance.Info(String.Format("Updating Saved Search Result Set. Opening database connection to: {0}", connectionString));
                conn.Open();
                using (System.Data.SqlClient.SqlTransaction trans = conn.BeginTransaction())
                {
                    //Insert the Saved Search Results Parent Entry
                    using (SqlCommand updateCommand = new System.Data.SqlClient.SqlCommand())
                    {
                        updateCommand.Connection = conn;
                        updateCommand.Transaction = trans;
                        updateCommand.CommandType = CommandType.StoredProcedure;
                        updateCommand.CommandText = "CreateSavedSearchResult";
                        updateCommand.Parameters.AddWithValue("@Id", newValues["Id"]);
                        updateCommand.Parameters.AddWithValue("@Name", newValues["Name"]);
                        updateCommand.Parameters.AddWithValue("@Description", newValues["Description"]);
                        updateCommand.Parameters.AddWithValue("@Query", newValues["OriginalQuery"]);
                        updateCommand.Parameters.AddWithValue("@Owner", newValues["Owner"]);

                        searchRowIdentity = (Int64)updateCommand.ExecuteScalar();
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

                            string[] shareWithAccounts =  newValues["ShareWith"].ToString().Split(',');
                            foreach (string account in shareWithAccounts)
                            {
                                permsInsertCommand.Parameters["@PermissionName"].Value = account.ToString();
                                permsInsertCommand.ExecuteNonQuery();
                            }
                        }
                    }
                    trans.Commit();
                }
            }
        }

        protected void savedSearchResultSetGrid_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            Logging.Logger.Instance.Info(String.Format("Row Command"));
            if (e != null)
            {
                if (e.CommandName == "ShowResults")
                {
                    Response.Redirect(Microsoft.SharePoint.Utilities.SPUrlUtility.CombineUrl(SPContext.Current.Web.Url, String.Format("_layouts/ecasesearch/SaveSearchResults.aspx?id={0}", e.CommandArgument)));
                }
            }
        }

        protected void savedSearchResultSetGrid_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            Logging.Logger.Instance.Info(String.Format("Row Updating"));

            DataControlFieldCell cell;
            int tableCellsCount = savedSearchResultSetGrid.Rows[e.RowIndex].Cells.Count;

            for (int i = 0; i < tableCellsCount; i++)
            {
                cell = savedSearchResultSetGrid.Rows[e.RowIndex].Cells[i] as DataControlFieldCell;
                cell.ContainingField.ExtractValuesFromCell(
                e.Keys,
                cell,
                DataControlRowState.Normal,
                true);
            }

            for (int i = 0; i < tableCellsCount; i++)
            {
                cell = savedSearchResultSetGrid.Rows[e.RowIndex].Cells[i] as DataControlFieldCell;
                cell.ContainingField.ExtractValuesFromCell(
                e.NewValues,
                cell,
                DataControlRowState.Edit,
                true);
            }
            UpdateSearchResultSet(e.NewValues);

            savedSearchResultSetGrid.EditIndex = -1;
            PopulateSavedSearchResultSetsGrid();
        }

        protected void savedSearchResultSetGrid_RowCancelingEdit(object sender, System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            Logging.Logger.Instance.Info(String.Format("Row Editing Canceled"));
            ((SPGridView)sender).EditIndex = -1;
            BindSavedSearchResultSetGrid();
        }
    }
}