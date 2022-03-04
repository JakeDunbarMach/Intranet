using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Mappers
{
    internal class EntitySearchResult : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="model">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>AppModel</returns>
        public static AppModel.EntitySearchResult MapEntitySearchResult(AppModel.EntitySearchResult entitySearchResult, SqlDataReader data)
        {
            AppModel.EntitySearchResult mappedEntitySearchResult = entitySearchResult;
            mappedEntitySearchResult.EntityID = GetInt32(data, "EntityID");
            mappedEntitySearchResult.EntityRef = GetString(data, "EntityRef");
            mappedEntitySearchResult.EntityDate = GetDateTime(data, "EntityDate");
            mappedEntitySearchResult.EntityTypeName = GetString(data, "EntityTypeName");
            return mappedEntitySearchResult;
        }

    }
}
