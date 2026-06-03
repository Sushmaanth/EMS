using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Dtos.Validation
{
    public class DateNotInFutureAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(
            object value,
            ValidationContext validationContext)
        {
            if (value is DateOnly date &&
                date > DateOnly.FromDateTime(DateTime.Today))
            {
                return new ValidationResult(
                    "Date of joining cannot be future date");
            }

            return ValidationResult.Success;
        }
    }
}
