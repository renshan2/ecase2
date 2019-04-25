using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPGroupExtensions
    {
        /// <summary>
        /// Removes all users from the group
        /// </summary>
        /// <param name="group">the effected group</param>
        public static void RemoveUsers(this SPGroup group)
        {
            foreach (SPUser u in group.Users)
                group.RemoveUser(u);
        }
    }
}
