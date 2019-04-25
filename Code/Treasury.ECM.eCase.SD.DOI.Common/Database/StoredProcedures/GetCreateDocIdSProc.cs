using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures
{
    public class GetCreateDocIdSProc : IeCaseStoredProc
    {
        private List<SqlParameter> parameters;
        public SqlParameter[] Parameters { get { return parameters.ToArray(); } }
        public string StoredProcedure { get { return "[dbo].[usp_GetCreateDocId]"; } }
        public GetCreateDocIdSProc(Guid siteGuid, Guid webGuid, Guid listItemGuid, bool forceUpdate, string prefix, uint id, int uniqueId)
        {
            parameters = new List<SqlParameter>();

            SqlParameter sParam = new SqlParameter("@siteGuid", SqlDbType.NChar);
            sParam.Value = siteGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@webGuid", SqlDbType.NChar);
            sParam.Value = webGuid.ToString();
            parameters.Add(sParam);

            sParam = new SqlParameter("@listItemGuid", SqlDbType.NChar);
            sParam.Value = listItemGuid.ToString();
            parameters.Add(sParam);
            
            sParam = new SqlParameter("@forceUpdate", SqlDbType.Bit);
            sParam.Value = forceUpdate;
            parameters.Add(sParam);

            sParam = new SqlParameter("@pre", SqlDbType.NVarChar);
            sParam.Direction = ParameterDirection.InputOutput;
            sParam.Value = prefix;
            parameters.Add(sParam);

            sParam = new SqlParameter("@docId", SqlDbType.BigInt);
            sParam.Direction = ParameterDirection.Output;
            sParam.Value = id;
            parameters.Add(sParam);

            sParam = new SqlParameter("@uniqueId", SqlDbType.BigInt);
            sParam.Value = uniqueId;
            parameters.Add(sParam);
        }
    }
}
