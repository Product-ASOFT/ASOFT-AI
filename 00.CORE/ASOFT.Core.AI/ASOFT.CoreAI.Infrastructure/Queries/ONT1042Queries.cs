using ASOFT.Core.Common.InjectionChecker;
using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Entities.ViewModels.AI;
using Dapper;
using DocumentFormat.OpenXml.VariantTypes;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Infrastructure.Queries
{
    public class ONT1042Queries : BusinessDataAccess, IONT1042Queries
    {
        public ONT1042Queries(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {

        }
        public async Task<IEnumerable<ONT1042ViewModel>> GetAllAsync(int caseType, string? parameterName = null, string? typeConfigID = null)
        {
            var dynamicParameters = new DynamicParameters();
            dynamicParameters.Add("@CaseType", caseType, DbType.Int32, ParameterDirection.Input);
            dynamicParameters.Add("@ParameterName", parameterName, DbType.String, ParameterDirection.Input);
            dynamicParameters.Add("@TypeConfigID", typeConfigID, DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(async connection =>
            {
                return await connection.QueryAsync<ONT1042ViewModel>("ONP1046", dynamicParameters, commandType: CommandType.StoredProcedure, commandTimeout: 120);
            });
        }

        public async Task<List<PromptContentViewModel>> GetDataPrompt(int caseType, string? parameterName = null, string? typeConfigID = null)
        {
            var dataPrompt = await GetAllAsync(caseType, parameterName, typeConfigID);
            if (dataPrompt == null || !dataPrompt.Any())
            {
                return new List<PromptContentViewModel>();
            }
            var result = new List<PromptContentViewModel>();
            foreach (var item in dataPrompt.ToList())
            {
                result.Add(new PromptContentViewModel
                {
                    APK = item.APK,
                    CriteriaName = item.CriteriaName,
                    PromptUser = item.PromptInput!,
                    PromptSystem = string.Format("{0} \n {1} \n {2}", item.PromptBussiness, item.PromptHandle, item.PromptOutput)
                });
            }
            return result;
        }
    }
}
