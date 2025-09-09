using System.ComponentModel.DataAnnotations;

namespace ASOFT.Core.Business.Users.Entities.Requests
{
    public class SignInBiometricsRequest
    {
        /// <summary>
        /// Tài khoản
        /// </summary>
        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        /// <summary>
        /// Chữ ký mật mã
        /// </summary>

        [Required]
        public string Signature { get; set; }

        public bool IsAsoft { get; set; } = false;
    }
}
