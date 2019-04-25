using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Microsoft.Office.DocumentManagement.DocumentSets;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPListItemExtensions
    {
        /// <summary>
        /// SharePoint is incapable of using this URL to display a list item.  If a URL to the View page is
        /// desired, use GetDisplayFormUrl
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GetServerRelativeUrl(this SPListItem item)
        {
            // SPListItem.Url is a web-relative URL (e.g. /Lists/ListName/ItemId), not server relative
            return string.Format("{0}/{1}", item.Web.ServerRelativeUrl, item.Url);
        }

        public static string GetDisplayFormUrl(this SPListItem item)
        {
            return string.Format("{0}?ID={1}", item.ParentList.DefaultDisplayFormUrl, item.ID);
        }

        public static bool IsDocumentSetItem(this SPListItem item)
        {
            bool documentSetItem = false;

            try
            {
                DocumentSet documentSet = null;
                if (null != item && null != item.File)
                {
                    documentSet = DocumentSet.GetDocumentSet(item.File.ParentFolder);
                    if (documentSet != null && documentSet.ContentType != null)
                    {
                        if (documentSet.ContentType.Id.IsChildOf(SPBuiltInContentTypeId.DocumentSet))
                            documentSetItem = true;
                    }
                }
            }
            catch (NullReferenceException nullEx)
            {
                //TODO: Find a better way to do this
                //if our content type is null, then we are likely dealing with a document library
                //so we'll swallow this here
            }
            return documentSetItem;
        }

        public static SPUser GetFieldAsSPUser(this SPListItem item, Guid userFieldGuid)
        {
            SPUser user = null;
            // Verify that a user field exists
            SPFieldUser userField = item.Fields[userFieldGuid] as SPFieldUser;
            if (userField != null && item[userFieldGuid] != null)
            {
                // Convert the user field into an SPUser object
                SPFieldUserValue userFieldValue = userField.GetFieldValue(item[userFieldGuid].ToString()) as SPFieldUserValue;
                if (userFieldValue != null)
                    user = userFieldValue.User;
                else
                {
                    Logger.Instance.Error(string.Format("Cannot get user field {0} at {1}", userFieldGuid.ToString(), item.Url), DiagnosticsCategories.eCaseExtensions);
                }
            }
            else
            {
                Logger.Instance.Error(string.Format("Field with GUID {0} not found in List {1}", userFieldGuid, item.ParentList.RootFolder.Url), DiagnosticsCategories.eCaseExtensions);
            }

            return user;
        }

        public static void SetFieldAsSPUser(this SPListItem item, Guid userFieldGuid, SPUser user)
        {
            SPFieldUser userField = item.Fields[userFieldGuid] as SPFieldUser;
            if (userField != null && user != null)
                item[userFieldGuid] = new SPFieldUserValue(item.Web, user.ID, user.Name);
        }

        public static SPFieldLookupValue GetFieldAsSPLookup(this SPListItem item, Guid fieldGuid)
        {
            SPFieldLookupValue spflv = null;
            try
            {
                string fieldValue = item[fieldGuid] as string;
                spflv = new SPFieldLookupValue(fieldValue);
            }
            catch (Exception x)
            {
                throw x;
            }
            return spflv;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="fieldName"></param>
        /// <param name="parentList"></param>
        /// <returns></returns>
        public static SPListItem GetFieldLookupAsSPListItem(this SPListItem item, string fieldName)
        {
            SPListItem lookupItem = null;
            try
            {
                SPFieldLookup spfl = item.ParentList.Fields.GetFieldByInternalName(fieldName) as SPFieldLookup;
                SPFieldLookupValue spflv = item.GetFieldAsSPLookup(spfl.Id);
                lookupItem = spflv.GetListItem(spfl);
            }
            catch (Exception x)
            {
                throw x;
            }

            return lookupItem;
        }

        /// <summary>
        /// Removes the specified role from the specified principal's role assignments.  This method will throw an 
        /// exception if the supplied principal cannot be found.
        /// </summary>
        /// <param name="item">the effected list item</param>
        /// <param name="principalName">the login name of a user, or name of a group</param>
        /// <param name="roleName">the name of a role definition in the item's parent web</param>
        /// <returns>True if permission is found and removed, otherwise false</returns>
        public static bool TryRemovePermission(this SPListItem item, string principalName, string roleName)
        {
            bool retVal = false;
            SPPrincipal principal;
            if (item.Web.TryGetPrincipal(principalName, out principal))
            {
                SPRoleDefinition roleDef;
                if (item.Web.TryGetRoleDefinition(roleName, out roleDef))
                    retVal = item.TryRemovePermission(principal, roleDef);
            }
            else
                throw new Exception(string.Format("Cannot find Principal: {0}", principalName));
            return retVal;
            
        }
        /// <summary>
        /// Removes the specified role from the specified principal's role assignments.  This method will not 
        /// break role inheritance, and instead will return False.
        /// </summary>
        /// <param name="item">the effected list item</param>
        /// <param name="principal">the principal whose permissions will be modified</param>
        /// <param name="roleDef">the role to remove from the principal's role assignments. If null, all permissions are removed</param>
        /// <returns>True if the role is successfully removed, otherwise false</returns>
        public static bool TryRemovePermission(this SPListItem item, SPPrincipal principal, SPRoleDefinition roleDef)
        {
            bool retVal = true;
            if (principal == null || item.HasUniqueRoleAssignments == false)
                retVal = false;
            else
            {
                try
                {
                    SPRoleAssignment roleAssignment = item.RoleAssignments.GetAssignmentByPrincipal(principal);
                    if (roleAssignment != null)
                    {
                        item.RoleAssignments.Remove(principal);
                        if (roleDef != null && roleAssignment.RoleDefinitionBindings.Contains(roleDef))
                        {
                            roleAssignment.RoleDefinitionBindings.Remove(roleDef);
                            if (roleAssignment.RoleDefinitionBindings.Count > 0)
                                item.RoleAssignments.Add(roleAssignment);
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { }
                catch (ArgumentException) { }
            }
            return retVal;
        }

        public static bool TryRemovePermissions(this SPListItem item, SPPrincipal principal)
        {
            bool retVal = true;
            if (principal == null || item.HasUniqueRoleAssignments == false)
                retVal = false;
            else
            {
                try
                {
                    SPRoleAssignment roleAssignment = item.RoleAssignments.GetAssignmentByPrincipal(principal);
                    if (roleAssignment != null)
                        item.RoleAssignments.Remove(principal);
                }
                catch (ArgumentOutOfRangeException) { }
                catch (ArgumentException) { }
            }
            return retVal;
        }

        public static bool TryGrantPermission(this SPListItem item, SPPrincipal principal, SPRoleType roleType)
        {
            return item.TryGrantPermission(principal, item.Web.RoleDefinitions.GetByType(roleType));
        }

        /// <summary>
        /// Grants the specified principal the specified role definition for this item.
        /// </summary>
        /// <param name="item">the effected object</param>
        /// <param name="principal">the target entity (SPUser, SPGroup)</param>
        /// <param name="roleDef">the permission level (Reader, Contributor, etc)</param>
        /// <returns></returns>
        public static bool TryGrantPermission(this SPListItem item, SPPrincipal principal, SPRoleDefinition roleDef)
        {
            if (principal == null || item.HasUniqueRoleAssignments == false)
                return false;
            else
            {
                SPRoleAssignment roleAssignment = null;
                try
                {
                    roleAssignment = item.RoleAssignments.GetAssignmentByPrincipal(principal);
                }
                catch (ArgumentOutOfRangeException) { }
                catch (ArgumentException) { }

                if (roleAssignment == null)
                {
                    roleAssignment = new SPRoleAssignment(principal);
                    roleAssignment.RoleDefinitionBindings.Add(roleDef);

                    // set principal role assignment
                    item.RoleAssignments.Add(roleAssignment);
                }
                else
                {
                    if (!roleAssignment.RoleDefinitionBindings.Contains(roleDef))
                    {
                        SPRoleDefinitionBindingCollection binding = new SPRoleDefinitionBindingCollection();
                        binding.Add(roleDef);
                        roleAssignment.ImportRoleDefinitionBindings(binding);
                        item.RoleAssignments.Add(roleAssignment);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Grants the specified principal the specified role definition for this item.
        /// </summary>
        /// <param name="item">the effected object</param>
        /// <param name="principal">the login name of a user, or name of a group</param>
        /// <param name="roleDef">the name of a permission level (Reader, Contributor, etc)</param>
        /// <returns></returns>
        public static bool TryGrantPermission(this SPListItem item, string principalName, string roleName)
        {
            bool retVal = false;
            SPPrincipal principal;
            if (item.Web.TryGetPrincipal(principalName, out principal))
            {
                SPRoleDefinition roleDef;
                if (item.Web.TryGetRoleDefinition(roleName, out roleDef))
                    retVal = item.TryGrantPermission(principal, roleDef);
            }
            return retVal;
        }

        /// <summary>
        /// This method should be used instead of BreakRoleInheritance(true).
        /// 
        /// The OOTB method replicates all permissions from the parent, including the "Limited Access" permissions.  
        /// In environments with many users, there can be many of these meaningless permissions, which bloats the 
        /// SharePoint permission tables in SQL. This method reviews all RoleDefinitions in each principal's 
        /// RoleAssignments, stripping "Limited Access", before adding them to the item.
        /// </summary>
        /// <param name="item"></param>
        public static void BreakCopyRoleInheritance(this SPListItem item)
        {
            SPRoleAssignmentCollection roleAssignments = item.RoleAssignments;

            var activeAssignments = from SPRoleAssignment p in roleAssignments
                                    where p.RoleDefinitionBindings.Count >= 1
                                    select p;
            
            item.BreakRoleInheritance(false);

            foreach (SPRoleAssignment p in activeAssignments)
            {
                SPRoleAssignment assignment = new SPRoleAssignment(p.Member);
                SPRoleDefinitionBindingCollection bindings = new SPRoleDefinitionBindingCollection();

                foreach (SPRoleDefinition roleDef in p.RoleDefinitionBindings)
                {
                    if (roleDef.Name != "Limited Access")
                        bindings.Add(roleDef);
                }
                if (bindings.Count > 0)
                {
                    assignment.ImportRoleDefinitionBindings(bindings);
                    item.RoleAssignments.Add(assignment);
                }
            }
        }
    }
}
