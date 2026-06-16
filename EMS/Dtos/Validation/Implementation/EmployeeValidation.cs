using Dtos.Constants;
using Dtos.Validation.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.EntityFrameworkCore;

namespace Dtos.Validation.Implementation
{
    public class EmployeeValidation: IEmployeeValidation
    {
        private readonly AppDbContext context;

        public EmployeeValidation(AppDbContext context)
        {
            this.context = context;
        }

        private void AddError(Dictionary<string, List<string>> errors,string key, string message)
        {
            if (!errors.ContainsKey(key))
            {
                errors[key] = new List<string>();
            }

            errors[key].Add(message);
        }

        public async Task<Dictionary<string, List<string>>> Validate(CreateEmployeeDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrWhiteSpace(dto.EmployeeCode))
            {
                errors["EmployeeCode"] = new List<string> { "Employee code is required" };
            }
            else
            {
                bool codeExists = await context.Employees
                   .AnyAsync(x => x.EmployeeCode == dto.EmployeeCode.Trim());

                if (codeExists)
                    errors["EmployeeCode"] = new List<string> { "Employee code already exists" };
            }

                bool emailExists = await context.Employees
                .AnyAsync(x => x.EmailId.ToLower().Trim() == dto.EmailId.ToLower().Trim());

            bool mobileExists = await context.Employees
                .AnyAsync(x => x.Mobile == dto.Mobile);

            if (emailExists)
            {
                AddError(errors, "EmailId", "Email already exists");
            }

            if (mobileExists)
            {
                AddError(errors, "Mobile", "Mobile already exists");
            }

            if (dto.DateOfJoining >DateOnly.FromDateTime(DateTime.Today))
            {
                AddError(errors, "DateOfJoining", "Date of joining cannot be future date");
            }

            //Invalid Role Selected
            var role = await context.Role.FindAsync(dto.RoleId);

            if (role == null)
            {
                AddError(errors, "RoleId", "Role does not exist");
            }

            //Department is required
            if ((dto.RoleId == RoleConstants.Employee || dto.RoleId == RoleConstants.Manager)
                && !dto.DepartmentId.HasValue)
            {
                AddError(errors,"DepartmentId", "Department is required");
            }

            //Invalid Department
            if (dto.DepartmentId.HasValue)
            {
                var dept = await context.Departments.FindAsync(dto.DepartmentId);

                if (dept == null)
                {
                    AddError(errors,"DepartmentId", "Invalid department");
                }
            }

            //manager validation
            if (dto.ManagerId.HasValue)
            {
                var manager = await context.Employees
                    .FirstOrDefaultAsync(x => x.Id == dto.ManagerId);

                if (manager == null)
                    AddError(errors,"ManagerId", "Manager does not exist");

                else
                {
                    if (manager.RoleId != RoleConstants.Manager && manager.RoleId != RoleConstants.Admin)
                        AddError(errors,"ManagerId", "Selected employee is not a manager");

                    if (dto.DepartmentId != manager.DepartmentId)
                        AddError(errors,"ManagerId", "Manager must be in same department");

                    if (dto.Id == dto.ManagerId)
                        AddError(errors,"ManagerId", "Employee cannot be their own manager");
                }
            }

            //Role base error
            if (dto.RoleId == RoleConstants.Admin && dto.ManagerId.HasValue)
                AddError(errors,"ManagerId", "Admin cannot have a manager");

           //Employee - Own manager
            if (dto.Id == dto.ManagerId)
            {
                AddError(errors,"ManagerId","Employee cannot be their own manager");
            }

            return errors;
        }
    }
}
