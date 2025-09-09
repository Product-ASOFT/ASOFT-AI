namespace ASOFT.Core.API.Httpss.ApiResponse
{
    /// <summary>
    /// Success response v2
    /// </summary>
    public class SuccessResponse<T> : BaseResponse
    {
        /// <summary>
        /// Success response v2
        /// </summary>
        public SuccessResponse()
        {
        }

        /// <summary>
        /// Success response v2
        /// </summary>
        /// <param name="data"></param>
        public SuccessResponse(T data)
        {
            Data = data;
        }


        /// <summary>
        /// Data
        /// </summary>
        public T Data { get; set; }
    }
}