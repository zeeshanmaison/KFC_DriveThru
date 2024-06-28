using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DriveThru.Integration.Core.Helpers
{
    public class OracleHelper
    {
        public static DataSet GetDataSet(string connectionString, string sql, CommandType commandType)
        {
            DataSet ds = new DataSet();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(sql, connection))
                    {
                        using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                        {
                            adapter.SelectCommand.Transaction = transaction;
                            adapter.SelectCommand.InitialLONGFetchSize = 1000;
                            adapter.SelectCommand.CommandType = commandType;
                            adapter.SelectCommand.BindByName = true;

                            adapter.Fill(ds);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }

            return ds;
        }

        public static int ExecuteNonQuery(string connectionString, string sql, CommandType commandType, OracleParameter[] parameters)
        {
            int affectedRows = 0;

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                connection.Open();

                try
                {
                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {
                        command.CommandType = commandType;

                        if (parameters != null && parameters.Length > 0)
                        {
                            foreach (var param in parameters)
                            {
                                command.Parameters.Add(param);
                            }

                            affectedRows = command.ExecuteNonQuery();
                        }
                        else
                        {
                            affectedRows = command.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    connection.Close();
                }

                return affectedRows;
            }
        }

        public static DataSet ExecuteQuery(string connectionString, string sql, CommandType commandType)
        {
            return GetDataSet(connectionString, sql, commandType);
        }
    }
}
