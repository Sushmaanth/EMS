using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos.Validation
{
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int minimumAge;

        public MinimumAgeAttribute(int minimumAge)
        {
            this.minimumAge = minimumAge;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            DateOnly dob = (DateOnly)value;

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            int age = today.Year - dob.Year;

            if (dob > today.AddYears(age))
            {
                age--;
            }

            if (age< minimumAge)
            {
                return new ValidationResult(ErrorMessage??$"Minimum age must be {minimumAge}");
            }

            return ValidationResult.Success;
        }
    }
}
