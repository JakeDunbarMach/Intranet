using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppData
{
    public class GenericList : AppData.BaseClasses.DataClassBase
    {
        #region Read Methods

        /// <summary>
        /// Gets data used to populate a select list
        /// </summary>
        /// <param name="storedProcName">Name of the stored proceedure</param>
        /// <returns></returns>
        public List<AppModel.GenericList> GetGenericList(string storedProcName, int? filterINT = null, int? filterINT2 = null)
        {
            List<AppModel.GenericList> oList = null;
            AppModel.GenericList oGenericList = null;
            SqlCommand cmd = new SqlCommand();
            SqlDataReader oReader = default(SqlDataReader);

            try
            {

                cmd = this.CreateSQLCommand();
                cmd.CommandText = storedProcName;
                if (filterINT > 0)
                {
                    cmd.Parameters.AddWithValue("@filterINT", NullCheck(filterINT));
                }
                if (filterINT2 > 0)
                {
                    cmd.Parameters.AddWithValue("@filterINT2", NullCheck(filterINT2));
                }
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = this.OpenConnection();
                oReader = cmd.ExecuteReader();

                if (oReader.HasRows)
                {
                    oList = new List<AppModel.GenericList>();
                    while (oReader.Read())
                    {
                        oGenericList = new AppModel.GenericList();
                        oGenericList = AppData.Mappers.GenericList.MapGenericList(oGenericList, oReader);
                        oList.Add(oGenericList);
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

        #region PopulateList Methods

        public List<AppModel.GenericList> GetEntityTypeList()
        {
            return GetGenericList("usp_EntityType_Select_Active");
        }

        public List<AppModel.GenericList> GetAvailableSitesList(int userID)
        {
            return GetGenericList("usp_SetupUserSite_Select_All_By_UserID", userID);
        }


        #endregion
    }
}
