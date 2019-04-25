using System.Collections.Generic;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.SharePoint;
using Microsoft.Office.Server.Search.Query;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.FAST
{
    /// <summary>
    /// Read in all fql created scopes
    /// Used for building fql with the correct data types
    /// This source code is released under the MIT license
    /// </summary>
    internal class FastScopeReader
    {
        public static void PopulateScopes(Dictionary<string, string> scopeLookup)
        {
            Logger.Instance.Info("Entered PopulateScopes", DiagnosticsCategories.eCaseSearch);
            SPSecurity.RunWithElevatedPrivileges(
                delegate
                {
                    using (SPSite currentSite = new SPSite(SPContext.Current.Site.ID))
                    {                        
                        RemoteScopes remoteScopes = new RemoteScopes(SPServiceContext.GetContext(currentSite));

                        foreach (Scope scope in remoteScopes.GetScopesForSite(new System.Uri(currentSite.Url)))
                        {
                            scopeLookup[scope.Name.ToLower()] = scope.Filter;
                        }
                    }

                }
        );
        }
    }
}