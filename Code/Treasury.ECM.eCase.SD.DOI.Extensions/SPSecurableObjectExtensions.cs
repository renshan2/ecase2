using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPSecurableObjectExtensions
    {
        public static bool TryGrantPermission(this SPSecurableObject secObj, SPPrincipal principal, SPRoleDefinition roleDef)
        {
            bool retVal = false;
            try
            {
                SPRoleAssignment roleAssignment = new SPRoleAssignment(principal);
                roleAssignment.RoleDefinitionBindings.Add(roleDef);
                secObj.RoleAssignments.Add(roleAssignment);
            }
            catch (Exception x) { Logger.Instance.Error("Error in SPSecurableObject::TryGrantPermission", x, DiagnosticsCategories.eCaseExtensions); }

            return retVal;

        }
    }
}
