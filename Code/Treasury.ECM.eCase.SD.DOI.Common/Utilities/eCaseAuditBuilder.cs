﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.Office.Server;
using Microsoft.Office.Server.Administration;
using Microsoft.Office.Server.UserProfiles;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities
{
    public class eCaseAuditBuilder
    {
        #region Private Objects
        #endregion

        #region Public Properties
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public String DocumentLibraryName { get; set; }
        public String ReportSavePath { get; set; }
        #endregion

        #region Constructors
        public eCaseAuditBuilder()
        {
        }
        #endregion

        #region Public Methods
        public Boolean ProcessAuditReport()
        {
            Boolean bSuccess = false;
            try
            {
                ProcessAuditLogReport();
                bSuccess = true;
                return (bSuccess);
            }
            catch (Exception ex)
            {
                String error = ex.Message;
                bSuccess = false;
                return (bSuccess);
            }
        }
        #endregion

        #region Private Methods
        private void ProcessAuditLogReport()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                {
                    using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
                    {
                        //limit query to a specific site
                        SPAuditQuery query = new SPAuditQuery(site);

                        //set the query date range
                        query.SetRangeStart(this.StartDate);
                        query.SetRangeEnd(this.EndDate);

                        SPAuditEntryCollection auditCol = web.Audit.GetEntries(query);
                        PrepareAuditReport(auditCol, web);
                    }
                }
            });
        }

        private void PrepareAuditReport(SPAuditEntryCollection auditCollection, SPWeb web)
        {
            // build a memory stream of our file contents
            using (MemoryStream exportStream = CreateAuditReport(auditCollection, web))
            {
                // save our export file to our doc libary
                web.AllowUnsafeUpdates = true;
                WriteAuditReport(exportStream, web);
                web.AllowUnsafeUpdates = false;
            }
        }

        private MemoryStream CreateAuditReport(SPAuditEntryCollection auditCollection, SPWeb web)
        {
            String currentWebName = web.Name.Trim();

            StringBuilder fileContents = new StringBuilder();
            StringBuilder headerContents = new StringBuilder();
            StringBuilder fieldContents = new StringBuilder();

            string delimiter = ",";

            // build our header line and add our header line to our file
            headerContents.AppendLine("Audit Report" + delimiter);
            headerContents.AppendLine("Location,Item Type,Location Type,Date Occurred,Source Name,User ID,Event Type,Event Name,Event Source,Event Data");
            fileContents.AppendLine(headerContents.ToString());

            // build our file contents
            foreach (SPAuditEntry audit in auditCollection)
            {
                String docLocation = audit.DocLocation == null ? delimiter : audit.DocLocation.ToString() + delimiter;
                if (docLocation.Contains(currentWebName))
                {
                    String itemType = audit.ItemType == null ? delimiter : audit.ItemType.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String locationType = audit.LocationType == null ? delimiter : audit.LocationType.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String occurred = audit.Occurred == null ? delimiter : audit.Occurred.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String sourceName = audit.SourceName == null ? delimiter : audit.SourceName.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String userId = RetrieveUsernameById(audit.UserId, web).Replace(";", "").Replace(",", "") + delimiter;
                    String eventType = audit.Event == null ? delimiter : audit.Event.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String eventName = audit.EventName == null ? delimiter : audit.EventName.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String eventSource = audit.EventSource == null ? delimiter : audit.EventSource.ToString().Replace(";", "").Replace(",", "") + delimiter;
                    String eventData = audit.EventData == null ? delimiter : audit.EventData.ToString().Replace(";", "").Replace(",", "") + delimiter;

                    String reportLine = docLocation + itemType + locationType + occurred + sourceName + userId + eventType + eventName + eventSource + eventData;

                    //Append the information from the audit collection
                    fieldContents.Append(reportLine);
                    fileContents.AppendLine(fieldContents.ToString());

                    //Clear the audit row StringBuilder
                    fieldContents.Length = 0;
                    fieldContents.Capacity = 0;
                }
            }

            // create a file and return it to the caller - this object must be disposed by the caller
            MemoryStream output = new MemoryStream();
            StreamWriter writer = new StreamWriter(output, Encoding.UTF8);
            writer.Write(fileContents);
            writer.Flush();

            return output;
        }

        private String RetrieveUsernameById(int userID, SPWeb web)
        {
            String userName = String.Empty;

            try
            {
                SPUser siteUser = web.SiteUsers.GetByID(userID);
                userName = siteUser.Name;
            }
            catch
            {
                //Logging as informational and throwing away the error as processing should continue even if the user no longer exists. 
                Logging.Logger.Instance.Info(String.Format("Unable to find user: {0} in site {1}", userID, web.Url), Logging.DiagnosticsCategories.eCaseWeb);
            }

            return userName;
        }

        private void WriteAuditReport(MemoryStream exportStream, SPWeb docWeb)
        {
            string fileName = "eCase_SiteAuditReport_" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + ".csv";

            exportStream.Position = 0;
            //byte[] contents = new byte[exportStream.Length];
            //exportStream.Read(contents, 0, (int)exportStream.Length);

            //need to get the list.RootFoler instead of just filename to add the file
            SPList auditList = docWeb.Lists[this.DocumentLibraryName];
            String reportSavePath = auditList.RootFolder + "/" + fileName;
            SPFile csvFile = docWeb.Files.Add(reportSavePath, exportStream, false);
            exportStream.Close();

            this.ReportSavePath = docWeb.Site.MakeFullUrl(auditList.RootFolder.ServerRelativeUrl); ;
        }
        #endregion
    }
}