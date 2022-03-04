using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    public class Entity : BaseClasses.DataClassBase
    {
        #region Read Methods

        /// <summary>
        /// Retrieve from ID
        /// </summary>
        /// <param name="ID">ID to edit</param>
        /// <param name="userID">ID of user</param>
        /// <returns>Model</returns>
        public AppModel.Entity GetEntity(int ID, int userID)
        {
            AppModel.Entity oItem = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandText = "usp_Entity_Select_By_ID";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EntityID", NullCheck(ID));
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(userID));
                cmd.Connection = OpenConnection();
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oReader.Read();
                    oItem = new AppModel.Entity();
                    oItem = Mappers.Entity.MapEntity(oItem, oReader);
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

            return oItem;
        }

        #endregion

        #region Update Methods

        /// <summary>
        /// Update a record
        /// </summary>
        /// <param name="model">Model from form</param>
        /// <param name="userID">ID of current user</param>
        /// <returns>int showing success of update</returns>
        public int UpdateEntity(AppModel.Entity entity, int userID)
        {
            int ID;
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "usp_Entity_Update";
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(userID));
                cmd.Parameters.AddWithValue("@EntityID", NullCheck(entity.EntityID));
                cmd.Parameters.AddWithValue("@EntityDate", NullCheck(entity.EntityDate));
                cmd.Parameters.AddWithValue("@EntityTypeID", NullCheck(entity.EntityTypeID));
                cmd.Connection = OpenConnection();
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

        #region Insert Methods

        /// <summary>
        /// Insert a new record
        /// </summary>
        /// <param name="model">Model from form</param>
        /// <param name="userID">ID of current user</param>
        /// <returns>ID of inserted record</returns>
        public int InsertEntity(AppModel.Entity entity, int userID)
        {
            int ID;
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "usp_Entity_Insert";
                cmd.Parameters.AddWithValue("EmployeeID", NullCheck(userID));
                cmd.Parameters.AddWithValue("@EntityDate", NullCheck(entity.EntityDate));
                cmd.Parameters.AddWithValue("@EntityTypeID", NullCheck(entity.EntityTypeID));
                cmd.Connection = OpenConnection();
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
        /// Delete record by ID
        /// </summary>
        /// <param name="ID">ID of record to delete</param>
        /// <param name="userID">ID of current user</param>
        public int DeleteEntity(int entityID, int userID)
        {
            int ID;
            SqlCommand cmd = new SqlCommand();

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "usp_Entity_Delete";
                cmd.Parameters.AddWithValue("@EntityID", NullCheck(entityID));
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(userID));
                cmd.Connection = OpenConnection();
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
