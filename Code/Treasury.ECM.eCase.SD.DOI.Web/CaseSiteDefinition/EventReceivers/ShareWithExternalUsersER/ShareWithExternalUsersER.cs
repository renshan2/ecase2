using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.EventReceivers.ShareWithExternalUsersER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ShareWithExternalUsersER : SPItemEventReceiver
    {
        /// <summary>
        /// An item is being added.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {

        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite Site = new SPSite(properties.WebUrl))
                    {
                        using (SPWeb web = Site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            var myItem = web.Lists[properties.ListId].GetItemById(properties.ListItem.ID);
                            // Remove the current permissions
                            RemoveAllPermisions(myItem);
                            // Set our new permissions for anyone the item is shared with
                            AddCustomPermissions(myItem, web);
                            web.AllowUnsafeUpdates = false;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                //do some logging...
                throw ex;
            }
        }

        private void AddCustomPermissions(SPListItem myItem, SPWeb web)
        {
            // Give Read to anyone that the item has been shared with
            if (myItem["ShareWithUser"] != null)
            {
                var fieldValue = myItem["ShareWithUser"].ToString();
                SPFieldUserValueCollection values = new SPFieldUserValueCollection(myItem.Web, fieldValue);

                foreach (SPFieldUserValue value in values)
                {
                    // Get a user from the key
                    var sharedUser = value.User;
                    // Change our role definition to Read
                    var roledefinition = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                    // Change our role assignment to the new user
                    var roleAssignment = new SPRoleAssignment(sharedUser);
                    // Bind the new role
                    roleAssignment.RoleDefinitionBindings.Add(roledefinition);
                    myItem.RoleAssignments.Add(roleAssignment);
                }
            }
        }

        public static SPUser GetSPUser(SPListItem item, string key)
        {
            SPFieldUser field = item.Fields[key] as SPFieldUser;
            if (field != null)
            {
                SPFieldUserValue fieldValue =
                 field.GetFieldValue(item[key].ToString()) as SPFieldUserValue;
                if (fieldValue != null)
                    return fieldValue.User;
            }

            return null;
        }

        private void RemoveAllPermisions(SPListItem currentListItem)
        {
            //The below function Breaks the role assignment inheritance for the list and gives the current list its own copy of the role assignments
            currentListItem.BreakRoleInheritance(true);

            //Get the list of Role Assignments to list item and remove one by one.
            SPRoleAssignmentCollection SPRoleAssColn = currentListItem.RoleAssignments;
            for (int i = SPRoleAssColn.Count - 1; i >= 0; i--)
            {
                SPRoleAssColn.Remove(i);
            }
        }

        /// <summary>
        /// An item is being deleted.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleting(properties);
        }


    }
}