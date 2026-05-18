using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos
{
    public class ActivateAccountDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Id")]
        [StringLength(255, ErrorMessage = "Email Id cannot exceed more than 255 characters")]
        public string EmailId { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$", 
            ErrorMessage = "Password must contain uppercase, lowercase, number, special character and minimum 6 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage ="Confirm password is required")]
        [Compare("Password",ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
