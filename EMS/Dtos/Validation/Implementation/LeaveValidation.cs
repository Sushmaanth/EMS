using Dtos.LeaveRequestDto;
using Dtos.Validation.Abstraction;
using Entities.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dtos.Validation.Implementation
{
    public class LeaveValidation : ILeaveValidation
    {
        private readonly AppDbContext _context;

        public LeaveValidation(AppDbContext context)
        {
            _context = context;
        }

        private void AddError(Dictionary<string, List<string>> errors,
                            string key,
                            string message)
        {
            if (!errors.ContainsKey(key))
            {
                errors[key] = new List<string>();
            }

            errors[key].Add(message);
        }

        public async Task<Dictionary<string, List<string>>> Validate(
           ApplyLeaveDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Id == dto.EmployeeId);

            if (employee == null)
            {
                AddError(errors,"EmployeeId","Employee does not exist");

                return errors;
            }
        
            // Employee must have manager
    

            if (!employee.ManagerId.HasValue)
            {
                AddError(errors,"ManagerId","No manager assigned to employee");
            }

            // Start Date <= End Date

            if (dto.StartDate > dto.EndDate)
            {
                AddError(errors,"StartDate","Start date cannot be after end date");
            }

            if (dto.StartDate < DateOnly.FromDateTime(DateTime.Today))
            {
                AddError(errors,"StartDate","Cannot apply leave for past dates");
            }

            // Leave Type Valid

            if (!Enum.IsDefined(dto.LeaveType))
            {
                AddError(errors,"LeaveType","Invalid leave type");
            }

            // Reason

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                AddError(errors,"Reason","Reason is required");
            }

            // Prevent overlapping leave requests

            bool overlappingLeave =
                await _context.LeaveRequests.AnyAsync(l =>
                    l.EmployeeId == dto.EmployeeId
                    &&
                    l.Status != Entities.Enums.LeaveStatus.Rejected
                    &&
                    dto.StartDate <= l.EndDate
                    &&
                    dto.EndDate >= l.StartDate);

            if (overlappingLeave)
            {
                AddError(errors,"StartDate","Leave overlaps with an existing leave request");
            }

            return errors;
        }
    }
}
