using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures
{
    public interface IeCaseStoredProc
    {
        /// <summary>
        /// A string containing the name of the Stored Procedure to be executed
        /// </summary>
        string StoredProcedure { get; }

        /// <summary>
        /// A collection of SqlParamters to be added to the collection of the SqlCommand
        /// </summary>
        SqlParameter[] Parameters { get; }
    }
}
