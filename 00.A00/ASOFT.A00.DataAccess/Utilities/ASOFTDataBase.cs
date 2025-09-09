// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    12/09/2020      Tấn Thành       Tạo mới
// #################################################################

using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ASOFT.A00.DataAccess.Utilities
{
    public class ASOFTDataBase
    {
        public static DynamicParameters AddParameter(DynamicParameters dynamicParameters, string[] dl, string[] dt, Dictionary<string, byte[]> images = null, List<string> isNoUpdate = null, string sysTable = null, bool overrideValue = false)
        {
            isNoUpdate = isNoUpdate ?? new List<string>();

            // 20/04/2020 - [Vĩnh Tâm] - Begin add
            // Bổ sung 2 checkbox của DevTool vào danh sách không update dữ liệu
            isNoUpdate.Add("chkDbAdmin");
            isNoUpdate.Add("chkDbERP");
            // 20/04/2020 - [Vĩnh Tâm] - End add

            for (int i = 0; i < dt.Length; i++)
            {
                if (string.IsNullOrEmpty(dt[i]) || isNoUpdate.IndexOf(dl[i]) != -1)
                    continue;

                string COL = dl[i];
                string[] data = dt[i].Split(',');
                if (data.Length > 1)
                {
                    object value;
                    DbType dbtype = CommonDataAccess.GetDBTypeColumn(Convert.ToInt16(data[0]));
                    if (dbtype.ToString().Equals("Binary"))
                    {
                        byte[] avatar;
                        images.TryGetValue(COL + "_" + sysTable, out avatar);
                        dynamicParameters.Add(COL + "_" + sysTable, avatar, dbtype, ParameterDirection.Input);
                    }
                    else
                    {
                        if (data.Length > 2)
                        {
                            string data1 = "";
                            for (int j = 1; j < data.Length; j++)
                            {
                                data1 = data1 + data[j] + ",";
                            }
                            value = CommonDataAccess.GetDBTypeValue(dbtype, data1.Remove(data1.Length - 1));
                        }
                        else
                        {
                            value = CommonDataAccess.GetDBTypeValue(dbtype, data[1]);
                        }
                        dynamicParameters.Add(COL, value, dbtype, ParameterDirection.Input);
                    }
                }
            }
            return dynamicParameters;
        }
    }
}
