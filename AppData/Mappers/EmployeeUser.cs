using System.Data.SqlClient;

namespace AppData.Mappers
{
    internal class EmployeeUser : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="setupUser">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>Model.User</returns>
        public static AppModel.EmployeeUser MapEmployeeUser(AppModel.EmployeeUser setupUser, SqlDataReader data)
        {
            AppModel.EmployeeUser mappedUser = setupUser;
            mappedUser.EmployeeID = GetInt32(data, "EmployeeID");
            mappedUser.EmployeeFullName = GetString(data, "EmployeeFullName");
            mappedUser.IsActive = GetBoolean(data, "IsActive");
            if (CheckColumnExists(data, "EmployeeTypeID"))
            {
                mappedUser.UserEmployeeTypeID = GetInt32(data, "EmployeeTypeID");
            }
            if (CheckColumnExists(data, "ReportsToID"))
            {
                mappedUser.ManagedByID = GetInt32Nullable(data, "ReportsToID");
            }
            return mappedUser;
        }
    }
}
