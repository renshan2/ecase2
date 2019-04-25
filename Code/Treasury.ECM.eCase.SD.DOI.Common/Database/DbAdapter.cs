using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using Treasury.ECM.eCase.SusDeb.DOI.Common.Database.StoredProcedures;

namespace Treasury.ECM.eCase.SusDeb.DOI.Common.Database
{
    public class DbAdapter : IDisposable
    {
        /// <summary>
        /// Connection string attributes used to initiate connection to SQL Server Database Instance
        /// </summary>
        public SqlConnectionStringBuilder ConnectionString;
        private SqlConnection _sqlConnection;
        private SqlDataReader _dataReader;

        /// <summary>
        /// Returns a data reader populated by the execution of a stored procedure that binds local data reader
        /// </summary>
        public SqlDataReader DataReader { get { return _dataReader; } }

        public bool IsConnected 
        { 
            get 
            {
                bool retVal = false;
                if (_sqlConnection != null &&
                    _sqlConnection.State == System.Data.ConnectionState.Open)
                    retVal = true;
                return retVal;
            } 
        }

        /// <summary>
        /// Private constructor to be used by static SqlConnection method
        /// </summary>
        public DbAdapter()
        {
            ConnectionString = new SqlConnectionStringBuilder();
        }

        /// <summary>
        /// Creates a SQL Connection and Opens it using the instance's ConnectionString
        /// If already connected, disconnects first.
        /// </summary>
        /// <exception cref="Exception">Wraps an method identifying message around the resulting exception from <see cref="System.Data.SqlClient.SqlException"/> </exception>
        public void Connect()
        {
            // Disconnect if a connection is already established before attempting to connect
            if (_sqlConnection != null)
                Disconnect();

            _sqlConnection = new SqlConnection(ConnectionString.ConnectionString);
            try
            {
                _sqlConnection.Open();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                _sqlConnection = null;
                throw new Exception(string.Format("Failed to connect to database.  Check connection string: {0}", ConnectionString), ex);
            }
            catch (Exception x)
            {
                _sqlConnection = null;
                throw new Exception(string.Format("Failed to connect to database.  Check connection string: {0}", ConnectionString), x);
            }
        }

        /// <summary>
        /// If already connected, compares new string vs. existing string.  If identical, does nothing.
        /// Otherwise, creates a SQL Connection and Opens it using the supplied connectionString, disconnecting
        /// an existing connection.
        /// </summary>
        /// <param name="connectionString">SQL connection string to use</param>
        public void Connect(string connectionString)
        {
            if (_sqlConnection == null ||
                string.Compare(connectionString, ConnectionString.ConnectionString) != 0)
            {
                ConnectionString = new SqlConnectionStringBuilder(connectionString);
                Connect();
            }
        }

        /// <summary>
        /// Closes any open connections and disposes of the underlying SqlConnection object
        /// </summary>
        public void Disconnect()
        {
                if (_dataReader != null)
                    _dataReader.Dispose();

                if (_sqlConnection != null)
                {
                    _sqlConnection.Close();
                    _sqlConnection.Dispose();
                    _sqlConnection = null;
                }
        }

        public SqlDataReader Query(string query)
        {
            if (_sqlConnection == null)
                throw new NullReferenceException(string.Format("No database connection exists. You must connect first. ConnectionString: {0}", ConnectionString));
            else if (string.IsNullOrEmpty(query))
                throw new NullReferenceException("Query must be specified");

            try
            {
                using (SqlCommand sqlCmd = new SqlCommand(query, _sqlConnection))
                {
                    if (_dataReader != null)
                    {
                        _dataReader.Close();
                        _dataReader.Dispose();
                        _dataReader = null;
                    }
                    _dataReader = sqlCmd.ExecuteReader(System.Data.CommandBehavior.Default);
                }
            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Failed executing Query: {0}\n{1}", query, x.ToString()), x);
            }

            return _dataReader;
        }

        /// <summary>
        /// Executes the supplied stored procedure synchronously.
        /// </summary>
        /// <remarks>Requires an open SqlConnection</remarks>
        /// <param name="sProc"></param>
        public void ExecuteNonQueryStoredProcedure(IeCaseStoredProc sProc)
        {
            if (_sqlConnection == null)
                throw new NullReferenceException(string.Format("No database connection exists. You must connect first. ConnectionString: {0}", ConnectionString));
            else if (sProc == null || string.IsNullOrEmpty(sProc.StoredProcedure))
                throw new NullReferenceException("Stored Procedure must be specified");

            try
            {
                using (SqlCommand sqlCmd = new SqlCommand(sProc.StoredProcedure, _sqlConnection))
                {
                    sqlCmd.Parameters.AddRange(sProc.Parameters);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    if (_dataReader != null)
                    {
                        _dataReader.Close();
                        _dataReader.Dispose();
                        _dataReader = null;
                    }

                    sqlCmd.ExecuteNonQuery();
                }
            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Failed executing Stored Procedure: {0}\n{1}", sProc.StoredProcedure, x.ToString()), x);
            }
        }

        /// <summary>
        /// Executes the supplied stored proedure synchronously, returning the first column of the first row.
        /// </summary>
        /// <remarks>Returns a maximum of 2033 characters</remarks>
        /// <param name="sProc"></param>
        /// <returns>The first column of the first row, or null</returns>
        public object ExecuteScalarStoredProcedure(IeCaseStoredProc sProc)
        {
            object retVal = null;
            if (_sqlConnection == null)
                throw new Exception(string.Format("No database connection exists. You must Connect() first. ConnectionString: {0}", ConnectionString));
            else if (sProc == null || string.IsNullOrEmpty(sProc.StoredProcedure))
                throw new Exception("Stored Procedure must be specified");

            try
            {
                using (SqlCommand sqlCmd = new SqlCommand(sProc.StoredProcedure, _sqlConnection))
                {
                    sqlCmd.Parameters.AddRange(sProc.Parameters);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    if (_dataReader != null)
                    {
                        _dataReader.Close();
                        _dataReader.Dispose();
                        _dataReader = null;
                    }

                    retVal = sqlCmd.ExecuteScalar();
                }
            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Failed executing Stored Procedure: {0}\n{1}", sProc.StoredProcedure, x.ToString()), x);
            }

            return retVal;
        }

        /// <summary>
        /// Executes the supplied stored procedure synchronously, returning a data reader
        /// </summary>
        /// <param name="sProc"></param>
        public void ExecuteReaderStoredProcedure(IeCaseStoredProc sProc)
        {
            if (_sqlConnection == null)
                throw new Exception(string.Format("No database connection exists. You must Connect() first. ConnectionString: {0}", ConnectionString));

            try
            {

                using (SqlCommand sqlCmd = new SqlCommand(sProc.StoredProcedure, _sqlConnection))
                {
                    sqlCmd.Parameters.AddRange(sProc.Parameters);
                    sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

                    if (_dataReader != null)
                    {
                        _dataReader.Close();
                        _dataReader.Dispose();
                        _dataReader = null;
                    }

                    _dataReader = sqlCmd.ExecuteReader();
                }
            }
            catch (Exception x)
            {
                throw new Exception(string.Format("Failed executing Stored Procedure: {0}\n{1}", sProc.StoredProcedure, x.ToString()), x);
            }
            finally
            {

            }
        }

        #region IDisposable Members
        /// <summary>
        /// Dispose of any Disposable Objects initiated by this Adapter
        /// </summary>
        void IDisposable.Dispose()
        {
            Disconnect();
        }
        #endregion
    }
}
