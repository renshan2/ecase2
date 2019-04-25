using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;

namespace Treasury.ECM.eCase.SusDeb.DOI.Web.CaseSiteDefinition.EventReceivers.GroupProxyER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class GroupProxyER : SPItemEventReceiver
    {
        /// <summary>
        /// An item is being updated.
        /// </summary>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(properties.SiteId))
                {
                    using (SPWeb web = site.OpenWeb(properties.Web.ID))
                    {
                        SPListItem item = web.GetListItem(properties.ListItem.GetServerRelativeUrl());
                        SPGroup group;
                        string groupName;
                        string groupSuffix = properties.List.RootFolder.Name;
                        if (TryGetGroup(groupSuffix, web, out groupName, out group))
                        {
                            // Get the old user as an SPUser
                            SPUser oldUser = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.GROUPPROXY_LIST_GROUPMEMBER);
                            if (oldUser != null)
                            {
                                if (group.Owner != oldUser)
                                {
                                    SPUser newUser = properties.AfterProperties.GetFieldAsSPUser("GroupMember", web);
                                    if (newUser != null)
                                    {
                                        group.RemoveUser(oldUser);
                                        group.AddUser(newUser);
                                        group.Update();
                                    }
                                    else
                                    {
                                        properties.ErrorMessage = string.Format("User {0} cannot be found.", properties.AfterProperties["GroupMember"]);
                                        properties.Cancel = true;
                                        properties.Status = SPEventReceiverStatus.CancelWithError;
                                    }
                                }
                                else
                                {
                                    properties.ErrorMessage = string.Format("User {0} is the group owner and cannot be removed.", item[eCaseConstants.FieldGuids.GROUPPROXY_LIST_GROUPMEMBER]);
                                    properties.Cancel = true;
                                    properties.Status = SPEventReceiverStatus.CancelWithError;
                                }
                            }
                            else
                            {
                                properties.ErrorMessage = string.Format("User {0} cannot be found.", properties.BeforeProperties["GroupMember"]);
                                properties.Cancel = true;
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                            }

                        }
                        else
                        {
                            properties.ErrorMessage = string.Format("No group with a name of {0} can be found.", groupName);
                            properties.Cancel = true;
                            properties.Status = SPEventReceiverStatus.CancelWithError;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// An item is being deleted.
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(properties.SiteId))
                {
                    using (SPWeb web = site.OpenWeb(properties.Web.ID))
                    {
                        SPListItem item = web.GetListItem(properties.ListItem.GetServerRelativeUrl());

                        SPGroup group;
                        string groupName;
                        string groupSuffix = properties.List.RootFolder.Name;
                        if (TryGetGroup(groupSuffix, web, out groupName, out group))
                        {
                            // Convert the user string to an SPUser object
                            SPUser user = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.GROUPPROXY_LIST_GROUPMEMBER);
                            if (user != null)
                            {
                                if (group.Owner != user)
                                {
                                    group.RemoveUser(user);
                                    group.Update();
                                }
                                else
                                {
                                    properties.ErrorMessage = string.Format("User {0} is the group owner and cannot be deleted.", item[eCaseConstants.FieldGuids.GROUPPROXY_LIST_GROUPMEMBER]);
                                    properties.Cancel = true;
                                    properties.Status = SPEventReceiverStatus.CancelWithError;
                                }
                            }
                            else
                            {
                                properties.ErrorMessage = string.Format("User {0} cannot be found.", item[eCaseConstants.FieldGuids.GROUPPROXY_LIST_GROUPMEMBER]);
                                properties.Cancel = true;
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                            }
                        }
                        else
                        {
                            properties.ErrorMessage = string.Format("No group with a name of {0} can be found.", groupName);
                            properties.Cancel = true;
                            properties.Status = SPEventReceiverStatus.CancelWithError;
                        }
                    }
                }
            });
        }

        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(properties.SiteId))
                {
                    using (SPWeb web = site.OpenWeb(properties.Web.ID))
                    {
                        SPGroup group;
                        string groupName;
                        string groupSuffix = properties.List.RootFolder.Name;
                        if (TryGetGroup(groupSuffix, web, out groupName, out group))
                        {
                            // Convert the user string to an SPUser object
                            SPUser user = properties.AfterProperties.GetFieldAsSPUser("GroupMember", web);
                            if (user != null)
                            {
                                group.AddUser(user);
                                group.Update();
                            }
                            else
                            {
                                properties.ErrorMessage = string.Format("User {0} cannot be found.", properties.AfterProperties["GroupMember"]);
                                properties.Cancel = true;
                                properties.Status = SPEventReceiverStatus.CancelWithError;
                            }
                        }
                        else
                        {
                            properties.ErrorMessage = string.Format("No group with a name of {0} can be found.", groupName);
                            properties.Cancel = true;
                            properties.Status = SPEventReceiverStatus.CancelWithError;
                        }
                    }
                }
            });
        }

        private bool TryGetGroup(string folderName, SPWeb web, out string groupName, out SPGroup group)
        {
            bool retVal = true;
            switch (folderName)
            {
                case "Bureau":
                    groupName = eCaseGroups.OwnersName(web.Name);
                    group = eCaseGroups.GetOwners(web);
                    break;
                case "Investigator":
                    groupName = eCaseGroups.ParticipantsName(web.Name);
                    group = eCaseGroups.GetParticipants(web);
                    break;
                default:
                    retVal = false;
                    groupName = string.Empty;
                    group = null;
                    // Log to ULS indicating the group could not be found
                    Logger.Instance.Error(string.Format("Unable to find group proxy list {0} at {1}", folderName, web.Url), DiagnosticsCategories.eCaseWeb);
                    break;
            }
            return retVal;
        }
    }
}