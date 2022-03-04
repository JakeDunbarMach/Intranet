using AppModel.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AppModel
{
    /// <summary>
    /// Based on this example http://stackoverflow.com/questions/5366549/why-does-asp-net-mvc-range-attribute-take-a-type
    /// Flags the model property as valid/invalid and Model.IsValid is checked on save
    /// </summary>

    public class CustomDateRangeAttribute : ValidationAttribute
    {
        private const string DateFormat = "dd/MM/yyyy";
        private const string DefaultErrorMessage = Validation.FAILURE_DATE_RANGE;

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        public CustomDateRangeAttribute()
            : base(DefaultErrorMessage)
        {
            //Read the valid min and max dates from Validation.cs
            MinDate = ParseDate(Validation.DATETIME_MIN_VALUE.ToShortDateString());
            MaxDate = ParseDate(Validation.DATETIME_MAX_VALUE.ToShortDateString());
        }

        /// <summary>
        /// Override IsValid checking if the date is between the date range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null || !(value is DateTime))
            {
                return true;
            }
            DateTime dateValue = (DateTime)value;
            return MinDate <= dateValue && dateValue <= MaxDate;
        }

        /// <summary>
        /// Override the error message returned to use the min and max dates specified above
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString,
                name, MinDate.ToShortDateString(), MaxDate.ToShortDateString());
        }

        /// <summary>
        /// Parse the data with the format specified in constant DateFormat
        /// </summary>
        /// <param name="dateValue"></param>
        /// <returns></returns>
        private static DateTime ParseDate(string dateValue)
        {
            return DateTime.ParseExact(dateValue, DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
