using Dtos.Validation.Abstraction;
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

        public async Task<Dictionary<string, List<string>>> Validate(CreateEmployeeDto dto)
        {
            var errors = new Dictionary<string, List<string>>();

            bool emailExists = await context.Employees
                .AnyAsync(x => x.EmailId.ToLower().Trim() == dto.EmailId.ToLower().Trim());

            bool mobileExists = await context.Employees
                .AnyAsync(x => x.Mobile == dto.Mobile);

            if (emailExists)
            {
                errors.Add("EmailId", new List<string>
        {
            "Email already exists"
        });
            }

            if (mobileExists)
            {
                errors.Add("Mobile", new List<string>
        {
            "Mobile already exists"
        });
            }

            if (dto.DateOfJoining >
                DateOnly.FromDateTime(DateTime.Today))
            {
                errors.Add("DateOfJoining", new List<string>
        {
            "Date of joining cannot be future date"
        });
            }

            return errors;
        }
    }
}
