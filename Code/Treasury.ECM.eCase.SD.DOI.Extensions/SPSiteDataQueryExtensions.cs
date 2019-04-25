using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPSiteDataQueryExtensions
    {
        const string VIEW_FIELD_FORMAT = @"<FieldRef Name='{0}' />";

        /// <summary>
        /// Provide the view fields as a string[].
        /// Example: query.SetViewFields(new string[] {"ID", "Title"});
        /// </summary>
        /// <param name="viewFields"></param>
        /// <returns></returns>
        public static void SetViewFields(this SPSiteDataQuery query, string[] viewFields)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in viewFields)
                sb.AppendFormat(VIEW_FIELD_FORMAT, s);

            query.ViewFields = sb.ToString();
        }
    }
}
