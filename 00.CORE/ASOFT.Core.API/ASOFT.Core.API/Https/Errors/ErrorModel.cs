using ASOFT.Core.Common.InjectionChecker;
using System.Collections.Generic;

namespace ASOFT.Core.API.Httpss.Errors
{
    /// <summary>
    /// Error model
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Status code
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// Error code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Error code string
        /// </summary>
        public string CodeString { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Details
        /// </summary>
        public Dictionary<string, object> Details { get; set; }

        /// <summary>
        /// Error model
        /// </summary>
        public ErrorModel()
        {
        }

        /// <summary>
        /// Error model
        /// </summary>
        /// <param name="errorCode"></param>
        public ErrorModel(ErrorCode errorCode)
        {
            Checker.NotNull(errorCode, nameof(errorCode));
            Code = errorCode.Code;
            CodeString = errorCode.CodeString;
        }

        /// <summary>
        /// Error model
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        public ErrorModel(ErrorCode errorCode, string message) : this(errorCode) => Message = message;

        /// <summary>
        /// Error code
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static ErrorModel FromErrorCode(ErrorCode errorCode) => new ErrorModel(errorCode);

        /// <summary>
        /// Error code
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ErrorModel FromErrorCode(ErrorCode errorCode, string message) => new ErrorModel(errorCode)
        {
            Message = message
        };
    }
}