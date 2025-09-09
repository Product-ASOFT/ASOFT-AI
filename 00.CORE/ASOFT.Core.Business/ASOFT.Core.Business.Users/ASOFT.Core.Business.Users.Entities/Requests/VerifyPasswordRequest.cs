using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Entities.Requests
{
    public class VerifyPasswordRequest
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
    }
}
