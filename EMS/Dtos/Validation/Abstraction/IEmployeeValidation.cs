using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Validation.Abstraction
{
    public interface IEmployeeValidation
    {
        Task<Dictionary<string, List<string>>> Validate(CreateEmployeeDto dto);
    }
}
