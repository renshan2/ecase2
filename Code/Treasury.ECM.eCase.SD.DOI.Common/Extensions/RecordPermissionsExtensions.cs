using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Extensions
{
    public static class RecordPermissionsExtensions
    {
        public static void RecordPermissions(this SPSecurableObject secObj, DbAdapter dbConnection, Guid siteGuid, Guid caseWebGuid, Guid? childWebGuid, Guid? listGuid, Guid? listItemGuid)
        {
            string roleAssignmentsXml = secObj.RoleAssignments.Xml;
            CreateSPObjPermSProc sProc = new CreateSPObjPermSProc(siteGuid, caseWebGuid, childWebGuid, listGuid, listItemGuid, roleAssignmentsXml);
            dbConnection.ExecuteNonQueryStoredProcedure(sProc);
        }

        public static void RecordPermissions(this SPWeb web, SPWeb caseWeb, DbAdapter dbConnection)
        {
            foreach (SPList list in web.Lists)
                list.RecordPermissions(caseWeb, dbConnection);

            foreach (SPWeb childWeb in web.Webs)
            {
                try
                {
                    if (childWeb.HasUniqueRoleAssignments)
                    {
                        if (dbConnection.IsConnected)
                            ((SPSecurableObject)childWeb).RecordPermissions(dbConnection, web.Site.ID, caseWeb.ID, childWeb.ID, null, null);
                    }

                    childWeb.RecordPermissions(caseWeb, dbConnection);
                }
                catch (Exception x)
                { Logger.Instance.Error(string.Format("Failed to record SPWeb permissions at {0}", childWeb.Url), x, DiagnosticsCategories.eCaseExtensions); }
                finally { childWeb.Dispose(); }
            }
        }

        public static void RecordPermissions(this SPList list, SPWeb caseWeb, DbAdapter dbConnection)
        {
            if (list.HasUniqueRoleAssignments)
            {
                if (dbConnection.IsConnected)
                    ((SPSecurableObject)list).RecordPermissions(dbConnection, list.ParentWeb.Site.ID, caseWeb.ID, list.ParentWeb.ID, list.ID, null);
            }

            foreach (SPListItemInfo itemInfo in list.GetItemsWithUniquePermissions())
            {
                SPListItem item = list.GetItemById(itemInfo.Id);
                if (dbConnection.IsConnected)
                    ((SPSecurableObject)item).RecordPermissions(dbConnection, item.ParentList.ParentWeb.Site.ID, caseWeb.ID,
                        item.ParentList.ParentWeb.ID, item.ParentList.ID, item.UniqueId);
            }
        }
    }
}
