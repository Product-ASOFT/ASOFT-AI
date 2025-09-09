// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    17/12/2020      Tấn Thành       Tạo mới
// #################################################################

using ASOFT.Core.Business.Common.DataAccess.Interfaces;
using ASOFT.Core.Business.Common.Entities;
using ASOFT.Core.Common.InjectionChecker;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Queries
{
    public class VoucherQueries : IVoucherQueries
    {
        private string businessConnString = string.Empty;
        private readonly ILogger _logger;
        private string key_log = "VoucherNo";
        private string func_location = "ASOFT.A00.DataAccess.Interfaces.VoucherQueries.";

        public VoucherQueries(IConfiguration config, ILoggerFactory logger)
        {
            businessConnString = config["DbConnectionStrings:ConnectionStrings:Business"].ToString();
            _logger = Checker.NotNull(logger, nameof(logger)).CreateLogger(GetType());
        }

        public VoucherQueries()
        {

        }
        private const string SQL_GET_SETTING_VOUCHERNO = @"
            SELECT APK, DivisionID, VoucherTask, TaskHourDecimal, VoucherStatus, VoucherTaskSample, VoucherStep
                , VoucherProcess, VoucherProjectSample, VoucherProject, VoucherIssues, VoucherRequest
                , CreateDate, CreateUserID, LastModifyDate, LastModifyUserID
            FROM OOT0060 WITH (NOLOCK)
            WHERE DivisionID = @DivisionID";

        /// <summary>
        /// Lấy thiết lập sinh mã tự động cho các loại chứng từ nghiệp vụ
        /// </summary>
        /// <param name="divisionID"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]  Tạo mới [17/12/2020]
        /// </history>
        public async Task<OOT0060> GetSettingVoucherNoByDivisionID(string divisionID, CancellationToken cancellationToken = default)
        {
            using (SqlConnection businessConnection = new SqlConnection(businessConnString))
            {
                await businessConnection.OpenAsync();
                try
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@DivisionID", divisionID, DbType.String, ParameterDirection.Input);
                    return await businessConnection.QueryFirstOrDefaultAsync<OOT0060>(SQL_GET_SETTING_VOUCHERNO, dynamicParameters);
                }
                catch (Exception ex)
                {
                    _logger.LogError(key_log + " - " + "Exception: " + ex.Message + ". At: " + func_location + MethodBase.GetCurrentMethod().Name);
                    return null;
                }
            }
        }
    }
}
