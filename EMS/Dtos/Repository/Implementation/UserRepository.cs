using Dtos.Repository.Abstraction;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dtos.Repository.Implementation
{
    public class UserRepository : IUserRepository<ActivateAccountDTO>
    {
        private readonly AppDbContext context;
        private readonly IPasswordHasher<User> passwordHasher;

        public UserRepository(AppDbContext context, IPasswordHasher<User> passwordHasher)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
        }
        public ActivateAccountResponseDTO AccountActivation(ActivateAccountDTO data)
        {
            try
            {
                var employeeExists = context.Employees.FirstOrDefault(e => e.EmailId == data.EmailId);

                if (employeeExists == null)
                {
                    return new ActivateAccountResponseDTO
                    {
                        Success = false,
                        Message = "Employee email not found"
                    };
                }

                var employeeRole = context.Role
                    .FirstOrDefault(e => e.RoleName == "User");

                if (employeeRole == null)
                {
                    throw new Exception("Role configuration missing");
                }

                var userExists = context.Users
                .Any(u => u.EmailId == data.EmailId);

                if (userExists)
                {
                    return new ActivateAccountResponseDTO
                    {
                        Success = false,
                        Message = "Account already activated"
                    };
                }

                User user = new()
                {
                    EmailId = data.EmailId,
                    IsActive = true,
                    EmployeeId = employeeExists.Id,
                    RoleId = employeeRole.Id=2
                };

                user.PasswordHash = passwordHasher.HashPassword(user, data.Password);

                context.Add(user);
                int result = context.SaveChanges();

                    return new ActivateAccountResponseDTO
                    {
                        Success = true,
                        EmailId = user.EmailId,
                        Message = "Account activated successfully"
                    };
            }
            catch
            {
                throw;
            }
        }
    }
}

