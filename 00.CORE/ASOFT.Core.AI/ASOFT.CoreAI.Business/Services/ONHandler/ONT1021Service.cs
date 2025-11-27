using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using ASOFT.CoreAI.Infrastructure;
using Dapper;
using System.Data;

namespace ASOFT.CoreAI.Business
{
    public class ONT1021Service : BusinessDataAccess, IONT1021Service
    {
        private readonly IBusinessContext<ONT1021ViewModel> _businessContext;

        public ONT1021Service(IDbConnectionProvider dbConnectionProvider,
            IBusinessContext<ONT1021ViewModel> businessContext) : base(dbConnectionProvider)
        {
            _businessContext = businessContext;
        }

        public async Task<IEnumerable<ONT1021ViewModel>> GetAllAsync(List<int> categoryIDs)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@DivisionID", "", DbType.String, ParameterDirection.Input);
            parameters.Add("@CategoryIDList", string.Join(",", categoryIDs), DbType.String, ParameterDirection.Input);

            return await UseConnectionAsync(
                async connection
                => await connection.QueryAsync<ONT1021ViewModel>
                ("ONP1024", parameters, commandType: CommandType.StoredProcedure), CancellationToken.None);
        }
    }
}