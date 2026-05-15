using Dtos.Validation;
using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models
{
    public class CreateEmployeeViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2–100 characters")]
        [RegularExpression(@"^[a-zA-Z\s\.,-]+$", ErrorMessage = "Invalid name format")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [MinimumAge(18, ErrorMessage = "Employee must be atleast 18 years old")]
        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Id")]
        [StringLength(255, ErrorMessage = "Email Id cannot exceed more than 255 characters")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
        public long Mobile { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Date Of Joining is required")]
        public DateOnly DateOfJoining { get; set; }

        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
    }
}
