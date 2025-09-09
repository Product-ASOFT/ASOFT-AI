using ASOFT.Core.Common.InjectionChecker;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ASOFT.Core.API.Validation.Attributes
{
    /// <summary>
    /// Validation attribute cho date time range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
    public class DateTimeRangeAttribute : ValidationAttribute
    {
        /// <summary>
        /// From date in date range.
        /// </summary>
        public readonly DateTime FromDate;

        /// <summary>
        /// To date in date range.
        /// </summary>
        public readonly DateTime ToDate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">From date in date range.</param>
        /// <param name="toDate">To date in date range.</param>
        public DateTimeRangeAttribute(DateTime fromDate, DateTime toDate) : base(() => VSR.Format_InDateTimeRange)
        {
            Checker.NotNull(fromDate, nameof(fromDate));
            Checker.NotNull(toDate, nameof(toDate));

            FromDate = fromDate;
            ToDate = toDate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate">From date in date range.</param>
        /// <param name="toDate">To date in date range.</param>
        /// <param name="errorMessageAccessor">Function return error message.</param>
        public DateTimeRangeAttribute(DateTime fromDate, DateTime toDate, Func<string> errorMessageAccessor) : base(
            errorMessageAccessor)
        {
            Checker.NotNull(fromDate, nameof(fromDate));
            Checker.NotNull(toDate, nameof(toDate));

            FromDate = fromDate;
            ToDate = toDate;
        }

        /// <summary>
        /// Check object value in date range otherwise return false;
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value is DateTime date)
            {
                return FromDate <= date && date <= ToDate;
            }

            return true;
        }


        /// <summary>
        /// Format error message with date range and input name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            var shortDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            return string.Format(ErrorMessageString, name, FromDate.ToString(shortDateFormat),
                ToDate.ToString(shortDateFormat));
        }
    }
}