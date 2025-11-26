using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Entities
{
    public class AiRuntimeConfigViewModel
    {
        public string AiApiBaseUrl { get; set; }
        public string AiOcrBaseUrl { get; set; }
        public string AiOcrExternalApiKey { get; set; }

        public bool OcrUseLocalService { get; set; }

        public int ChatHistoryMaxRecords { get; set; }
        public int TrainingMaxRecords { get; set; }

        public string RedisConnectionString { get; set; }
        public int RedisDefaultExpireDays { get; set; }
    }
}
