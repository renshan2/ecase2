using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures
{
    public class GetCaseWebsSProc : IeCaseStoredProc
    {
        private List<SqlParameter> parameters;
        public SqlParameter[] Parameters { get { return parameters.ToArray(); } }
        public string StoredProcedure { get { return "[dbo].[usp_GetCaseWebs]"; } }
        public GetCaseWebsSProc(Guid siteGuid)
        {
            parameters = new List<SqlParameter>();

            SqlParameter sParam = new SqlParameter("@siteGuid", SqlDbType.NChar);
            sParam.Value = siteGuid.ToString();
            parameters.Add(sParam);
        }
    }
}
