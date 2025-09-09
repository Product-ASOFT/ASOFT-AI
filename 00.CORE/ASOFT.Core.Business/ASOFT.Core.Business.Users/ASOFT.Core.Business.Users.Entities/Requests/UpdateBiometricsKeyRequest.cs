using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.Core.Business.Users.Entities.Requests
{
    public class UpdateBiometricsKeyRequest
    {
        /// <summary>
        /// Tài khoản
        /// </summary>
        [StringLength(50)]
        public string UserID { get; set; }

        /// <summary>
        /// Biometrics public key
        /// </summary>
        [Required]
        public string BiometricsKey { get; set; }

        public string DivisionID { get; set; }
    }
}
