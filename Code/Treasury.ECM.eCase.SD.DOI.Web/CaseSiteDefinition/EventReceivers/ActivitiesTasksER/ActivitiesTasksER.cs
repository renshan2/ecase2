using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.EventReceivers.ActivitiesTasksER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ActivitiesTasksER : SPItemEventReceiver
    {
        /// <summary>
        /// An item was deleted.
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            CasesNextDueDate.UpdateNextDueDate(properties);
        }

        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            CasesNextDueDate.UpdateNextDueDate(properties);
        }

        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            CasesNextDueDate.UpdateNextDueDate(properties);
        }


    }
}