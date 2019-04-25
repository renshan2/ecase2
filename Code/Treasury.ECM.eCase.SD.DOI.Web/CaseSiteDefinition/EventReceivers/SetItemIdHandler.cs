using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.EventReceivers
{
    public static class SetItemIdHandler
    {
        public static void HandleItemMoving(SPItemEventProperties properties)
        {
            string destinationString = properties.AfterUrl.Substring(0, properties.AfterUrl.LastIndexOf("/"));
            SPFolder destinationFolder = properties.Web.GetFolder(destinationString);
            bool updateRequired = false;

            //if we are dealing with a document set, we need to do a few things
            int nextNumber = 1;
            if (properties.ListItem.File != null && SPFolderExtensions.IsDocumentSetFolder(destinationFolder))
            {
                // handle our SetItemID
                SPList docSetList = properties.List;
                SPQuery query = new SPQuery();
                query.RowLimit = 1;
                query.Folder = destinationFolder;
                query.Query = "<OrderBy><FieldRef Name='SetItemID' Ascending='FALSE' /></OrderBy>";
                SPListItemCollection Items = docSetList.GetItems(query);
                if (Items.Count > 0)
                {
                    SPListItem item = Items[0];
                    nextNumber = Convert.ToInt32(item["SetItemID"]) + 1;
                }

                properties.ListItem["SetItemID"] = nextNumber.ToString();
                updateRequired = true;
            }
            else
            {
                if (properties.ListItem["SetItemID"] != null && !string.IsNullOrEmpty(properties.ListItem["SetItemID"].ToString()))
                {
                    properties.ListItem["SetItemID"] = null;
                    updateRequired = true;
                }
            }

            if (updateRequired)
                properties.ListItem.Update();

            ReOrderSetItemID(properties, true);
        }

        public static void HandleItemAdded(SPItemEventProperties properties)
        {
            bool updateRequired = false;
            string webURL = properties.ListItem.Web.Url;
            //if we are dealing with a document set, we need to do a few things
            int nextNumber = 0;
            if (properties.ListItem.File != null && SPListItemExtensions.IsDocumentSetItem(properties.ListItem))
            {
                // handle our SetItemID
                if (properties.ListItem["SetItemID"] == null)
                {
                    SPList docSetList = properties.List;

                    SPQuery query = new SPQuery();
                    query.RowLimit = 1;
                    query.Folder = properties.ListItem.File.ParentFolder;
                    query.Query = "<OrderBy><FieldRef Name='SetItemID' Ascending='FALSE' /></OrderBy>";
                    SPListItemCollection Items = docSetList.GetItems(query);
                    if (Items.Count > 0)
                    {
                        SPListItem item = Items[0];
                        nextNumber = Convert.ToInt32(item["SetItemID"]) + 1;
                    }
                    properties.ListItem["SetItemID"] = nextNumber.ToString();
                    updateRequired = true;
                }
            }
            else
            {
                if (properties.ListItem["SetItemID"] != null && !string.IsNullOrEmpty(properties.ListItem["SetItemID"].ToString()))
                {
                    properties.ListItem["SetItemID"] = null;
                    updateRequired = true;
                }
            }

            if (updateRequired)
                properties.ListItem.Update();
        }

        public static void HandleItemDeleting(SPItemEventProperties properties)
        {
            var webURL = properties.ListItem.Web.Url;
            //if we are dealing with a document set, we need to do a few things
            if (properties.ListItem.File != null && SPListItemExtensions.IsDocumentSetItem(properties.ListItem))
            {
                //1. handle our SetItemID
                if (properties.BeforeProperties["SetItemID"] == null)
                {
                    SPList docSetList = properties.List;

                    SPQuery query = new SPQuery();
                    query.Folder = properties.ListItem.File.ParentFolder;
                    query.Query = "<Where><Neq><FieldRef Name='ID' /><Value Type='Integer'>" + properties.ListItem.ID + "</Value></Neq></Where><OrderBy><FieldRef Name='SetItemID' Ascending='TRUE' /></OrderBy>";
                    SPListItemCollection Items = docSetList.GetItems(query);
                    if (Items.Count > 0)
                    {
                        var j = 1;
                        SPListItem item = null;
                        for (int i = 0; i <= Items.Count - 1; i++)
                        {
                            item = Items[i];
                            item["SetItemID"] = j;
                            item.Update();
                            j = j + 1;
                        }
                    }
                }
            }
        }

        private static void ReOrderSetItemID(SPItemEventProperties properties, bool isSource)
        {
            string urlString = null;
            SPFolder spFolder = null;
            //This is intentionally started at 1 as we do not want a document with an ID of 0.
            int i = 1;

            //are we dealing with a source or destination library?
            if (isSource)
            {
                urlString = properties.BeforeUrl.Substring(0, properties.BeforeUrl.LastIndexOf("/"));
                spFolder = properties.Web.GetFolder(urlString);
            }
            else
            {
                urlString = properties.AfterUrl.Substring(0, properties.AfterUrl.LastIndexOf("/"));
                spFolder = properties.Web.GetFolder(urlString);
            }

            // reorder our SetItemID values in the source folder
            SPList docSetList = properties.List;
            SPQuery query = new SPQuery();
            query.Folder = spFolder;
            query.Query = "<OrderBy><FieldRef Name='SetItemID' Ascending='FALSE' /></OrderBy>";
            SPListItemCollection items = docSetList.GetItems(query);
            if (items.Count > 0)
            {
                foreach (SPListItem item in items)
                {
                    if (item.ID != properties.ListItem.ID)
                    {
                        item["SetItemID"] = i;
                        item.Update();
                        i++;
                    }
                }
            }
        }
    }
}
