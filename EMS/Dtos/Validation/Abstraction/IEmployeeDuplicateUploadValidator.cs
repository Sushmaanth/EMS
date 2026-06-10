using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Validation.Abstraction
{
    public interface IEmployeeDuplicateUploadValidator
    {
        List<string> Validate(EmployeeExcelUploadDto dto,EmployeeUploadValidationContext context);
    }
}
