using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Mappers
{
    internal class Entity : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="model">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>AppModel</returns>
        public static AppModel.Entity MapEntity(AppModel.Entity entity, SqlDataReader data)
        {
            AppModel.Entity mappedEntity = entity;
            mappedEntity.EntityID = GetInt32(data, "EntityID");
            mappedEntity.EntityRef = GetString(data, "EntityRef");
            mappedEntity.EntityDate = GetDateTime(data, "EntityDate");
            mappedEntity.EntityTypeID = GetInt32(data, "EntityTypeID");
            return mappedEntity;
        }
    }
}
