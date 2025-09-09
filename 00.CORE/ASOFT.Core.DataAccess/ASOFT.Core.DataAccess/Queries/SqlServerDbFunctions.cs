using System;

namespace ASOFT.Core.DataAccess
{
    /// <summary>
    /// Sql server db functions
    /// </summary>
    public static class SqlServerDbFunctions
    {
        /// <summary>
        /// Sql server DATEFROMPARTS function for building datetime.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static DateTime DateFromParts(int year, int month, int day) => throw new NotImplementedException();

        /// <summary>
        /// Sql server EOMONTH function for getting the last date of month by input datetime.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime EoMonth(DateTime date) => throw new NotImplementedException();
    }
}