namespace ASOFT.Core.API.Httpss.Errors
{
    /// <summary>
    /// Error code
    /// </summary>
    public sealed class ErrorCode
    {
        /// <summary>
        /// Code
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Code string
        /// </summary>
        public string CodeString { get; }

        /// <summary>
        /// Error code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeString"></param>
        public ErrorCode(int code, string codeString)
        {
            Code = code;
            CodeString = codeString;
        }
    }
}