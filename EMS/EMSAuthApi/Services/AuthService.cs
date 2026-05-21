using Dtos;
using Entities;
using Entities.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EMSAuthApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext context;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly TokenService tokenService;

        public AuthService(AppDbContext context, IPasswordHasher<User> passwordHasher,TokenService tokenService)
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

                string refreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);

                context.SaveChanges();
                return new LoginResponseDTO
                {
                    Token = token,
                    EmailId = user.EmailId,
                    Role = user.Role.RoleName,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public LoginResponseDTO RefreshToken(RefreshTokenDTO dto)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            string email = principal.FindFirst(ClaimTypes.Email)?.Value;

            var user = context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u =>
                u.EmailId == email);


            if (user == null)
            {
                throw new Exception("User not found");
            }

            if (user.RefreshToken != dto.RefreshToken)
            {
                throw new Exception("Invalid refresh token");
            }

            if (user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                throw new Exception("Refresh token expired");
            }

            string newAccessToken = tokenService.GenerateToken(user);

            string newRefreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(1);

            context.SaveChanges();

            return new LoginResponseDTO
            {
                Token = newAccessToken,

                RefreshToken = newRefreshToken,

                EmailId = user.EmailId,

                Role = user.Role.RoleName
            };
        }

        internal object MicrosoftLogin(string email)
        {
            try
            {
                var user = context.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u =>
                        u.EmailId == email);

                if (user == null)
                {
                    throw new Exception(
                        "User not registered in EMS");
                }

                if (!user.IsActive)
                {
                    throw new Exception(
                        "User account inactive");
                }

                string token = tokenService.GenerateToken(user);

                string refreshToken = tokenService.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime =DateTime.Now.AddMinutes(30);

                context.SaveChanges();

                return new LoginResponseDTO
                {
                    Token = token,

                    RefreshToken = refreshToken,

                    EmailId = user.EmailId,

                    Role = user.Role.RoleName
                };
            }

            catch (Exception e)
            {
                throw new Exception($"Exception: {e.Message}");
            }
        }
    }
}
