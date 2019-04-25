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

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Web
{
    public partial class BatchTagging : LayoutsPageBase
    {
        protected List<SPListItem> listItems;
        internal const int MAX_LENGTH = 255;
        internal static readonly Regex SpaceRegex = new Regex("[ ]{2,}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public BatchTagging()
        {
            this.RightsCheckMode = RightsCheckModes.None;
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
            if (Request.QueryString["items"] != null && Request.QueryString["source"] != null && Request.QueryString["sourceurl"] != null)
            {
                string source = Request.QueryString["source"];
                string[] items = Request.QueryString["items"].ToString().Split('|');
                string currentWeb = Request.QueryString["sourceurl"].Replace("'", string.Empty);

                lblInstructions.Text = "You have selected the following documents to tag.<br><br>";
                source = source.Substring(1, source.Length - 2).ToLower();
                Guid sourceId = new Guid(source);

                SPWeb web = SPContext.Current.Web;
                SPDocumentLibrary sourceDocLibrary = (SPDocumentLibrary)web.Lists[sourceId];
                listItems = new List<SPListItem>();

                //start at 1 due to items split containing a leading empty value
                for (int i = 1; i < items.Length; i++)
                {
                    SPListItem currentListItem = sourceDocLibrary.GetItemById(int.Parse(items[i]));
                    listItems.Add(currentListItem);
                    lblItems.Text += currentListItem.Name + "<br>";
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //get our keywords; remove any trailing semi-colon to prevent an empty tag
                var keywordString = txtTags.Text.Trim().TrimEnd(';');
                IEnumerable<string> keywords = keywordString.Split(';');

                //loop through our list of selected items
                foreach (SPListItem item in listItems)
                {
                    //find our taxonomy field
                    TaxonomyField managedField = item.Fields.TryGetFieldByStaticName("TaxKeyword") as TaxonomyField;

                    if (managedField != null)
                    {
                        TaxonomySession session = new TaxonomySession(item.Web.Site, false);
                        TermStore termStore = session.TermStores[managedField.SspId];
                        TermSet termSet = termStore.GetTermSet(managedField.TermSetId);

                        var terms = new List<Term>();

                        item.Web.AllowUnsafeUpdates = true;
                        WriteTagsToFolksonomyColumn(keywords, item, managedField);
                        item.Update();
                        item.Web.AllowUnsafeUpdates = false;

                        //close the modal
                        Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, '{0}');</script>", listItems.Count));
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //TODO: some logging?
            }
        }

        private string GetValidTermName(string name)
        {
            //if (NameRegex.IsMatch(name))
            //{
            //    name = NameRegex.Replace(name, " ");
            //}
            name = SpaceRegex.Replace(name, " ");
            name = name.Length > MAX_LENGTH ? name.Substring(0, MAX_LENGTH) : name;

            //Normalize the ampersands
            name = name.Replace(Convert.ToChar(38), Convert.ToChar(65286));

            //trim any whitespace
            return name.Trim();
        }

        private void WriteTagsToFolksonomyColumn(IEnumerable<string> tags, SPListItem item, TaxonomyField column)
        {
            if (!column.IsTermSetValid) return;
            if (!column.Open) return;

            var session = new TaxonomySession(item.Web.Site);
            var mms = session.TermStores[column.SspId];
            var ts = mms.GetTermSet(column.TermSetId);

            if (!ts.IsOpenForTermCreation) return;

            var addedTerms = new List<Term>();
            bool hasChanges = false;

            foreach (var tag in tags)
            {
                if (tag.Length > MAX_LENGTH) continue;

                var validTag = GetValidTermName(tag);

                Term matchingTerm;

                if (column.IsKeyword)
                {
                    matchingTerm = mms.GetTerms(validTag, false, StringMatchOption.ExactMatch, 1, true).FirstOrDefault();
                }
                else
                {
                    matchingTerm = ts.GetTerms(validTag, false, StringMatchOption.ExactMatch, 1, true).FirstOrDefault();
                }

                if (matchingTerm == null)
                {
                    matchingTerm = ts.CreateTerm(validTag, mms.WorkingLanguage);
                    hasChanges = true;
                }

                if (!addedTerms.Contains(matchingTerm))
                {
                    addedTerms.Add(matchingTerm);
                }
            }

            if (hasChanges) mms.CommitAll();
            if (addedTerms.Count > 0)
            {
                if (column.AllowMultipleValues)
                {
                    column.SetFieldValue(item, addedTerms, mms.WorkingLanguage);
                }
                else
                {
                    column.SetFieldValue(item, addedTerms.First());
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //close the window on cancel
            Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(0, '{0}');</script>", listItems.Count));
            Response.Flush();
            Response.End();
        }
    }
}
