using Dtos;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EMSAuthApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext context;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly TokenService tokenService;

        public AuthService(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        TokenService tokenService)
        {
            this.context = context;
            this.passwordHasher = passwordHasher;
            this.tokenService = tokenService;
        }
        public LoginResponseDTO LoginEmployee(LoginDto dto)
        {
            try
            {
                var user = context.Users.Include(u => u.Role)
                  .FirstOrDefault(u => u.EmailId == dto.Email);

                if (user == null)
                {
                    throw new Exception("Invalid Email");
                }

                bool isValidPassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Success;

                if (!isValidPassword)
                {
                    throw new Exception("Invalid Password");
                }
                if (!user.IsActive)
                {
                    throw new Exception("User account inactive");
                }

                string token = tokenService.GenerateToken(user);

                return new LoginResponseDTO
                {
                    Token = token,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName
                };
            }
            catch (Exception e)
            {
                throw new Exception($"Exception: {e.Message} and Inner Exception : {e.InnerException?.Message}");
            }
        }
    }
}
