using System;

namespace AppModel.Constants
{
    /// <summary>
    /// Static constants used for validation messages
    /// </summary>
    public class Validation
    {
        #region Dates

        public static DateTime DATETIME_MIN_VALUE = new DateTime(1753, 1, 1, 00, 00, 00);
        public static DateTime DATETIME_MAX_VALUE = new DateTime(9999, 12, 31, 23, 59, 59);
        public static DateTime DATETIME_NULL_VALUE = new DateTime(1753, 01, 01, 00, 00, 00);

        #endregion

        #region Validation

        public const string FAILURE_STRING_LENGTH = "The field {0} must not be longer than {1} ";
        public const string FAILURE_STRING_NUMERIC = "The field {0}  must be a number";
        public const string FAILURE_DATE_RANGE = "The {0} field must be between {1} and {2}";

        #endregion

    }
}
