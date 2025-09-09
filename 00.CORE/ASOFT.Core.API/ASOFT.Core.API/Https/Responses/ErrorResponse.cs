namespace ASOFT.Core.API.Httpss.ApiResponse
{
    /// <summary>
    /// Error response version 2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ErrorResponse<T> : BaseResponse
    {
        /// <summary>
        /// Error response v2
        /// </summary>
        public T Error { get; set; }

        /// <summary>
        /// Error response version 2
        /// </summary>
        public ErrorResponse()
        {
        }

        /// <summary>
        /// Error response version 2
        /// </summary>
        /// <param name="error"></param>
        public ErrorResponse(T error) => Error = error;
    }

    /// <summary>
    /// Object error response v2
    /// </summary>
    public class ErrorResponse : ErrorResponse<object>
    {
        /// <summary>
        /// Error response v2
        /// </summary>
        public ErrorResponse()
        {
        }

        /// <summary>
        /// Error response v2
        /// </summary>
        /// <param name="error"></param>
        public ErrorResponse(object error) : base(error)
        {
        }

        /// <summary>
        /// Create error response
        /// </summary>
        /// <param name="error"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ErrorResponse<T> Create<T>(T error) => new ErrorResponse<T>(error);
    }
}