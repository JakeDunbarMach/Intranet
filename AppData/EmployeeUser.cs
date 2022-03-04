
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppData
{
    public class EmployeeUser : AppData.BaseClasses.DataClassBase
    {
        #region Read Methods

        /// <summary>
        /// Lookup User from network user name (windows identity)
        /// </summary>
        /// <param name="windowsIdentity">windows identity of current user</param>
        /// <returns>Model.User</returns>
        public AppModel.EmployeeUser GetUserData(string windowsIdentity, int systemID)
        {
            AppModel.EmployeeUser oUser = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandText = "usp_SetupEmployee_Select_User";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@WindowsIdentity", NullCheck(windowsIdentity));
                cmd.Parameters.AddWithValue("@SystemID", NullCheck(systemID));
                cmd.Connection = OpenConnection(true);
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oReader.Read();
                    oUser = new AppModel.EmployeeUser();
                    oUser = Mappers.EmployeeUser.MapEmployeeUser(oUser, oReader);

                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                ReleaseSQLDataReader(oReader);
                ReleaseSQLConnection(cmd.Connection);
                ReleaseSQLCommand(cmd);
            }

            return oUser;
        }

        #endregion

    }
}
