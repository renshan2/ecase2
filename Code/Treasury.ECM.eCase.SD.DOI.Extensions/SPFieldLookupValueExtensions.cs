using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPFieldLookupValueExtensions
    {
        public static SPListItem GetListItem(this SPFieldLookupValue spflv, SPFieldLookup spfl)
        {
            SPListItem item = null;
            try
            {
                SPList lookupList = spfl.ParentList.ParentWeb.Lists[new Guid(spfl.LookupList)];
                item = lookupList.GetItemById(spflv.LookupId);
            }
            catch (Exception x)
            {
                throw x;
            }
            return item;
        }
    }
}
