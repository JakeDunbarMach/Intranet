using System.Data.SqlClient;

namespace AppData.Mappers
{
    internal class SystemEmployeePermission : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="setupUserPermission">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>Model.UserPermission</returns>
        public static AppModel.EmployeePermission MapEmployeePermission(AppModel.EmployeePermission setupUserPermission, SqlDataReader data)
        {
            AppModel.EmployeePermission mappedUserPermission = setupUserPermission;
            mappedUserPermission.EmployeePermID = GetInt16(data, "EmployeeSystemID");
            mappedUserPermission.EmployeeID = GetInt32(data, "EmployeeID");
            mappedUserPermission.SectionID = GetInt16(data, "SystemSectionID");
            mappedUserPermission.Section = GetString(data, "SectionName");
            mappedUserPermission.PermissionID = GetInt16(data, "PermissionID");
            mappedUserPermission.Permission = GetString(data, "PermissionName");
            mappedUserPermission.IsActive = GetBoolean(data, "IsActive");
            return mappedUserPermission;
        }
    }
}
