// #################################################################
// # Copyright (C) 2019-2020, asoft JSC.  All Rights Reserved.                       
// #
// # History：                                                                        
// #	Date Time	    Updated		    Content                
// #    11/01/2021      Tấn Thành       Tạo mới
// #################################################################

using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.A00.Entities;
using ASOFT.Core.DataAccess;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace ASOFT.A00.DataAccess.Utilities
{
    public class ASOFTCommonQueries : BusinessDataAccess, IASOFTCommonQueries
    {
        public ASOFTCommonQueries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }
        private const string SQL_GetConfigST2101ByGroup = @"SELECT DivisionID, TypeID, GroupID, KeyName, KeyValue, ValueDataType, DefaultValue, ModuleID, IsEnvironment, Description 
            FROM ST2101 WITH(NOLOCK) 
            WHERE GroupID = @GroupID";
        public async Task<IEnumerable<ST2101>> GetConfigSt2101ByGroup(int groupID, CancellationToken cancellationToken = default)
        {

            try
            {
                return await UseConnectionAsync(async connection =>
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@GroupID", groupID, DbType.Int16, ParameterDirection.Input);

                    return await connection.QueryAsync<ST2101>(SQL_GetConfigST2101ByGroup, dynamicParameters);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private const string SQL_GetConfigST2101ByKey = @"
            SELECT TOP 1 DivisionID, TypeID, GroupID, KeyName, KeyValue, ValueDataType, DefaultValue, ModuleID, IsEnvironment, Description 
            FROM ST2101 WITH(NOLOCK) 
            WHERE GroupID = @GroupID AND KeyName = @KeyName";
        /// <summary>
        /// Get Config by key - ST2101
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="keyName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]   Created [25/01/2021]
        /// </history>
        public async Task<ST2101> GetConfigST2101ByKey(int groupID, string keyName, CancellationToken cancellationToken = default)
        {
            try
            {
                return await UseConnectionAsync(async connection =>
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@GroupID", groupID, DbType.Int16, ParameterDirection.Input);
                    dynamicParameters.Add("@KeyName", keyName, DbType.String, ParameterDirection.Input);

                    return await connection.QueryFirstOrDefaultAsync<ST2101>(SQL_GetConfigST2101ByKey, dynamicParameters);
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get Value Config by key - ST2101
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="keyName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <history>
        ///     [Tấn Thành]   Created [03/02/2021]
        /// </history>
        public async Task<string> GetValueConfigST2101(int groupID, string keyName, CancellationToken cancellationToken = default)
        {
            try
            {
                return await UseConnectionAsync(async connection =>
                {
                    DynamicParameters dynamicParameters = new DynamicParameters();
                    dynamicParameters.Add("@GroupID", groupID, DbType.Int16, ParameterDirection.Input);
                    dynamicParameters.Add("@KeyName", keyName, DbType.String, ParameterDirection.Input);

                    var st2101 = await connection.QueryFirstOrDefaultAsync<ST2101>(SQL_GetConfigST2101ByKey, dynamicParameters);

                    return st2101 == null ? string.Empty : st2101.KeyValue;
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
