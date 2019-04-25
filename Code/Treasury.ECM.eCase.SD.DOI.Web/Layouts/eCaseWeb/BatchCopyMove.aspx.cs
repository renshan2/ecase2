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
using Microsoft.SharePoint.Administration;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.Layouts.Treasury.ECM.eCase.SusDeb.DOI.Web
{
    public partial class BatchCopyMove : LayoutsPageBase
    {
        StringBuilder errorMessages = null;

        SPWeb currWeb = null;

        public BatchCopyMove()
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

        #region Events

        protected override void OnLoad(EventArgs e)
        {

            if (!IsPostBack)
            {
                Logger.Instance.Info("Batch Copy/Move - OnLoad Executing", DiagnosticsCategories.eCaseWeb);
                // Populate our list of docs the user selected
                PopulateSelectedDocs();

                currWeb = SPContext.Current.Web;

                // Get our list of site collections to search
                List<SPSite> siteCollectionList = GetSiteCollectionListFromPropertyBag();

                foreach (SPSite siteCollection in siteCollectionList)
                {
                    Logger.Instance.Info(String.Format("Batch Copy/Move - OnLoad - Processing Site Collection: {0}",
                                            siteCollection), DiagnosticsCategories.eCaseWeb);
                    using (SPSite site = new SPSite(siteCollection.Url))
                    {
                        using (SPWeb rootWeb = site.OpenWeb())
                        {
                            DocLibRecursive(rootWeb);                           
                        }
                    }
                }

                // Hide Other Location textbox initially
                hidOtherLocationVisible.Value = "false";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string action = string.Empty;
            string actionDesc = string.Empty;
            LinkButton button = sender as LinkButton;
            if (button.ID == "btnCopy") 
            {
                action = "copy";
                actionDesc = "copied";
            }
            else if (button.ID == "btnMove")
            {
                action = "move";
                actionDesc = "moved";
            }
            var destination = FormatFileDestination(txtDestination.Text.Trim());
            SPFile fileProcessed = null;

            SPLongOperation longOp = new SPLongOperation(this.Page);
            longOp.LeadingHTML = "Please wait while your file(s) are being " + actionDesc + "...";
            longOp.Begin();

            var fileList = GetSelectedFiles();

            foreach (SPFile file in fileList)
            {
                try
                {
                    if (!SPFileExtensions.Exists(file, destination))
                    {
                        fileProcessed = action == "copy" ? CopyFile(file, destination) : MoveFile(file, destination);
                        Logger.Instance.Info("Processed file " + action + " for: " + fileProcessed.Name + " from " + file.Url + " to " + fileProcessed.Url);
                    }
                    else
                    {
                        Logger.Instance.Error("Error in Batch File Copy/Move: User attempted to copy " + file.Name + " to a location where the file already exists.", DiagnosticsCategories.eCaseWeb);
                        errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Error(s):<br /> " + file.Name + " already exists. <br />") : errorMessages.Append(file.Name + " already exists. <br />");
                        lblItems.Text = null;
                        continue;
                    }
                }
                catch (System.ArgumentException argEx)
                {
                    Logger.Instance.Error("Error in Batch File Copying: " + file.Name, argEx, DiagnosticsCategories.eCaseWeb);
                    errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Error(s):<br />The file you are trying to copy does not exist. <br />") : errorMessages.Append("The file you are trying to copy does not exist. <br />");
                    lblItems.Text = null;
                    continue;
                }
                catch (System.IO.FileNotFoundException fnfEx)
                {
                    Logger.Instance.Error("Error in Batch File Copying: " + file.Name, fnfEx, DiagnosticsCategories.eCaseWeb);
                    errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Error(s):<br />The file you are trying to copy could not be found. <br />") : errorMessages.Append("The file you are trying to copy could not be found. <br />");
                    lblItems.Text = null;
                    continue;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error("Error in Batch File Copying: " + file.Name, ex, DiagnosticsCategories.eCaseWeb);
                    errorMessages = errorMessages == null ? errorMessages = new StringBuilder("Error(s):<br />This URL contains invalid characters. <br />") : errorMessages.Append("This URL contains invalid characters. <br />");
                    lblItems.Text = null;
                    continue;
                }
            }
            //show any error messages that we have
            lblErrors.Text = errorMessages == null ? null : errorMessages.ToString();

            if ((errorMessages == null || errorMessages.Length == 0))
            {
                //close the modal
                // Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(1, '{0}');</script>", treeViewSelectedDocs.Nodes.Count));
                // Response.Flush();
                // Response.End();
                string strScript = "try { alert('File(s) " + actionDesc + " successfully.'); window.frameElement.commitPopup(); } catch (e) {}";
                if (SPContext.Current.IsPopUI)
                {
                    longOp.EndScript(strScript);
                }
            }
            else
            {
                errorMessages.Append("<br/><br/>Please <a href='#' onclick='history.go(-1);return false;'>go back</a> and try again.");
                longOp.EndScript("document.getElementById('s4-simple-card-content').innerHTML = \"<br/><br/><br/><h4>Errors have occurred during the submission.</h4>" + errorMessages.ToString() + " \";");
            }
        }

        protected void treeView_SelectedNodeChanged(object sender, EventArgs e)
        {
            txtDestination.Text = treeView.SelectedNode.Value;
            ValidateFiles();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            //close the window on cancel
            Response.Write(string.Format(CultureInfo.InvariantCulture, "<script type=\"text/javascript\">window.frameElement.commonModalDialogClose(0, '{0}');</script>", treeView.Nodes.Count));
            Response.Flush();
            Response.End();
        }

        protected void btnValidate_Click(object sender, EventArgs e)
        {
            lblErrors.Text = null;            
            try
            {
                ValidateFiles();
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error in File Copy Validation", ex, DiagnosticsCategories.eCaseWeb);
            }
        }

        protected void UrlValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string txtToValidate = txtDestination.Text;
            string pattern = @"(http|https):\/\/[\w\-_]+(\.[\w-_]+)?([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!reg.IsMatch(txtToValidate))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        #endregion

        #region Private Methods

        private static Boolean DoesUserHavePermissions(SPWeb web, String listname)
        {
            Boolean catchException = SPSecurity.CatchAccessDeniedException;
            SPSecurity.CatchAccessDeniedException = false;
            try
            {
                SPList list = web.Lists[listname];
                if (list.DoesUserHavePermissions(SPBasePermissions.DeleteListItems) == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                //reset the flag to original value        
                SPSecurity.CatchAccessDeniedException = catchException;
            }
        }

        private List<SPSite> GetSiteCollectionListFromPropertyBag()
        {
            SPWebApplication webApp = SPContext.Current.Site.WebApplication;
            string siteCollectionString = webApp.Properties["SiteCollectionSearchList"].ToString();
            Logger.Instance.Info(String.Format("Retreiving list of sites from Web Application Property Bag Entry 'SiteCollectionSearchList': {0}",
                        siteCollectionString),
                        DiagnosticsCategories.eCaseWeb);
            return GetSiteCollectionsFromString(siteCollectionString);
        }

        private List<SPSite> GetSiteCollectionsFromString(string siteCollectionString)
        {
            List<SPSite> spSites = new List<SPSite>();
            List<string> scl = siteCollectionString.Split('|').ToList<string>();
            foreach (string s in scl)
            {
                try
                {
                    Logger.Instance.Info(String.Format("Batch Copy/Move - Retrieving SPSite for Site: '{0}'", s),
                                            DiagnosticsCategories.eCaseWeb);
                    using (SPSite site = new SPSite(s))
                    {
                        Logger.Instance.Info(String.Format("Batch Copy/Move - Adding SPSite to list of Sites: '{0}'", s),
                                                DiagnosticsCategories.eCaseWeb);
                        Logger.Instance.Info(String.Format("Batch Copy/Move - Effective Base Permissions: '{0}'", site.RootWeb.EffectiveBasePermissions),
                                                DiagnosticsCategories.eCaseWeb);
                        //Intent: only add a site if the user has permissions to view the site, even without permissions to view it we may be able to open it
                        // If a user doesn't have view rights, a later call to GetSubwebsForCurrentUser in DocLibRecursive will fail with an AccessDeniedException
                        // We're using the ViewPages permission as a surrogate for "Read" rights or better in this site
                        if (site.RootWeb.DoesUserHavePermissions(SPBasePermissions.ViewPages))
                        {
                            spSites.Add(site);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Error("Error accessing site collection defined in WebApplication Property Bag Setting 'SiteCollectionSearchList'", ex, DiagnosticsCategories.eCaseSearch);
                }
            }
            return spSites;
        }

        private static string FormatFileDestination(string destination)
        {
            destination = (!destination.EndsWith("/")) ? destination + "/" : destination;
            return destination;
        }

        private void ValidateFiles()
        {
            // Need to validate files not just for tree view, but also for manually entered urls
            Logger.Instance.Info("Batch Copy/Move - Validating Files", DiagnosticsCategories.eCaseWeb);

            // var nodeValue = treeView.SelectedNode.Value.Trim();
            var destination = txtDestination.Text.Trim();
            string sourceUrl = null;

            if (SiteExists(destination) == false)
            {
                // Site doesn't exist
                var errorMessage = "The destination you have chosen is not an existing SharePoint site.";
                string script = "<script language='javascript'>alert('" + errorMessage + "')</script>";
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidateFilesScript", script);
                return;
            }

            Logger.Instance.Info(String.Format("Batch Copy/Move - Validating Destination Url: {0}", destination), DiagnosticsCategories.eCaseWeb);
            // using (SPSite site = new SPSite(nodeValue))
            using (SPSite site = new SPSite(destination))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    List<SPFile> listFiles = GetSelectedFiles();                    

                    // SPList destinationList = web.GetList(destination);
                    SPList destinationList;
                    try
                    {
                        destinationList = web.GetList(destination);
                    }
                    catch (Exception)
                    {
                        destinationList = null;
                    }

                    if (null != destinationList)
                    {                        
                        if (destinationList.BaseType != SPBaseType.DocumentLibrary)
                        {                            
                            var errorMessage = "The destination you have chosen is not a valid SharePoint document library.";
                            string script = "<script language='javascript'>alert('" + errorMessage + "')</script>";
                            Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidateFilesScript", script);
                            return;
                        }
                    }
                    else
                    {                        
                        var errorMessage = "The destination you have chosen is not a valid SharePoint list.";
                        string script = "<script language='javascript'>alert('" + errorMessage + "')</script>";
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidateFilesScript", script);
                        return;
                    }

                    var destinationFolderUrl = FormatFileDestination(destination);

                    Logger.Instance.Info(String.Format("Batch Copy/Move - Getting Destination Folder: {0}", destinationFolderUrl),
                                                DiagnosticsCategories.eCaseWeb);
                    if (web.GetFolder(destinationFolderUrl).Exists)
                    {
                        lblItems.Text = null;
                        lblItems.Text += "<b>File Validation Summary</b></br />";                        
                        
                        foreach (SPFile file in listFiles)
                        {
                            sourceUrl = SPContext.Current.Web.Url + "/" + file.Url;

                            if (sourceUrl == destinationFolderUrl + file.Name)
                            {
                                lblItems.Text += file.Name + " - Source and destination are the same. You cannot copy/move a file to itself.<br />";
                            }
                            else if (!web.GetFile(destinationFolderUrl + file.Name).Exists)
                            {
                                lblItems.Text += file.Name + " - Ready to copy/move.<br />";
                            }
                            else
                            {
                                lblItems.Text += file.Name + " - File exists. Click <a href=" + destinationFolderUrl + ">here</a> to view.<br />";
                            }
                        }
                    }
                    else
                    {
                        var errorMessage = "The destination you have chosen does not exist.";
                        string script = "<script language='javascript'>alert('" + errorMessage + "')</script>";
                        Page.ClientScript.RegisterClientScriptBlock(GetType(), "ValidateFilesScript", script);
                    }
                }
            }
        }

        private static bool SiteExists(string url)
        {
            try
            {
                Logger.Instance.Info(String.Format("Batch Copy/Move - Checking if Site Exists: {0}", url),
                                        DiagnosticsCategories.eCaseWeb);
                using (SPSite site = new SPSite(url))
                {
                    using (SPWeb web = site.OpenWeb(url, true))
                    {
                        return true;
                    }
                }
            }
            catch (System.IO.FileNotFoundException)
            {
                return false;
            }
        }

        private List<SPFile> GetSelectedFiles()
        {
            SPWeb web = SPContext.Current.Web;
            List<SPFile> fileList = new List<SPFile>();

            foreach (TreeNode node in treeViewSelectedDocs.Nodes)
            {
                if (node.Checked == true)
                {
                    fileList.Add(web.GetFile(node.Value));
                }                
            }

            return fileList;
        }

        private void PopulateSelectedDocs()
        {
            string source = Request.QueryString["source"];
            source = source.Substring(1, source.Length - 2).ToLower();
            string[] items = Request.QueryString["items"].ToString().Split('|');
            Guid sourceId = new Guid(source);
            SPWeb web = SPContext.Current.Web;
            SPDocumentLibrary sourceDocLibrary = (SPDocumentLibrary)web.Lists[sourceId];

            //start at 1 due to items split containing a leading empty value
            for (int i = 1; i < items.Length; i++)
            {
                SPListItem currentListItem = sourceDocLibrary.GetItemById(int.Parse(items[i]));
                if (currentListItem.File != null)
                {
                    TreeNode tn = new TreeNode();
                    tn.ShowCheckBox = true;
                    tn.SelectAction = TreeNodeSelectAction.None;
                    tn.Checked = true;
                    tn.Text = currentListItem.File.Name;
                    tn.Value = web.Url + "/" + currentListItem.File.Url;
                    tn.ImageUrl = web.Url + "/_layouts/images/" + currentListItem.File.IconUrl;
                    treeViewSelectedDocs.Nodes.Add(tn);
                    tn = null;
                }
            }
        }

        private void DocLibRecursive(SPWeb web)
        {
            Logger.Instance.Info(String.Format("Batch Copy/Move - Executing DocLibRecursive For Web: {0}", web.Url),
                                        DiagnosticsCategories.eCaseWeb);
            List<SPDocumentLibrary> docLibList = new List<SPDocumentLibrary>();

            string strWebTemplate = web.WebTemplate;
            string strcurrWeb = currWeb.Title.ToString();
            string strWeb = web.Title.ToString();

            if (!strWebTemplate.Contains("SEARCH")) // Don't show search site libraries
            {
                if (web.DoesUserHavePermissions(SPBasePermissions.Open))
                {
                    var parentNode = new TreeNode(web.Title.ToString());
                    parentNode.SelectAction = TreeNodeSelectAction.None;
                    treeView.Nodes.Add(parentNode);

                    foreach (SPList list in web.Lists)
                    {
                        if (DoesUserHavePermissions(web, list.Title) && list.BaseType == SPBaseType.DocumentLibrary && !list.IsApplicationList && !list.Hidden && list.Title != "Form Templates" && list.Title != "Customized Reports" && list.Title != "Site Collection Documents" && list.Title != "Site Collection Images" && list.Title != "Images")
                        {
                            SPDocumentLibrary docLib = (SPDocumentLibrary)list;
                            if (docLib.IsCatalog == false)
                            {
                                Logger.Instance.Info(String.Format("Batch Copy/Move - DocLibRecursive - Building Tree Node For Library: {0}",
                                                        docLib.RootFolder.Url), DiagnosticsCategories.eCaseWeb);
                                TreeNode tn = new TreeNode();
                                TreeNode child = new TreeNode();

                                tn.ImageUrl = docLib.ImageUrl;
                                tn.Text = docLib.Title;
                                tn.Value = web.Url + "/" + docLib.RootFolder.Url;
                                if (docLib.Folders.Count > 0)
                                {
                                    var childItems = docLib.Folders;
                                    foreach (SPListItem childItem in childItems)
                                    {
                                        child = new TreeNode();
                                        child.ImageUrl = tn.ImageUrl; ;
                                        child.Text = childItem.Name;
                                        child.Value = web.Url + "/" + childItem.Url;
                                        tn.ChildNodes.Add(child);
                                        child = null;
                                    }
                                }
                                tn.ChildNodes.Add(tn);
                                parentNode.ChildNodes.Add(tn);
                                tn = null;
                            }
                        }
                    }

                    // Collapse all except current site 
                    if (strWeb == strcurrWeb)
                    {
                        parentNode.ExpandAll();
                    }
                    else
                    {
                        parentNode.CollapseAll();
                    }

                    //This fails if the user has the ability to open the web, but not enough rights to view the subwebs.
                    foreach (SPWeb subWeb in web.GetSubwebsForCurrentUser())
                    {
                        DocLibRecursive(subWeb);
                        subWeb.Dispose();
                    }
                }                
            }            
        }

        #region Copy File Methods

        private static SPFile CopyFile(SPFile copyFile, string destination)
        {
            string destinationFolderUrl = FormatFileDestination(destination);
            SPFile retVal = null;

            using (SPSite site = new SPSite(destinationFolderUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    if (web.Url == destinationFolderUrl) // copying within site collection
                    {
                        CopyInternal(web, copyFile, destinationFolderUrl, destination);
                    }
                    else // copying across site collections or elsewhere
                    {
                        CopyExternal(web, copyFile, destinationFolderUrl, destination);
                    }

                    retVal = web.GetFile(web.Url + "/" + destinationFolderUrl + copyFile.Name);
                }
            }

            return retVal;
        }

        private static SPFile CopyInternal(SPWeb web, SPFile copyFile, string destinationFolderUrl, string destination)
        {
            web.AllowUnsafeUpdates = true;
            //move the source file to the new location
            copyFile.CopyTo(SPUrlUtility.CombineUrl(destination, copyFile.Name), false);
            web.AllowUnsafeUpdates = false;

            return web.GetFile(web.Url + "/" + destinationFolderUrl + copyFile.Name);
        }

        private static SPFile CopyExternal(SPWeb web, SPFile copyFile, string destinationFolderUrl, string destination)
        {
            //copy the source file to the external location
            web.AllowUnsafeUpdates = true;
            SPFileCollection spFiles = web.GetFolder(destinationFolderUrl).Files;
            byte[] bFile = copyFile.OpenBinary();
            spFiles.Add(SPUrlUtility.CombineUrl(destination, copyFile.Name), bFile, false);
            web.AllowUnsafeUpdates = false;

            return web.GetFile(web.Url + "/" + destinationFolderUrl + copyFile.Name);
        }

        # endregion

        # region Move File Methods

        private static SPFile MoveFile(SPFile moveFile, string destination)
        {
            string destinationFolderUrl = FormatFileDestination(destination);
            SPFile retVal = null;

            // "move" the source file to the new location
            using (SPSite site = new SPSite(destinationFolderUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    if (web.Url == destinationFolderUrl) // moving within site collection
                    {
                        retVal = MoveInternal(web, moveFile, destinationFolderUrl, destination);
                    }
                    else
                    {
                        retVal = MoveExternal(web, moveFile, destinationFolderUrl, destination);
                    }
                }
            }

            return retVal;
        }

        private static SPFile MoveInternal(SPWeb web, SPFile moveFile, string destinationFolderUrl, string destination)
        {
            web.AllowUnsafeUpdates = true;
            //move the source file to the new location
            moveFile.MoveTo(SPUrlUtility.CombineUrl(destination, moveFile.Name), false);
            web.AllowUnsafeUpdates = false;

            return web.GetFile(web.Url + "/" + destinationFolderUrl + moveFile.Name);
        }

        private static SPFile MoveExternal(SPWeb web, SPFile moveFile, string destinationFolderUrl, string destination)
        {
            web.AllowUnsafeUpdates = true;
            SPFileCollection spFiles = web.GetFolder(destinationFolderUrl).Files;
            byte[] bFile = moveFile.OpenBinary();
            spFiles.Add(SPUrlUtility.CombineUrl(destination, moveFile.Name), bFile, false);
            web.AllowUnsafeUpdates = false;

            // delete the source file to complete our "move"
            SPWeb sourceWeb = SPContext.Current.Web;
            SPFolder sourceFolder = moveFile.ParentFolder;
            SPFile sourceFile = sourceFolder.Files[moveFile.Name];
            sourceFile.Delete();

            return web.GetFile(web.Url + "/" + destinationFolderUrl + moveFile.Name);
        }

        #endregion

        #endregion
    }
}
