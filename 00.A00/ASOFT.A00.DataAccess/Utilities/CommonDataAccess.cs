// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    12/08/2020      Tấn Thành       Tạo mới
// #################################################################

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace ASOFT.A00.DataAccess.Utilities
{
    public class CommonDataAccess
    {
        static GregorianCalendar _gc = new GregorianCalendar();

        /// <summary>
        ///     Get DbType
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành] Created [08/12/2020]
        /// </history>
        public static DbType GetDBTypeColumn(int type)
        {
            DbType db;
            switch (type)
            {
                case 1:
                    db = DbType.Guid;
                    break;
                case 6:
                    db = DbType.Byte;
                    break;
                case 2:
                    db = DbType.Int32;
                    break;
                case 5:
                    db = DbType.Int32;
                    break;
                case 7:
                    db = DbType.String;
                    break;
                case 4:
                    db = DbType.Int64;
                    break;
                case 9:
                    db = DbType.DateTime;
                    break;
                case 3:
                    db = DbType.String;
                    break;
                case 10:
                    db = DbType.Boolean;
                    break;
                case 11:
                    db = DbType.DateTime;
                    break;
                case 12:
                    db = DbType.String;
                    break;
                case 13:
                    db = DbType.DateTime;
                    break;
                case 8:
                    db = DbType.Decimal;
                    break;
                case 14:
                    db = DbType.Binary;
                    break;
                case 16: // TIME GRID
                    db = DbType.Int32;
                    break;
                default:
                    db = DbType.String;
                    break;
            }
            return db;
        }

        /// <summary>
        /// Get dữ liệu theo DbType
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành] Create  [08/12/2020]
        /// </history>
        public static object GetDBTypeValue(DbType type, string data)
        {
            try
            {
                if (type.ToString().Equals("Guid"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return Guid.NewGuid();
                    }
                    return Guid.Parse(data);
                }
                if (type.ToString().Equals("Byte"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return 0;
                    }
                    return Convert.ToByte(data);
                }
                if (type.ToString().Equals("Int32"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    return Convert.ToInt32(data);
                }
                if (type.ToString().Equals("Int64"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    return Convert.ToInt64(data);
                }
                if (type.ToString().Equals("Boolean"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return false;
                    }
                    if (data.Equals("1"))
                    {
                        return Convert.ToBoolean(true);
                    }
                    else
                    {
                        return Convert.ToBoolean(false);
                    }
                }
                if (type.ToString().Equals("Decimal"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    return Convert.ToDecimal(data);
                }
                if (type.ToString().Equals("DateTime"))
                {
                    if (string.IsNullOrEmpty(data))
                    {
                        return null;
                    }
                    return DateTime.Parse(data);
                }
            }
            catch (Exception ex)
            {
                string message = "\r\n";
                string value = string.IsNullOrEmpty(data) ? "NULL" : data;
                message = string.Concat(message, "GetDBTypeValue(DbType type = {0}, string data = {1})\r\n");
                message = string.Format(message, type.ToString(), value);
                //_ASOFTLogger.Error(message);
                throw ex;
            }

            return !string.IsNullOrEmpty(data) ? data : null;
        }

      
        private static int GetWeekNumberOfMonth(DateTime date)
        {
            DateTime first = new DateTime(date.Year, date.Month, 1);
            return GetWeekOfYear(date) - GetWeekOfYear(first) + 1;
        }

        /// <summary>
        /// Lấy số thứ tự tuần trong năm
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Created [17/12/2020]
        /// </history>
        private static int GetWeekOfYear(DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }

        /// <summary>
        /// Lấy data từ key-dt-cl
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dt"></param>
        /// <param name="cl"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Created [30/12/2020]
        /// </history>
        public static string GetDataByKey(string key, string[] dt, string[] cl)
        {
            int count = dt.Length;
            for (int i = 0; i < count; i++)
            {
                // equal key and null check
                if (cl[i].Equals(key) && dt[i] != null)
                {
                    // Get string array result
                    List<String> resultList = dt[i].Split(',').ToList();
                    // Get length
                    int length = resultList.Count;

                    // Check if length > 2 khử đối tượng đầu tiên trong list và return result
                    if (length >= 2)
                    {
                        int firstPosition = 0;
                        resultList.RemoveAt(firstPosition);
                        return String.Join(",", resultList);
                    }
                    // Tìm thấy key nhưng không thỏa điều kiện trên nên break vòng lập return String.Empty
                    break;
                }
            }
            return String.Empty;
        }
    }
}
