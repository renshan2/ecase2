using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures
{
    public class CreateSPObjPermSProc : IeCaseStoredProc
    {
        private List<SqlParameter> parameters;
        public SqlParameter[] Parameters { get { return parameters.ToArray(); } }
        public string StoredProcedure { get { return "[dbo].[usp_CreateSPObjectPermission]"; } }
        public CreateSPObjPermSProc(Guid siteGuid, Guid caseWebGuid, Guid? childWebGuid, Guid? listGuid, Guid? listItemGuid, string roleAssignmentsXml)
        {
            parameters = new List<SqlParameter>();

            SqlParameter sParam = new SqlParameter("@siteGuid", SqlDbType.NChar);
            sParam.Value = siteGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@caseWebGuid", SqlDbType.NChar);
            sParam.Value = caseWebGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@childWebGuid", SqlDbType.NChar);
            sParam.Value = (childWebGuid == null) ? (object)DBNull.Value : (object)childWebGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@listGuid", SqlDbType.NChar);
            sParam.Value = (listGuid == null) ? (object)DBNull.Value : (object)listGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@listItemGuid", SqlDbType.NChar);
            sParam.Value = (listItemGuid == null) ? (object)DBNull.Value : (object)listItemGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@roleAssignments", SqlDbType.Xml);
            sParam.Value = roleAssignmentsXml;
            parameters.Add(sParam);
        }
    }
}
