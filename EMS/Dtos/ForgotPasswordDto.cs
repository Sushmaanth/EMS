using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Id")]
        [StringLength(255, ErrorMessage = "Email Id cannot exceed more than 255 characters")]
        public string Email { get; set; }
    }
}
