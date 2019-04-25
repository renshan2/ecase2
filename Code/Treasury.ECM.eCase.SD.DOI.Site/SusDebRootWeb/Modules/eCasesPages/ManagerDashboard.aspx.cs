using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Text;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.eCaseRootWeb.Modules.eCasesPages
{
    public partial class ManagerDashboard : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Find ContentPlaceHolder on page
                ContentPlaceHolder PlaceHolderAdditionalPageHead = (ContentPlaceHolder)Page.Master.FindControl("PlaceHolderAdditionalPageHead");

                // Find Data Array Literal control on page
                Literal litCaseDataArray = (Literal)PlaceHolderAdditionalPageHead.FindControl("litCaseDataArray");
                Literal litPieDataArray = (Literal)PlaceHolderAdditionalPageHead.FindControl("litPieDataArray");
                // If Data Array Literal control exists, populate it with data from the Cases list
                if (litCaseDataArray != null)
                {
                    PopulateCaseDataArray(litCaseDataArray);                    
                }
                if (litPieDataArray != null)
                {
                    PopulatePieDataArray(litPieDataArray);
                }
            }
        }

        /// <summary>
        /// Populate Data Array Literal control with data from the Cases list
        /// </summary>
        private void PopulateCaseDataArray(Literal litCaseDataArray)
        {
            String strArray = "";
            StringBuilder sb = new StringBuilder();

            SPList listCases = GetList(SPContext.Current.Web, "Cases");
            if (listCases != null)
            {                
                SPQuery queryCases = new SPQuery
                {
                    Query = "<OrderBy><FieldRef Name='CaseOpeningDate' /></OrderBy>"
                };
                SPListItemCollection listCaseItems = listCases.GetItems(queryCases);
                StringBuilder sb2 = new StringBuilder();

                if (listCaseItems != null && listCaseItems.Count > 0)
                {
                    
                    sb2.Append("// Cases Query Returned Results. ");
                    sb2.Append(Environment.NewLine);
                    for (int i = 0; i < listCaseItems.Count; i++)
                    {
                        SPListItem listCaseItem = listCaseItems[i];
                        String strCaseID = listCaseItem["Unique ID"].ToString();
                        String strStep = listCaseItem["Case Step"].ToString();
                        //String strStatus = listCaseItem["Case Status"].ToString();
                        SPFieldLookupValue lookValStatus = new SPFieldLookupValue(Convert.ToString(listCaseItem["Case Status"]));
                        String strStatus = lookValStatus.LookupValue;                        
                        int intStep = GetStepNumber(strStep);
                        int intStatus = GetStatusNumber(strStatus);
                        DateTime dtCaseOpening = (DateTime)listCaseItem["Case Opening Date"];
                        String strBarColor = GetBarColor(intStep, dtCaseOpening);
                        String strFillColor = GetFillColor(intStep, dtCaseOpening);
                        String strURL = "";
                        if (listCaseItem["Case Url"] != null)
                        {
                            strURL = new SPFieldUrlValue(listCaseItem["Case Url"].ToString()).Url;
                        }                        
                        SPUser userInvestigator = GetUser(listCaseItem, listCaseItem.Fields["Assigned Bureau IG Investigator"]);
                        String strInvestigator = "";
                        if (userInvestigator != null)
                        {
                            strInvestigator = userInvestigator.Name;
                        }                        

                        sb2.Append("{ name: '" + strCaseID + "', "
                                    + "status: " + intStatus + ", "
                                    + "statustext: '" + strStatus + "', "
                                    //+ "steptext: '" + intStep.ToString() + " " + strStep + "', "
                                    + "steptext: '" + strStep + "', "
                                    + "barcolor: '" + strBarColor + "', "
                                    + "fillcolor: '" + strFillColor + "', "
                                    + "url: '" + strURL + "', "
                                    + "investigator: '" + strInvestigator + "'"
                                    + " }");
                        sb2.Append(",");
                    }
                }
                
                if (listCaseItems.Count > 0)
                {
                    sb.Append("allCaseData = [");
                    sb.Append(Environment.NewLine);
                    sb2.Length = sb2.Length - 1;
                    sb.Append(sb2.ToString());
                    sb.Append(Environment.NewLine);
                    sb.Append("];");
                }
            }   
            
            strArray = sb.ToString();
            litCaseDataArray.Text = strArray;
        }

        /// <summary>
        /// Populate Data Array Literal control with data from the Cases list
        /// </summary>
        private void PopulatePieDataArray(Literal litPieDataArray)
        {
            String strArray = "";
            StringBuilder sb = new StringBuilder();

            SPList listCases = GetList(SPContext.Current.Web, "Cases");
            if (listCases != null)
            {
                List<string> bureauList = GetChoiceFieldValues(listCases, "Bureau IG");

                if (bureauList != null && bureauList.Count > 0)
                {
                    int intResultCount = 0;
                    StringBuilder sb2 = new StringBuilder();

                    for (int a = 0; a < bureauList.Count; a++)
                    {
                        int intBureauCount = 0;
                        String strBureau = bureauList[a];

                        sb.Append("// Iterating through BureauIG.  BureauIG: " + strBureau);
                        sb.Append(Environment.NewLine);
                        SPQuery queryStatus = new SPQuery
                        {
                            Query = "<Where><Eq><FieldRef Name='BureauIG' /><Value Type='Choice'>" + strBureau + "</Value></Eq></Where>"
                        };
                        SPListItemCollection listCaseItems = listCases.GetItems(queryStatus);
                        if (listCaseItems != null && listCaseItems.Count > 0)
                        {
                            sb.Append("// Cases Bureau Query Returned Results. ");
                            sb.Append(Environment.NewLine);
                            intResultCount++;
                            intBureauCount = listCaseItems.Count;
                            sb2.Append("['" + strBureau + "', " + intBureauCount + "]");
                            sb2.Append(",");
                        }
                    }
                    if (intResultCount > 0)
                    {
                        sb.Append("pieData = [");
                        sb.Append(Environment.NewLine);
                        sb2.Length = sb2.Length - 1;
                        sb.Append(sb2.ToString());
                        sb.Append(Environment.NewLine);
                        sb.Append("];");
                    }
                }
            }

            strArray = sb.ToString();
            litPieDataArray.Text = strArray;
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
        /// Get Step Number
        /// </summary>
        private static int GetStepNumber(string stepText)
        {
            int returnNumber = 0;
            switch (stepText.ToLower())
            {
                case "collect all case materials":
                    returnNumber = 1;
                    break;
                case "inter-agency coordination":
                    returnNumber = 2;
                    break;
                case "review of referral":
                    returnNumber = 3;
                    break;
                case "recommendation to gler":
                    returnNumber = 4;
                    break;
                case "doj clearance":
                    returnNumber = 5;
                    break;
                default:
                    returnNumber = 0;
                    break;
            }
            return returnNumber;
        }

        /// <summary>
        /// Get Status Number
        /// </summary>
        private static int GetStatusNumber(string statusText)
        {
            int returnNumber = 0;
            string strStatusNumber = "0";
            strStatusNumber = statusText.Substring(0, 2);
            try
            {
                returnNumber = Convert.ToInt32(strStatusNumber);
            }
            catch
            {
                returnNumber = 0;
            }  
            return returnNumber;
        }

        /// <summary>
        /// Get Bar Color
        /// </summary>
        private static string GetBarColor(int intStep, DateTime dtCaseOpening)
        {
            string barColor = "#4673a7"; // Blue            
            // Future logic for changing color based on step and current date vs. opening date?
            // string barColor = "#209028"; // Green
            // string barColor = "#e7af26"; // Gold
            // string barColor = "#aa4744"; // Red
            return barColor;
        }

        /// <summary>
        /// Get Filler Color
        /// </summary>
        private static string GetFillColor(int intStep, DateTime dtCaseOpening)
        {
            string fillColor = "#afc7dd"; // Light Blue
            // Future logic for changing color based on step and current date vs. opening date?
            // string fillColor = "#c5d99c"; // Light Green
            // string fillColor = "#eec767"; // Light Gold
            // string fillColor = "#ddb7af"; // Light Red
            return fillColor;
        }

        /// <summary>
        /// Get User from SPField
        /// </summary>
        private static SPUser GetUser(SPListItem item, SPField userField)
        {
            SPUser returnUser = null;
            try
            {
                string currentValue = item[userField.Title].ToString();
                SPFieldUser field = (SPFieldUser)userField;
                SPFieldUserValue fieldValue = (SPFieldUserValue)field.GetFieldValue(currentValue);
                returnUser = fieldValue.User; 
            }
            catch
            {
                Logger.Instance.Error(string.Format("Exception looking for user: {0}", "userField"), DiagnosticsCategories.eCaseSite);
                returnUser = null;
            }
            return returnUser;
        }

        /// <summary>
        /// Get Choice Field Values from List
        /// </summary>
        private static List<string> GetChoiceFieldValues(SPList spList, string fieldName)
        {
            List<string> fieldList = null;
            try
            {
                SPFieldChoice field = (SPFieldChoice)spList.Fields[fieldName];
                fieldList = new List<string>();
                foreach (string strChoice in field.Choices)
                {
                    fieldList.Add(strChoice);
                }
            }
            catch
            {
                Logger.Instance.Error(string.Format("Exception looking for choice field values: {0}", fieldName), DiagnosticsCategories.eCaseSite);
                fieldList = null;
            }
            return fieldList;
        }

    }
}