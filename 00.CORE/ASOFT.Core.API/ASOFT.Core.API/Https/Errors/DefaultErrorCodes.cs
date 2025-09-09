namespace ASOFT.Core.API.Httpss.Errors
{
    /// <summary>
    /// Các error codes mặc định
    /// </summary>
    public static class DefaultErrorCodes
    {
        /// <summary>
        /// Invalid input parameters
        /// </summary>
        public static readonly string InvalidInputParameters = "error.default.invalid_input_parameters";

        /// <summary>
        /// Error not found
        /// </summary>
        public static readonly string NotFound = "error.default.not_found";

        /// <summary>
        /// Concurrent error
        /// </summary>
        public static readonly string ConcurrentUpdateError = "error.default.concurrent_update";

        /// <summary>
        /// Invalid user name or password
        /// </summary>
        public static readonly string InvalidUserNameOrPassword = "error.default.invalid_username_or_password";

        /// <summary>
        /// Not support api version
        /// </summary>
        public static readonly string NotSupportApiVersion = "error.default.not_support_api_version";

        /// <summary>
        /// Trùng ID
        /// </summary>
        public static readonly string DuplicateID = "error.duplicate_id";

    }
}