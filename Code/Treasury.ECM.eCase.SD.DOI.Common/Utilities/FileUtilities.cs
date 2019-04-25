using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities
{
    public static class FileUtilities
    {
        public static string FormatFileDestination(string destination)
        {
            destination = (!destination.EndsWith("/")) ? destination + "/" : destination;
            return destination;
        }

        #region Copy File Methods

        public static SPFile CopyFile(SPFile copyFile, string destination)
        {
            string destinationFolderUrl = FormatFileDestination(destination);
            SPFile retVal = null;

            using (SPSite site = new SPSite(destinationFolderUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //if (web.Url != destinationFolderUrl) // copying within site collection
                    if (destinationFolderUrl.StartsWith(web.Url)) //copying within site collection
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

        public static SPFile CopyInternal(SPWeb web, SPFile copyFile, string destinationFolderUrl, string destination)
        {
            web.AllowUnsafeUpdates = true;
            //move the source file to the new location
            copyFile.CopyTo(SPUrlUtility.CombineUrl(destination, copyFile.Name), false);
            web.AllowUnsafeUpdates = false;

            return web.GetFile(web.Url + "/" + destinationFolderUrl + copyFile.Name);
        }

        public static SPFile CopyExternal(SPWeb web, SPFile copyFile, string destinationFolderUrl, string destination)
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

        //Note that these overloads for MoveFile are both here right now as they were consolidated from multiple locations. 
        //They could likely be combined into one method with some time to test and validate the end results.
        public static SPFile MoveFile(SPFile moveFile, string destination)
        {
            string destinationFolderUrl = FormatFileDestination(destination);
            SPFile retVal = null;

            // "move" the source file to the new location
            using (SPSite site = new SPSite(destinationFolderUrl))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    //if (web.Url != destinationFolderUrl) // moving within site collection
                    if (destinationFolderUrl.StartsWith(web.Url)) // moving within site collection
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

        //Note that these overloads for MoveFile are both here right now as they were consolidated from multiple locations. 
        //They could likely be combined into one method with some time to test and validate the end results.
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


        public static SPFile MoveInternal(SPWeb web, SPFile moveFile, string destinationFolderUrl, string destination)
        {
            web.AllowUnsafeUpdates = true;
            //move the source file to the new location
            moveFile.MoveTo(SPUrlUtility.CombineUrl(destination, moveFile.Name), false);
            web.AllowUnsafeUpdates = false;

            return web.GetFile(web.Url + "/" + destinationFolderUrl + moveFile.Name);
        }

        public static SPFile MoveExternal(SPWeb web, SPFile moveFile, string destinationFolderUrl, string destination)
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

    }
}
