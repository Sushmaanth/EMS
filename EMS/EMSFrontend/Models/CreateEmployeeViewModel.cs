using Dtos.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EMSFrontend.Models
{
    public class CreateEmployeeViewModel
    {

        [Required(ErrorMessage = "Employee Code is required")]
        [RegularExpression(@"^NQAI\d{3}R$", ErrorMessage = "Employee Code must be in format NQAI000R")]
        [Remote(action: "CheckEmployeeCode",controller: "Validation",ErrorMessage = "Employee Code already exists")]
        public string EmployeeCode { get; set; }

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
        [Remote(action: "CheckEmail",controller: "Validation",ErrorMessage = "Email already exists")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Invalid mobile number")]
        [Remote(action: "CheckMobile",controller: "Validation",ErrorMessage = "Mobile already exists")]
        public long Mobile { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Date Of Joining is required")]
        public DateOnly DateOfJoining { get; set; }

        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int? RoleId { get; set; }

        public string RoleName { get; set; }
        public int? ManagerId { get; set; }

        public string ManagerName { get; set; }
    }
}
