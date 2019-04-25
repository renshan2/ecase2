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
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Microsoft.SharePoint.Utilities;
using System.Text;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Web
{
    public partial class BatchMoving : LayoutsPageBase
    {
        protected List<SPFile> listFiles;
        StringBuilder errorMessages = null;

        public BatchMoving()
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
                if (Request.QueryString["items"] != null && Request.QueryString["source"] != null && Request.QueryString["sourceurl"] != null)
                {
                    string source = Request.QueryString["source"];
                    string[] items = Request.QueryString["items"].ToString().Split('|');
                    string currentWeb = Request.QueryString["sourceurl"].Replace("'", string.Empty);

                    lblItems.Text = null;
                    lblInstructions.Text = "You have selected the following documents to move.<br><br>";
                    source = source.Substring(1, source.Length - 2).ToLower();
                    Guid sourceId = new Guid(source);

                    SPWeb web = SPContext.Current.Web;
                    SPDocumentLibrary sourceDocLibrary = (SPDocumentLibrary)web.Lists[sourceId];
                    listFiles = new List<SPFile>();

                    //start at 1 due to items split containing a leading empty value
                    for (int i = 1; i < items.Length; i++)
                    {
                        SPListItem currentListItem = sourceDocLibrary.GetItemById(int.Parse(items[i]));
                        if (currentListItem.File != null)
                        {
                            listFiles.Add(currentListItem.File);
                            lblItems.Text += currentListItem.Name + "<br>";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error while loading documents for batch moving.", ex, DiagnosticsCategories.eCaseWeb);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                lblErrors.Text = null;
                var destination = FormatFileDestination(txtDestination.Text.Trim());
                SPFile fileMoved = null;

                //loop through our list of selected Files and move them to their new destination
                foreach (SPFile moveFile in listFiles)
                {
                    try
                    {
                        if (!SPFileExtensions.Exists(moveFile, destination))
                        {
                            fileMoved = MoveFile(moveFile.Web, moveFile, destination);
                            Logger.Instance.Info("Moved file: " + fileMoved.Name + " from " + moveFile.Url + " to " + moveFile.Url);
                            errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Error(s):<br />The file (" + moveFile.Name + ") you are trying to move exists at the destination." + "<br />") : errorMessages.Append("The file (" + moveFile.Name + ") you are trying to move exists at the destination." + "<br />");
                            lblItems.Text = null;
                        }
                        else
                        {
                            Logger.Instance.Error("Error in Batch File Moving: User attempted to copy " + moveFile.Name + " to a location where the file already exists.", DiagnosticsCategories.eCaseWeb);
                        }
                    }
                    catch (System.ArgumentException argEx)
                    {
                        Logger.Instance.Error("Error in Batch File Moving: " + moveFile.Name, argEx, DiagnosticsCategories.eCaseWeb);
                        errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Error(s):<br />The file you are trying to move, does not exist." + "<br />") : errorMessages.Append("The file you are trying to move, does not exist." + "<br />");
                        lblItems.Text = null;
                        continue;
                    }
                    catch (System.IO.FileNotFoundException fnfEx)
                    {
                        Logger.Instance.Error("Error in Batch File Moving: " + moveFile.Name, fnfEx, DiagnosticsCategories.eCaseWeb);
                        errorMessages = errorMessages == null ? errorMessages = new StringBuilder("The file you are trying to move could not be found. <br />") : errorMessages.Append("The file you are trying to move could not be found. + <br />");
                        lblItems.Text = null;
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Logger.Instance.Error("Error in Batch File Moving: " + moveFile.Name, ex, DiagnosticsCategories.eCaseWeb);
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
        }

        public static SPFile MoveFile(SPWeb web, SPFile moveFile, string destination)
        {
            web.AllowUnsafeUpdates = true;
            string destinationFolderUrl = destination;
            destinationFolderUrl = (!destinationFolderUrl.EndsWith("/")) ? destinationFolderUrl + "/" : destinationFolderUrl;
            //move the source file to the new location
            moveFile.MoveTo(SPUrlUtility.CombineUrl(destination, moveFile.Name), true);
            web.Update();
            web.AllowUnsafeUpdates = false;

            return web.GetFile(web.Url + "/" + destinationFolderUrl + moveFile.Name);
        }

        public static void DeleteFile(SPWeb web, SPFile fileToDelete)
        {
            web.AllowUnsafeUpdates = true;
            fileToDelete.Delete();
            web.AllowUnsafeUpdates = false;
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
            var destinationFolderUrl = FormatFileDestination(destination);

            SPWeb web = SPContext.Current.Web;
            if (web.GetFolder(destinationFolderUrl).Exists)
            {
                lblItems.Text = null;
                //loop through our list of selected Files and move them to their new destination
                foreach (SPFile copyFile in listFiles)
                {
                    if (!web.GetFile(destinationFolderUrl + copyFile.Name).Exists)
                    {
                        lblItems.Text += copyFile.Name + " - Ready to move.<br />";
                    }
                    else
                    {
                        lblItems.Text += copyFile.Name + " - Warning! File exists at: <a href=" + destinationFolderUrl + ">View</a><br />";
                    }
                }
            }
            else
            {
                var errorMessage = "This URL format does not support file copying or moving. Please check the URL and try again.";
                string script = "<script language='javascript'>alert('" + errorMessage + "')</script>";
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidateFilesScript", script);
            }
        }

        private static string FormatFileDestination(string destination)
        {
            destination = (!destination.EndsWith("/")) ? destination + "/" : destination;
            return destination;
        }

        protected void UrlValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string txtToValidate = txtDestination.Text;
            string pattern = @"(http|https):\/\/[\w\-_]+(\.[\w-_]+)?([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!reg.IsMatch(txtToValidate) || !SPContext.Current.Web.GetFolder(txtToValidate).Exists)
                args.IsValid = false;
            else
                args.IsValid = true;
        }

    }
}
