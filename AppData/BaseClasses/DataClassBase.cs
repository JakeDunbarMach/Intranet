using System;
using System.Data.SqlClient;
using System.Configuration;

namespace AppData.BaseClasses
{
    public abstract class DataClassBase
    {
        #region SQlCommands

        /// <summary>
        /// Creates a new SQLCommand and sets the CommandTimeout from web config.
        /// </summary>
        protected SqlCommand CreateSQLCommand()
        {
            try
            {
                SqlCommand cmdCommand = new SqlCommand();
                cmdCommand.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings[Constants.Configuration.COMMAND_TIMEOUT_IN_SECONDS]);
                return cmdCommand;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Dispose of the SqlCommand
        /// </summary>
        /// <param name="command"></param>
        protected void ReleaseSQLCommand(SqlCommand command)
        {
            try
            {
                if ((command != null))
                {
                    command.Dispose();
                    command = null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Data Object Release Methods

        /// <summary>
        /// Dispose of the SqlDataAdapter
        /// </summary>
        /// <param name="darDataAdaptor"></param>
        protected void ReleaseSQLDataAdaptor(SqlDataAdapter darDataAdaptor)
        {
            try
            {
                if ((darDataAdaptor != null))
                {
                    darDataAdaptor.Dispose();
                    darDataAdaptor = null;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Closes and disposes the SQLDataReader.
        /// </summary>
        /// <param name="oReader"></param>
        protected void ReleaseSQLDataReader(SqlDataReader oReader)
        {
            try
            {
                if (oReader != null)
                {
                    if (!oReader.IsClosed)
                    {
                        oReader.Close();
                        oReader.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Connection Methods

        /// <summary>
        /// Open a connection to the SQL Database
        /// </summary>
        /// <returns>SQLConnection</returns>
        protected SqlConnection OpenConnection(bool? Maintenance = null)
        {
            SqlConnection functionReturnValue = new SqlConnection();
            SqlConnection conConnection = new SqlConnection();
            SqlConnectionStringBuilder oBuilder = new SqlConnectionStringBuilder();

            try
            {
                if(Maintenance == true)
                {
                    conConnection.ConnectionString = ConfigurationManager.ConnectionStrings["MaintenanceConnection"].ConnectionString;
                }
                else
                {
                    conConnection.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                }
                conConnection.Open();
                functionReturnValue = conConnection;
                conConnection = null;
            }
            catch (Exception)
            {
                throw;
            }
            return functionReturnValue;
        }

        /// <summary>
        /// Disposes of the SqlConnection
        /// </summary>
        /// <param name="connection"></param>
        protected void ReleaseSQLConnection(SqlConnection connection)
        {
            try
            {
                if ((connection != null))
                {
                    //Dispose calls close too so safe to use
                    connection.Dispose();
                    connection = null;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region NullCheck

        /// <summary>
        /// Check if the parameter (objSQLParameterValue) is null and convert to DBNull
        /// </summary>
        /// <param name="objSQLParameterValue"></param>
        /// <returns></returns>
        protected object NullCheck(object objSQLParameterValue)
        {
            // Empty strings should be converted to Null SQL Parameter values
            if ((objSQLParameterValue == null))
                return DBNull.Value;
            if (object.ReferenceEquals(objSQLParameterValue.GetType(), typeof(string)) && Convert.ToString(objSQLParameterValue) == string.Empty)
                return DBNull.Value;

            return objSQLParameterValue;

        }

        #endregion

        #region Formatting

        /// <summary>
        /// Convert int array to string - for use with multiselect dropdowns
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        internal string IntArrayToString(int[] array)
        {
            string output = "";

            if (array != null)
            {
                output = string.Join(",", array);
            }
            else
            {
                output = null;
            }
            return output;
        }

        #endregion

    }
}
