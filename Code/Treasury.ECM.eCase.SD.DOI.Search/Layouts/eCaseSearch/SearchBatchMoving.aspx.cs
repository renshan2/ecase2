using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Globalization;
using Microsoft.SharePoint.Utilities;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.Layouts.eCaseSearch
{
    public partial class SearchBatchMoving : LayoutsPageBase
    {
        protected List<string> listFiles;
        StringBuilder errorMessages = null;

        public SearchBatchMoving()
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
            try
            {
                if (Request.QueryString["items"] != null && Request.QueryString["sourceurl"] != null)
                {
                    string[] items = Request.QueryString["items"].ToString().Split('|');
                    string currentWeb = Request.QueryString["sourceurl"].Replace("'", string.Empty);

                    lblInstructions.Text = "You have selected the following documents to move. <br><br>";

                    listFiles = new List<string>();

                    //start at 1 due to items split containing a leading empty value
                    for (int i = 1; i < items.Length; i++)
                    {
                        if (items[i] != null && items[i] != string.Empty)
                        {
                            listFiles.Add(items[i]);
                            lblItems.Text += items[i] + "<br />";
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error while loading documents for batch moving.", ex, DiagnosticsCategories.eCaseSearch);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            var destination = txtDestination.Text.Trim();

            //loop through our list of selected Files and move them to their new destination
            foreach (string url in listFiles)
            {
                try
                {
                    using (SPSite site = new SPSite(url))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPFile file = web.GetFile(url);
                            FileUtilities.MoveFile(web, file, destination);
                        }
                    }
                }
                catch (System.ArgumentException argEx)
                {
                    Logger.Instance.Error("Error in Batch File Moving: " + url, argEx, DiagnosticsCategories.eCaseSearch);
                    errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Errors:<br />The file you are trying to move, does not exist." + "<br />") : errorMessages.Append("The file you are trying to move, does not exist." + "<br />");
                    lblItems.Text = null;
                    continue;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error("Error in Batch File Moving: " + url, ex, DiagnosticsCategories.eCaseSearch);
                    errorMessages = errorMessages == null ? errorMessages = new StringBuilder(ex.Message + "<br />") : errorMessages.Append(ex.Message + "<br />");
                    lblItems.Text = null;
                    continue;
                }
            }

            //show any error messages that we have
            lblErrors.Text = errorMessages == null ? null : errorMessages.ToString();

            if (errorMessages == null || errorMessages.Length == 0)
            {
                //close the modal
                Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, '{0}');</script>", listFiles.Count));
                Response.Flush();
                Response.End();
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //close the window on cancel
            Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(0, '{0}');</script>", listFiles.Count));
            Response.Flush();
            Response.End();
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateFiles();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error in File Move Validation", ex, DiagnosticsCategories.eCaseWeb);
            }
        }

        protected void ValidateFiles()
        {
            var destination = txtDestination.Text.Trim();
            var destinationFolderUrl = FileUtilities.FormatFileDestination(destination);
            lblItems.Text = null;

            //loop through our list of selected Files and move them to their new destination
            foreach (String fileUrl in listFiles)
            {
                using (SPSite site = new SPSite(fileUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPFile file = web.GetFile(fileUrl);
                        if (file != null)
                        {
                            if (!web.GetFile(destinationFolderUrl + file.Name).Exists)
                            {
                                lblItems.Text += fileUrl + " - Ready to move.<br />";
                            }
                            else
                            {
                                lblItems.Text += fileUrl + " - Warning! File exists at: <a href=" + destinationFolderUrl + ">View</a><br />";
                            }
                        }
                    }
                }
            }
        }

        protected void UrlValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            string pattern = @"(http|https):\/\/[\w\-_]+(\.[\w-_]+)?([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!reg.IsMatch(txtDestination.Text))
                args.IsValid = false;
        }
    }
}

