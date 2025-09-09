using ASOFT.A00.DataAccess.Interfaces;
using ASOFT.Core.DataAccess.Enums;
using Microsoft.Extensions.Configuration;

namespace ASOFT.CoreAI.Business
{
    public class SettingsManager
    {
        private readonly IConfiguration _configuration;
        private IASOFTCommonQueries _aSOFCommonQueries;

        public SettingsManager(IConfiguration configuration, IASOFTCommonQueries aSOFTCommonQueries)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _aSOFCommonQueries = aSOFTCommonQueries ?? throw new ArgumentNullException(nameof(aSOFTCommonQueries));
        }

        public (int maxChat, int maxTraining) GetNumberRecords()
        {
            int maxChatRecords = _configuration.GetValue<int>("ChatHistorySettings:MaxRecords");
            int maxTrainingRecords = _configuration.GetValue<int>("TrainingDataSettings:MaxRecords");
            return (maxChatRecords, maxTrainingRecords);
        }

        public async Task<string> GetExternalApi()
        {
            string API_Domain = (await _aSOFCommonQueries.GetConfigST2101ByKey((int)GroupConfig.HostingNAPI, "MainURL")).KeyValue;
            string API_PORT = (await _aSOFCommonQueries.GetConfigST2101ByKey((int)GroupConfig.HostingNAPI, "MainPort")).KeyValue;
            string strHttp = @"http://";
            string newUrl = string.Empty;
            if (!API_Domain.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !API_Domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                newUrl = strHttp + API_Domain;
            }
            else
            {
                newUrl = API_Domain;
            }
            newUrl += ":" + API_PORT;
            return newUrl;
        }

        public string GetKeyReadOCR()
        {
            var apiConfig = _configuration.GetValue<string>("ReadConfigOCR:key-ocr");
            return apiConfig ?? string.Empty;
        }
    }
}