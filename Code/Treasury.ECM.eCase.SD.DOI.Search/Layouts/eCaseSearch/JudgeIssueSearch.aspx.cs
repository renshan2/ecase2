using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch
{
    public partial class JudgeIssueSearch : LayoutsPageBase
    {
        internal static readonly string ECASE_TERMS_GROUP_NAME = "eCase Terms";
        internal static readonly string ECASE_TERMS_JUDGE_TERMSET_NAME = "Tax Court Judges";
        internal static readonly string ECASE_TERMS_LIL_TERMSET_NAME = "Law Issue List";

        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigureTaxControls();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string validationReason;
            //if (taxJudge.Validate(out validationReason) && taxIssue.Validate(out validationReason) && taxJudge.Text != string.Empty && taxIssue.Text != string.Empty)
            if (lawIssue.Validate(out validationReason) && lawIssue.Text != string.Empty)
            {
                Microsoft.SharePoint.Taxonomy.TaxonomySession taxSession = new Microsoft.SharePoint.Taxonomy.TaxonomySession(SPContext.Current.Web.Site);
                Microsoft.SharePoint.Taxonomy.TermStore taxStore = taxSession.TermStores[0];
                Microsoft.SharePoint.Taxonomy.Group taxGroup = GetTermStoreGroup(taxStore, ECASE_TERMS_GROUP_NAME);
                Microsoft.SharePoint.Taxonomy.TermSet issueTermSet = null;
                Microsoft.SharePoint.Taxonomy.TermSet judgeTermSet = null;

                //we need to attach our managed metadata UI controls to the term sets...
                IEnumerator<Microsoft.SharePoint.Taxonomy.TermSet> termsets = taxGroup.TermSets.GetEnumerator();
                while (termsets.MoveNext())
                {
                    Microsoft.SharePoint.Taxonomy.TermSet curTermset = termsets.Current;

                    if (curTermset.Name == ECASE_TERMS_LIL_TERMSET_NAME)
                        issueTermSet = curTermset;

                    if (curTermset.Name == ECASE_TERMS_JUDGE_TERMSET_NAME)
                        judgeTermSet = curTermset;

                }
                pnlResults.Visible = true;
                //Guid judgeTermId = new Guid(taxJudge.Text.Split('|')[1]);
                //int[] judgeIntCollection = TaxonomyField.GetWssIdsOfTerm(SPContext.Current.Web.Site, taxStore.Id,
                //                                                         judgeTermSet.Id, judgeTermId, false, 50);

                Guid issueTermId = new Guid(lawIssue.Text.Split('|')[1]);
                int[] issueIntCollection = TaxonomyField.GetWssIdsOfTerm(SPContext.Current.Web.Site, taxStore.Id,
                                                                         issueTermSet.Id, issueTermId, false, 50);

                //String CAML_QUERY =
                //    @"<Where><And><In><FieldRef LookupId=""True"" Name=""Judge"" /><Values>{0}</Values></In><In><FieldRef LookupId=""True"" Name=""UniformIssueList"" /><Values>{1}</Values></In></And></Where>";
                //String VALUES = @"<Value Type=""Integer"">{0}</Value>";

                String CAML_QUERY =
                    @"<Where><In><FieldRef LookupId=""True"" Name=""LawIssueList"" /><Values>{0}</Values></In></Where>";
                String VALUES = @"<Value Type=""Integer"">{0}</Value>";

                //StringBuilder strJudgeValues = new StringBuilder();
                StringBuilder strIssueValues = new StringBuilder();

                //foreach (int judgeInt in judgeIntCollection)
                //{
                //    strJudgeValues.AppendLine(String.Format(VALUES, judgeInt));
                //}

                foreach (int issueInt in issueIntCollection)
                {
                    strIssueValues.AppendLine(String.Format(VALUES, issueInt));
                }


                //String strQuery = String.Format(CAML_QUERY, strJudgeValues.ToString(), strIssueValues.ToString());
                String strQuery = String.Format(CAML_QUERY, strIssueValues.ToString());
                litIssue.Text = lawIssue.Text.Split('|')[0];
                //litJudge.Text = taxJudge.Text.Split('|')[0];

                //if (strIssueValues.Length == 0 || strJudgeValues.Length == 0)
                if (strIssueValues.Length == 0)
                    litOwners.Text = "<b>There are no results</b>";
                else
                {
                    SPQuery sPQuery = new SPQuery();
                    sPQuery.Query = strQuery;

                    SPListItemCollection results = null;
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                                                             {
                                                                 using (
                                                                     SPSite site =
                                                                         new SPSite(SPContext.Current.Web.Site.ID))
                                                                 {
                                                                     SPWeb web = site.RootWeb;
                                                                     SPList casesList = web.Lists["Cases"];
                                                                     results = casesList.GetItems(sPQuery);
                                                                 }
                                                             });

                    litOwners.Text = string.Empty;
                    foreach (SPListItem result in results)
                    {
                        litOwners.Text += new SPFieldLookupValue(result["AssignedTo"].ToString()).LookupValue + "<br />";
                    }

                    if (litOwners.Text == string.Empty)
                        litOwners.Text = "<b>There are no results</b>";
                }
            }
            else
            {
                pnlResults.Visible = true;
                litOwners.Text = "<b>The data entered in the Law Issues List fields is invalid.  Please ensure the field contains valid values.</b>";
            }

        }


        protected void ConfigureTaxControls()
        {
            Microsoft.SharePoint.Taxonomy.TaxonomySession taxSession = new Microsoft.SharePoint.Taxonomy.TaxonomySession(SPContext.Current.Web.Site);
            Microsoft.SharePoint.Taxonomy.TermStore taxStore = taxSession.TermStores[0];
            Microsoft.SharePoint.Taxonomy.Group taxGroup = GetTermStoreGroup(taxStore, ECASE_TERMS_GROUP_NAME);
            Microsoft.SharePoint.Taxonomy.TermSet issueTermSet = null;
            Microsoft.SharePoint.Taxonomy.TermSet judgeTermSet = null;

            //we need to attach our managed metadata UI controls to the term sets...
            IEnumerator<Microsoft.SharePoint.Taxonomy.TermSet> termsets = taxGroup.TermSets.GetEnumerator();
            while (termsets.MoveNext())
            {
                Microsoft.SharePoint.Taxonomy.TermSet curTermset = termsets.Current;

                if (curTermset.Name == ECASE_TERMS_LIL_TERMSET_NAME)
                    issueTermSet = curTermset;

                if (curTermset.Name == ECASE_TERMS_JUDGE_TERMSET_NAME)
                    judgeTermSet = curTermset;

            }

            try
            {
                //ConfigureTaxonomyControl(this.taxJudge, taxStore.Id.ToString(), taxGroup.Id, judgeTermSet.Id.ToString(), taxSession.TermStores[0].DefaultLanguage, false);
                ConfigureTaxonomyControl(this.lawIssue, taxStore.Id.ToString(), taxGroup.Id, issueTermSet.Id.ToString(), taxSession.TermStores[0].DefaultLanguage, false);
            }
            catch (Exception sendersEx)
            {
            }
        }
        protected void ConfigureTaxonomyControl(Microsoft.SharePoint.Taxonomy.TaxonomyWebTaggingControl taxWebTagControl, string taxStoreId,
                                        Guid groupId, string termSetId, int taxLanguage, bool isMulti)
        {
            taxWebTagControl.SSPList = taxStoreId;

            taxWebTagControl.TermSetList = termSetId;

            taxWebTagControl.GroupId = groupId;

            taxWebTagControl.AllowFillIn = true;
            //taxWebTagControl.IsAddTerms = true;
            taxWebTagControl.IsAddTerms = false;
            taxWebTagControl.IsDisplayPickerButton = true;
            taxWebTagControl.IsMulti = isMulti;
            taxWebTagControl.IsSpanTermSets = false;
            taxWebTagControl.IsSpanTermStores = false;
            taxWebTagControl.IsUseCommaAsDelimiter = false;
            taxWebTagControl.Language = taxLanguage;
        }
        internal static Group GetTermStoreGroup(TermStore store, string groupName)
        {
            Group group = null;

            try
            {
                IEnumerator<Group> gs = store.Groups.GetEnumerator();

                while (gs.MoveNext())
                {
                    Group curGroup = gs.Current;
                    if (curGroup.Name == groupName)
                        return curGroup;
                }

                return group;
            }
            catch (Exception ex)
            {
                    throw ex;
            }
        }
    }
}
