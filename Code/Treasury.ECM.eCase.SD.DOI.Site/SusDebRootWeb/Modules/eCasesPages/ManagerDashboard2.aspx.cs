using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.DataSetExtensions;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.eCaseRootWeb.Modules.eCasesPages
{
    public partial class ManagerDashboard2 : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Find ContentPlaceHolder on page
                ContentPlaceHolder PlaceHolderAdditionalPageHead = (ContentPlaceHolder)Page.Master.FindControl("PlaceHolderAdditionalPageHead");

                // Find Data Array Literal controls on page
                Literal litPie1DataArray = (Literal)PlaceHolderAdditionalPageHead.FindControl("litPie1DataArray");
                Literal litPie2DataArray = (Literal)PlaceHolderAdditionalPageHead.FindControl("litPie2DataArray");
                Literal litPie3DataArray = (Literal)PlaceHolderAdditionalPageHead.FindControl("litPie3DataArray");
                Literal litPie4DataArray = (Literal)PlaceHolderAdditionalPageHead.FindControl("litPie4DataArray");

                // If Data Array Literal controls exist, populate them with data from the Cases list
                if ((litPie1DataArray != null) && (litPie2DataArray != null) && (litPie3DataArray != null) && (litPie4DataArray != null))
                {
                    PopulatePieDataArrays(litPie1DataArray, litPie2DataArray, litPie4DataArray);
                    PopulateIssueListPieDataArray(litPie3DataArray);
                }
            }
        }

        /// <summary>
        /// Populate Data Array Literal controls with data from the Cases list
        /// </summary>
        private void PopulatePieDataArrays(Literal litPie1DataArray, Literal litPie2DataArray, Literal litPie4DataArray)
        {      
            SPList listCases = GetList(SPContext.Current.Web, "Cases");
            if (listCases != null)
            {
                SPQuery queryCases = new SPQuery
                {
                    Query = "<OrderBy><FieldRef Name='UniqueID' /></OrderBy>"
                };
                DataTable dt = listCases.GetItems(queryCases).GetDataTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    StringBuilder sb2 = new StringBuilder();

                    DataTable pie1GroupedTable = GroupBy("AssignedTo", "UniqueCaseID", dt);                    
                    DataTable pie2GroupedTable = GroupBy("CaseStep", "UniqueCaseID", dt);
                    DataTable pie4GroupedTable = GroupBy("BureauIG", "UniqueCaseID", dt);
                    
                    if (pie1GroupedTable != null && pie1GroupedTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in pie1GroupedTable.Rows)
                        {
                            string strInvestigator = row["AssignedTo"].ToString();
                            string strCount = row["Count"].ToString();
                            sb2.Append("['" + strInvestigator + "', " + strCount + "]");
                            sb2.Append(",");                            
                        }
                    }
                    if (pie1GroupedTable.Rows.Count > 0)
                    {
                        sb.Append("pie1Data = [");
                        sb.Append(Environment.NewLine);
                        sb2.Length = sb2.Length - 1;
                        sb.Append(sb2.ToString());
                        sb.Append(Environment.NewLine);
                        sb.Append("];");
                    }
                    litPie1DataArray.Text = sb.ToString();
                    // Clear the StringBuilders for reuse
                    sb.Length = 0;
                    sb.Capacity = 16;
                    sb2.Length = 0;
                    sb2.Capacity = 16;

                    if (pie2GroupedTable != null && pie2GroupedTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in pie2GroupedTable.Rows)
                        {
                            string strInvestigator = row["CaseStep"].ToString();
                            string strCount = row["Count"].ToString();
                            sb2.Append("['" + strInvestigator + "', " + strCount + "]");
                            sb2.Append(",");
                        }
                    }
                    if (pie2GroupedTable.Rows.Count > 0)
                    {
                        sb.Append("pie2Data = [");
                        sb.Append(Environment.NewLine);
                        sb2.Length = sb2.Length - 1;
                        sb.Append(sb2.ToString());
                        sb.Append(Environment.NewLine);
                        sb.Append("];");
                    }
                    litPie2DataArray.Text = sb.ToString();
                    // Clear the StringBuilders for reuse
                    sb.Length = 0;
                    sb.Capacity = 16;
                    sb2.Length = 0;
                    sb2.Capacity = 16;

                    if (pie4GroupedTable != null && pie4GroupedTable.Rows.Count > 0)
                    {
                        foreach (DataRow row in pie4GroupedTable.Rows)
                        {
                            string strInvestigator = row["BureauIG"].ToString();
                            string strCount = row["Count"].ToString();
                            sb2.Append("['" + strInvestigator + "', " + strCount + "]");
                            sb2.Append(",");
                        }
                    }
                    if (pie4GroupedTable.Rows.Count > 0)
                    {
                        sb.Append("pie4Data = [");
                        sb.Append(Environment.NewLine);
                        sb2.Length = sb2.Length - 1;
                        sb.Append(sb2.ToString());
                        sb.Append(Environment.NewLine);
                        sb.Append("];");
                    }
                    litPie4DataArray.Text = sb.ToString();
                    // Clear the StringBuilders for reuse
                    sb.Length = 0;
                    sb.Capacity = 16;
                    sb2.Length = 0;
                    sb2.Capacity = 16;                                        
                }
            }
        }

        /// <summary>
        /// Populate Issue List Data Array Literal control with data from the Cases list
        /// </summary>
        private void PopulateIssueListPieDataArray(Literal litPie3DataArray)
        {
            String strArray = "";
            StringBuilder sb = new StringBuilder();

            SPList listCases = GetList(SPContext.Current.Web, "Cases");
            if (listCases != null)
            {
                SPQuery queryCases1 = new SPQuery
                {
                    Query = "<Where><Contains><FieldRef Name='LawIssueList' /><Value Type='Text'>19.800</Value></Contains></Where>"
                };
                SPListItemCollection listCaseItems1 = listCases.GetItems(queryCases1);

                SPQuery queryCases2 = new SPQuery
                {
                    Query = "<Where><Contains><FieldRef Name='LawIssueList' /><Value Type='Text'>9.406</Value></Contains></Where>"
                };
                SPListItemCollection listCaseItems2 = listCases.GetItems(queryCases2);

                SPQuery queryCases3 = new SPQuery
                {
                    Query = "<Where><Contains><FieldRef Name='LawIssueList' /><Value Type='Text'>9.407</Value></Contains></Where>"
                };
                SPListItemCollection listCaseItems3 = listCases.GetItems(queryCases3);

                SPQuery queryCases4 = new SPQuery
                {
                    Query = "<Where><IsNull><FieldRef Name='LawIssueList' /></IsNull></Where>"
                };
                SPListItemCollection listCaseItems4 = listCases.GetItems(queryCases4);

                sb.Append("pie3Data = [");
                sb.Append(Environment.NewLine);
                bool boolResultsFound = false;
                if (listCaseItems1 != null && listCaseItems1.Count > 0)
                {
                    sb.Append("['19.800 Causes for debarment', " + listCaseItems1.Count + "],");
                    boolResultsFound = true;
                }
                if (listCaseItems2 != null && listCaseItems2.Count > 0)
                {
                    sb.Append("['9.406-2 Causes for debarment', " + listCaseItems2.Count + "],");
                    boolResultsFound = true;
                }
                if (listCaseItems3 != null && listCaseItems3.Count > 0)
                {
                    sb.Append("['9.407-2 Causes for suspension', " + listCaseItems3.Count + "],");
                    boolResultsFound = true;
                }
                if (listCaseItems4 != null && listCaseItems4.Count > 0)
                {
                    sb.Append("['(Not specified)', " + listCaseItems4.Count + "],");
                    boolResultsFound = true;
                }
                if (boolResultsFound == true)
                {
                    sb.Length = sb.Length - 1;
                }
                sb.Append(Environment.NewLine);
                sb.Append("];");                
            }

            strArray = sb.ToString();
            litPie3DataArray.Text = strArray;                 
        }

        /// <summary>
        /// Looks for a list by display name
        /// </summary>
        private static SPList GetList(SPWeb web, string listName)
        {
            SPList returnList = null;

            try
            {

                int listCount = web.Lists.Count;
                for (int i = 0; i < listCount; i++)
                {
                    if (web.Lists[i].Title.Equals(listName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        returnList = web.Lists[i];
                        break;
                    }
                }
                if (returnList != null)
                {
                    return returnList;
                }
            }
            catch
            {
                Logger.Instance.Error(string.Format("Exception looking for list: {0}", listName), DiagnosticsCategories.eCaseSite);
                returnList = null;
            }
            return returnList;
        }

        /// <summary>
        /// Get Grouped Table from DataTable
        /// </summary>
        private DataTable GroupBy(string i_sGroupByColumn, string i_sAggregateColumn, DataTable i_dSourceTable)
        {

            DataView dv = new DataView(i_dSourceTable);

            //getting distinct values for group column
            DataTable dtGroup = dv.ToTable(true, new string[] { i_sGroupByColumn });

            //adding column for the row count
            dtGroup.Columns.Add("Count", typeof(int));

            //looping thru distinct values for the group, counting
            foreach (DataRow dr in dtGroup.Rows)
            {
                dr["Count"] = i_dSourceTable.Compute("Count(" + i_sAggregateColumn + ")", i_sGroupByColumn + " = '" + dr[i_sGroupByColumn] + "'");
            }

            //returning grouped/counted result
            return dtGroup;
        }

    }
}