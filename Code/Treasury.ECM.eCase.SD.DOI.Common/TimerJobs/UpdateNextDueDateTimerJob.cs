using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

using Treasury.ECM.eCase.SusDeb.DOI.Logging;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Utilities;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.TimerJobs
{
    public class UpdateNextDueDateTimerJob : SPJobDefinition
    {
        [Persisted]
        public string SiteCollectionGuid;

        public UpdateNextDueDateTimerJob()
        {
        }

        public UpdateNextDueDateTimerJob(string jobName, SPWebApplication webApp, SPServer server, SPJobLockType lockType, string siteCollectionGuid)
            : base(jobName, webApp, server, lockType)
        {
            this.Title = jobName;
            SiteCollectionGuid = siteCollectionGuid;
        }

        public override void Execute(Guid targetInstanceId)
        {
            using (SPSite site = new SPSite(new Guid(SiteCollectionGuid)))
            {
                string _connectionString = site.RootWeb.Properties[eCaseConstants.PropertyBagKeys.ECASE_DB_CONNECTION_STRING];
                using (DbAdapter dbAdapter = new DbAdapter())
                {
                    dbAdapter.Connect(_connectionString);
                    try
                    {
                        GetCaseWebsSProc sProc = new GetCaseWebsSProc(new Guid(SiteCollectionGuid));
                        dbAdapter.ExecuteReaderStoredProcedure(sProc);
                        while (dbAdapter.DataReader.Read())
                        {
                            string caseWebGuid = dbAdapter.DataReader["CaseWebGuid"].ToString();
                            try
                            {
                                using (SPWeb caseWeb = site.OpenWeb(new Guid(caseWebGuid)))
                                { CasesNextDueDate.UpdateNextDueDate(caseWeb); /* This method logs errors and consumes them */ }
                            }
                            catch (Exception x)
                            { Logger.Instance.Error(string.Format("UpdateNextDueDate Timer Job Failed to Locate Case Web {0}", caseWebGuid), x, DiagnosticsCategories.eCaseCommon); }
                        }
                    }
                    catch (Exception x)
                    { Logger.Instance.Error(string.Format("UpdateNextDueDate Timer Job Failed for Site Collection {0}", site.Url), x, DiagnosticsCategories.eCaseCommon); }
                }
            }
        }
    }
}
