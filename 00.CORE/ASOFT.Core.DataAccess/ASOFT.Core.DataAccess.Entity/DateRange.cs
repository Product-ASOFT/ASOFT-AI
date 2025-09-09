using System;

namespace ASOFT.Core.DataAccess.Entities
{
    /// <summary>
    /// The class hold date time range.
    /// See <see cref="FromDate"/>.
    /// See <see cref="ToDate"/>
    /// </summary>
    public class DateRange
    {
        /// <summary>
        /// FromDate value in date range.
        /// </summary>
        public DateTime FromDate { get; set; }

        /// <summary>
        /// ToDate value in date range.
        /// </summary>
        public DateTime ToDate { get; set; }
    }
}