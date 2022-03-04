using System;
using System.Data.SqlClient;

namespace AppData.BaseClasses
{
    public abstract class MapperClassBase
    {
        /// <summary>
        /// Casting and converting datatypes for mapping classes
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="column"></param>
        /// <returns></returns>

        protected static Guid GetGuid(SqlDataReader reader, string column)
        {
            Guid data = (Guid)reader[column];
            return data;
        }

        protected static short GetInt16(SqlDataReader reader, string column)
        {
            short data = short.Parse(reader[column].ToString());
            return data;
        }

        protected static int GetInt32(SqlDataReader reader, string column)
        {
            int data = int.Parse(reader[column].ToString());
            return data;
        }

        protected static long GetInt64(SqlDataReader reader, string column)
        {
            long data = long.Parse(reader[column].ToString());
            return data;
        }

        protected static decimal GetDecimal(SqlDataReader reader, string column)
        {
            decimal data = decimal.Parse(reader[column].ToString());
            return data;
        }

        protected static bool GetBoolean(SqlDataReader reader, string column)
        {

            bool data = bool.Parse(reader[column].ToString());
            return data;

        }

        protected static byte GetByte(SqlDataReader reader, string column)
        {
            byte data = byte.Parse(reader[column].ToString());
            return data;
        }

        protected static byte[] GetByteArray(SqlDataReader reader, string column)
        {
            byte[] data = (byte[])reader[column];
            return data;
        }

        protected static string GetString(SqlDataReader reader, string column)
        {
            string data = (reader.IsDBNull(reader.GetOrdinal(column)))
                                   ? null : reader[column].ToString();
            return data;
        }

        protected static char GetChar(SqlDataReader reader, string column)
        {
            char data = Convert.ToChar(reader[column]);
            return data;
        }

        protected static DateTime GetDateTime(SqlDataReader reader, string column)
        {
            DateTime data = (DateTime)reader[column];
            return data;
        }

        protected static double GetDouble(SqlDataReader reader, string column)
        {
            double data = (double)reader[column];
            return data;
        }

        protected static float GetFloat(SqlDataReader reader, string column)
        {
            float data = float.Parse(reader[column].ToString());
            return data;
        }


        protected static Guid? GetGuidNullable(SqlDataReader reader, string column)
        {
            Guid? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                                    ? new Guid?() : (Guid)reader[column];
            return data;
        }

        protected static short? GetInt16Nullable(SqlDataReader reader, string column)
        {
            short? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                                    ? new short?() : Convert.ToInt16(reader[column]);
            return data;
        }

        protected static int? GetInt32Nullable(SqlDataReader reader, string column)
        {
            int? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                                    ? new int?() : Convert.ToInt32(reader[column]);
            return data;
        }

        protected static long? GetInt64Nullable(SqlDataReader reader, string column)
        {
            long? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                                    ? new long?() : Convert.ToInt64(reader[column]);
            return data;
        }

        protected static decimal? GetDecimalNullable(SqlDataReader reader, string column)
        {
            decimal? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                        ? new decimal?() : Convert.ToDecimal(reader[column]);
            return data;
        }

        protected static bool? GetBooleanNullable(SqlDataReader reader, string column)
        {
            bool? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                                     ? new bool?() : (bool)reader[column];
            return data;
        }

        protected static byte? GetByteNullable(SqlDataReader reader, string column)
        {
            byte? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                        ? new byte?() : (byte)reader[column];
            return data;
        }

        protected static DateTime? GetDateTimeNullable(SqlDataReader reader, string column)
        {
            DateTime? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                               ? new DateTime?() : (DateTime)reader[column];
            return data;
        }

        protected static double? GetDoubleNullable(SqlDataReader reader, string column)
        {
            double? data = (reader.IsDBNull(reader.GetOrdinal(column)))
                        ? new double?() : Convert.ToDouble(reader[column]);
            return data;
        }

        /// <summary>
        /// Filter the dataReader and check if column exists
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName">String of column name to check if exists</param>
        /// <returns></returns>
        public static bool CheckColumnExists(SqlDataReader dataReader, string columnName)
        {
            bool retVal = false;
            try
            {
                dataReader.GetSchemaTable().DefaultView.RowFilter = string.Format("ColumnName= '{0}'", columnName);
                if (dataReader.GetSchemaTable().DefaultView.Count > 0)
                {
                    retVal = true;
                }
            }

            catch (Exception)
            {
                throw;
            }

            return retVal;
        }

    }
}
