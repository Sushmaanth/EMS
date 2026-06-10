using Dtos.Validation.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Validation.Implementation
{
    public class EmployeeDuplicateUploadValidator : IEmployeeDuplicateUploadValidator
    {
        public List<string> Validate(EmployeeExcelUploadDto dto, EmployeeUploadValidationContext context)
        {
            var errors = new List<string>();

            if (!context.UploadedEmployeeCodes.Add(dto.EmployeeCode))
            {
                errors.Add("Duplicate Employee Code found in uploaded file.");
            }

            if (context.ExistingEmployeeCodes
                .Contains(dto.EmployeeCode))
            {
                errors.Add("Employee Code already exists.");
            }

            if (!context.UploadedEmails.Add(dto.EmailId))
            {
                errors.Add("Duplicate Email Id found in uploaded file.");
            }

            if (context.ExistingEmails.Contains(dto.EmailId))
            {
                errors.Add("Email Id already exists.");
            }

            var mobile = dto.Mobile;

            if (!context.UploadedMobiles.Add(mobile))
            {
                errors.Add(
                    "Duplicate Mobile Number found in uploaded file.");
            }

            if (context.ExistingMobiles.Contains(mobile))
            {
                errors.Add("Mobile Number already exists.");
            }

            return errors;
        }
    }
}
