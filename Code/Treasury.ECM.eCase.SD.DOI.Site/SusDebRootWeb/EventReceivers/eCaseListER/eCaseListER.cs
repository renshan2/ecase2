using System;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;
using Treasury.ECM.eCase.SusDeb.DOI.Extensions;
using Treasury.ECM.eCase.SusDeb.DOI.Logging;

namespace Treasury.ECM.eCase.SusDeb.DOI.Site.SusDebRootWeb.EventReceivers.eCaseListER
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class eCaseListER : SPItemEventReceiver
    {
        private string _connectionString = string.Empty;

        public override void ItemAdding(SPItemEventProperties properties)
        {
            properties.AfterProperties["CaseUrl"] = string.Format("{0}/_layouts/1033/error.htm, {1}", properties.WebUrl, "Creating Site...");
            properties.AfterProperties["UniqueCaseID"] = BuildUniqueCaseId(properties);
            properties.AfterProperties["Title"] = properties.AfterProperties["UniqueCaseID"];
        }

        /// <summary>
        /// An item has been added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            this.EventFiringEnabled = false; // FINALLY CLAUSE AT END WILL ENSURE IT IS IN PROPER STATE
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite site = new SPSite(properties.SiteId))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            string uniqueUrl = string.Empty;

                            try
                            {
                                SPList casesList = web.Lists[properties.ListId];
                                SPListItem item = casesList.GetItemById(properties.ListItemId);

                                #region Get LockCase Value
                                bool lockCase;
                                SPListItem caseStatusItem = item.GetFieldLookupAsSPListItem("CaseStatusLookup");
                                bool.TryParse(caseStatusItem[eCaseConstants.FieldGuids.ECASE_STATUSES_LOCK_SITE].ToString(), out lockCase);
                                #endregion

                                #region Get Users
                                SPUser assignedTo, supervisor;
                                GetAssignedToAndSupervisor(item, out assignedTo, out supervisor);
                                #endregion

                                #region Get Other Properties
                                string title = item[eCaseConstants.FieldGuids.OOTB_TITLE].ToString();
                                string description = (string)item[eCaseConstants.FieldGuids.ECASES_LIST_DESCRIPTION] ?? string.Empty;
                                string uniqueCaseId = item[eCaseConstants.FieldGuids.ECASES_LIST_UNIQUECASEID].ToString();
                                #endregion

                                // Get a unique URL that doesn't currently exist
                                uniqueUrl = web.GetValidNewWebUrl(uniqueCaseId);

                                SPWeb caseWeb;
                                string caseWebUrl = string.Empty;
                                try
                                {
                                    using (caseWeb = CreateCaseWeb(web, uniqueUrl, title, description))
                                    {
                                        #region Configure Proxy Group Lists Permissions
                                        SPGroup ownersGroup = eCaseGroups.GetOwners(caseWeb);
                                        SPGroup participantsGroup = eCaseGroups.GetParticipants(caseWeb);
                                        SPGroup externalSharingGroup = eCaseGroups.GetExternalUsers(caseWeb);

                                        SPRoleDefinition fullControl;
                                        caseWeb.TryGetRoleDefinition("Full Control", out fullControl);
                                        SPRoleDefinition reader;
                                        caseWeb.TryGetRoleDefinition("Read", out reader);
                                        SPRoleDefinition contributor;
                                        caseWeb.TryGetRoleDefinition("Contribute", out contributor);                                        

                                        SPList ownersList = caseWeb.GetListByInternalName("Bureau");
                                        ownersList.BreakRoleInheritance(false);
                                        ownersList.TryGrantPermission(ownersGroup, fullControl);
                                        ownersList.TryGrantPermission(participantsGroup, reader);

                                        SPList participantsList = caseWeb.GetListByInternalName("Investigator");
                                        participantsList.BreakRoleInheritance(false);
                                        participantsList.TryGrantPermission(ownersGroup, fullControl);
                                        participantsList.TryGrantPermission(participantsGroup, reader);
                                        #endregion

                                        #region Configure Group Memberships via Proxy
                                        this.EventFiringEnabled = true; // TURN ON EVENT FIRING SO THAT PROXY GROUP EVENT RECEIVER FIRES
                                        UpdateProxyGroups(caseWeb, assignedTo, supervisor, null, null);
                                        if (assignedTo.LoginName != properties.UserLoginName && (supervisor == null || supervisor.LoginName != properties.UserLoginName))
                                        {
                                            SPList ownersProxyList = eCaseGroups.OwnersList(caseWeb);
                                            eCaseGroups.AddGroupProxyItem(ownersProxyList, web.EnsureUserProperly(properties.UserLoginName));
                                        }
                                        this.EventFiringEnabled = false; // FINALLY CLAUSE AT END WILL ENSURE IT IS IN PROPER STATE
                                        #endregion

                                        #region Configure Sharing With External Party
                                        SPList shareWithExternalUsersList = caseWeb.GetListByInternalName("ShareWithExternalUsers");
                                        shareWithExternalUsersList.BreakRoleInheritance(false);
                                        shareWithExternalUsersList.TryGrantPermission(ownersGroup, fullControl);
                                        shareWithExternalUsersList.TryGrantPermission(participantsGroup, contributor);
                                        shareWithExternalUsersList.TryGrantPermission(externalSharingGroup, reader);
                                        #endregion

                                        ConfigureItemPermissions(item, caseWeb, assignedTo, supervisor);

                                        #region Configure Group Permissions on Root Web
                                        //bool allowunsafeupdate = web.AllowUnsafeUpdates;
                                        //web.AllowUnsafeUpdates = true;
                                        //SPRoleDefinition topreader;
                                        //web.TryGetRoleDefinition("Read", out topreader);
                                        //web.TryGrantPermission(ownersGroup, topreader);
                                        //web.TryGrantPermission(participantsGroup, topreader);
                                        //web.TryGrantPermission(reviewersGroup, topreader);
                                        //web.Update();
                                        //web.AllowUnsafeUpdates = allowunsafeupdate;
                                        #endregion

                                        #region Configure eCase Statuses List Permissions
                                        //SPList statusList = web.GetListByInternalName("eCaseStatuses");
                                        //if (!statusList.HasUniqueRoleAssignments)
                                        //{
                                        //    statusList.BreakRoleInheritance(true);
                                        //    statusList.Update();
                                        //}
                                        //statusList.TryGrantPermission(ownersGroup, topreader);
                                        //statusList.TryGrantPermission(participantsGroup, topreader);
                                        //statusList.TryGrantPermission(reviewersGroup, topreader);
                                        //statusList.Update();
                                        #endregion                                        

                                        UpdateDefaultAspx(item, caseWeb);

                                        // Record item guid in caseweb's property bag so methods can find the parent item easily
                                        caseWeb.AddProperty(eCaseConstants.PropertyBagKeys.ECASE_CASE_LIST_ITEM_GUID, item.UniqueId.ToString());
                                        caseWeb.Update();

                                        try
                                        {
                                            _connectionString = web.Properties[eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
                                            using (DbAdapter dbAdapter = new DbAdapter())
                                            {
                                                dbAdapter.Connect(_connectionString);

                                                try
                                                {
                                                    
                                                    #region Add Case Site Info to eCaseManagement Database
                                                    Guid siteGuid = caseWeb.Site.ID;
                                                    Guid caseWebGuid = caseWeb.ID;
                                                    Guid activitiesTasksGuid = caseWeb.GetListByInternalName(eCaseConstants.ListInternalNames.ACTIVITIES_AND_TASKS).ID;
                                                    Guid caseRelatedDatesGuid = caseWeb.GetListByInternalName(eCaseConstants.ListInternalNames.CASE_RELATED_DATES).ID;

                                                    CreateCaseWebSProc sProc = new CreateCaseWebSProc(siteGuid, item.UniqueId, caseWebGuid, activitiesTasksGuid, caseRelatedDatesGuid);
                                                    dbAdapter.ExecuteNonQueryStoredProcedure(sProc);
                                                    #endregion
                                                }
                                                catch (Exception x)
                                                { Logger.Instance.Error(string.Format("Failed while recording new CaseWeb at {0} in database", caseWeb.Url), x, DiagnosticsCategories.eCaseSite); }

                                                if (lockCase)
                                                    LockCaseSite(item, caseWeb, dbAdapter);
                                            }
                                        }
                                        catch (Exception x)
                                        { Logger.Instance.Error(string.Format("Failed while accessing database at {0} with connection string {1}", caseWeb.Url, _connectionString), x, DiagnosticsCategories.eCaseSite); }

                                        caseWebUrl = string.Format("{0}, {1}", caseWeb.Url, "View Case");
                                    }
                                }
                                catch (Exception x)
                                {
                                    caseWebUrl = string.Format("{0}/_layouts/1033/error.htm, {1}", web.Url, "ERROR!");
                                    Logger.Instance.Error(string.Format("Failed to create Case Web for {0}", uniqueCaseId), x, DiagnosticsCategories.eCaseSite);
                                    if (web.Webs[uniqueUrl].Exists)
                                        RemoveCaseWeb(properties, uniqueUrl);

                                    throw x;
                                }
                                finally
                                {
                                    item[eCaseConstants.FieldGuids.ECASES_LIST_CASEURL] = caseWebUrl;
                                    item.SystemUpdate(); // Update the change in the DB using the system account
                                }
                            }
                            catch (Exception x)
                            {
                                Logger.Instance.Error(string.Format("Failed to create Case Web for {0}", uniqueUrl), x, DiagnosticsCategories.eCaseSite);
                                throw x;
                            }
                        }
                    }
                });
            }
            catch (Exception x)
            { throw x; }
            finally
            { this.EventFiringEnabled = true; }
        }

        private void LockItemPermissions(SPListItem item, SPWeb caseWeb, SPUser assignedTo, SPUser supervisor)
        {
            item.BreakRoleInheritance(false);
            item.TryGrantPermission(assignedTo, SPRoleType.Administrator);
            item.TryGrantPermission(supervisor, SPRoleType.Administrator);
            item.TryGrantPermission(eCaseGroups.GetOwners(caseWeb), SPRoleType.Administrator);
            item.TryGrantPermission(eCaseGroups.GetParticipants(caseWeb), SPRoleType.Reader);
        }

        private void ConfigureItemPermissions(SPListItem item, SPWeb caseWeb, SPUser assignedTo, SPUser supervisor)
        {
            item.BreakRoleInheritance(false);
            item.TryGrantPermission(assignedTo, SPRoleType.Administrator);
            item.TryGrantPermission(supervisor, SPRoleType.Administrator);
            item.TryGrantPermission(eCaseGroups.GetOwners(caseWeb), SPRoleType.Administrator);
            item.TryGrantPermission(eCaseGroups.GetParticipants(caseWeb), SPRoleType.Contributor);
        }

        private void UpdateDefaultAspx(SPListItem item, SPWeb caseWeb)
        {
            SPFile defaultAspx = caseWeb.GetFile("default.aspx");
            if (defaultAspx.Exists)
            {
                byte[] contents = defaultAspx.OpenBinary();
                if (contents.Length > 0)
                {
                    System.Text.UTF8Encoding utf8Enc = new System.Text.UTF8Encoding();
                    string contentsString = utf8Enc.GetString(contents);
                    
                    // Update all occurrences of ListItemId token with the eCases List Item ID
                    contentsString = contentsString.Replace("{ListItemId}", item.ID.ToString().ToUpper());

                    SPList caseRelatedDatesList = caseWeb.GetListByInternalName(eCaseConstants.ListInternalNames.CASE_RELATED_DATES);
                    contentsString = contentsString.Replace("{CaseRelatedDatesGuid}", caseRelatedDatesList.ID.ToString().ToUpper());

                    SPList activitiesAndTasksList = caseWeb.GetListByInternalName(eCaseConstants.ListInternalNames.ACTIVITIES_AND_TASKS);
                    contentsString = contentsString.Replace("{ActivitiesAndTasksGuid}", activitiesAndTasksList.ID.ToString().ToUpper());
                    contents = utf8Enc.GetBytes(contentsString);
                    defaultAspx.SaveBinary(contents);
                    defaultAspx.Update();
                }
            }
        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            try
            {
                // This represents the item as it WAS, not as it WILL BE
                SPListItem item = properties.ListItem;
                SPUser prevAssignedToUser = null;
                SPUser prevSupervisorUser = null;

                #region Determine Locking Behavior
                bool newLockCase, oldLockCase, lockCase, unlockCase;
                SPListItem oldCaseStatusItem = properties.ListItem.GetFieldLookupAsSPListItem("CaseStatusLookup");
                SPListItem newCaseStatusItem = properties.AfterProperties.GetFieldLookupAsSPListItem("CaseStatusLookup", properties.List);

                if (bool.TryParse(newCaseStatusItem[eCaseConstants.FieldGuids.ECASE_STATUSES_LOCK_SITE].ToString(), out newLockCase) &&
                    bool.TryParse(oldCaseStatusItem[eCaseConstants.FieldGuids.ECASE_STATUSES_LOCK_SITE].ToString(), out oldLockCase))
                {
                    if (string.Compare(newCaseStatusItem[eCaseConstants.FieldGuids.OOTB_TITLE].ToString(), oldCaseStatusItem[eCaseConstants.FieldGuids.OOTB_TITLE].ToString()) != 0)
                    {
                        if (oldLockCase && !newLockCase) // If the status was locked, and now not, lets unlock
                        {
                            unlockCase = true;
                            lockCase = false;
                        }
                        else if (oldLockCase && newLockCase) // If the status was locked, and still locked, do nothing
                            lockCase = unlockCase = false;
                        else if (!oldLockCase && newLockCase) // If the status was not locked, and now is locked, need to lock
                        {
                            unlockCase = false;
                            lockCase = true;
                        }
                        else // !oldLockCase && !lockCase -- do nothing
                            lockCase = unlockCase = false;
                    }
                    else // the status didn't change
                        lockCase = unlockCase = false;
                }
                else // Something's wrong if we cannot parse one
                    throw new InvalidOperationException("Cannot parse Case Status Values");
                #endregion

                string caseWebUrl = item[eCaseConstants.FieldGuids.ECASES_LIST_CASEURL].ToString().Split(',')[0];

                SPSite site;
                SPWeb caseWeb;
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (site = new SPSite(caseWebUrl))
                    {
                        using (caseWeb = site.OpenWeb())
                        {
                            SPUser assignedToUser = null;
                            SPUser supervisorUser = null;
                            GetAssignedToAndSupervisor(item, properties.AfterProperties, caseWeb, ref assignedToUser, ref prevAssignedToUser, ref supervisorUser, ref prevSupervisorUser);
                            if (assignedToUser != null || supervisorUser != null)
                                UpdateProxyGroups(caseWeb, assignedToUser, supervisorUser, prevAssignedToUser, prevSupervisorUser);
                        }
                    }
                });

                if (prevAssignedToUser == null)
                    prevAssignedToUser = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.OOTB_ASSIGNEDTO);
                if (prevSupervisorUser == null)
                    prevSupervisorUser = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.ECASES_LIST_ASSIGNEDTOSUPERVISOR);

                if (item.DoesUserHavePermissions(SPBasePermissions.ManagePermissions))
                {
                    item.TryRemovePermissions(prevAssignedToUser);
                    item.TryRemovePermissions(prevSupervisorUser);
                }
                else // We need to elevate to do this
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (site = new SPSite(properties.SiteId))
                        {
                            using (SPWeb rootWeb = site.OpenWeb())
                            {
                                SPList list = rootWeb.GetListByInternalName(properties.List.RootFolder.Name);
                                SPListItem i = list.GetItemById(item.ID);
                                i.TryRemovePermissions(prevAssignedToUser);
                                i.TryRemovePermissions(prevSupervisorUser);
                            }
                        }
                    });
                }

                if (lockCase || unlockCase)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (site = new SPSite(caseWebUrl))
                        {
                            using (caseWeb = site.OpenWeb())
                            {
                                using (DbAdapter dbAdapter = new DbAdapter())
                                {
                                    SPList list = site.RootWeb.GetListByInternalName(properties.List.RootFolder.Name);
                                    SPListItem i = list.GetItemById(item.ID);

                                    _connectionString = site.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
                                    dbAdapter.Connect(_connectionString);
                                    if (lockCase)
                                        LockCaseSite(i, caseWeb, dbAdapter);
                                    else
                                        UnlockCaseSite(i, caseWeb, dbAdapter);
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception x) { Logger.Instance.Error("Error in Cases list ItemUpdating", x, DiagnosticsCategories.eCaseSite); }

        }
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            try
            {
                SPListItem item = properties.ListItem;
                SPUser assignedTo = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.OOTB_ASSIGNEDTO);
                SPUser assignedToSupervisor = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.ECASES_LIST_ASSIGNEDTOSUPERVISOR);
                string caseTitle = item[eCaseConstants.FieldGuids.OOTB_TITLE] as string;

                string caseWebUrl = item[eCaseConstants.FieldGuids.ECASES_LIST_CASEURL].ToString();
                caseWebUrl = caseWebUrl.Split(',')[0];
                SPSite site;
                SPWeb caseWeb;
                bool titleUpdated = false;
                using (site = new SPSite(caseWebUrl))
                {
                    using (caseWeb = site.OpenWeb())
                    {
                        if (caseWeb.DoesUserHavePermissions(SPBasePermissions.ManageWeb))
                        {
                            UpdateCaseWebTitle(caseWeb, caseTitle);
                            titleUpdated = true;
                        }
                    }
                }

                if (titleUpdated == false)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (site = new SPSite(caseWebUrl))
                        {
                            using (caseWeb = site.OpenWeb())
                            {
                                if (titleUpdated == false)
                                    UpdateCaseWebTitle(caseWeb, caseTitle);
                            }
                        }

                    });
                }

                if (item.DoesUserHavePermissions(SPBasePermissions.ManagePermissions))
                {
                    item.TryGrantPermission(assignedTo, SPRoleType.Administrator);
                    item.TryGrantPermission(assignedToSupervisor, SPRoleType.Administrator);
                }
                else // We need to elevate to do this
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        using (site = new SPSite(properties.SiteId))
                        {
                            using (SPWeb rootWeb = site.OpenWeb())
                            {
                                SPList list = rootWeb.GetListByInternalName(properties.List.RootFolder.Name);
                                SPListItem i = list.GetItemById(item.ID);
                                i.TryGrantPermission(assignedToSupervisor, SPRoleType.Administrator);
                                i.TryGrantPermission(assignedTo, SPRoleType.Administrator);
                            }
                        }
                    });
                }
            }
            catch (Exception x) { Logger.Instance.Error("Error in Cases list ItemUpdated", x, DiagnosticsCategories.eCaseExtensions); }
        }

        /// <summary>
        /// Returns a new SPWeb object representing the case web.  Make sure you dispose this object.
        /// </summary>
        /// <param name="rootWeb"></param>
        /// <param name="uniqueUrl"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        private SPWeb CreateCaseWeb(SPWeb rootWeb, string uniqueUrl, string title, string description)
        {
            try
            {
                SPWebTemplate spWebTemplate = rootWeb.Site.GetWebTemplates(1033)["SusDebCaseSiteDefinition#0"];
                SPWeb caseWeb = rootWeb.Webs.Add(uniqueUrl, title, description, 1033, spWebTemplate, true, false); // Disposed outside of this scope
                caseWeb.AssociatedOwnerGroup = eCaseGroups.GetOwners(caseWeb);
                caseWeb.AssociatedMemberGroup = eCaseGroups.GetParticipants(caseWeb);
                caseWeb.Update(); // Push changes to the CaseWeb to the DB

                caseWeb.TryGrantPermission(caseWeb.AssociatedOwnerGroup, SPRoleType.Administrator);
                caseWeb.TryGrantPermission(caseWeb.AssociatedMemberGroup, SPRoleType.Contributor);
                caseWeb.TryGrantPermission(caseWeb.AssociatedVisitorGroup, SPRoleType.Reader);

                Logger.Instance.Info(string.Format("Created new Case Web at {0}", uniqueUrl), DiagnosticsCategories.eCaseSite);

                return caseWeb;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        private void RemoveCaseWeb(SPItemEventProperties properties, string caseUrl)
        {
            var caseWebUrl = properties.WebUrl + "/" + caseUrl;
            using (SPSite site = new SPSite(caseWebUrl))
            {
                using (SPWeb caseWeb = site.OpenWeb())
                {
                    //delete our owners, participants and reviewers groups
                    var ownersGroup = eCaseGroups.GetOwners(caseWeb);
                    caseWeb.SiteGroups.RemoveByID(ownersGroup.ID);
                    var participantsGroup = eCaseGroups.GetParticipants(caseWeb);
                    caseWeb.SiteGroups.RemoveByID(participantsGroup.ID);
                    caseWeb.Update();

                    //now delete the site
                    caseWeb.Delete();
                }
            }
        }

        private void UpdateCaseWebTitle(SPWeb caseWeb, string title)
        {
            if (caseWeb.Title != title)
            {
                caseWeb.Title = title;
                caseWeb.Update();
            }
        }

        private void UpdateProxyGroups(SPWeb caseWeb, SPUser assignedTo, SPUser supervisor, SPUser prevAssignedTo, SPUser prevSupervisor)
        {
            // Get the Owners Group
            SPGroup ownersGroup = eCaseGroups.GetOwners(caseWeb);
            SPGroup participantsGroup = eCaseGroups.GetParticipants(caseWeb);

            // Get the Owners Proxy List
            SPList ownersProxyList = eCaseGroups.OwnersList(caseWeb);
            SPList participantsProxyList = eCaseGroups.ParticipantsList(caseWeb);

            // The Assigned To should always be the Owner
            if (assignedTo != null && (prevAssignedTo == null || assignedTo.LoginName != prevAssignedTo.LoginName))
            {
                ownersGroup.Owner = assignedTo;
                eCaseGroups.AddGroupProxyItem(ownersProxyList, assignedTo);
            }

            if (supervisor != null && (prevSupervisor == null || supervisor.LoginName != prevSupervisor.LoginName))
                eCaseGroups.AddGroupProxyItem(participantsProxyList, supervisor);

            if (prevAssignedTo != null && (assignedTo == null || prevAssignedTo.LoginName != assignedTo.LoginName))
                eCaseGroups.DeleteGroupProxyItem(ownersProxyList, prevAssignedTo);

            if (prevSupervisor != null && (supervisor == null || prevSupervisor.LoginName != supervisor.LoginName))
                eCaseGroups.DeleteGroupProxyItem(participantsProxyList, prevSupervisor);
        }

        private void GetAssignedToAndSupervisor(SPListItem item, SPItemEventDataCollection afterProperties, SPWeb caseWeb, ref SPUser assignedToUser, 
            ref SPUser prevAssignedToUser, ref SPUser supervisorUser, ref SPUser prevSupervisorUser)
        {
            // Check if we the AssignedTo or AssignedToSupervisor was modified
            if (!string.IsNullOrEmpty(afterProperties["AssignedTo"].ToString()))
            {
                assignedToUser = afterProperties.GetFieldAsSPUser("AssignedTo", caseWeb);
                prevAssignedToUser = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.OOTB_ASSIGNEDTO);
            }

            //if (!string.IsNullOrEmpty(afterProperties["AssignedToSupervisor"].ToString()))
            //{

            //    supervisorUser = afterProperties.GetFieldAsSPUser("AssignedToSupervisor", caseWeb);
            //    prevSupervisorUser = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.ECASES_LIST_ASSIGNEDTOSUPERVISOR);
            //}
        }

        private void GetAssignedToAndSupervisor(SPListItem item, out SPUser assignedTo, out SPUser supervisor)
        {
            supervisor = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.ECASES_LIST_ASSIGNEDTOSUPERVISOR);
            assignedTo = item.GetFieldAsSPUser(eCaseConstants.FieldGuids.OOTB_ASSIGNEDTO);
        }
        
        private void LockCaseSite(SPListItem caseItem, SPWeb caseWeb, DbAdapter dbAdapter)
        {
            CreateSPObjPermSProc sProc;
            if (caseItem != null)
            {
                // Record permissions of the Case list item
                sProc = new CreateSPObjPermSProc(caseWeb.Site.ID, caseWeb.ID, caseItem.ParentList.ParentWeb.ID, caseItem.ParentList.ID, caseItem.UniqueId, caseItem.RoleAssignments.Xml);
                dbAdapter.ExecuteNonQueryStoredProcedure(sProc);

                // Reset permissions of case list item and reconfigure
                SPUser assignedTo, supervisor;
                GetAssignedToAndSupervisor(caseItem, out assignedTo, out supervisor);
                caseItem.ResetRoleInheritance();
                LockItemPermissions(caseItem, caseWeb, assignedTo, supervisor);
            }

            // Record the permissions of the case web
            sProc = new CreateSPObjPermSProc(caseWeb.Site.ID, caseWeb.ID, null, null, null, caseWeb.RoleAssignments.Xml);
            dbAdapter.ExecuteNonQueryStoredProcedure(sProc);
            caseWeb.RecordPermissions(caseWeb, dbAdapter);
            
            // Reset permissions of case web and reconfigure
            caseWeb.ResetRoleInheritance(); // Recursively redefine all object permissions and force them to inherit parent
            caseWeb.BreakRoleInheritance(false); // Break at web level and redefine permissions from top->down
            caseWeb.TryGrantPermission(caseWeb.AssociatedOwnerGroup, SPRoleType.Administrator);
            caseWeb.TryGrantPermission(caseWeb.AssociatedMemberGroup, SPRoleType.Reader);
            caseWeb.TryGrantPermission(caseWeb.AssociatedVisitorGroup, SPRoleType.Reader);
            caseWeb.Update();
        }

        private void UnlockCaseSite(SPListItem caseItem, SPWeb caseWeb, DbAdapter dbAdapter)
        {
            // Get SPObjectPermission rows out of the database for the Site/Web combo
            GetSPObjPermsSProc gspopSProc = new GetSPObjPermsSProc(caseWeb.Site.ID, caseWeb.ID);
            dbAdapter.ExecuteReaderStoredProcedure(gspopSProc);

            // Iterate over each row
            while (dbAdapter.DataReader.Read())
            {
                string childWebVal = dbAdapter.DataReader["ChildWebGuid"].ToString();
                Guid childWeb = (string.IsNullOrEmpty(childWebVal)) ? caseWeb.ID : new Guid(childWebVal);
                string listVal = dbAdapter.DataReader["ListGuid"].ToString();
                Guid? list = (string.IsNullOrEmpty(listVal)) ? (Guid?)null : new Guid(listVal);
                string listItemVal = dbAdapter.DataReader["ListItemGuid"].ToString();
                Guid? listItem = (string.IsNullOrEmpty(listItemVal)) ? (Guid?)null : new Guid(listItemVal);
                string roleAssignmentsXml = dbAdapter.DataReader["RoleAssignments"].ToString();

                using (SPWeb web = caseWeb.Site.OpenWeb(childWeb))
                {
                    SPList theList = null;
                    SPListItem theItem = null;
                    if (list != null)
                    {
                        theList = web.Lists[(Guid)list];
                        if (listItem != null)
                            theItem = theList.Items[(Guid)listItem];
                    }

                    SPSecurableObject spSecObj = null;
                    // Figure out which object is the one we want to operate on
                    if (theItem != null)
                        spSecObj = theItem as SPSecurableObject;
                    else if (theList != null)
                        spSecObj = theList as SPSecurableObject;
                    else
                        spSecObj = web as SPSecurableObject;

                    if (spSecObj.HasUniqueRoleAssignments)
                    {
                        // Clean out all current role assignments so we can restore them
                        for (int lcv = spSecObj.RoleAssignments.Count - 1; lcv >= 0; lcv--)
                            spSecObj.RoleAssignments.Remove(lcv);
                    }
                    else
                        spSecObj.BreakRoleInheritance(false);
                    spSecObj.RoleAssignments.Add(roleAssignmentsXml, web);
                }
            }

            // If no errors were encountered, delete SPObjectPermission rows from the database for the Site/Web combo
            DeleteSPObjPermsSProc dspopSProc = new DeleteSPObjPermsSProc(caseWeb.Site.ID, caseWeb.ID);
            dbAdapter.ExecuteNonQueryStoredProcedure(dspopSProc);
        }


        private string BuildUniqueCaseId(SPItemEventProperties properties)
        {
            var uniqueId = new StringBuilder();
            var bureauIG = properties.AfterProperties["BureauIG"].ToString();
            var susDebSeqNum = GetSusDebCaseSeqNum(properties.Web);
            var bureauCaseSeqNum = GetBureauCaseSeqNum(bureauIG, properties.Web);
            /* This line was originally returning a date prior to the current date. This was due to converting from string, to date,
             * and back again. During the conversion process the DateTime was being converted from Universal to local time. Since local time
             * in the US East coast is 4 (or 5) hours prior to Universal time the "date" portion of the DateTime would be adjusted
             * backwards by one day. (2014-10-31T00:00:00Z = 2014-10-20T20:00:00ET)
             * Although this has been addressed as a "fix" it's also worth noting that it's odd to use a date in mmddyyyy format in this way
             */
            var intakeDate = DateTime.Parse(properties.AfterProperties["CaseOpeningDate"].ToString(),
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                            System.Globalization.DateTimeStyles.AdjustToUniversal).ToString("MMddyyyy");
            var intakeSource = properties.AfterProperties["IntakeSource"].ToString() == "Web" ? "1" : "2";
            //var taxDebt = bool.Parse(properties.AfterProperties["TaxDebt"].ToString()) ? "T" : "N";
            var taxDebt = properties.AfterProperties["TaxDebt"].ToString() == "Tax" ? "T" : "N";
            uniqueId.Append(bureauIG + "-");
            uniqueId.Append(bureauCaseSeqNum + "-");
            uniqueId.Append(susDebSeqNum + "-");
            uniqueId.Append(intakeDate + "-");
            uniqueId.Append(intakeSource + "-");
            uniqueId.Append(taxDebt);

            return uniqueId.ToString();
        }

        private int GetSusDebCaseSeqNum(SPWeb web)
        {
            try
            {
                var autoNumberList = web.Lists[eCaseConstants.ListInternalNames.SAND_AUTONUMBER_LIST];
                var autoNumberItem = autoNumberList.Items[0];

                var nextAvailableInteger = Convert.ToInt32(autoNumberItem["AutoNumberId"]);

                nextAvailableInteger++;

                autoNumberItem["AutoNumberId"] = nextAvailableInteger;
                autoNumberItem.Update();

                return nextAvailableInteger;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private int GetBureauCaseSeqNum(string bureauIG, SPWeb web)
        {
            var autoNumberList = web.Lists[eCaseConstants.ListInternalNames.BUREAU_AUTONUMBER_LIST];
            SPListItem matchingItem =
                (from SPListItem listitem in autoNumberList.Items
                 where
                    listitem["Title"].ToString().Equals(bureauIG, StringComparison.CurrentCultureIgnoreCase)
                 select listitem).SingleOrDefault();
            int nextAvailableInteger = Convert.ToInt32(matchingItem["AutoNumberId"]);

            nextAvailableInteger++;

            matchingItem["AutoNumberId"] = nextAvailableInteger;
            matchingItem.Update();

            return nextAvailableInteger;
        }
    }
}