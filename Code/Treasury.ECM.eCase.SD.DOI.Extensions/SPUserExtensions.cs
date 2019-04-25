using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.Office.Server;
using Microsoft.Office.Server.UserProfiles;

using System.DirectoryServices;
using System.Security.Principal;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Extensions
{
    public static class SPUserExtensions
    {
        /// <summary>
        /// Retrieve all the AD groups of a supplied SPUser.
        /// </summary>
        /// <param name="spUser"></param>
        /// <returns>A list of NTAccount objects representing AD groups of which the supplied user is a member</returns>
        public static List<NTAccount> ActiveDirectorySecurityGroups(this SPUser spUser)
        {
            List<NTAccount> adSecurityGroups = new List<NTAccount>();

            if (spUser.RawSid != null)
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        DirectoryEntry deUser = new DirectoryEntry(string.Format("LDAP://<SID={0}>", new SecurityIdentifier(spUser.RawSid, 0).Value));
                        deUser.RefreshCache(new string[] { "tokenGroups" });

                        foreach (byte[] sidBytes in deUser.Properties["tokenGroups"])
                        {
                            SecurityIdentifier sid = new SecurityIdentifier(sidBytes, 0);
                            try
                            {
                                adSecurityGroups.Add((NTAccount)sid.Translate(typeof(NTAccount)));
                            }
                            catch (IdentityNotMappedException inmp)
                            {
                                Logger.Instance.Info(string.Format("Could not find object for SID: {0}. {1}", sid, inmp.ToString()), DiagnosticsCategories.eCaseExtensions);
                            }

                        }
                    });
                }
                catch (DirectoryServicesCOMException dsce)
                {
                    Logger.Instance.Error(string.Format("Failed to find AD Sec Groups for user {0}. {1}", spUser.LoginName, dsce.ToString()), DiagnosticsCategories.eCaseExtensions);
                    throw dsce;
                }
            }

            return adSecurityGroups;
        }

        /// <summary>
        /// Retrieves the specified user profile property by name for the supplied user.
        /// </summary>
        /// <param name="spUser"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetProfileProperty(this SPUser spUser, string propertyName)
        {
            string retVal = string.Empty;
            try
            {
                UserProfileManager upm = new UserProfileManager(SPServiceContext.Current);
                if (upm.UserExists(spUser.LoginName))
                {
                    UserProfile up = upm.GetUserProfile(spUser.LoginName);
                    retVal = up[propertyName].Value.ToString();
                }
            }
            catch (Exception x)
            {
                Logger.Instance.Error(string.Format("Failed to find property {0} for {1}: {2}", propertyName, spUser.LoginName, x), DiagnosticsCategories.eCaseExtensions);
            }

            return retVal;
        }

        public static bool InGroup(this SPUser user, SPGroup group)
        {
            return user.Groups.Cast<SPGroup>()
              .Any(g => g.ID == group.ID);
        }
    }
}
