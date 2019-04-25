using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebPartPages;
using System.Web.UI.WebControls;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.Modules.eCasePages
{
    public class DefaultPage: WebPartPage
    {
        /// <summary>
        /// On Page Load, find literal controls, then populate with content
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //        
            }
        }

    }
}
