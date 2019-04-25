using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;

using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities
{
    public static class eCaseGroups
    {
        public static string OwnersName(string caseWebName)
        {
            return string.Format("{0}-Owners", caseWebName);
        }

        public static string ParticipantsName(string caseWebName)
        {
            return string.Format("{0}-Participants", caseWebName);
        }

        public static string ShareWithExternalUsers(string caseWebName)
        {
            return string.Format("{0}-ExternalUsers", caseWebName);
        }

        public static SPList OwnersList(SPWeb caseWeb)
        {
            return caseWeb.GetListByInternalName("Bureau");
        }

        public static SPList ParticipantsList(SPWeb caseWeb)
        {
            return caseWeb.GetListByInternalName("Investigator");
        }

        public static SPList SharingWithExternalPartyList(SPWeb caseWeb)
        {
            return caseWeb.GetListByInternalName("ShareWithExternalUsers");
        }

        public static SPGroup GetOwners(SPWeb caseWeb)
        {
            SPGroup ownersGroup = null;

            if (!caseWeb.TryGetGroup(OwnersName(caseWeb.Name), out ownersGroup))
            {
                string description = string.Format("Bureaus of the {0} Case Site", caseWeb.Name);
                ownersGroup = caseWeb.CreateGroup(OwnersName(caseWeb.Name), description, caseWeb.Site.RootWeb.AssociatedOwnerGroup as SPMember);
            }

            return ownersGroup;
        }

        public static SPGroup GetParticipants(SPWeb caseWeb)
        {
            SPGroup participantsGroup = null;

            if (!caseWeb.TryGetGroup(ParticipantsName(caseWeb.Name), out participantsGroup))
            {
                string description = string.Format("Investigators of the {0} Case Site", caseWeb.Name);
                participantsGroup = caseWeb.CreateGroup(ParticipantsName(caseWeb.Name), description, GetOwners(caseWeb) as SPMember);
            }

            return participantsGroup;
        }

        public static SPGroup GetExternalUsers(SPWeb caseWeb)
        {
            SPGroup externalPartyGroup = null;

            if (!caseWeb.TryGetGroup(ShareWithExternalUsers(caseWeb.Name), out externalPartyGroup))
            {
                string description = string.Format("External Parties of the {0} Case Site", caseWeb.Name);
                externalPartyGroup = caseWeb.CreateGroup(ShareWithExternalUsers(caseWeb.Name), description, GetOwners(caseWeb) as SPMember);
            }

            return externalPartyGroup;
        }

        public static SPListItem GetGroupProxyItem(SPList groupProxyList, SPUser user)
        {
            SPListItem theItem = null;
            SPQuery query = new SPQuery();
            query.SetViewFields(new string[] { "GroupMember" });
            query.Query = string.Format(@"
                <Where>
                    <Eq>
                        <FieldRef Name='GroupMember'/>
                        <Value Type='User'>{0}</Value>
                    </Eq>
                </Where>
            ", user.Name);
            SPListItemCollection items = groupProxyList.GetItems(query);
            
            if (items.Count == 1)
                theItem = items[0];
            
            return theItem;
        }

        public static void AddGroupProxyItem(SPList groupProxyList, SPUser user)
        {
            SPListItem newOwner = groupProxyList.AddItem();
            newOwner.SetFieldAsSPUser(eCaseConstants.FieldGuids.GROUPPROXY_LIST_GROUPMEMBER, user);
            newOwner.Update();
        }

        public static void DeleteGroupProxyItem(SPList groupProxyList, SPUser user)
        {
            SPListItem item = GetGroupProxyItem(groupProxyList, user);
            if (item != null)
                item.Delete();
        }
    }
}
