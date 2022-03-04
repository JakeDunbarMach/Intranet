using System.Data.SqlClient;

namespace AppData.Mappers
{
    internal class SystemPermission : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="permission">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>Model.Permission</returns>
        public static AppModel.SystemPermission MapPermission(AppModel.SystemPermission permission, SqlDataReader data)
        {
            AppModel.SystemPermission mappedPermission = permission;
            mappedPermission.PermissionID = GetInt32(data, "PermissionID");
            mappedPermission.PermissionName = GetString(data, "PermissionName");
            mappedPermission.IsActive = GetBoolean(data, "IsActive");
            return mappedPermission;
        }

    }
}