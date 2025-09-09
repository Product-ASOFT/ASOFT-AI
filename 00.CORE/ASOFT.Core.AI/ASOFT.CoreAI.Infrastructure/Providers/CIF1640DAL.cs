using ASOFT.Core.DataAccess;
using ASOFT.CoreAI.Entities;
using Dapper;

namespace ASOFT.CoreAI.Infrastructure
{
    public class CIF1640DAL : BusinessDataAccess, ICIF1640DAL
    {
        public CIF1640DAL(IDbConnectionProvider dbConnectionProvider) : base(dbConnectionProvider)
        {
        }

        private const string SQL_GetConfigModelAIIsUse =
        @"SELECT TOP 1 APK, ChatbotName, UrlAPI, APIKey, MaxToken, IsSendImage, IsUse, S1.Description as ChatBotAPIType,  S2.Description as ChatbotModel
        FROM CIT1641 C
        LEFT JOIN ST0099 S1 ON C.ChatBotAPIType = S1.ID AND S1.CodeMaster = 'ChatbotAPIType'
        LEFT JOIN ST0099 S2 ON C.ChatbotModel = S2.ID  AND S2.CodeMaster = 'ChatbotModel'
        WHERE IsUse = 1 ";

        public async Task<ChatbotConfig> GetConfigModelAI()
        {
            var dynamicParameters = new DynamicParameters();
            return await UseConnectionAsync(
            async connection =>
          await connection.QueryFirstOrDefaultAsync<ChatbotConfig>(SQL_GetConfigModelAIIsUse, dynamicParameters));
        }
    }
}