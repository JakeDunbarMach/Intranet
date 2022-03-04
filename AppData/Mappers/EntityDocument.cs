using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData.Mappers
{
    internal class EntityDocument : BaseClasses.MapperClassBase
    {

        /// <summary>
        /// Map the data returned from SQL to the properties on the model
        /// </summary>
        /// <param name="model">Model to be mapped</param>
        /// <param name="data">Data returned from SQL</param>
        /// <returns>AppModel</returns>
        public static AppModel.EntityDocument MapEntityDocument(AppModel.EntityDocument EntityDocument, SqlDataReader data)
        {
            AppModel.EntityDocument mappedEntityDocument = EntityDocument;
            mappedEntityDocument.EntityDocumentID = GetInt32(data, "EntityDocumentID");
            mappedEntityDocument.EntityID = GetInt32(data, "EntityID");
            mappedEntityDocument.DocumentPath = GetString(data, "DocumentPath");
            mappedEntityDocument.DocumentName = GetString(data, "DocumentName");
            return mappedEntityDocument;
        }

    }
}
