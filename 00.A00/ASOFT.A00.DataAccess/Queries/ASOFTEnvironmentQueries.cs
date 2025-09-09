// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    04/01/2021      Tấn Thành       Tạo mới
// #################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.Entities;
using ASOFT.A00.Entities.Enums;
using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess.Enums;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using entities = ASOFT.A00.Entities;

namespace ASOFT.A00.DataAccess.Queries
{
    public class ASOFTEnvironmentQueries : IASOFTEnvironment
    {
        public string DB_ADMIN_NAME = string.Empty;
        public string DB_ADMIN_CONN_STRING = string.Empty;
        public string DB_ERP_NAME = string.Empty;
        public string DB_ERP_CONN_STRING = string.Empty;
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public ASOFTEnvironmentQueries(IConfiguration config, ILoggerFactory logger)
        {
            _config = config;
            DB_ADMIN_CONN_STRING = config["DbConnectionStrings:ConnectionStrings:Admin"].ToString();
            DB_ERP_CONN_STRING = config["DbConnectionStrings:ConnectionStrings:Business"].ToString();
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }

        #region Sử dụng cho PipeLine
        #region Get Instance
        /// <summary>
        /// Get Tên Db Admin
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        public string GetDbAdminName()
        {
            var dbConnection = new SqlConnection(DB_ADMIN_CONN_STRING);
            DB_ADMIN_NAME = dbConnection.Database;
            dbConnection.Dispose();
            dbConnection = null;
            return DB_ADMIN_NAME;
        }

        /// <summary>
        /// Get chuỗi kết nối Db Admin
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        public string GetConnectionAdmin()
        {
            return DB_ADMIN_CONN_STRING;
        }

        /// <summary>
        /// Get tên Db Admin
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        public string GetDbErpName()
        {
            var dbConnection = new SqlConnection(DB_ERP_CONN_STRING);
            DB_ERP_NAME = dbConnection.Database;
            dbConnection.Dispose();
            dbConnection = null;
            return DB_ERP_NAME;
        }

        /// <summary>
        /// Get chuỗi kết nối Db ERP
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [04/01/2021]
        /// </history>
        public string GetConnectionErp()
        {
            return DB_ERP_CONN_STRING;
        }
        #endregion Get Instance
        #endregion Sử dụng cho PipeLine

        /// <summary>
        /// Set biến môi trường
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [25/01/2021]
        /// </history>
        public void SetEnvironment()
        {
            IEnumerable<ST2101> listEnvironment = null;
            listEnvironment =  GetConfigEnvirionment();

            ASOFTEnvironment.CallCenter = new Dictionary<string, string>();
            ASOFTEnvironment.Automation = new Dictionary<string, string>();
            if (listEnvironment != null)
            {
                foreach (var item in listEnvironment)
                {
                    switch (item.GroupID)
                    {
                        // Group Hosting & API
                        case (int)GroupConfig.CallCenter:

                            switch (item.KeyName)
                            {

                                case "cdrsSipServer":

                                    entities.ASOFTEnvironment.CallCenter.Add("CDRSSIPSERVER", item.KeyValue);
                                    break;

                                case "registrationsSipServer":

                                    entities.ASOFTEnvironment.CallCenter.Add("REGISTRATIONSIPSERVER", item.KeyValue);
                                    break;

                                case "api_key":

                                    entities.ASOFTEnvironment.CallCenter.Add("APIKEY", item.KeyValue);
                                    break;

                                case "api_secret":

                                    entities.ASOFTEnvironment.CallCenter.Add("APISECRET", item.KeyValue);
                                    break;

                                default:

                                    break;
                            }
                            break;

                        case (int)GroupConfig.Automation:

                            switch (item.KeyName)
                            {
                                case "Status":

                                    entities.ASOFTEnvironment.Automation.Add("status", item.KeyValue);
                                    break;

                                case "TimeScan":

                                    entities.ASOFTEnvironment.Automation.Add("timeScan", item.KeyValue);
                                    break;

                                case "TypeOfTime":

                                    entities.ASOFTEnvironment.Automation.Add("typeOfTime", item.KeyValue);
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        #region Private Method

        private const string SQL_GetEnvironmentConfig = @"SELECT DivisionID, TypeID, GroupID, KeyName, KeyValue, ValueDataType, DefaultValue, ModuleID, IsEnvironment, Description 
            FROM ST2101 WITH(NOLOCK) 
            WHERE IsEnvironment = @IsEnvironment";

        /// <summary>
        /// Set giá trị cho biến môi trường
        /// </summary>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [25/01/2021]
        /// </history>
        public IEnumerable<ST2101> GetConfigEnvirionment()
        {
            using (SqlConnection connection = new SqlConnection(DB_ERP_CONN_STRING))
            {
                connection.Open();
                try
                {
                    var dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("IsEnvironment", 1, DbType.Int16, ParameterDirection.Input);
                    return  connection.Query<ST2101>(SQL_GetEnvironmentConfig, dynamicParameters);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception: " + ex.Message + ". At: " + ex.StackTrace + " With Message: " + ex.Message);
                    return null;
                }
                finally
                {
                }
            }
        }
        #endregion Private Method
    }
}
