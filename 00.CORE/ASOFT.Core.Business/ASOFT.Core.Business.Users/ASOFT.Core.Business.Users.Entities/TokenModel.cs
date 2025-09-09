namespace ASOFT.Core.Business.Users.Models
{
    /// <summary>
    /// Đối tượng token trả về client
    /// </summary>
    public class TokenModel
    {
        /// <summary>
        /// Id của token
        /// </summary>
        public string IDToken { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Số giây hết hạn tính từ thời điểm đăng nhập thành công
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Loại token
        /// </summary>
        public string TokenType { get; set; }

        public string RefreshToken { get; set; }
        public string Scope { get; set; }
    }
}