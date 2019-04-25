using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Microsoft.Office.DocumentManagement.DocumentSets;
using System.Web;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.EventReceivers.FinishedWorkProduceER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class FinishedWorkProduceER : SPItemEventReceiver
    {
        public override void ItemFileMoved(SPItemEventProperties properties)
        {
            base.ItemFileMoved(properties);
        }

        public override void ItemFileMoving(SPItemEventProperties properties)
        {
            try
            {
                SetItemIdHandler.HandleItemMoving(properties);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error in ItemFileMoving", ex, DiagnosticsCategories.eCaseWeb);
            }
        }

        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                SetItemIdHandler.HandleItemAdded(properties);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error in ItemAdded", ex, DiagnosticsCategories.eCaseWeb);
            }
        }

        public override void ItemDeleting(SPItemEventProperties properties)
        {
            try
            {
                SetItemIdHandler.HandleItemDeleting(properties);
            }
            catch (Exception ex)
            {
                Logger.Instance.Error("Error in ItemDeleting", ex, DiagnosticsCategories.eCaseWeb);
            }
        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            //try
            //{
            //    var webURL = properties.ListItem.Web.Url;
            //    //if we are dealing with a document set, we need to do a few things
            //    var nextNumber = 0;
            //    if (properties.ListItem.File != null && SPListItemExtensions.IsDocumentSetItem(properties.ListItem))
            //    {
            //        //1. handle our SetItemID
            //        if (properties.BeforeProperties["SetItemID"] == null)
            //        {
            //            SPList docSetList = properties.List;

            //            SPQuery query = new SPQuery();
            //            query.RowLimit = 1;
            //            query.Folder = properties.ListItem.File.ParentFolder;
            //            query.Query = "<OrderBy><FieldRef Name='SetItemID' Ascending='FALSE' /></OrderBy>";
            //            SPListItemCollection Items = docSetList.GetItems(query);
            //            if (Items.Count > 0)
            //            {
            //                SPListItem item = Items[0];
            //                nextNumber = Convert.ToInt32(item["SetItemID"]) + 1;
            //            }
            //            properties.AfterProperties["SetItemID"] = nextNumber.ToString();
            //        }

            //        ////2. handle our OriginalDocLink field
            //        //if (properties.BeforeProperties["OriginalDocLink"] == null)
            //        //    properties.AfterProperties["OriginalDocLink"] = webURL + "/" + HttpUtility.UrlEncode(properties.ListItem.File.Url);

            //        ////3. update our DocSetLinks field
            //        //if (properties.AfterProperties["DocSetLinks"] == null)
            //        //{
            //        //    properties.AfterProperties["DocSetLinks"] = webURL + "/" + properties.ListItem.File.ParentFolder.Url;
            //        //}
            //        //else
            //        //{
            //        //    properties.AfterProperties["DocSetLinks"] = properties.BeforeProperties["DocSetLinks"] + ";" + webURL + "/" + HttpUtility.UrlEncode(properties.ListItem.File.ParentFolder.Url);
            //        //}

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Instance.Error("Error in ItemUpdating", ex, DiagnosticsCategories.eCaseSite);
            //}
        }
    }
}
