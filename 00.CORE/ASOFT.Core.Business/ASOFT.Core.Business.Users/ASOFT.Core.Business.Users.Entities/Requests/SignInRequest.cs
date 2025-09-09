using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.Business.Users.Entities.Requests
{
    public class SignInRequest
    {
        /// <summary>
        /// Tài khoản
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        public bool IsAsoft { get; set; } = false;
    }
}