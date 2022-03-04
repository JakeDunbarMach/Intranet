using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    public class EntityDocument : AppData.BaseClasses.DataClassBase
    {
        #region Read Methods

        /// <summary>
        /// Retrieve a Document record
        /// </summary>
        /// <param name="DocumentID">ID of Document record to view</param>
        /// <returns>List of AppModel.Document</returns>
        public AppModel.EntityDocument GetDocumentItem(int EntityDocumentID)
        {
            AppModel.EntityDocument oEntityDocument = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = this.CreateSQLCommand();
                cmd.CommandText = "usp_EntityDocument_Select_By_ID";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EntityDocumentID", NullCheck(EntityDocumentID));
                cmd.Connection = this.OpenConnection();
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oReader.Read();
                    oEntityDocument = new AppModel.EntityDocument();
                    oEntityDocument = AppData.Mappers.EntityDocument.MapEntityDocument(oEntityDocument, oReader);
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

            return oEntityDocument;

        }



        /// <summary>
        /// Retrieve the list of documents for ID
        /// </summary>
        /// <param name="ID">ID of related item</param>
        /// <returns>List of AppModel.Document</returns>
        public List<AppModel.EntityDocument> GetDocumentList(int EntityID)
        {
            List<AppModel.EntityDocument> oList = null;
            AppModel.EntityDocument oEntityDocument = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = this.CreateSQLCommand();
                cmd.CommandText = "usp_EntityDocument_Select_By_CSID";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EntityID", NullCheck(EntityID));
                cmd.Connection = this.OpenConnection();
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oList = new List<AppModel.EntityDocument>();
                    while (oReader.Read())
                    {
                        oEntityDocument = new AppModel.EntityDocument();
                        oEntityDocument = AppData.Mappers.EntityDocument.MapEntityDocument(oEntityDocument, oReader);
                        oList.Add(oEntityDocument);
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

        #region Insert Methods

        /// <summary>
        /// Insert a new Document record
        /// </summary>
        /// <param name="document">Model from form</param>
        /// <param name="userID">ID of current user</param>
        /// <returns>ID of inserted record</returns>
        public int InsertDocument(AppModel.EntityDocument document, int userID)
        {
            int ID;
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "usp_EntityDocument_Insert";
                cmd.Parameters.AddWithValue("@EntityID", NullCheck(document.EntityID));
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(userID));
                cmd.Parameters.AddWithValue("@DocumentName", NullCheck(document.DocumentName));
                cmd.Parameters.AddWithValue("@DocumentPath", NullCheck(document.DocumentPath));
                cmd.Connection = this.OpenConnection();
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ReleaseSQLConnection(cmd.Connection);
                ReleaseSQLCommand(cmd);
            }

            return ID;
        }

        #endregion

        #region Delete Methods

        /// <summary>
        /// Delete Document record by documentID
        /// </summary>
        /// <param name="DocumentID">ID of record to delete</param>
        /// <param name="userID">ID of current user</param>
        public int DeleteDocument(int EntityDocumentID, int userID)
        {
            int ID;
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "usp_EntityDocument_Delete";
                cmd.Parameters.AddWithValue("@EntityDocumentID", NullCheck(EntityDocumentID));
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(userID));
                cmd.Connection = this.OpenConnection();
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ReleaseSQLConnection(cmd.Connection);
                ReleaseSQLCommand(cmd);
            }

            return ID;
        }


        #endregion 
    }
}
