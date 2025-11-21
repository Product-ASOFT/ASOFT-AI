namespace ASOFT.CoreAI.Common
{
    public static class EnumConstants
    {
        public enum AccessTypeName
        {
            None = 0,
            Internal = 1,
            External = 2,
            Both = 3,
            SF2130 = 4, // Quyền được hỏi dữ liệu ngoài internet
            SF2140 = 5, // Quyền được hỏi dữ liệu nội bộ
        }

        public enum ChatSessionStatus
        {
            Active = 0,
            Deleted = 1,
        }

        public enum ChatResponseType
        {
            AI = 0,
            User = 1,
            System = 2,
            Tool = 3,
        }

        public enum TypeChat
        {
            Normal = 0,
            Plugin = 1,
        }

        public enum StatusCompareOCR
        {
            UNDEFINED = 0,
        }
        public enum StatusProcessCompareOCR
        {
            PROCESSING = 0,
            COMPLETED = 1,
            FAILED = 2,
        }
        public enum StatusResultCompare
        {
            NG = 0,
            OK = 1,
            BLANK = 2,
        }
    }
}