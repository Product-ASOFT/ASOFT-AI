namespace ASOFT.CoreAI.Common
{
    public static class AIConstants
    {
        public const string FIELD_EMBEDDING = "EmbeddingVector";
        public const string ModelAIKey = "ModelAIKey";
        public const string RedisConfig = "Redis:Configuration";
        public const string RedisConfigDatabaseName = "Redis:DatabaseName";
        public const string APIOCRConfig = "APIOCR:URL";
        public const string ONT1021_ALL_SETTINGS = "ONT1021_ALL_SETTINGS";
        public const string ErrorType = "ErrorType";
        public const string RawErrorMessage = "RawErrorMessage";

        #region tạo các lệnh Redis

        public const string CreateIndex = "FT.CREATE";
        public const string Search = "FT.SEARCH";
        public const string DropIndex = "FT.DROPINDEX";
        public const string AlterIndex = "FT.ALTER";
        public const string Info = "FT.INFO";
        public const string AddDocument = "FT.ADD";
        public const string DeleteDocument = "FT.DEL";

        #endregion tạo các lệnh Redis

        #region Tạo các key cho Agent

        public static class AgentKeys
        {
            public const string OO_AGENT_OOF2110 = "OO_AGENT_OOF2110";
            public const string OO_AGENT_OOF2160 = "OO_AGENT_OOF2160";
            public const string OO_AGENT_OOF2190 = "OO_AGENT_OOF2190";
            public const string CRM_AGENT_CRMF2030 = "CRM_AGENT_CRMF2030";
            public const string CRM_AGENT_CRMF2050 = "CRM_AGENT_CRMF2050";
            public const string CRM_AGENT_CRMF2160 = "CRM_AGENT_CRMF2160";
            public const string RESEARCH_AGENT = "RESEARCH_AGENT";
            public const string READFILE_AGENT = "READFILE_AGENT";
            public const string BEM_AGENT_BEMF2000 = "BEM_AGENT_BEMF2000";
            public const string TYPE_QUESTION = "TYPE_QUESTION";
            public const string BEM_AGENT_BEMF2000_CREATEFILE = "BEM_AGENT_BEMF2000_CREATEFILE";
            public const string HRM_AGENT_HRMF2220 = "HRM_AGENT_HRMF2220";
            public const string HRM_AGENT_HRMF1030 = "HRM_AGENT_HRMF1030";
            public const string BEM_AGENT_BEMF2000_SUMMARY = "BEM_AGENT_BEMF2000_SUMMARY";
            public const string BEM_AGENT_BEMF2000_READFILE = "BEM_AGENT_BEMF2000_READFILE";

          
        }
        public static class AgentCriteriaKeys
        {
            // Criteria keys
            public const string CRITERIA_SUPPLIER_NAME = "CRITERIA_SUPPLIER_NAME";
            public const string CRITERIA_INVOICE_NO = "CRITERIA_INVOICE_NO";
            public const string CRITERIA_INVOICE_DATE = "CRITERIA_INVOICE_DATE";
            public const string CRITERIA_AMOUNT = "CRITERIA_AMOUNT";
            public const string CRITERIA_AMOUNT_CUSTOMSHEET = "CRITERIA_AMOUNT_CUSTOMSHEET";
            public const string CRITERIA_CURRENCY = "CRITERIA_CURRENCY";
            public const string CRITERIA_INCOTERM = "CRITERIA_INCOTERM";
            public const string CRITERIA_PAYMENT_DEADLINE = "CRITERIA_PAYMENT_DEADLINE";
            public const string CRITERIA_CHECK_COMPLETED_DATE = "CRITERIA_CHECK_COMPLETED_DATE";
            public const string CRITERIA_SIGNATURE_STAMP = "CRITERIA_SIGNATURE_STAMP";
        }
        public static class AgentTypeKeys
        {
            public const string BEM_AGENT_BEMF2000_WAREHOUSE = "BEM_AGENT_BEMF2000_WAREHOUSE";
            public const string BEM_AGENT_BEMF2000_MACHINE = "BEM_AGENT_BEMF2000_MACHINE";
            public const string BEM_AGENT_BEMF2000_SERVICE = "BEM_AGENT_BEMF2000_SERVICE";
            public const string BEM_AGENT_BEMF2000_BUILD = "BEM_AGENT_BEMF2000_BUILD";
            public const string BEM_AGENT_BEMF2000_OTHER = "BEM_AGENT_BEMF2000_OTHER";
        }
        #endregion Tạo các key cho Agent
        public static class MimeTypesConstants
        {
            //  Ảnh
            public static readonly string[] ImageTypes =
            {
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/bmp",
                "image/tiff"
            };

            //  PDF
            public const string Pdf = "application/pdf";

            //  Word
            public const string WordDocx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            public const string WordDoc = "application/msword";

            //  Excel
            public const string ExcelXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            public const string ExcelXls = "application/vnd.ms-excel";

            // PowerPoint 
            public const string Pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            public const string Ppt = "application/vnd.ms-powerpoint";
        }

    }
    public static class APIConfigKeys
    {
        public const string AI_API_BASEURL = "AI_BaseApiUrl"; // key cấu hình API AI ở webconfig của ERP9
        public const string AI_ERP_BASEURL = "AI_ERP:BaseUrl"; // key cấu hình URL của ERP9
        public const string AI_OCR_BASEURL = "AI_OCR:BaseUrl"; // key cấu hình URL của API OCR
        public const string REDIS_CONNECTIONSTRING = "Redis:ConnectionString"; // key cấu hình chuỗi kết nối Redis
        public const string OCR_USE_LOCAL_SERVICE = "OCR:UseLocalService"; // key cấu hình sử dụng dịch vụ OCR nội bộ hay không
        public const string AI_OCR_EXTERNAL_API_KEY = "AI_OCR:ExternalApiKey"; // key cấu hình API Key của dịch vụ OCR bên ngoài
        public const string CHAT_HISTORY_MAX_RECORDS = "Chat:HistoryMaxRecords"; // key cấu hình số bản ghi tối đa của lịch sử chat
        public const string AI_TRAINING_MAX_RECORDS = "AI_Training:MaxRecords"; // key cấu hình số bản ghi tối đa của dữ liệu huấn luyện AI
        public const string REDIS_DEFAULT_EXPIRE_DAYS = "Redis:DefaultExpireDays"; // key cấu hình số ngày hết hạn mặc định cho Redis
        public const string REDIS_DATABASE_NAME = "Redis:DatabaseName"; // key cấu hình tên database Redis
        public const string REDIS_DATABASE_USERNAME = "Redis:Username"; // key cấu hình tên Username Redis
        public const string REDIS_DATABASE_PASSWORD = "Redis:Password"; // key cấu hình tên Password Redis
        public const string AI_MODEL_EMBEDDING = "ModelEmbedding:ModelName"; // key cấu hình model Embedding
        public const string AI_AI_LLM_BASEURL = "AI_LLM:BaseUrl"; // key cấu hình gọi API
        public const string AI_AI_LLM_ISUSE = "AI_LLM:IsUse"; // key cấu hình có/không sử dụng API LLM 
        public const string AI_AI_LLM_MAXTOKEN = "AI_LLM:MaxToken"; // key cấu hình max token
        public const string AI_AI_LLM_TEMPERATURE = "AI_LLM:Temperature"; // key cấu hình mức độ trả lời ngẫu nhiên
    }
    public static class AIRoleName
    {
        public const string ROLE_USER = "user";
        public const string ROLE_SYSTEM = "system";
    }
}