using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPFeatureReceiverPropertiesExtensions
    {
        public static void GetSiteAndWeb(this SPFeatureReceiverProperties properties, out SPSite site, out SPWeb web)
        {
            site = properties.Feature.Parent as SPSite;
            if (site != null)
                web = site.RootWeb; // Disposed outside of this scope
            else
            {
                web = properties.Feature.Parent as SPWeb;
                site = web.Site;
            }

        }
    }
}
