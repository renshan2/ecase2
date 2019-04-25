using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPItemEventDataCollectionExtensions
    {
        public static SPFieldLookupValue GetFieldAsSPLookup(this SPItemEventDataCollection afterProperties, string fieldName)
        {
            SPFieldLookupValue spflv = null;
            try
            {
                string fieldValue = afterProperties[fieldName] as string;
                spflv = new SPFieldLookupValue(fieldValue);
            }
            catch (Exception x)
            {
                throw x;
            }
            return spflv;
        }

        public static SPListItem GetFieldLookupAsSPListItem(this SPItemEventDataCollection afterProperties, string fieldName, SPList parentList)
        {
            SPListItem item = null;
            try
            {
                SPFieldLookup spfl = parentList.Fields.GetFieldByInternalName(fieldName) as SPFieldLookup;
                SPFieldLookupValue spflv = afterProperties.GetFieldAsSPLookup(fieldName);
                item = spflv.GetListItem(spfl);
            }
            catch (Exception x)
            {
                throw x;
            }

            return item;
        }

        /// <summary>
        /// Converts the AfterProperties contents of a field to an SPUser using SPFieldUserValue and EnsureUserProperly as necessary.
        /// </summary>
        /// <param name="afterProperties">AfterProperties collection, typically from SPItemEventProperties</param>
        /// <param name="fieldName">the name of the field in AfterProperties.  If not found, null is returned</param>
        /// <param name="web">the web in which the SPFieldUserValue resides, typically properties.Web</param>
        /// <returns>An SPUser object that represents the stored user</returns>
        public static SPUser GetFieldAsSPUser(this SPItemEventDataCollection afterProperties, string fieldName, SPWeb web)
        {
            SPUser user = null;
            try
            {
                string fieldValue = afterProperties[fieldName] as string;
                SPFieldUserValue fuv = new SPFieldUserValue(web, fieldValue);
                if (fuv.User != null)
                    user = fuv.User;
                else
                    user = web.EnsureUserProperly(fuv.LookupValue);
            }
            catch (Exception x)
            {
                throw x;
            }

            return user;
        }
    }
}
