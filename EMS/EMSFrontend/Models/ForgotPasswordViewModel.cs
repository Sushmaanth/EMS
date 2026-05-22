using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Id")]
        [StringLength(255, ErrorMessage = "Email Id cannot exceed more than 255 characters")]
        [MaxLength(255)]
        public string Email { get; set; }

    }
}
