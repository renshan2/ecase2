﻿using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.WebParts.EmptyWebPart
{
    [ToolboxItem(false)]
    public class EmptyWebPart : WebPart
    {
        protected override void CreateChildControls()
        {
        }
    }
}
