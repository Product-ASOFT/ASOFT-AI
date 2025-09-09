namespace ASOFT.CoreAI.Common
{
    public static class AIConstants
    {
        public const string FIELD_EMBEDDING = "EmbeddingVector";
        public const string ModelAIKey = "ModelAIKey";
        public const string RedisConfig = "Redis:Configuration";

        #region tạo các lệnh Redis

        public const string CreateIndex = "FT.CREATE";
        public const string Search = "FT.SEARCH";
        public const string DropIndex = "FT.DROPINDEX";
        public const string AlterIndex = "FT.ALTER";
        public const string Info = "FT.INFO";
        public const string AddDocument = "FT.ADD";
        public const string DeleteDocument = "FT.DEL";

        #endregion tạo các lệnh Redis

        #region Tạo tên các Index

        public const string IndexOO = "oo_content";
        public const string IndexCRM = "crm_content";
        public const string IndexReSearch = "search_content";

        #endregion Tạo tên các Index

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
        }

        #endregion Tạo các key cho Agent
    }
}