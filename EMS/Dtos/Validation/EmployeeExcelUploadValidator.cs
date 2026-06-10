using FluentValidation;
using System.Text.RegularExpressions;

namespace Dtos.Validation
{
    public class EmployeeExcelUploadValidator: AbstractValidator<EmployeeExcelUploadDto>
    {
        public EmployeeExcelUploadValidator(EmployeeUploadValidationContext context)
        {
            RuleFor(x => x.EmployeeCode)
            .NotEmpty()
            .WithMessage("Employee Code is required.");

            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Employee Name is required.");

            RuleFor(x => x.Gender)
            .NotEmpty()
            .Must(g =>
                new[] { "Male", "Female", "Other" }
                .Contains(g, StringComparer.OrdinalIgnoreCase))
            .WithMessage("Invalid Gender.");

            RuleFor(x => x.EmailId)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid Email Id.");

            RuleFor(x => x.Mobile)
            .NotNull()
            .Must(x => x >= 6000000000 && x <= 9999999999)
            .WithMessage("Invalid Mobile Number.");

            RuleFor(x => x.Salary)
            .GreaterThan(0)
            .WithMessage("Salary must be greater than zero.");

            RuleFor(x => x.DepartmentName)
            .Must(x =>context.Departments.ContainsKey(x))
            .WithMessage("Department does not exist.");

            RuleFor(x => x.DateOfBirth)
           .Must(dob =>
           {
               var today =
                   DateOnly.FromDateTime(DateTime.Today);

               int age = today.Year - dob.Year;

               if (dob > today.AddYears(-age))
                   age--;

               return age >= 18;
           })

           .WithMessage("Employee must be at least 18 years old.");

            RuleFor(x => x.DateOfJoining)
              .GreaterThan(x => x.DateOfBirth)
              .WithMessage("Date Of Joining must be after Date Of Birth.");
        }
    }
}
