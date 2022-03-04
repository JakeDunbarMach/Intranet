using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppData
{
    public class SystemPermission : AppData.BaseClasses.DataClassBase
    {
        #region Read Methods
        /// <summary>
        /// Retrieve the list of permissions in the system
        /// </summary>
        /// <returns>List of Model.Permission</returns>
        public List<AppModel.SystemPermission> GetPermissionList(int? sectionID)
        {
            List<AppModel.SystemPermission> oList = null;
            AppModel.SystemPermission oPermission = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandText = "usp_SetupPermission_Select_All";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SectionID", sectionID);
                cmd.Connection = OpenConnection();
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oList = new List<AppModel.SystemPermission>();
                    while (oReader.Read())
                    {
                        oPermission = new AppModel.SystemPermission();
                        oPermission = AppData.Mappers.SystemPermission.MapPermission(oPermission, oReader);
                        oList.Add(oPermission);
                    }
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

            return oList;

        }

        #endregion
    }
}
