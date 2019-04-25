using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.eCaseRootWeb.EventReceivers.SavedQueriesListER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class SavedQueriesListER : SPItemEventReceiver
    {
        /// <summary>
        /// An item has been added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
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
                        // Set our new permissions for the author and anyone the item is shared with
                        AddCustomPermissions(myItem, web);
                        web.AllowUnsafeUpdates = false;
                    }
                }
            }
            );
        }

        private void AddCustomPermissions(SPListItem myItem, SPWeb web)
        {
            // Give Contribute to the author
            SPRoleDefinition roledefinition = web.RoleDefinitions.GetByType(SPRoleType.Contributor);
            SPRoleAssignment roleAssignment = new SPRoleAssignment(GetSPUser(myItem, "Author"));
            roleAssignment.RoleDefinitionBindings.Add(roledefinition);
            myItem.RoleAssignments.Add(roleAssignment);

            // Give Read to anyone that the item has been shared with
            if (myItem["SharedWith"] != null)
            {
                SPFieldUserValueCollection values = (SPFieldUserValueCollection)myItem["SharedWith"];

                foreach (SPFieldUserValue value in values)
                {
                    // Get a user from the key
                    var sharedUser = value.User;
                    // Change our role definition to Read
                    roledefinition = web.RoleDefinitions.GetByType(SPRoleType.Reader);
                    // Change our role assignment to the new user
                    roleAssignment = new SPRoleAssignment(sharedUser);
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
        /// An item has been updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
        }
    }
}