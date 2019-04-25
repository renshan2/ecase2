using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPWebExtensions
    {
        /// <summary>
        /// First attempts to locate an OOTB role definition from the SPRoleType enumeration.  If no role matches the 
        /// provided name, it iterates over all roles in the SPWeb until it finds the matching definition.
        /// </summary>
        /// <param name="web">the web containing role definitions</param>
        /// <param name="roleName">the name of the role desired</param>
        /// <param name="roleDef"></param>
        /// <returns>returns True if a match is found, otherwise false</returns>
        public static bool TryGetRoleDefinition(this SPWeb web, string roleName, out SPRoleDefinition roleDef)
        {
            bool retVal = false;
            
            SPRoleDefinitionCollection roles = web.RoleDefinitions;
            try
            {
                // Try to get the definition based on the OOTB RoleTypes (Reader, Contributor, etc), ignoring case
                roleDef = roles.GetByType((SPRoleType)Enum.Parse(typeof(SPRoleType), roleName, true));
            }
            catch (Exception x) 
            {
                Logger.Instance.Info("Error in TryGetRoleDefinition", x, DiagnosticsCategories.eCaseExtensions);
                roleDef = null; 
            }

            if (roleDef == null)
            {
                foreach (SPRoleDefinition role in roles)
                {
                    // Case insensitive comparison
                    if (string.Compare(role.Name, roleName, true) == 0)
                    {
                        roleDef = role;
                        break;
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Grants the specified user or group the specified permission level
        /// </summary>
        /// <param name="web">the effected object</param>
        /// <param name="roleName">the SPRoleType or SPRoleDefinition Name</param>
        /// <param name="principalName">the login name of the user, or name of the group</param>
        /// <returns></returns>
        public static bool TryGrantPermission(this SPWeb web, string principalName, string roleName)
        {
            bool retVal = false;
            bool origAllowUnsafeUpdatesBool = web.AllowUnsafeUpdates;
            try
            {
                web.AllowUnsafeUpdates = true;
                SPPrincipal principal;
                if (web.TryGetPrincipal(principalName, out principal))
                {
                    SPRoleDefinition roleDefinition;
                    if (web.TryGetRoleDefinition(roleName, out roleDefinition))
                        web.TryGrantPermission(principalName, roleDefinition);
                }
            }
            catch (Exception x) { Logger.Instance.Error("Error in TryGrantPermission", x, DiagnosticsCategories.eCaseExtensions); }
            finally
            {
                web.AllowUnsafeUpdates = origAllowUnsafeUpdatesBool;
            }

            return retVal;
        }
        public static bool TryGrantPermission(this SPWeb web, string principalName, SPRoleType roleType)
        {
            bool retVal = false;
            SPRoleDefinition roleDef;
            if (web.TryGetRoleDefinition(Enum.GetName(typeof(SPRoleType), roleType), out roleDef))
                retVal = web.TryGrantPermission(principalName, roleDef);
            return retVal;
        }
        public static bool TryGrantPermission(this SPWeb web, SPPrincipal principal, SPRoleType roleType)
        {
            return web.TryGrantPermission(principal, web.RoleDefinitions.GetByType(roleType));
        }
        public static bool TryGrantPermission(this SPWeb web, string principalName, SPRoleDefinition roleDef)
        {
            bool retVal = false;
            bool origAllowUnsafeUpdatesBool = web.AllowUnsafeUpdates;
            try
            {
                web.AllowUnsafeUpdates = true;
                SPPrincipal principal;
                if (web.TryGetPrincipal(principalName, out principal))
                    web.TryGrantPermission(principal, roleDef);
            }
            catch (Exception x) { Logger.Instance.Error("Error in TryGrantPermission", x, DiagnosticsCategories.eCaseExtensions); }
            finally
            {
                web.AllowUnsafeUpdates = origAllowUnsafeUpdatesBool;
            }

            return retVal;

        }
        public static bool TryGrantPermission(this SPWeb web, SPPrincipal principal, SPRoleDefinition roleDef)
        {
            bool retVal = false;
            bool origAllowUnsafeUpdatesBool = web.AllowUnsafeUpdates;
            try
            {
                web.AllowUnsafeUpdates = true;
                SPRoleAssignment roleAssignment = new SPRoleAssignment(principal);
                roleAssignment.RoleDefinitionBindings.Add(roleDef);

                //adds permissions to site
                web.RoleAssignments.Add(roleAssignment);
            }
            catch (Exception x) { Logger.Instance.Error("Error in TryGrantPermission", x, DiagnosticsCategories.eCaseExtensions); }
            finally
            {
                web.AllowUnsafeUpdates = origAllowUnsafeUpdatesBool;
            }

            return retVal;

        }
        
        /// <summary>
        /// Creates a group, if the group does not already exist.  Sets the following properties:
        /// AllowRequestToJoinLeave = true
        /// OnlyAllowMembersViewMembership = true
        /// AllowMembersEditMembership = false
        /// </summary>
        /// <param name="web">The web where the group will be created</param>
        /// <param name="groupName">The name of the group</param>
        /// <param name="groupDescription">The group description</param>
        /// <param name="owner">The group's owner</param>
        public static SPGroup CreateGroup(this SPWeb web, string groupName, string groupDescription, SPMember owner)
        {
            SPGroup group;
            if (web.TryGetGroup(groupName, out group) == false)
            {
                SPGroupCollection gc = web.SiteGroups;
                gc.Add(groupName, owner, web.Site.Owner, groupDescription);

                group = gc[groupName];
                group.AllowRequestToJoinLeave = false;
                group.OnlyAllowMembersViewMembership = true;
                group.AllowMembersEditMembership = false;
                group.Update();
            }
            return group;
        }
       
        /// <summary>
        /// Retrieves an SPPrincipal object based on its name.  It first checks SiteGroups matching group name, and then proceeds 
        /// to check SiteUsers matching login name.  If neither check locates a principal, null is returned.
        /// </summary>
        /// <param name="user">The affected user.</param>
        /// <param name="web">The affected web.</param>
        /// <returns>boolean indicating whether a matching SPPrincipal was found</returns>
        public static bool TryGetPrincipal(this SPWeb web, string principalName, out SPPrincipal principal)
        {
            // First check if it's a SPGroup, otherwise check if it's a user
            try { principal = web.SiteGroups[principalName]; }
            catch (Exception) 
            { 
                try { principal = web.SiteUsers[principalName]; }
                catch (Exception) { principal = null; }
            }

            bool retVal = false;
            if (principal != null)
                retVal = true;

            return retVal;
        }
        
        /// <summary>
        /// Retrieves an SPGroup object from SiteGroups, returning whether it succeeded or not.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupName"></param>
        /// <param name="group"></param>
        /// <returns>True if the group was found, otherwise False</returns>
        public static bool TryGetGroup(this SPWeb web, string groupName, out SPGroup group)
        {
            bool retVal = false;
            try 
            { 
                group = web.SiteGroups[groupName];
                retVal = true;
            }
            catch (Exception x) 
            {
                Logger.Instance.Error(string.Format("Group {0} not found at {1}", groupName, web.Url), x, DiagnosticsCategories.eCaseExtensions);
                group = null; 
            }
            return retVal;
        }

        /// <summary>
        /// Use this method instead of the OOTB EnsureUser method.  This method is a wrapper for the aforementioned method, which
        /// first checks SPWeb.SiteUsers for the selected user, and, if not found, determines if Elevation is required to complete the 
        /// operation (EnsureUser typically requires Elevation if the user is not already present).
        /// </summary>
        /// <param name="web"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        public static SPUser EnsureUserProperly(this SPWeb web, string loginName) 
        {   
            SPUser user = null;
            
            // Check SiteUsers for login first
            try { user = web.SiteUsers[loginName]; }
            catch (Exception x)
            {
                Logger.Instance.Info(string.Format("User {0} not found at {1}", loginName, web.Url), x, DiagnosticsCategories.eCaseExtensions);
                if (web.CurrentUser.IsSiteAdmin)
                    user = web.SafeEnsureUser(loginName);
                else
                {
                    Guid siteID = new Guid(web.Site.ID.ToByteArray());
                    Guid webID = new Guid(web.ID.ToByteArray());
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (SPSite elevatedSite = new SPSite(siteID))
                        {
                            using (SPWeb elevatedWeb = elevatedSite.OpenWeb(webID))
                            {
                                elevatedWeb.SafeEnsureUser(loginName);
                            }
                        }
                    });
                }
            }

            // User should exist at this point, but SPUser object might not be populated
            if (user == null)
            {
                try
                {
                    user = web.SiteUsers[loginName];
                }
                catch (Exception x) { Logger.Instance.Info(string.Format("EnsureUserProperly: {0} does not exist at {1}", loginName, web.Url), x, DiagnosticsCategories.eCaseExtensions); }
            }

            return user;
        }
        private static SPUser SafeEnsureUser(this SPWeb web, string loginName)
        {
            SPUser user = null;
            bool oldAllowUnsafeUpdate = web.AllowUnsafeUpdates;
            try
            {
                web.AllowUnsafeUpdates = true;
                user = web.EnsureUser(loginName);
            }
            catch (Exception x) { Logger.Instance.Error(string.Format("SafeEnsureUser: {0} does not exist", loginName), x, DiagnosticsCategories.eCaseExtensions); }
            finally
            {
                web.AllowUnsafeUpdates = oldAllowUnsafeUpdate;
            }
            return user; 
        }

        /// <summary>
        /// First attempt to get the list by standard Url pattern (e.g. http://web/url/Lists/ListInternalName)
        /// If that fails, iterates over all lists in the web and compares SPList.RootFolder.Name property to internalName
        /// </summary>
        /// <param name="web"></param>
        /// <param name="internalName">The root folder name</param>
        /// <returns></returns>
        public static SPList GetListByInternalName(this SPWeb web, string internalName)
        {
            SPList returnList = null;
            try
            {
                // Lets try to find the list quickly by using the standard URL (e.g. http://web/url/Lists/ListInternalName)
                returnList = web.GetList(string.Format("{0}/Lists/{1}", web.ServerRelativeUrl.TrimEnd('/'), internalName));
            }
            catch (Exception x)
            {
                // We failed to find the list in the default location, we'll have to inspect each list in the web
                returnList = (SPList)(from SPList list in web.Lists
                                      where list.RootFolder.Name.Equals(internalName, StringComparison.InvariantCulture)
                                      select list).FirstOrDefault();
            }

            return returnList;
        }

        public static string GetValidNewWebUrl(this SPWeb web, string desiredUrl)
        {
            string validUrl = desiredUrl;
            if (!string.IsNullOrEmpty(desiredUrl))
            {
                //// Illegal Chars: " # & * / \ : < > ? + ; ' 
                //Regex _illegalPathChars = new Regex(@"^\.|[\x00-\x1F,\x7B-\x9F,"",#,%,&,*,/,:,<,>,?,\\,+,;, ,']+|(\.\.)+|\.$", RegexOptions.Compiled);
                //validUrl = _illegalPathChars.Replace(HttpUtility.UrlDecode(desiredUrl.Trim()), "_");
                validUrl = desiredUrl.GenerateSlug();

                if (validUrl.Length > 128)
                    validUrl = validUrl.Substring(0, 126); // Only take 126, as we may need to add 2-digit numbers to the URL in a later step

                string tempUrl;
                int count = 1;
                SPWeb tempWeb = null;
                try
                {
                    tempWeb = web.Webs[validUrl];
                    if (tempWeb.Exists)
                    {
                        // Add numbers incrementally to the URL until a valid one is found
                        do
                        {
                            tempUrl = validUrl;
                            tempUrl += count.ToString();

                            if (count > 99)
                                throw new InvalidOperationException(string.Format("Unable to create case site. Sites already exist at {0}.", desiredUrl));
                            else
                                count++;

                            tempWeb.Dispose();
                            tempWeb = web.Webs[tempUrl];
                        } while (tempWeb.Exists);
                        validUrl = tempUrl;
                    }
                }
                catch (Exception x) 
                { 
                    Logger.Instance.Error(string.Format("Failed to identify new valid web at {0} with seed {1}", web.Url, desiredUrl), x, DiagnosticsCategories.eCaseExtensions);
                    throw x;
                }
                finally { if (tempWeb != null && tempWeb.Exists) tempWeb.Dispose(); }
            }
            else
                throw new ArgumentNullException("desiredUrl may not be null or empty");

            return HttpUtility.UrlEncode(validUrl);
        }


    }
}
