namespace ASOFT.Core.API.Https
{
    /// <summary>
    /// Status codes and functions for checking status code use in ASOFT projects.
    /// </summary>
    /// <history>
    /// Luan Le [Created] 2019/15/08.
    /// </history>
    public static class ApiStatusCodes
    {
        #region Client error status codes

        /// <summary>
        /// When user seding bad or invalid request.
        /// </summary>
        public const int BadRequest400 = 400;

        /// <summary>
        /// When request is un authorize.
        /// </summary>
        public const int UnAuthorize401 = 401;

        /// <summary>
        /// When request is forbidden.
        /// </summary>
        public const int Forbidden403 = 403;

        /// <summary>
        /// When url or data is not found.
        /// </summary>
        public const int NotFound404 = 404;

        #endregion

        #region Server error status codes

        /// <summary>
        /// Has error in server.
        /// </summary>
        public const int InternalServerError500 = 500;

        /// <summary>
        /// Bad gateway
        /// </summary>
        public const int BadGateway502 = 502;

        #endregion

        #region Success status codes

        /// <summary>
        /// When request is succeed.
        /// </summary>
        public const int Ok200 = 200;

        /// <summary>
        /// When record is created.
        /// </summary>
        public const int Created201 = 201;

        /// <summary>
        /// When has no content, or delete record succeed.
        /// </summary>
        public const int NoContent204 = 204;

        /// <summary>
        /// Use for cache server will return this code when data is not modified. Client checking this staus code.
        /// </summary>
        public const int NotModified304 = 304;

        /// <summary>
        /// Temporary redirect
        /// </summary>
        public const int TemporaryRedirect307 = 307;

        #endregion

        /// <summary>
        /// Check status code is client error status code.
        /// </summary>
        /// <param name="statusCode">Status code for check.</param>
        /// <returns>
        /// Return <c>true</c> if status code is client error status code otherwise return <c>false</c>.
        /// </returns>
        /// <history>
        /// Luan Le [Created] 2019/08/15.
        /// </history>
        public static bool IsClientErrorStatusCode(int statusCode) => 400 <= statusCode && statusCode < 500;

        /// <summary>
        /// Check status code is server error code.
        /// </summary>
        /// <param name="statusCode">Status code for check.</param>
        /// <returns>
        /// Return <c>true</c> if status code is server error status code otherwise return <c>false</c>.
        /// </returns>
        /// <history>
        /// Luan Le [Created] 2019/08/15.
        /// </history>
        public static bool IsServerErrorStatusCode(int statusCode) => 500 <= statusCode && statusCode < 600;

        /// <summary>
        /// Check status code is error code. Check both <see cref="IsClientErrorStatusCode(int)"/>
        /// and <see cref="IsServerErrorStatusCode(int)"/>
        /// </summary>
        /// <param name="statusCode">Status code for check.</param>
        /// <returns>
        /// Return <c>true</c> if status code is error status code otherwise return <c>false</c>.
        /// </returns>
        /// <history>
        /// Luan Le [Created] 2019/08/15.
        /// </history>
        public static bool IsErrorStatusCode(int statusCode) => 400 <= statusCode && statusCode < 600;

        /// <summary>
        /// Check status code is success status code.
        /// </summary>
        /// <param name="statusCode">Status code for check.</param>
        /// <returns>
        /// Return <c>true</c> if status code is success status code otherwise return <c>false</c>.
        /// </returns>
        /// <history>
        /// Luan Le [Created] 2019/08/15.
        /// </history>
        public static bool IsSuccessStatusCode(int statusCode) => 200 <= statusCode && statusCode < 400;
    }
}