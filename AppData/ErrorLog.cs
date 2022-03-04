using System;
using System.Data.SqlClient;

namespace AppData
{
    public class ErrorLog : BaseClasses.DataClassBase
    {
        public int InsertError(string guid, string error)
        {

            SqlCommand cmd = new SqlCommand();
            int ID;

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "usp_ErrorLog_Insert";

                cmd.Parameters.AddWithValue("@Guid", guid);
                cmd.Parameters.AddWithValue("@Description", error);
                cmd.Connection = OpenConnection();
                ID = Convert.ToInt32(cmd.ExecuteScalar());
                return ID;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ReleaseSQLConnection(cmd.Connection);
                ReleaseSQLCommand(cmd);
            }
        }
    }
}
