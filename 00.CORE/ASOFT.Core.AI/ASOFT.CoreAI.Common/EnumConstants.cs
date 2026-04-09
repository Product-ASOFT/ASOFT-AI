using System.ComponentModel.DataAnnotations;

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
            UNPROCESSED = 3,
        }
        public enum StatusResultCompare
        {
            NG = 0,
            OK = 1,
            BLANK = 2,
        }
        public enum IntegrationServiceType
        {
            [Display(Name = "AI")]
            AI = 1,

            [Display(Name = "Ecommerce")]
            ECOMMERCE = 2,

            [Display(Name = "Social Network")]
            SOCIAL_NETWORK = 3,

            [Display(Name = "Call Center")]
            CALL_CENTER = 4,

            [Display(Name = "External System Connection")]
            EXTERNAL_SYSTEM_CONNECTION = 5,

            [Display(Name = "E-Invoice")]
            E_INVOICE = 6,

            [Display(Name = "Agent")]
            AGENT = 7,

        }
        public enum AIKeyStatus
        {
            Valid,
            InvalidKey,
            OutOfCredit,
            UnknownError
        }
        public enum ParameterRole
        {
            ViewLabel = 0, // Dùng để hiển thị label cho các trường dữ liệu động
            ViewValue = 1,  // Dùng để hiển thị value cho các trường dữ liệu động
            SaveData = 2, // Dùng để lưu dữ liệu động vào database
            FileType = 3, // Dùng để cấu hình các loại file
        }
        public enum TypeCase
        {
            E_ParameterID = 1, // Dùng để lấy theo ParameterID ở ONT1041
            E_TypeConfigID = 2, // Dùng để lấy theo TypeConfig ở ONT1040
        }
    }
}