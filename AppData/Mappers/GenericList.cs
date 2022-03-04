using System.Data.SqlClient;

namespace AppData.Mappers
{
    internal class GenericList : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="genericList">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>Model.GenericList</returns>
        public static AppModel.GenericList MapGenericList(AppModel.GenericList genericList, SqlDataReader data)
        {
            AppModel.GenericList mappedGenericList = genericList;
            mappedGenericList.ID = GetInt32(data, "ID");
            mappedGenericList.Name = GetString(data, "Name");
            if (CheckColumnExists(data, "Selected"))
            {
                mappedGenericList.Selected = GetBoolean(data, "Selected");
            }
            if (CheckColumnExists(data, "ParentID"))
            {
                mappedGenericList.ParentID = GetInt32Nullable(data, "ParentID");
            }
            if (CheckColumnExists(data, "Parent"))
            {
                mappedGenericList.Parent = GetString(data, "Parent");
            }
            return mappedGenericList;
        }

    }
}
