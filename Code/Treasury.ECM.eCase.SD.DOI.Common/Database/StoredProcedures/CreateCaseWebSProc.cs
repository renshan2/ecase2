using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures
{
    public class CreateCaseWebSProc : IeCaseStoredProc
    {
        private List<SqlParameter> parameters;
        public SqlParameter[] Parameters { get { return parameters.ToArray(); } }
        public string StoredProcedure { get { return "[dbo].[usp_CreateCaseWeb]"; } }
        public CreateCaseWebSProc(Guid siteGuid, Guid caseListItemGuid, Guid caseWebGuid, Guid activitiesTasksGuid, Guid caseRelatedDatesGuid)
        {
            parameters = new List<SqlParameter>();

            SqlParameter sParam = new SqlParameter("@siteGuid", SqlDbType.NChar);
            sParam.Value = siteGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@caseListItemGuid", SqlDbType.NChar);
            sParam.Value = caseListItemGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@caseWebGuid", SqlDbType.NChar);
            sParam.Value = caseWebGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@activitiesTasksGuid", SqlDbType.NChar);
            sParam.Value = activitiesTasksGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@caseRelatedDatesGuid", SqlDbType.NChar);
            sParam.Value = caseRelatedDatesGuid.ToString();
            parameters.Add(sParam);
        }
    }
}
