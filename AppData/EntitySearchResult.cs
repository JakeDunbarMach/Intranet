using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    public class EntitySearchResult : BaseClasses.DataClassBase
    {
        #region Read Methods

        /// <summary>
        /// Retrieve Search results
        /// </summary>
        /// <param name="oSearch">Related Search Model for search parameters</param>
        /// <param name="userID">ID of current user</param>
        /// <returns>List of AppModel.*SearchResult</returns>
        public List<AppModel.EntitySearchResult> GetEntityList(AppModel.EntitySearch oSearch, int userID)
        {
            List<AppModel.EntitySearchResult> oList = null;
            AppModel.EntitySearchResult oEntitySearchResult = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {
                cmd = CreateSQLCommand();
                cmd.CommandText = "usp_Entity_Select_Search";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeID", NullCheck(userID));
                cmd.Parameters.AddWithValue("@EntityRef", NullCheck(oSearch.EntityRef));
                cmd.Parameters.AddWithValue("@EntityTypeID", NullCheck(oSearch.EntityTypeID));
                cmd.Parameters.AddWithValue("@DateFrom", NullCheck(oSearch.DateFrom));
                cmd.Parameters.AddWithValue("@DateTo", NullCheck(oSearch.DateTo));
                cmd.Connection = OpenConnection();
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oList = new List<AppModel.EntitySearchResult>();
                    while (oReader.Read())
                    {
                        oEntitySearchResult = new AppModel.EntitySearchResult();
                        oEntitySearchResult = AppData.Mappers.EntitySearchResult.MapEntitySearchResult(oEntitySearchResult, oReader);
                        oList.Add(oEntitySearchResult);
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

    }
}
