using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Treasury.ECM.eCase.SusDeb.DOI.Search.eCaseFastSearchSiteDef.eCaseSearchMasterPages
{
    //This is a partial solution only that does not accomplish the goal but also does not have any visible effects
    //referrer is not set when navigating here from the search box
    //future idea is to utilize JS to contact a web service that will receive and properly set the last visited case site for each user
    public partial class eCase_minimal : System.Web.UI.MasterPage
    {
        protected string SavedSearchSiteReferrerUrlString = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            Uri currentUrl = null;
            Uri referrerUrl = null;
            object savedSearchSiteReferrerUrlObject = null;

            currentUrl = Request.Url;
            referrerUrl = Request.UrlReferrer;

            //compare the current url to the referrer url if it is set
            if (referrerUrl != null && currentUrl != null)
            {
                //goal: is the current site the same as the referrer site?
                string currentSiteUrl = Microsoft.SharePoint.SPContext.Current.Web.Url;

                if (!referrerUrl.ToString().StartsWith(currentSiteUrl, StringComparison.CurrentCultureIgnoreCase))
                {
                    //start with basic storage of the previous url and add it to the page output for use in JavaScript
                    Session[SusDeb.DOI.Common.Utilities.eCaseConstants.SessionKeys.SEARCH_STORED_REFERRER_URL] = referrerUrl.ToString();

                    //TODO: Are there exceptions where certain urls will knowingly not work properly?
                }
            }

            savedSearchSiteReferrerUrlObject = Session[SusDeb.DOI.Common.Utilities.eCaseConstants.SessionKeys.SEARCH_STORED_REFERRER_URL];

            if (savedSearchSiteReferrerUrlObject is string)
            {
                SavedSearchSiteReferrerUrlString = savedSearchSiteReferrerUrlObject as string;
            }

            if (!String.IsNullOrEmpty(SavedSearchSiteReferrerUrlString))
            {
                returnToSiteLink.Text = SavedSearchSiteReferrerUrlString;
                returnToSiteLink.NavigateUrl = SavedSearchSiteReferrerUrlString;
                returnToSiteLink.Visible = true;
            }
            else
            {
                returnToSiteLink.Text = string.Empty;
                returnToSiteLink.NavigateUrl = string.Empty;
                returnToSiteLink.Visible = false;
            }
        }
    }
}
