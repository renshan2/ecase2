using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Search.Extended.Administration;
using Microsoft.SharePoint.Search.Extended.Administration.Keywords;
using Keyword = Microsoft.SharePoint.Search.Extended.Administration.Keywords.Keyword;
using Synonym = Microsoft.SharePoint.Search.Extended.Administration.Keywords.Synonym;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.FAST
{
    /// <summary>
    /// Read in all synonyms and store as a lookup table
    /// Used for building fql with the correct data types
    /// This source code is released under the MIT license
    /// </summary>
    class FastSynonymReader
    {
        public static void PopulateSynonyms(Dictionary<string, List<string>> synonymLookup)
        {
            SPSecurity.RunWithElevatedPrivileges(
                delegate
                {
                    try
                    {
                        var ssaProxy = (SearchServiceApplicationProxy)SearchServiceApplicationProxy.GetProxy(SPServiceContext.Current);
                        if (ssaProxy.FASTAdminProxy != null)
                        {
                            var fastProxy = ssaProxy.FASTAdminProxy;

                            KeywordContext keywordContext = fastProxy.KeywordContext;
                            SearchSettingGroupCollection searchSettingGroupCollection = keywordContext.SearchSettingGroups;

                            DateTime currentDate = DateTime.Now;

                            foreach (SearchSettingGroup searchSettingGroup in searchSettingGroupCollection)
                            {
                                foreach (Keyword keyword in searchSettingGroup.Keywords)
                                {
                                    foreach (Synonym synonym in keyword.Synonyms)
                                    {
                                        if (synonym.StartDate < currentDate || synonym.EndDate > currentDate) continue;

                                        AddSynonym(keyword.Term, synonym.Term, synonymLookup);
                                        if (synonym.ExpansionType == SynonymExpansionType.TwoWay)
                                        {
                                            AddSynonym(synonym.Term, keyword.Term, synonymLookup);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (SecurityException secEx)
                    {
                        Logger.Instance.Error("Failed to populated synonyms", secEx, DiagnosticsCategories.eCaseSearch);
                        throw secEx;
                    }
                }
                );

        }

        private static void AddSynonym(string keywordTerm, string synonymTerm, Dictionary<string, List<string>> synonymLookup)
        {
            List<string> value;
            if (!synonymLookup.TryGetValue(keywordTerm, out value))
            {
                synonymLookup[keywordTerm] = value = new List<string>();
            }
            value.Add(synonymTerm);
        }
    }
}
