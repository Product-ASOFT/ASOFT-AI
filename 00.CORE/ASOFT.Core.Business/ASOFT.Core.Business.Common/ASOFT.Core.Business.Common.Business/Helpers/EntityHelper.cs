using ASOFT.Core.Business.Common.Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASOFT.Core.Business.Common.Business.Helpers
{
    public class EntityHelper
    {
        /// <summary>
        /// Lấy prop của 1 object theo tên
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="instanceObject"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/03/2021]
        /// </history>
        public static object GetPropertyValue(Type type, string propertyName, object instanceObject)
        {
            try
            {
                var d = type.GetProperty(propertyName).GetValue(instanceObject, null);
                return d;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Set prop của 1 object theo tên
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="instanceObject"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <history>
        ///     [Đoàn Duy]   Created [04/03/2021]
        /// </history>
        public static void SetPropertyValue(Type type, string propertyName, object instanceObject, object value)
        {
            type.GetProperty(propertyName).SetValue(instanceObject, value);
        }

        /// <summary>
        /// Kiểm tra 2 entity và trả ra các trường dữ liệu thay đổi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="valueUpdated"></param>
        /// <returns></returns>
        public static List<EntityCompareResult> CompareEntity<T>(T value, T valueUpdated)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            var result = new List<EntityCompareResult>();
            foreach (PropertyInfo pi in properties)
            {

                object oldValue = pi.GetValue(value), newValue = pi.GetValue(valueUpdated);

                if (!object.Equals(oldValue, newValue) && pi.Name != "LastModifyUserID" && pi.Name != "LastModifyDate")
                {
                    result.Add(new EntityCompareResult(pi.Name, oldValue, newValue));
                }
            }

            return result;
        }
    }
}
