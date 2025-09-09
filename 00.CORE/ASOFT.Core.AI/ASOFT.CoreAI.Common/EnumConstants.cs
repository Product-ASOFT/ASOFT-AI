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
            SFXXX1 = 4, // Quyền được hỏi dữ liệu ngoài internet
            SFXXX2 = 5, // Quyền được hỏi dữ liệu nội bộ
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
    }
}