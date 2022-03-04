using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace AppData
{
    public class SystemEmployeePermission : AppData.BaseClasses.DataClassBase
    {
        #region Read Methods

        /// <summary>
        /// Lookup permissions for a userID and sectionID
        /// </summary>
        /// <param name="sectionID">ID of section accessing i.e. Complaint</param>
        /// <param name="employeeID">ID of current user</param>
        /// <returns>Model.UserPermission</returns>
        public AppModel.EmployeePermission GetEmployeeSectionPermission(int employeeID, int sectionID, int systemID)
        {
            AppModel.EmployeePermission oUserPermission = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandText = "usp_SetupEmployeeSystems_SectionPermissions_Select_By_EmployeeID";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(employeeID));
                cmd.Parameters.AddWithValue("@SectionID", NullCheck(sectionID));
                cmd.Parameters.AddWithValue("@SystemID", NullCheck(systemID));
                cmd.Connection = OpenConnection(true);
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oReader.Read();
                    oUserPermission = new AppModel.EmployeePermission();
                    oUserPermission = Mappers.SystemEmployeePermission.MapEmployeePermission(oUserPermission, oReader);
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

            return oUserPermission;
        }

        #endregion
    }
}
