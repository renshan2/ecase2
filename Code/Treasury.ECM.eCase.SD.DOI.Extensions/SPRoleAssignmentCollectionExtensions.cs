using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Microsoft.SharePoint;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPRoleAssignmentCollectionExtensions
    {
        /// <summary>
        /// This method will add RoleAssignment instances to a RoleAssignmentCollection based on the Xml supplied.  The
        /// Xml must conform to the SPPermissions.Xml format which is returned by the SPRoleAssignmentCollection.Xml property.
        /// </summary>
        /// <param name="roleAssignments">the object on which to operate</param>
        /// <param name="roleAssignmentsXml">the XML string to retrieve RoleAssignments from</param>
        /// <param name="web">the web containing the RoleDefinitions and SPPrincipals contained in the XML</param>
        /// <returns></returns>
        public static bool Add(this SPRoleAssignmentCollection roleAssignments, string roleAssignmentsXml, SPWeb web)
        {
            bool retVal = true;
            try
            {
                // Create XML Document from xml string
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(roleAssignmentsXml);
                XmlNodeList permissions = xDoc.SelectNodes("//permission");
                foreach (XmlNode permission in permissions)
                {
                    int memberId = Convert.ToInt32(permission.Attributes["memberid"].Value);
                    SPPrincipal principal = null;
                    try { principal = web.SiteUsers.GetByID(memberId) as SPPrincipal; }
                    catch (Exception x) { Logger.Instance.Info(string.Format("User with ID {0} not found at {1}", memberId, web.Url, x), DiagnosticsCategories.eCaseExtensions); }
                    if (principal == null)
                    {
                        try { principal = web.SiteGroups.GetByID(memberId) as SPPrincipal; }
                        catch (Exception x) 
                        {
                            Logger.Instance.Info(string.Format("Group with ID {0} not found at {1}", memberId, web.Url, x), DiagnosticsCategories.eCaseExtensions);
                            throw x;
                        }
                    }

                    ulong permissionMask = Convert.ToUInt64(permission.Attributes["mask"].Value);
                    SPRoleDefinition roleDefinition = null;
                    foreach (SPRoleDefinition roleDef in web.RoleDefinitions)
                    {
                        ulong mask = (ulong)roleDef.BasePermissions;
                        if (permissionMask == mask)
                        {
                            roleDefinition = roleDef;
                            break;
                        }
                    }
                    roleAssignments.ParentSecurableObject.TryGrantPermission(principal, roleDefinition);
                }
            }
            catch (Exception x) 
            {
                retVal = false;
                Logger.Instance.Error(string.Format("{0}: Failed to Add Permissions {1}", web.Url, roleAssignmentsXml), x, DiagnosticsCategories.eCaseExtensions); 
            }
            return retVal;
        }
    }
}
