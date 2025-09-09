using System.ComponentModel.DataAnnotations;

namespace ASOFT.A00.Entities.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string DivisionID { get; set; }
        public string DeviceID { get; set; }
    }
}
