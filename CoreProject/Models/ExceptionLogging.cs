using System;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace CoreProject.Models
{
    public interface IExceptionLogging
        {
            bool LogAppException(string ErrorMessage);
        }
    public class ExceptionLoggingToSQL : IExceptionLogging
    {        
        private string dbConnectionString;
        IConfiguration Configuration;
        public bool LogAppException(string ErrorMessage)
        {
            dbConnectionString = Configuration["DBStrings"];
            try
            {
                using (SqlCommand cmd = new SqlCommand(
                "ErrorLog", new SqlConnection(dbConnectionString)))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Descr", SqlDbType.UniqueIdentifier);
                    cmd.Parameters["@Descr"].Value = ErrorMessage;
                    cmd.Connection.Open();
                    cmd.ExecuteReader();
                }
                return true;
            }
            catch (Exception ex)
            {
                // you can use some other logging method like Elastic here.
                return false;
            }
        }
    }
}