using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPListExtensions
    {
        /// <summary>
        /// Removes the specified role from the specified principal's role assignments.  This method will throw an 
        /// exception if the supplied principal cannot be found.
        /// </summary>
        /// <param name="list">the effected list</param>
        /// <param name="principalName">the login name of a user, or name of a group</param>
        /// <param name="roleName">the name of a role definition in the item's parent web</param>
        /// <returns>True if permission is found and removed, otherwise false</returns>
        public static bool TryRemovePermission(this SPList list, string principalName, string roleName)
        {
            bool retVal = false;
            SPPrincipal principal;
            if (list.ParentWeb.TryGetPrincipal(principalName, out principal))
            {
                SPRoleDefinition roleDef;
                if (list.ParentWeb.TryGetRoleDefinition(roleName, out roleDef))
                    retVal = list.TryRemovePermission(principal, roleDef);
            }
            else
                throw new Exception(string.Format("Cannot find Principal: {0}", principalName));
            return retVal;
            
        }
        /// <summary>
        /// Removes the specified role from the specified principal's role assignments.  This method will not 
        /// break role inheritance, and instead will return False.
        /// </summary>
        /// <param name="list">the effected list</param>
        /// <param name="principal">the principal whose permissions will be modified</param>
        /// <param name="roleDef">the role to remove from the principal's role assignments</param>
        /// <returns>True if the role is successfully removed, otherwise false</returns>
        public static bool TryRemovePermission(this SPList list, SPPrincipal principal, SPRoleDefinition roleDef)
        {
            bool retVal = true;
            if (principal == null || list.HasUniqueRoleAssignments == false)
                retVal = false;
            else
            {
                try
                {
                    SPRoleAssignment roleAssignment = list.RoleAssignments.GetAssignmentByPrincipal(principal);
                    if (roleAssignment != null)
                    {
                        list.RoleAssignments.Remove(principal);
                        if (roleDef != null && roleAssignment.RoleDefinitionBindings.Contains(roleDef))
                        {
                            roleAssignment.RoleDefinitionBindings.Remove(roleDef);
                            if (roleAssignment.RoleDefinitionBindings.Count > 0)
                                list.RoleAssignments.Add(roleAssignment);
                        }
                    }
                }
                catch (ArgumentOutOfRangeException) { }
                catch (ArgumentException) { }
            }
            return retVal;
        }

        /// <summary>
        /// Grants the specified principal the specified role definition for this item.
        /// </summary>
        /// <param name="list">the effected list</param>
        /// <param name="principal">the target entity (SPUser, SPGroup)</param>
        /// <param name="roleDef">the permission level (Reader, Contributor, etc)</param>
        /// <returns></returns>
        public static bool TryGrantPermission(this SPList list, SPPrincipal principal, SPRoleDefinition roleDef)
        {
            if (principal == null || list.HasUniqueRoleAssignments == false)
                return false;
            else
            {
                SPRoleAssignment roleAssignment = null;
                try
                {
                    roleAssignment = list.RoleAssignments.GetAssignmentByPrincipal(principal);
                }
                catch (ArgumentOutOfRangeException) { }
                catch (ArgumentException) { }

                if (roleAssignment == null)
                {
                    roleAssignment = new SPRoleAssignment(principal);
                    roleAssignment.RoleDefinitionBindings.Add(roleDef);

                    // set principal role assignment
                    list.RoleAssignments.Add(roleAssignment);
                }
                else
                {
                    if (!roleAssignment.RoleDefinitionBindings.Contains(roleDef))
                    {
                        SPRoleDefinitionBindingCollection binding = new SPRoleDefinitionBindingCollection();
                        binding.Add(roleDef);
                        roleAssignment.ImportRoleDefinitionBindings(binding);
                        list.RoleAssignments.Add(roleAssignment);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Grants the specified principal the specified role definition for this item.
        /// </summary>
        /// <param name="list">the effected list</param>
        /// <param name="principal">the login name of a user, or name of a group</param>
        /// <param name="roleDef">the name of a permission level (Reader, Contributor, etc)</param>
        /// <returns></returns>
        public static bool TryGrantPermission(this SPList list, string principalName, string roleName)
        {
            bool retVal = false;
            SPPrincipal principal;
            if (list.ParentWeb.TryGetPrincipal(principalName, out principal))
            {
                SPRoleDefinition roleDef;
                if (list.ParentWeb.TryGetRoleDefinition(roleName, out roleDef))
                    retVal = list.TryGrantPermission(principal, roleDef);
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
        /// <param name="list"></param>
        public static void BreakCopyRoleInheritance(this SPList list)
        {
            SPRoleAssignmentCollection roleAssignments = list.RoleAssignments;

            var activeAssignments = from SPRoleAssignment p in roleAssignments
                                    where p.RoleDefinitionBindings.Count >= 1
                                    select p;

            list.BreakRoleInheritance(false);

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
                    list.RoleAssignments.Add(assignment);
                }
            }
        }


    }
}
