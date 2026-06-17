using Dtos.LeaveRequestDto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Validation.Abstraction
{
    public interface ILeaveValidation
    {
        Task<Dictionary<string, List<string>>> Validate(int employeeId, ApplyLeaveDto dto);
    }
}
